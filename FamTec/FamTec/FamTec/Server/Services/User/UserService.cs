using ClosedXML.Excel;
using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using FamTec.Shared.Server.DTO.User;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamTec.Server.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly IAdminUserInfoRepository AdminUserInfoRepository;
        private readonly IAdminPlacesInfoRepository AdminPlaceInfoRepository;
        private readonly IPlaceInfoRepository PlaceInfoRepository;

        private readonly IFileService FileService;
        private readonly IConfiguration Configuration;
        private ILogService LogService;

        DirectoryInfo? di;
        string? PlaceFileFolderPath = String.Empty;

        public UserService(IUserInfoRepository _userinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforpeository,
            IConfiguration _configuration,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.UserInfoRepository = _userinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforpeository;

            this.FileService = _fileservice;
            this.Configuration = _configuration;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<string?>> LoginSelectPlaceService(HttpContext context, int placeid)
        {
            List<Claim> authClaims = new List<Claim>();
            string? jsonConvert = String.Empty;

            try
            {
                string? adminidx = Convert.ToString(context.Items["AdminIdx"]);
                if(adminidx is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                List<AdminPlaceTb>? adminplace = await AdminPlaceInfoRepository.GetMyWorksList(Convert.ToInt32(adminidx));
                if(adminplace is [_, ..])
                {
                    AdminPlaceTb? select = adminplace.FirstOrDefault(m => m.PlaceTbId == placeid);
                    if (select is not null)
                    {
                        PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(placeid);
                        if(placetb is not null)
                        {
                            authClaims.Add(new Claim("UserIdx", context.Items["UserIdx"].ToString())); // USER 인덱스
                            authClaims.Add(new Claim("Name", context.Items["Name"].ToString())); // 이름
                            authClaims.Add(new Claim("jti", context.Items["jti"].ToString()));
                            authClaims.Add(new Claim("AlarmYN", context.Items["AlarmYN"].ToString())); // 알람 받을지 여부
                            authClaims.Add(new Claim("AdminYN", context.Items["AdminYN"].ToString())); // 관리자 여부
                            authClaims.Add(new Claim("UserType", context.Items["UserType"].ToString()));
                            authClaims.Add(new Claim("AdminIdx", context.Items["AdminIdx"].ToString())); // 관리자 인덱스
                            authClaims.Add(new Claim("PlaceIdx", placetb.Id!.ToString()));// 사업장 인덱스
                            authClaims.Add(new Claim("PlaceName", placetb.Name!.ToString()));// 사업장 이름

                            if (context.Items["Role"].ToString() == "시스템관리자")
                            {
                                authClaims.Add(new Claim("Role", "시스템관리자"));
                                authClaims.Add(new Claim(ClaimTypes.Role, "SystemManager"));
                            }

                            if (context.Items["Role"].ToString() == "마스터")
                            {
                                authClaims.Add(new Claim("Role", "마스터"));
                                authClaims.Add(new Claim(ClaimTypes.Role, "Master"));
                            }

                            if (context.Items["Role"].ToString() == "매니저")
                            {
                                authClaims.Add(new Claim("Role", "매니저"));
                                authClaims.Add(new Claim(ClaimTypes.Role, "Manager"));
                            }

                            JObject items = new JObject();

                            /* 메뉴 접근권한 */
                            items.Add("UserPerm_Basic", context.Items["UserPerm_Basic"].ToString());
                            items.Add("UserPerm_Machine", context.Items["UserPerm_Machine"].ToString());
                            items.Add("UserPerm_Elec", context.Items["UserPerm_Elec"].ToString());
                            items.Add("UserPerm_Lift", context.Items["UserPerm_Lift"].ToString());
                            items.Add("UserPerm_Fire", context.Items["UserPerm_Fire"].ToString());
                            items.Add("UserPerm_Construct", context.Items["UserPerm_Construct"].ToString());
                            items.Add("UserPerm_Network", context.Items["UserPerm_Network"].ToString());
                            items.Add("UserPerm_Beauty", context.Items["UserPerm_Beauty"].ToString());
                            items.Add("UserPerm_Security", context.Items["UserPerm_Security"].ToString());
                            items.Add("UserPerm_Material", context.Items["UserPerm_Material"].ToString());
                            items.Add("UserPerm_Energy", context.Items["UserPerm_Energy"].ToString());
                            items.Add("UserPerm_User", context.Items["UserPerm_User"].ToString());
                            items.Add("UserPerm_Voc", context.Items["UserPerm_Voc"].ToString());
                            jsonConvert = JsonConvert.SerializeObject(items);
                            authClaims.Add(new Claim("UserPerms", jsonConvert));

                            /* VOC 권한 */
                            
                            items = new JObject();
                            
                            items.Add("VocMachine", context.Items["VocMachine"].ToString()); // 기계민원 처리권한
                            items.Add("VocElec", context.Items["VocElec"].ToString()); // 전기민원 처리권한
                            items.Add("VocLift", context.Items["VocLift"].ToString()); // 승강민원 처리권한
                            items.Add("VocFire", context.Items["VocFire"].ToString()); // 소방민원 처리권한
                            items.Add("VocConstruct", context.Items["VocConstruct"].ToString()); // 건축민원 처리권한
                            items.Add("VocNetwork", context.Items["VocNetwork"].ToString()); // 통신민원 처리권한
                            items.Add("VocBeauty", context.Items["VocBeauty"].ToString()); // 미화민원 처리권한
                            items.Add("VocSecurity", context.Items["VocSecurity"].ToString()); // 보안민원 처리권한
                            items.Add("VocDefault", context.Items["VocDefault"].ToString()); // 기타 처리권한
                            jsonConvert = JsonConvert.SerializeObject(items);
                            authClaims.Add(new Claim("VocPerms", jsonConvert));
                            
                            /* PLACE 권한 */
                            items = new JObject();
                            items.Add("PlacePerm_Machine", placetb.PermMachine.ToString());
                            items.Add("PlacePerm_Elec", placetb.PermElec.ToString());
                            items.Add("PlacePerm_Lift", placetb.PermLift.ToString());
                            items.Add("PlacePerm_Fire", placetb.PermFire.ToString());
                            items.Add("PlacePerm_Construct", placetb.PermConstruct.ToString());
                            items.Add("PlacePerm_Network", placetb.PermNetwork.ToString());
                            items.Add("PlacePerm_Beauty", placetb.PermBeauty.ToString());
                            items.Add("PlacePerm_Security", placetb.PermSecurity.ToString());
                            items.Add("PlacePerm_Material", placetb.PermMaterial.ToString());
                            items.Add("PlacePerm_Energy", placetb.PermEnergy.ToString());
                            items.Add("PlacePerm_Voc", placetb.PermVoc.ToString());
                            jsonConvert = JsonConvert.SerializeObject(items);
                            authClaims.Add(new Claim("PlacePerms", jsonConvert));

                            // JWT 인증 페이로드 사인 비밀키
                            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: Configuration["JWT:Issuer"],
                                audience: Configuration["JWT:Audience"],
                                expires: DateTime.Now.AddDays(1),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                            return new ResponseUnit<string?>() { message = "로그인 성공(관리자).", data = accessToken, code = 200 };
                        }
                        else
                        {
                            return new ResponseUnit<string?>() { message = "사업장이 존재하지 않습니다.", data = null, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<string?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<string?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 일반페이지 유저 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> UserLoginService(LoginDTO dto)
        {
            List<Claim> authClaims = new List<Claim>();
            string? jsonConvert = String.Empty;

            try
            {
                UsersTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID!, dto.UserPassword!);
                if (usertb is null)
                    return new ResponseUnit<string?>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 200 };

                bool? AdminYN = usertb.AdminYn;
                if(AdminYN == false) // 일반유저
                {
                    PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(usertb.PlaceTbId!.Value);
                            
                    if(placetb is not null)
                    {
                        authClaims.Add(new Claim("UserIdx", usertb.Id.ToString())); // + USERID
                        authClaims.Add(new Claim("Name", usertb.Name!.ToString())); // + USERID
                        authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        authClaims.Add(new Claim("AlarmYN", usertb.AlarmYn!.ToString())); // 알람 받을지 여부
                        authClaims.Add(new Claim("AdminYN", usertb.AdminYn!.ToString())); // 관리자 여부
                        authClaims.Add(new Claim("UserType", "User"));
                        authClaims.Add(new Claim("Role", "User"));
                        authClaims.Add(new Claim(ClaimTypes.Role, "User"));
                        authClaims.Add(new Claim("PlaceIdx", placetb.Id!.ToString()));// 사업장 인덱스
                        authClaims.Add(new Claim("PlaceName", placetb.Name!.ToString()));// 사업장 이름
                        
                        JObject items = new JObject();

                        /* 메뉴 접근권한 */
                        items.Add("UserPerm_Basic", usertb.PermBasic);
                        items.Add("UserPerm_Machine", usertb.PermMachine);
                        items.Add("UserPerm_Elec", usertb.PermElec);
                        items.Add("UserPerm_Lift", usertb.PermLift);
                        items.Add("UserPerm_Fire", usertb.PermFire);
                        items.Add("UserPerm_Construct", usertb.PermConstruct);
                        items.Add("UserPerm_Network", usertb.PermNetwork);
                        items.Add("UserPerm_Beauty", usertb.PermBeauty);
                        items.Add("UserPerm_Security", usertb.PermSecurity);
                        items.Add("UserPerm_Material", usertb.PermMaterial);
                        items.Add("UserPerm_Energy", usertb.PermEnergy);
                        items.Add("UserPerm_User", usertb.PermUser);
                        items.Add("UserPerm_Voc", usertb.PermVoc);
                        jsonConvert = JsonConvert.SerializeObject(items);
                        authClaims.Add(new Claim("UserPerms", jsonConvert));
                                
                        items = new JObject();
                        /* VOC 권한 */
                        items.Add("VocMachine", usertb.VocMachine.ToString()); // 기계민원 처리권한
                        items.Add("VocElec", usertb.VocElec.ToString()); // 전기민원 처리권한
                        items.Add("VocLift", usertb.VocLift.ToString()); // 승강민원 처리권한
                        items.Add("VocFire", usertb.VocFire.ToString()); // 소방민원 처리권한
                        items.Add("VocConstruct", usertb.VocConstruct.ToString()); // 건축민원 처리권한
                        items.Add("VocNetwork", usertb.VocNetwork.ToString()); // 통신민원 처리권한
                        items.Add("VocBeauty", usertb.VocBeauty.ToString()); // 미화민원 처리권한
                        items.Add("VocSecurity", usertb.VocSecurity.ToString()); // 보안민원 처리권한
                        items.Add("VocDefault", usertb.VocEtc.ToString()); // 기타 처리권한
                        jsonConvert = JsonConvert.SerializeObject(items);
                        authClaims.Add(new Claim("VocPerms", jsonConvert));

                        /* 사업장 권한 */
                        items.Add("PlacePerm_Machine", placetb.PermMachine.ToString()); // 사업장 기계메뉴 권한
                        items.Add("PlacePerm_Elec", placetb.PermElec.ToString()); // 사업장 전기메뉴 권한
                        items.Add("PlacePerm_Lift", placetb.PermLift.ToString()); // 사업장 승강메뉴 권한
                        items.Add("PlacePerm_Fire", placetb.PermFire.ToString()); // 사업장 소방메뉴 권한
                        items.Add("PlacePerm_Construct", placetb.PermConstruct.ToString()); // 사업장 건축메뉴 권한
                        items.Add("PlacePerm_Network", placetb.PermNetwork.ToString()); // 사업장 통신메뉴 권한
                        items.Add("PlacePerm_Beauty", placetb.PermBeauty.ToString()); // 사업장 미화메뉴 권한
                        items.Add("PlacePerm_Security", placetb.PermSecurity.ToString()); // 사업장 보안메뉴 권한
                        items.Add("PlacePerm_Material", placetb.PermMaterial.ToString()); // 사업장 자재메뉴 권한
                        items.Add("PlacePerm_Energy", placetb.PermEnergy.ToString()); // 사업장 전기메뉴 권한
                        items.Add("PlacePerm_Voc", placetb.PermVoc.ToString()); // 사업장 VOC 권한
                                
                        jsonConvert = JsonConvert.SerializeObject(items);
                        authClaims.Add(new Claim("PlacePerms", jsonConvert));



                        // JWT 인증 페이로드 사인 비밀키
                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: Configuration["JWT:Issuer"],
                            audience: Configuration["JWT:Audience"],
                            expires: DateTime.Now.AddDays(1),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                        return new ResponseUnit<string?>() { message = "로그인 성공(유저).", data = accessToken, code = 200 };
                    }
                    else // 잘못된 요청입니다.
                    {
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
                else // 관리자
                {
                    // 위에만큼 담는데 (사업장 은 빼고)
                    AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);
                    
                    if (admintb is not null)
                    {
                        authClaims.Add(new Claim("UserIdx", usertb.Id.ToString())); // USER 인덱스
                        authClaims.Add(new Claim("Name", usertb.Name!.ToString())); // 이름
                        authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        authClaims.Add(new Claim("AlarmYN", usertb.AlarmYn!.ToString())); // 알람 받을지 여부
                        authClaims.Add(new Claim("AdminYN", usertb.AdminYn!.ToString())); // 관리자 여부
                        authClaims.Add(new Claim("UserType", "ADMIN"));
                        authClaims.Add(new Claim("AdminIdx", admintb.Id!.ToString())); // 관리자 인덱스
                        
                        if (admintb.Type == "시스템관리자")
                        {
                            authClaims.Add(new Claim("Role", "시스템관리자"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "SystemManager"));
                        }
                        if (admintb.Type == "마스터")
                        {
                            authClaims.Add(new Claim("Role", "마스터"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "Master"));
                        }
                        if (admintb.Type == "매니저")
                        {
                            authClaims.Add(new Claim("Role", "매니저"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "Manager"));
                        }

                        JObject items = new JObject();

                        // 메뉴 접근권한
                        items.Add("UserPerm_Basic", usertb.PermBasic);
                        items.Add("UserPerm_Machine", usertb.PermMachine);
                        items.Add("UserPerm_Elec", usertb.PermElec);
                        items.Add("UserPerm_Lift", usertb.PermLift);
                        items.Add("UserPerm_Fire", usertb.PermFire);
                        items.Add("UserPerm_Construct", usertb.PermConstruct);
                        items.Add("UserPerm_Network", usertb.PermNetwork);
                        items.Add("UserPerm_Beauty", usertb.PermBeauty);
                        items.Add("UserPerm_Security", usertb.PermSecurity);
                        items.Add("UserPerm_Material", usertb.PermMaterial);
                        items.Add("UserPerm_Energy", usertb.PermEnergy);
                        items.Add("UserPerm_User", usertb.PermUser);
                        items.Add("UserPerm_Voc", usertb.PermVoc);
                        jsonConvert = JsonConvert.SerializeObject(items);
                        authClaims.Add(new Claim("UserPerms", jsonConvert));

                        items = new JObject();
                        // VOC 권한
                        items.Add("VocMachine", usertb.VocMachine.ToString()); // 기계민원 처리권한
                        items.Add("VocElec", usertb.VocElec.ToString()); // 전기민원 처리권한
                        items.Add("VocLift", usertb.VocLift.ToString()); // 승강민원 처리권한
                        items.Add("VocFire", usertb.VocFire.ToString()); // 소방민원 처리권한
                        items.Add("VocConstruct", usertb.VocConstruct.ToString()); // 건축민원 처리권한
                        items.Add("VocNetwork", usertb.VocNetwork.ToString()); // 통신민원 처리권한
                        items.Add("VocBeauty", usertb.VocBeauty.ToString()); // 미화민원 처리권한
                        items.Add("VocSecurity", usertb.VocSecurity.ToString()); // 보안민원 처리권한
                        items.Add("VocDefault", usertb.VocEtc.ToString()); // 기타 처리권한
                        jsonConvert = JsonConvert.SerializeObject(items);
                        authClaims.Add(new Claim("VocPerms", jsonConvert));


                        // JWT 인증 페이로드 사인 비밀키
                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: Configuration["JWT:Issuer"],
                            audience: Configuration["JWT:Audience"],
                            expires: DateTime.Now.AddDays(1),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                        return new ResponseUnit<string?>() { message = "로그인 성공(관리자).", data = accessToken, code = 201 };
                    }
                    else
                    {
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
        

        /// <summary>
        /// 로그인한 사업장의 사용자 LIST 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<ListUser>> GetPlaceUserList(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };

               
                List<UsersTb>? model = await UserInfoRepository.GetPlaceUserList(Convert.ToInt32(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<ListUser>()
                    {
                        message = "요청이 정상 처리되었습니다",
                        data = model.Select(e => new ListUser()
                        {
                            Id = e.Id,
                            UserId = e.UserId,
                            Name = e.Name,
                            Type = e.Job,
                            Email = e.Email,
                            Phone = e.Phone,
                            Created = e.CreateDt.ToString(),
                            Status = e.Status
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<ListUser>() { message = "데이터가 존재하지 않습니다.", data = new List<ListUser>(), code = 200 };
                }
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ListUser>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<ListUser>(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 추가 서비스  0 : 퇴직 / 1 : 재직 / 2 :휴직
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UsersDTO>> AddUserService(HttpContext context, UsersDTO dto, IFormFile? files)
        {
            try
            {
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                if(files is not null)
                {
                    NewFileName = FileService.SetNewFileName(files);
                }

                if (context is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                // context에서 UserId가 --> context의 사업장에 있는지 검사 1.
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx));
                if (TokenChk is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                if (TokenChk.PermUser != 2)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                // 사용자 관련한 폴더 없으면 만들기
                PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Users", Common.FileServer, PlaceIdx.ToString());

                di = new DirectoryInfo(PlaceFileFolderPath);
                if (!di.Exists) di.Create();

                UsersTb model = new UsersTb();

                model.UserId = dto.USERID!; // 사용자아이디
                model.Password = dto.PASSWORD!; // 비밀번호
                model.Name = dto.NAME; // 이름
                model.Email = dto.EMAIL; // 이메일
                model.Phone = dto.PHONE; // 전화번호
                model.PermBasic = dto.PERM_BASIC; // 기본정보메뉴 권한
                model.PermMachine = dto.PERM_MACHINE; // 기계메뉴 권한
                model.PermElec = dto.PERM_ELEC; // 전기메뉴 권한
                model.PermLift = dto.PERM_LIFT; // 승강메뉴 권한
                model.PermFire = dto.PERM_FIRE; // 소방메뉴 권한
                model.PermConstruct = dto.PERM_CONSTRUCT; // 건축메뉴 권한
                model.PermNetwork = dto.PERM_NETWORK; // 통신메뉴 권한
                model.PermBeauty = dto.PERM_BEAUTY; // 미화메뉴 권한
                model.PermSecurity = dto.PERM_SECURITY; // 보안메뉴 권한
                model.PermMaterial = dto.PERM_MATERIAL; // 자재메뉴 권한
                model.PermEnergy = dto.PERM_ENERGY; // 에너지메뉴 권한
                model.PermUser = dto.PERM_USER; // 사용자메뉴 권한
                model.PermVoc = dto.PERM_VOC; // VOC메뉴 권한
                model.AdminYn = false; // 관리자 아님
                model.AlarmYn = dto.ALRAM_YN; // 알람 여부
                model.Status = 2; // 재직여부
                model.CreateDt = DateTime.Now;
                model.CreateUser = Creater; // 생성자
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = Creater; // 수정자
                model.Job = dto.JOB;
                model.VocMachine = dto.VOC_MACHINE; // VOC 기계권한
                model.VocElec = dto.VOC_ELEC; // VOC 전기권한
                model.VocLift = dto.VOC_LIFT; // VOC 승강권한
                model.VocFire = dto.VOC_FIRE; // VOC 소방권한
                model.VocConstruct = dto.VOC_CONSTRUCT; // VOC 건축권한
                model.VocNetwork = dto.VOC_NETWORK; // VOC 통신권한
                model.VocBeauty = dto.VOC_BEAUTY; // VOC 미화권한
                model.VocSecurity = dto.VOC_SECURITY; // VOC 보안권한
                model.VocEtc = dto.VOC_ETC; // VOC 기타권한
                model.PlaceTbId = Int32.Parse(PlaceIdx);
                
                if(files is not null)
                {
                    model.Image = NewFileName;
                }
                else
                {
                    model.Image = null;
                }

                bool? result = await UserInfoRepository.AddUserAsync(model);
                if (result == true)
                {
                    if(files is not null)
                    {
                        // 파일 넣기
                        bool? AddFile = await FileService.AddImageFile(NewFileName, PlaceFileFolderPath, files);
                    }

                    return new ResponseUnit<UsersDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else if(result == false)
                {
                    return new ResponseUnit<UsersDTO>() { message = "중복된 아이디입니다.", data = dto, code = 204 };
                }
                else
                {
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        public async ValueTask<ResponseUnit<UsersDTO>> GetUserDetails(HttpContext context, int id)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx));
                if (TokenChk is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                if (TokenChk.PermUser < 1)
                    return new ResponseUnit<UsersDTO>() { message = "접근 권한이 없습니다.", data = new UsersDTO(), code = 200 };

                UsersTb? model = await UserInfoRepository.GetUserIndexInfo(id);

                // 조회내용이 있으면 반환
                if (model is not null)
                {
                    UsersDTO dto = new UsersDTO();
                    dto.ID = model.Id;
                    dto.USERID = model.UserId;
                    dto.PASSWORD = model.Password;
                    dto.NAME = model.Name;
                    dto.EMAIL = model.Email;
                    dto.PHONE = model.Phone;
                    dto.PERM_BASIC = model.PermBasic;
                    dto.PERM_MACHINE = model.PermMachine;
                    dto.PERM_ELEC = model.PermElec;
                    dto.PERM_FIRE = model.PermFire;
                    dto.PERM_CONSTRUCT = model.PermConstruct;
                    dto.PERM_NETWORK = model.PermNetwork;
                    dto.PERM_BEAUTY = model.PermBeauty;
                    dto.PERM_SECURITY = model.PermSecurity;
                    dto.PERM_MATERIAL = model.PermMaterial;
                    dto.PERM_ENERGY = model.PermEnergy;
                    dto.PERM_USER = model.PermUser;
                    dto.PERM_VOC = model.PermVoc;
                    dto.ADMIN_YN = model.AdminYn;
                    dto.ALRAM_YN = model.AlarmYn;
                    dto.STATUS = model.Status;
                    dto.JOB = model.Job;
                    dto.VOC_MACHINE = model.VocMachine;
                    dto.VOC_ELEC = model.VocElec;
                    dto.VOC_LIFT = model.VocLift;
                    dto.VOC_FIRE = model.VocFire;
                    dto.VOC_CONSTRUCT = model.VocConstruct;
                    dto.VOC_NETWORK = model.VocNetwork;
                    dto.VOC_BEAUTY = model.VocBeauty;
                    dto.VOC_SECURITY = model.VocSecurity;
                    dto.VOC_ETC = model.VocEtc;

                    string PlaceFileName = String.Format(@"{0}\\{1}\\Users", Common.FileServer, placeid.ToString());
                    if(!String.IsNullOrWhiteSpace(model.Image))
                    {
                        dto.Image = await FileService.GetImageFile(PlaceFileName, model.Image);
                    }

                    return new ResponseUnit<UsersDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = dto,
                        code = 200
                    };
                }
                else
                {
                    return new ResponseUnit<UsersDTO>() { message = "데이터가 존재하지 않습니다.", data = new UsersDTO(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UsersDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 데이터 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteUserService(HttpContext context, List<int> del)
        {
            try
            {

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await UserInfoRepository.DeleteUserInfo(del, creater);
                
                if (DeleteResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if (DeleteResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UsersDTO>> UpdateUserService(HttpContext context, UsersDTO dto, IFormFile? files)
        {
            try
            {
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                if(files is not null)
                {
                    NewFileName = FileService.SetNewFileName(files);
                }

                if (context is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
                
                if(dto is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
                
                string? Name = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Name))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? model = await UserInfoRepository.GetUserIndexInfo(dto.ID!.Value);
                if(model is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                model.UserId = dto.USERID!;
                model.Password = dto.PASSWORD!;
                model.Name = dto.NAME;
                model.Email = dto.EMAIL;
                model.Phone = dto.PHONE;
                model.Job = dto.JOB;
                /* 메뉴권한 */
                model.PermBasic = dto.PERM_BASIC;
                model.PermMachine = dto.PERM_MACHINE;
                model.PermElec = dto.PERM_ELEC;
                model.PermLift = dto.PERM_LIFT;
                model.PermFire = dto.PERM_FIRE;
                model.PermConstruct = dto.PERM_CONSTRUCT;
                model.PermNetwork = dto.PERM_NETWORK;
                model.PermBeauty = dto.PERM_BEAUTY;
                model.PermSecurity = dto.PERM_SECURITY;
                model.PermMaterial = dto.PERM_MATERIAL;
                model.PermEnergy = dto.PERM_ENERGY;
                model.PermUser = dto.PERM_USER;
                model.PermVoc = dto.PERM_VOC;
                /* VOC권한 */
                model.VocMachine = dto.VOC_MACHINE;
                model.VocElec = dto.VOC_ELEC;
                model.VocLift = dto.VOC_LIFT;
                model.VocFire = dto.VOC_FIRE;
                model.VocConstruct = dto.VOC_CONSTRUCT;
                model.VocNetwork = dto.VOC_NETWORK;
                model.VocBeauty = dto.VOC_BEAUTY;
                model.VocSecurity = dto.VOC_SECURITY;
                model.VocEtc = dto.VOC_ETC;
                model.AlarmYn = dto.ALRAM_YN;
                model.Status = dto.STATUS;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = Name;
                    
                PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Users", Common.FileServer, placeid.ToString());
                    
                if(files is not null) // 파일이 공백이 아닌경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있을 경우
                    {
                        deleteFileName = model.Image; // 삭제할 이름을 넣는다.
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                    else // DB엔 없는경우
                    {
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                }
                else // 파일이 공백인경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                    {
                        deleteFileName = model.Image; // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image = null; // 모델의 파일명을 비운다.
                    }
                }

                UsersTb? updatemodel = await UserInfoRepository.UpdateUserInfo(model);
                if (updatemodel is not null)
                {
                    if(files is not null) // 파일이 공백이 아닌경우
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            // 파일 넣기
                            bool? AddFile = await FileService.AddImageFile(NewFileName, PlaceFileFolderPath, files);
                        }
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 파일 삭제
                            bool DeleteFile = FileService.DeleteImageFile(PlaceFileFolderPath, deleteFileName);
                        }
                    }
                    else // 파일이 공백인 경우
                    {
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 삭제할거
                            bool DeleteFile = FileService.DeleteImageFile(PlaceFileFolderPath, deleteFileName);
                        }
                    }

                    return new ResponseUnit<UsersDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<UsersDTO>() { message = "요청이 처리되지 않았습니다.", data = dto, code = 200 };
                }
              
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UsersDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> ImportUserService(HttpContext context, IFormFile? file)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (file is null)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (file.Length == 0)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<ExcelUserInfo> userlist = new List<ExcelUserInfo>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int total = worksheet.LastRowUsed().RowNumber(); // Row 개수 반환
                       

                        for (int i = 2; i <= total; i++)
                        {
                            var Data = new ExcelUserInfo();

                            Data.ID = Convert.ToString(worksheet.Cell("A" + i).GetValue<string>().Trim());
                            
                            if(String.IsNullOrWhiteSpace(Data.ID))// 공백 또는 Null이면 대체값이라도 들어가게
                            {
                                Data.ID = Guid.NewGuid().ToString();
                            }

                            Data.PassWord = Convert.ToString(worksheet.Cell("B" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.PassWord)) // 공백 또는 Null이면 대체값이라도 들어가게
                            {
                                Data.PassWord = Guid.NewGuid().ToString();
                            }

                            Data.Name = Convert.ToString(worksheet.Cell("C" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Name)) // 공백 또는 Null이면 대체값이라도 들어가게
                            {
                                Data.Name = Guid.NewGuid().ToString();
                            }

                            Data.Email = Convert.ToString(worksheet.Cell("D" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Email))
                            {
                                Data.Email = Guid.NewGuid().ToString();
                            }

                            Data.PhoneNumber = Convert.ToString(worksheet.Cell("E" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.PhoneNumber))
                            {
                                Data.PhoneNumber = Guid.NewGuid().ToString();
                            }

                            userlist.Add(Data);
                        }

                        var duple = userlist.GroupBy(x => x.ID).Where(p => p.Count() > 1).ToList();
                        if (duple.Count() > 0) // 엑셀에 중복된 데이터를 기입했는지 검사
                        {
                            return new ResponseUnit<string?>
                            {
                                message = $"시트에 {duple.Count}개의 중복이 있습니다. 중복 제거후 다시 시도하세요.",
                                data = duple.Count.ToString(),
                                code = 200
                            };
                        }
                        else
                        {
                            // DB에 중복된 ID가 있는지 검사
                            List<UsersTb>? placeusers = await UserInfoRepository.GetAllUserList();

                            if (placeusers is not null && placeusers.Count() > 0)
                            {
                                var result = placeusers.IntersectBy(userlist.Select(x => x.ID), x => x.UserId);

                                if (result.Count() > 0)
                                {
                                    // DB에 중복된 데이터가 하나 이상 있음. -- 해결하고 넣어야함.
                                    return new ResponseUnit<string?>() { message = $"이미 사용중인 아이디가 {result.Count()}개 있습니다.", data = result.Count().ToString(), code = 200 };
                                }
                                else
                                {
                                    // 중복된 데이터가 없음
                                    // 모델 클래스로 변환해서 디비에 넣어야함.
                                    List<UsersTb> model = userlist.Select(m => new UsersTb
                                    {
                                        UserId = m.ID,
                                        Password = m.PassWord,
                                        Phone = m.PhoneNumber,
                                        Email = m.Email,
                                        Name = m.Name,
                                        PermBasic = 2, // 기본정보관리 필수사용
                                        PermMachine = 0,
                                        PermElec = 0,
                                        PermLift = 0,
                                        PermFire = 0,
                                        PermConstruct = 0,
                                        PermNetwork = 0,
                                        PermBeauty = 0,
                                        PermSecurity = 0,
                                        PermMaterial = 0,
                                        PermEnergy = 0,
                                        PermUser = 2, // 사용자관리 필수사용
                                        PermVoc = 0,
                                        VocMachine = false,
                                        VocElec = false,
                                        VocLift = false,
                                        VocFire = false,
                                        VocConstruct = false,
                                        VocNetwork = false,
                                        VocBeauty = false,
                                        VocSecurity = false,
                                        VocEtc = false,
                                        Status = 2,
                                        CreateDt = DateTime.Now,
                                        CreateUser = creater,
                                        UpdateDt = DateTime.Now,
                                        UpdateUser = creater,
                                        Job = null,
                                        PlaceTbId = Int32.Parse(placeidx)
                                    }).ToList();

                                    for (int i = 0; i < model.Count; i++)
                                    {
                                        UsersTb? insert = await UserInfoRepository.AddAsync(model[i]);

                                        if (insert is null)
                                        {
                                            return new ResponseUnit<string?>() { message = $"데이터를 처리하지 못하였습니다.", data = insert.UserId, code = 201 };
                                        }
                                    }

                                    return new ResponseUnit<string?>
                                    {
                                        message = "요청이 정상 처리되었습니다.",
                                        data = $"{total - 1}",
                                        code = 200
                                    };
                                }
                            }
                            else // USERTB에 데이터가 아무것도 없을때
                            {
                                // 모델 클래스로 변환해서 디비에 넣어야함.
                                List<UsersTb> model = userlist.Select(m => new UsersTb
                                {
                                    UserId = m.ID!,
                                    Password = m.PassWord!,
                                    Phone = m.PhoneNumber,
                                    Email = m.Email,
                                    Name = m.Name,
                                    PermBasic = 2, // 기본정보관리 필수사용
                                    PermMachine = 0,
                                    PermElec = 0,
                                    PermLift = 0,
                                    PermFire = 0,
                                    PermConstruct = 0,
                                    PermNetwork = 0,
                                    PermBeauty = 0,
                                    PermSecurity = 0,
                                    PermMaterial = 0,
                                    PermEnergy = 0,
                                    PermUser = 2, // 사용자관리 필수사용
                                    PermVoc = 0,
                                    VocMachine = false,
                                    VocElec = false,
                                    VocLift = false,
                                    VocFire = false,
                                    VocConstruct = false,
                                    VocNetwork = false,
                                    VocBeauty = false,
                                    VocSecurity = false,
                                    VocEtc = false,
                                    Status = 2,
                                    CreateDt = DateTime.Now,
                                    CreateUser = creater,
                                    UpdateDt = DateTime.Now,
                                    UpdateUser = creater,
                                    Job = null,
                                    PlaceTbId = Int32.Parse(placeidx)
                                }).ToList();

                                for (int i = 0; i < model.Count; i++)
                                {
                                    UsersTb? insert = await UserInfoRepository.AddAsync(model[i]);

                                    if (insert is null)
                                    {
                                        return new ResponseUnit<string?>() { message = $"데이터를 처리하지 못하였습니다.", data = insert.UserId, code = 201 };
                                    }
                                }

                                return new ResponseUnit<string?>
                                {
                                    message = "요청이 정상 처리되었습니다.",
                                    data = $"{total - 1}",
                                    code = 200
                                };
                            }


                        }
                    }

                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

  
    }
}
