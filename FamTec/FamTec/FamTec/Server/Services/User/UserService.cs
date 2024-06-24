using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Login;
using FamTec.Shared.Server.DTO.User;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
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

        private readonly IConfiguration Configuration;
        private ILogService LogService;
       

        public UserService(IUserInfoRepository _userinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforpeository,
            IConfiguration _configuration,
            ILogService _logservice)
        {
            this.UserInfoRepository = _userinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforpeository;

            this.Configuration = _configuration;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<string>?> LoginSelectPlaceService(HttpContext context, int? placeid)
        {
            List<Claim> authClaims = new List<Claim>();
            string? jsonConvert = String.Empty;

            try
            {
                string? adminidx = Convert.ToString(context.Items["AdminIdx"]);
                if(adminidx is null)
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                if(placeid is null)
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };


                List<AdminPlaceTb>? adminplace = await AdminPlaceInfoRepository.GetMyWorksList(Convert.ToInt32(adminidx));
                if(adminplace is [_, ..])
                {
                    AdminPlaceTb? select = adminplace.FirstOrDefault(m => m.PlaceId == placeid);
                    if (select is not null)
                    {
                        PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(placeid);
                        if(placetb is not null)
                        {
                            // 토큰반환 - 다시만들어야함.
                            authClaims.Add(new Claim("UserIdx", context.Items["UserIdx"].ToString())); // USER 인덱스
                            authClaims.Add(new Claim("Name", context.Items["Name"].ToString())); // 이름
                            authClaims.Add(new Claim("jti", context.Items["jti"].ToString()));
                            authClaims.Add(new Claim("AlarmYN", context.Items["AlarmYN"].ToString())); // 알람 받을지 여부
                            authClaims.Add(new Claim("AdminYN", context.Items["AdminYN"].ToString())); // 관리자 여부
                            authClaims.Add(new Claim("UserType", context.Items["UserType"].ToString()));
                            authClaims.Add(new Claim("AdminIdx", context.Items["AdminIdx"].ToString())); // 관리자 인덱스

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
                            items.Add("PlaceIdx", placetb.Id.ToString());
                            items.Add("PlaceName", placetb.Name.ToString());
                            items.Add("PlacePerm_Machine", placetb.PermMachine.ToString());
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
                            return new ResponseUnit<string>() { message = "로그인 성공(관리자).", data = accessToken, code = 200 };
                        }
                        else
                        {
                            return new ResponseUnit<string>() { message = "사업장이 존재하지 않습니다.", data = null, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<string>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<string>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseUnit<string>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 일반페이지 유저 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string>?> UserLoginService(LoginDTO? dto)
        {
            List<Claim> authClaims = new List<Claim>();
            string? jsonConvert = String.Empty;

            try
            {
                if (dto?.UserID is null)
                    return new ResponseUnit<string>() { message = "아이디를 입력해주세요.", data = null, code = 200 };
                if(dto?.UserPassword is null)
                    return new ResponseUnit<string>() { message = "비밀번호를 입력해주세요.", data = null, code = 200 };

                if (!String.IsNullOrWhiteSpace(dto?.UserID) && !String.IsNullOrWhiteSpace(dto?.UserPassword))
                {
                    UserTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID, dto.UserPassword);
                    
                    if(usertb is not null)
                    {
                        int? AdminYN = usertb.AdminYn;
                        if(AdminYN == 0) // 일반유저
                        {
                            PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(usertb.PlaceTbId);
                            
                            if(placetb is not null)
                            {
                                authClaims.Add(new Claim("UserIdx", usertb.Id.ToString())); // + USERID
                                authClaims.Add(new Claim("Name", usertb.Name!.ToString())); // + USERID
                                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                                authClaims.Add(new Claim("AlarmYN", usertb.AlramYn!.ToString())); // 알람 받을지 여부
                                authClaims.Add(new Claim("AdminYN", usertb.AdminYn!.ToString())); // 관리자 여부
                                authClaims.Add(new Claim("UserType", "User"));
                                authClaims.Add(new Claim("Role", "User"));
                                authClaims.Add(new Claim(ClaimTypes.Role, "User"));

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
                                items.Add("VocDefault", usertb.VocDefault.ToString()); // 기타 처리권한
                                jsonConvert = JsonConvert.SerializeObject(items);
                                authClaims.Add(new Claim("VocPerms", jsonConvert));

                                /* 사업장 권한 */
                                items.Add("PlaceIdx", usertb.PlaceTbId.ToString()); // 사업장 인덱스
                                items.Add("PlaceName", placetb.Name.ToString()); // 사업장 이름
                                items.Add("PlacePerm_Machine", placetb.PermMachine.ToString()); // 사업장 기계메뉴 권한
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
                                return new ResponseUnit<string>() { message = "로그인 성공(유저).", data = accessToken, code = 200 };
                            }
                            else // 잘못된 요청입니다.
                            {
                                return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                            }
                        }
                        else // 관리자
                        {
                            // 위에만큼 담는데 (사업장 은 빼고)
                            // Return 201
                            AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);

                            if (admintb is not null)
                            {
                                authClaims.Add(new Claim("UserIdx", usertb.Id.ToString())); // USER 인덱스
                                authClaims.Add(new Claim("Name", usertb.Name!.ToString())); // 이름
                                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                                authClaims.Add(new Claim("AlarmYN", usertb.AlramYn!.ToString())); // 알람 받을지 여부
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
                                items.Add("VocDefault", usertb.VocDefault.ToString()); // 기타 처리권한
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
                                return new ResponseUnit<string>() { message = "로그인 성공(관리자).", data = accessToken, code = 201 };
                            }
                            else
                            {
                                return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                            }
                        }
                    }
                    else
                    {
                        return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                return new ResponseUnit<string>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
        

        /// <summary>
        /// 로그인한 사업장의 사용자 LIST 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<ListUser>> GetPlaceUserList(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };

                
                int? placeidx = Int32.Parse(context.Items["PlaceIdx"].ToString());

                if (placeidx is not null)
                {
                    List<UserTb>? model = await UserInfoRepository.GetPlaceUserList(placeidx);

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
                else
                {
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<ListUser>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<ListUser>(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UsersDTO>> AddUserService(HttpContext? context, UsersDTO? dto)
        {
            if (context is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
            if (dto is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            string? Creater = Convert.ToString(context.Items["Name"]);
            if (String.IsNullOrWhiteSpace(Creater))
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            string? CreaterPlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
            if (String.IsNullOrWhiteSpace(CreaterPlaceIdx))
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            // context에서 UserId가 --> context의 사업장에 있는지 검사 1.
            string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
            if(String.IsNullOrWhiteSpace(UserIdx))
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            UserTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx));
            if(TokenChk is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            if (TokenChk.PermUser != 2)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            UserTb model = new UserTb()
            {
                UserId = dto.USERID, // 사용자아이디
                Password = dto.PASSWORD, // 비밀번호
                Name = dto.NAME, // 이름
                Email = dto.EMAIL, // 이메일
                Phone = dto.PHONE, // 전화번호
                PermBasic = dto.PERM_BASIC, // 기본정보메뉴 권한
                PermMachine = dto.PERM_MACHINE, // 기계메뉴 권한
                PermElec = dto.PERM_ELEC, // 전기메뉴 권한
                PermLift = dto.PERM_LIFT, // 승강메뉴 권한
                PermFire = dto.PERM_FIRE, // 소방메뉴 권한
                PermConstruct = dto.PERM_CONSTRUCT, // 건축메뉴 권한
                PermNetwork = dto.PERM_NETWORK, // 통신메뉴 권한
                PermBeauty = dto.PERM_BEAUTY, // 미화메뉴 권한
                PermSecurity = dto.PERM_SECURITY, // 보안메뉴 권한
                PermMaterial = dto.PERM_MATERIAL, // 자재메뉴 권한
                PermEnergy = dto.PERM_ENERGY, // 에너지메뉴 권한
                PermUser = dto.PERM_USER, // 사용자메뉴 권한
                PermVoc = dto.PERM_VOC, // VOC메뉴 권한
                AdminYn = 0, // 관리자 아님
                AlramYn = dto.ALRAM_YN, // 알람 여부
                Status = 1, // 재직여부
                CreateDt = DateTime.Now,
                CreateUser = Creater, // 생성자
                UpdateDt = DateTime.Now,
                UpdateUser = Creater, // 수정자
                Job = dto.JOB,
                VocMachine = dto.VOC_MACHINE, // VOC 기계권한
                VocElec = dto.VOC_ELEC, // VOC 전기권한
                VocLift = dto.VOC_LIFT, // VOC 승강권한
                VocFire = dto.VOC_FIRE, // VOC 소방권한
                VocConstruct = dto.VOC_CONSTRUCT, // VOC 건축권한
                VocNetwork = dto.VOC_NETWORK, // VOC 통신권한
                VocBeauty = dto.VOC_BEAUTY, // VOC 미화권한
                VocSecurity = dto.VOC_SECURITY, // VOC 보안권한
                VocDefault = dto.VOC_DEFAULT, // VOC 기타권한
                PlaceTbId = Int32.Parse(CreaterPlaceIdx)
            };

            UserTb? result = await UserInfoRepository.AddAsync(model);
            if (result is not null)
            {
                return new ResponseUnit<UsersDTO>()
                {
                    message = "요청이 정상 처리되었습니다.",
                    data = new UsersDTO()
                    {
                        ID = result.Id,
                        USERID = result.UserId,
                        PASSWORD = result.Password,
                        NAME = result.Name,
                        EMAIL = result.Email,
                        PHONE = result.Phone,
                        PERM_BASIC = result.PermBasic,
                        PERM_MACHINE = result.PermMachine,
                        PERM_ELEC = result.PermElec,
                        PERM_LIFT = result.PermLift,
                        PERM_FIRE = result.PermFire,
                        PERM_CONSTRUCT = result.PermConstruct,
                        PERM_NETWORK = result.PermNetwork,
                        PERM_BEAUTY = result.PermBeauty,
                        PERM_SECURITY = result.PermSecurity,
                        PERM_MATERIAL = result.PermMaterial,
                        PERM_ENERGY = result.PermEnergy,
                        PERM_USER = result.PermUser,
                        PERM_VOC = result.PermVoc,
                        ADMIN_YN = result.AdminYn,
                        ALRAM_YN = result.AlramYn,
                        STATUS = result.Status,
                        JOB = result.Job,
                        VOC_MACHINE = result.VocMachine,
                        VOC_ELEC = result.VocElec,
                        VOC_LIFT = result.VocLift,
                        VOC_FIRE = result.VocFire,
                        VOC_CONSTRUCT = result.VocConstruct,
                        VOC_NETWORK = result.VocNetwork,
                        VOC_BEAUTY = result.VocBeauty,
                        VOC_SECURITY = result.VocSecurity,
                        VOC_DEFAULT = result.VocDefault
                    },
                    code = 200
                };
            }
            else
            {
                return new ResponseUnit<UsersDTO>()
                {
                    message = "잘못된 요청입니다.",
                    data = new UsersDTO(),
                    code = 404
                };
            }
        }


        public async ValueTask<ResponseUnit<UsersDTO>> GetUserDetails(HttpContext? context, int? id)
        {
            if (context is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
            if (id is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
            if (String.IsNullOrWhiteSpace(UserIdx))
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            UserTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx));
            if (TokenChk is null)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            if (TokenChk.PermUser != 2)
                return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

            UserTb? model = await UserInfoRepository.GetUserIndexInfo(id);
            
            // 조회내용이 있으면 반환
            if(model is not null)
            {
                return new ResponseUnit<UsersDTO>() 
                {
                    message = "요청이 정상 처리되었습니다.",
                    data = new UsersDTO()
                    {
                        ID = model.Id,
                        USERID = model.UserId,
                        PASSWORD = model.Password,
                        NAME = model.Name,
                        EMAIL = model.Email,
                        PHONE = model.Phone,
                        PERM_BASIC = model.PermBasic,
                        PERM_MACHINE = model.PermMachine,
                        PERM_ELEC = model.PermElec,
                        PERM_LIFT = model.PermLift,
                        PERM_FIRE = model.PermFire,
                        PERM_CONSTRUCT = model.PermConstruct,
                        PERM_NETWORK = model.PermNetwork,
                        PERM_BEAUTY = model.PermBeauty,
                        PERM_SECURITY = model.PermSecurity,
                        PERM_MATERIAL = model.PermMaterial,
                        PERM_ENERGY = model.PermEnergy,
                        PERM_USER = model.PermUser,
                        PERM_VOC = model.PermVoc,
                        ADMIN_YN = model.AdminYn,
                        ALRAM_YN = model.AlramYn,
                        STATUS = model.Status,
                        JOB = model.Job,
                        VOC_MACHINE = model.VocMachine,
                        VOC_ELEC = model.VocElec,
                        VOC_LIFT = model.VocLift,
                        VOC_FIRE = model.VocFire,
                        VOC_CONSTRUCT = model.VocConstruct,
                        VOC_NETWORK = model.VocNetwork,
                        VOC_BEAUTY = model.VocBeauty,
                        VOC_SECURITY = model.VocSecurity,
                        VOC_DEFAULT = model.VocDefault
                    },
                    code = 200 
                };
            }
            else
            {
                return new ResponseUnit<UsersDTO>() { message = "데이터가 존재하지 않습니다.", data = new UsersDTO(), code = 200 };
            }
        }

        /// <summary>
        /// 사용자 데이터 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> DeleteUserService(HttpContext? context, List<int>? del)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (del is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (del.Count == 0)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? Name = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Name))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                int? model = await UserInfoRepository.DeleteUserList(del, Name);
                if (model > 0)
                {
                    return new ResponseUnit<bool>() { message = $"요청이 {model}건 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
            }catch(Exception ex)
            {
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 404 };
            }
        }

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UpdateUserDTO>> UpdateUserService(HttpContext? context, UpdateUserDTO? dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<UpdateUserDTO>() { message = "잘못된 요청입니다.", data = new UpdateUserDTO(), code = 404 };
                if(dto is null)
                    return new ResponseUnit<UpdateUserDTO>() { message = "잘못된 요청입니다.", data = new UpdateUserDTO(), code = 404 };
                
                string? Name = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Name))
                    return new ResponseUnit<UpdateUserDTO>() { message = "잘못된 요청입니다.", data = new UpdateUserDTO(), code = 404 };

                UserTb? model = await UserInfoRepository.GetUserIndexInfo(dto.ID);
                if (model is not null)
                {
                    model.UserId = dto.USERID;
                    model.Password = dto.PASSWORD;
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
                    model.VocDefault = dto.VOC_DEFAULT;
                    model.AlramYn = dto.ALRAM_YN;
                    model.Status = dto.STATUS;

                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = Name;

                    UserTb? updatemodel = await UserInfoRepository.UpdateUserInfo(model);
                    if (updatemodel is not null)
                    {
                        return new ResponseUnit<UpdateUserDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<UpdateUserDTO>() { message = "요청이 처리되지 않았습니다.", data = dto, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<UpdateUserDTO>() { message = "잘못된 요청입니다.", data = new UpdateUserDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseUnit<UpdateUserDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateUserDTO(), code = 404 };
            }
        }
    }
}
