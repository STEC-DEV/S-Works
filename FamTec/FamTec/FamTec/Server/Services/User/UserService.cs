using ClosedXML.Excel;
using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Services.Redis;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Excel;
using FamTec.Shared.Server.DTO.Login;
using FamTec.Shared.Server.DTO.Place;
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
        private readonly IDepartmentInfoRepository DepartmentInfoRepository;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly IRedisService RedisService; /* Redis 서비스 */
        private readonly IConfiguration Configuration; /* AppSettings.json */
        private readonly IWebHostEnvironment WebHostEnvironment; /* 호스팅 정보 */
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<UserService> CreateBuilderLogger; /* 콘솔로그 */
        DirectoryInfo? di; /* 디렉터리 객체 */
        string? PlaceFileFolderPath = String.Empty; /* 디렉터리 경로 */

        public UserService(IUserInfoRepository _userinforepository,
            IAdminUserInfoRepository _adminuserinforepository,
            IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforpeository,
            IDepartmentInfoRepository _departmentinforepository,
            IFileService _fileservice,
            IRedisService _redisservice,
            IConfiguration _configuration,
            IWebHostEnvironment _webhostenvironment,
            IHttpContextAccessor _httpcontextAccessor,
            ILogService _logservice,
            ConsoleLogService<UserService> _createbuilderlogger)
        {
            this.UserInfoRepository = _userinforepository;
            this.AdminUserInfoRepository = _adminuserinforepository;
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforpeository;
            this.DepartmentInfoRepository = _departmentinforepository;
            this.FileService = _fileservice;
            this.RedisService = _redisservice;
            this.Configuration = _configuration;
            this.WebHostEnvironment = _webhostenvironment;
            this.HttpContextAccessor = _httpcontextAccessor;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 일반화면 가이드 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadUserGuidForm()
        {
            try
            {
                string? filePath = Path.Combine(WebHostEnvironment.ContentRootPath, "GuideForm", "S-Works_사용자설명서_KO_SWS.pdf");
                if (String.IsNullOrWhiteSpace(filePath))
                    return null;

                byte[]? filesBytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
                if (filesBytes is not null)
                    return filesBytes;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 사용자 엑셀 양식다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadUserForm()
        {
            try
            {
                string? filePath = Path.Combine(WebHostEnvironment.ContentRootPath, "ExcelForm", "사용자정보(양식).xlsx");
                if (String.IsNullOrWhiteSpace(filePath))
                    return null;

                byte[]? fileBytes = await System.IO.File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
                if (fileBytes is not null)
                    return fileBytes;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 사용자 엑셀 IMPORT - USERID는 중복불가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> ImportUserService(IFormFile? file)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 400 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                List<ExcelUserInfo> userlist = new List<ExcelUserInfo>();

                using (var stream = new MemoryStream())
                {
                    await file!.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int total = worksheet.LastRowUsed().RowNumber(); // Row 개수 반환

                        if (worksheet.Cell("A2").GetValue<string>().Trim() != "*아이디")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("B2").GetValue<string>().Trim() != "이름")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("C2").GetValue<string>().Trim() != "이메일")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("D2").GetValue<string>().Trim() != "전화번호")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("E2").GetValue<string>().Trim() != "직책")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };

                        for (int i = 3; i <= total; i++)
                        {
                            var Data = new ExcelUserInfo();

                            Data.UserID = Convert.ToString(worksheet.Cell("A" + i).GetValue<string>().Trim());

                            if (String.IsNullOrWhiteSpace(Data.UserID))
                            {
                                return new ResponseUnit<bool>() { message = "시트의 아이디는 공백이 될 수 없습니다.", data = false, code = 204 };
                                //Data.UserID = Guid.NewGuid().ToString(); // 공백 또는 Null이면 대체값이라도 들어가게
                            }

                            
                            Data.Name = Convert.ToString(worksheet.Cell("B" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Name)) 
                            {
                                Data.Name = null;
                                //return new ResponseUnit<bool>() { message = "시트의 이름은 공백이 될 수 없습니다.", data = false, code = 204 };
                                // Data.PassWord = Guid.NewGuid().ToString(); // 공백 또는 Null이면 대체값이라도 들어가게
                            }

                            Data.Email = Convert.ToString(worksheet.Cell("C" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Email)) 
                            {
                                Data.Email = null;
                                //return new ResponseUnit<bool>() { message = "시트의 이메일은 공백이 될 수 없습니다.", data = false, code = 204 };
                                //Data.Name = Guid.NewGuid().ToString(); // 공백 또는 Null이면 대체값이라도 들어가게
                            }

                            Data.PhoneNumber = Convert.ToString(worksheet.Cell("D" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.PhoneNumber))
                            {
                                Data.PhoneNumber = null;
                                //return new ResponseUnit<bool>() { message = "시트의 전화번호는 공백이 될 수 없습니다.", data = false, code = 204 };
                                //Data.Email = Guid.NewGuid().ToString(); // 공백 또는 Null이면 대체값이라도 들어가게
                            }

                            Data.Job = Convert.ToString(worksheet.Cell("E" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Job))
                            {
                                Data.Job = null;
                                //return new ResponseUnit<bool>() { message = "시트의 직책은 공백이 될 수 없습니다.", data = false, code = 204 };
                                //Data.PhoneNumber = Guid.NewGuid().ToString(); // 공백 또는 Null이면 대체값이라도 들어가게
                            }
                            userlist.Add(Data);
                        }

                        if (userlist is not [_, ..])
                            return new ResponseUnit<bool>() { message = "등록할 사용자 정보가 없습니다.", data = false, code = 204 };

                        // 엑셀에 중복된 데이터를 기입했는지 검사
                        var excelCheck = userlist.GroupBy(x => x.UserID).Where(p => p.Count() > 1).ToList();
                        if (excelCheck.Count() > 0)
                        {
                            return new ResponseUnit<bool>
                            {
                                message = $"사용자 ID는 중복이 될 수 없습니다. {excelCheck.Count}개의 중복이 있습니다. 중복 제거후 다시 시도하세요.",
                                data = false,
                                code = 204
                            };
                        }

                        // DB에서 사용자데이터 전체조회 - 공백일시 DB 중복검사 안함.
                        List<UsersTb>? placeusers = await UserInfoRepository.GetAllUserList().ConfigureAwait(false);
                        if (placeusers is [_, ..])
                        {
                            // 엑셀에 중복된 데이터를 기입했는지 검사
                            List<UsersTb> dbCheck = placeusers.IntersectBy
                                (userlist.Select(x => x.UserID), x => x.UserId).ToList();

                            if (dbCheck.Count() > 0)
                            {
                                // DB에 중복된 데이터가 하나 이상 있음.
                                return new ResponseUnit<bool>() { message = $"이미 사용중인 아이디가 {dbCheck.Count()}개 있습니다 중복 제거후 다시 시도하세요.", data = false, code = 200 };
                            }
                        }

                        // DB에 넣을 데이터 생성
                        List<UsersTb> model = userlist.Select(m => new UsersTb
                        {
                            UserId = m.UserID!, // 엑셀에 입력받은 사용자 ID
                            Password = m.UserID!, // 엑셀에 입력받은 사용자 비밀번호
                            Name = m.Name, // 엑셀에 입력받은 사용자 이름
                            Email = m.Email, // 엑셀에 입력받은 사용자 이메일
                            Phone = m.PhoneNumber, // 엑셀에 입력받은 사용자 전화번호
                            PermBasic = 2, // 기본정보관리 메뉴 권한 (필수사용)
                            PermMachine = 0, // 기계관리 메뉴 권한
                            PermElec = 0, // 전기관리 메뉴 권한
                            PermLift = 0, // 승강관리 메뉴 권한
                            PermFire = 0, // 소방관리 메뉴 권한
                            PermConstruct = 0, // 건축관리 메뉴 권한
                            PermNetwork = 0, // 통신관리 메뉴 권한
                            PermBeauty = 0, // 미화관리 메뉴 권한
                            PermSecurity = 0, // 보안 메뉴 권한
                            PermMaterial = 0, // 자재관리 메뉴 권한
                            PermEnergy = 0, // 에너지관리 메뉴 권한
                            PermUser = 0, // 사용자관리 메뉴 권한
                            PermVoc = 0, // VOC관리 메뉴 권한
                            VocMachine = false, // 기계 VOC 권한
                            VocElec = false, // 전기 VOC 권한
                            VocLift = false, // 승강 VOC 권한
                            VocFire = false, // 소방 VOC 권한
                            VocConstruct = false, // 건축 VOC 권한
                            VocNetwork = false, // 통신 VOC 권한
                            VocBeauty = false, // 미화 VOC 권한
                            VocSecurity = false, // 보안 VOC 권한
                            VocEtc = false, // 기타 VOC 권한
                            AdminYn = false, // 관리자 여부
                            AlarmYn = false, // 알람여부
                            Status = 2, // 재직여부 (재직)
                            CreateDt = ThisDate, // 생성일자
                            CreateUser = creater, // 생성자
                            UpdateDt = ThisDate, // 수정일자
                            UpdateUser = creater, // 수정자
                            Job = null,
                            PlaceTbId = Int32.Parse(placeidx)
                        }).ToList();

                        bool? AddResult = await UserInfoRepository.AddUserList(model).ConfigureAwait(false);

                        return AddResult switch
                        {
                            true => new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                            false => new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                            _ => new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }


        public async Task<ResponseUnit<string?>> GetQRLogin(QRLoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserId) || String.IsNullOrWhiteSpace(dto.UserPassword))
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                if(dto.placeid is 0)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                UsersTb? UserTB = await UserInfoRepository.GetUserInfo(dto.UserId, dto.UserPassword).ConfigureAwait(false);
                if (UserTB is null)
                    return new ResponseUnit<string?>() { message = "존재하지 않는 사용자입니다.", data = null, code = 204 };

                PlaceTb? PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(dto.placeid).ConfigureAwait(false);
                if (PlaceTB is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (PlaceTB.Status == false)
                    return new ResponseUnit<string?>() { message = "해약된 사업장은 접근이 불가능합니다.", data = null, code = 201 };



                if (!UserTB.AdminYn)
                {
                    // 일반사용자
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // USERID
                        new Claim("Name", UserTB.Name!.ToString()), // USERNAME
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", UserTB.AlarmYn!.ToString()), // 알람 받을지 여부
                        new Claim("AdminYN", UserTB.AdminYn!.ToString()), // 관리자 여부
                        new Claim("UserType", "User"),
                        new Claim("Role", "User"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("PlaceIdx", PlaceTB.Id!.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name!.ToString()), // 사업장 명칭
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd"))
                    };

                    /* 메뉴 접근권한 */
                    /* 메뉴 접근권한 */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic},
                        { "UserPerm_Machine", UserTB.PermMachine},
                        { "UserPerm_Elec", UserTB.PermElec},
                        { "UserPerm_Lift",UserTB.PermLift},
                        { "UserPerm_Fire",UserTB.PermFire},
                        { "UserPerm_Construct",UserTB.PermConstruct},
                        { "UserPerm_Network",UserTB.PermNetwork },
                        { "UserPerm_Beauty",UserTB.PermBeauty},
                        { "UserPerm_Security", UserTB.PermSecurity},
                        { "UserPerm_Material", UserTB.PermMaterial},
                        { "UserPerm_Energy", UserTB.PermEnergy},
                        { "UserPerm_User", UserTB.PermUser},
                        { "UserPerm_Voc", UserTB.PermVoc}
                    };

                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /* VOC 권한 */
                    var vocPermission = new JObject
                    {
                          { "VocMachine", UserTB.VocMachine}, // 기계민원 처리권한
                          { "VocElec", UserTB.VocElec}, // 전기민원 처리권한
                          { "VocLift",UserTB.VocLift}, // 승강민원 처리권한
                          { "VocFire", UserTB.VocFire}, // 소방민원 처리권한
                          { "VocConstruct", UserTB.VocConstruct}, // 건축민원 처리권한
                          { "VocNetwork", UserTB.VocNetwork}, // 통신민원 처리권한
                          { "VocBeauty", UserTB.VocBeauty}, // 미화민원 처리권한
                          { "VocSecurity", UserTB.VocSecurity}, // 보안민원 처리권한
                          { "VocDefault", UserTB.VocEtc}, // 기타 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermission)));

                    /* 사업장 권한 */
                    var PlacePermission = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec",PlaceTB.PermElec}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", PlaceTB.PermLift}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", PlaceTB.PermFire}, // 사업장 소방메뉴 권한
                        {"PlacePerm_Construct", PlaceTB.PermConstruct}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", PlaceTB.PermNetwork}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", PlaceTB.PermSecurity}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", PlaceTB.PermMaterial}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", PlaceTB.PermEnergy}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", PlaceTB.PermVoc} // 사업장 VOC 권한
                    };

                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(PlacePermission)));

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
                else
                {
                    // 관리자
                    AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);
                    if (admintb is null || String.IsNullOrWhiteSpace(admintb.Type))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    var authClaims = new List<Claim>
                    {
                         new Claim("UserIdx", UserTB.Id.ToString()), // USER 인덱스
                         new Claim("Name", UserTB.Name.ToString()!), // 이름
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim("AlarmYN", UserTB.AlarmYn!.ToString()), // 알람받을지 여부
                         new Claim("AdminYN", UserTB.AdminYn!.ToString()), // 관리자 여부
                         new Claim("UserType", "ADMIN"), // 직책
                         new Claim("AdminIdx", admintb.Id.ToString()), // 관리자 인덱스
                         new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                         new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 명
                         new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd"))
                    };

                    var roleMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                     {
                         { "시스템관리자", "SystemManager" },
                         { "마스터", "Master" },
                         { "매니저", "Manager" },
                         { "SystemManager", "SystemManager" },
                         { "Master", "Master" },
                         { "Manager", "Manager" }
                     };

                    if (roleMapping is null)
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    string? role = String.Empty;
                    if (roleMapping.TryGetValue(admintb.Type, out var mappedRole))
                    {
                        role = mappedRole;
                    }
                    else
                    {
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }

                    authClaims.Add(new Claim("Role", role));
                    authClaims.Add(new Claim(ClaimTypes.Role, role));

                    // 메뉴 접근권한
                    var UserPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic},
                        { "UserPerm_Machine", UserTB.PermMachine},
                        { "UserPerm_Elec", UserTB.PermElec},
                        { "UserPerm_Lift", UserTB.PermLift},
                        { "UserPerm_Fire", UserTB.PermFire},
                        { "UserPerm_Construct", UserTB.PermConstruct},
                        { "UserPerm_Network", UserTB.PermNetwork},
                        {"UserPerm_Beauty" ,UserTB.PermBeauty},
                        { "UserPerm_Security", UserTB.PermSecurity},
                        { "UserPerm_Material", UserTB.PermMaterial},
                        { "UserPerm_Energy", UserTB.PermEnergy},
                        { "UserPerm_User", UserTB.PermUser},
                        { "UserPerm_Voc", UserTB.PermVoc}
                    };


                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(UserPermissions)));

                    // VOC 권한
                    var VocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine}, // 기계민원 처리권한
                        { "VocElec", UserTB.VocElec}, // 전기민원 처리권한
                        { "VocLift", UserTB.VocLift}, // 승강민원 처리권한
                        { "VocFire", UserTB.VocFire}, // 소방민원 처리권한
                        { "VocConstruct", UserTB.VocConstruct}, // 건축민원 처리권한
                        { "VocNetwork", UserTB.VocNetwork}, // 통신민원 처리권한
                        { "VocBeauty", UserTB.VocBeauty}, // 미화민원 처리권한
                        { "VocSecurity", UserTB.VocSecurity}, // 보안민원 처리권한
                        { "VocDefault", UserTB.VocEtc} // 기타 처리권한
                    };

                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(VocPermissions)));

                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString() },
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString() },
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString() },
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString() },
                        { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString() },
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString() },
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString() },
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString() },
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString() },
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString() },
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString() }
                    };
                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));


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
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
       

        public async Task<string?> RefreshTokenService(int placeid, int useridx, bool isAdmin)
        {
            try
            {
                if(isAdmin)
                {
                    // 관리자모드
                    UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(useridx).ConfigureAwait(false);

                    if (UserTB is null)
                        return null;

                    if (UserTB.AdminYn != true)
                        return null;

                    AdminTb? AdminTB = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);
                    if (AdminTB is null)
                        return null;

                    DepartmentsTb? DepartmentTB = await DepartmentInfoRepository.GetDepartmentInfo(AdminTB.DepartmentTbId).ConfigureAwait(false);
                    if (DepartmentTB is null)
                        return null;

                    List<Claim> authClaims = new List<Claim>();

                    // 로그인성공
                    authClaims.Add(new Claim("UserIdx", UserTB.Id.ToString()));
                    authClaims.Add(new Claim("Name", UserTB.Name!.ToString()));
                    authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    authClaims.Add(new Claim("UserType", "ADMIN"));
                    authClaims.Add(new Claim("AdminIdx", AdminTB.Id.ToString()));
                    authClaims.Add(new Claim("DepartIdx", AdminTB.DepartmentTbId.ToString()));
                    authClaims.Add(new Claim("DepartmentName", DepartmentTB.Name.ToString()));

                    switch (AdminTB.Type.Trim())
                    {
                        case "시스템관리자":
                            authClaims.Add(new Claim("Role", "시스템관리자"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "SystemManager"));
                            break;

                        case "마스터":
                            authClaims.Add(new Claim("Role", "마스터"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "Master"));
                            break;

                        case "매니저":
                            authClaims.Add(new Claim("Role", "매니저"));
                            authClaims.Add(new Claim(ClaimTypes.Role, "Manager"));
                            break;
                    }

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    var token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        //expires: DateTime.Now.AddSeconds(20),
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                    
                    return accessToken;
                }
                else
                {
                    UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(useridx).ConfigureAwait(false);
                    if (UserTB is null)
                        return null;

                    List<Claim> authClaims = new List<Claim>();
                    authClaims.Add(new Claim("UserIdx", UserTB.Id.ToString())); // USERID)
                    authClaims.Add(new Claim("Name", UserTB.Name!.ToString())); // USERNAME
                    authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    authClaims.Add(new Claim("AlarmYN", UserTB.AlarmYn!.ToString())); // 알람 받을지 여부
                    authClaims.Add(new Claim("AdminYN", UserTB.AdminYn!.ToString())); // 관리자 여부
                    if (UserTB.AdminYn)
                    {
                        // 관리자
                        authClaims.Add(new Claim("UserType", "ADMIN"));

                        AdminTb? AdminTB = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);
                        if (AdminTB is null)
                            return null;

                        var roleMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                             { "시스템관리자", "SystemManager" },
                             { "마스터", "Master" },
                             { "매니저", "Manager" },
                             { "SystemManager", "SystemManager" },
                             { "Master", "Master" },
                             { "Manager", "Manager" }
                        };

                        if (roleMapping is null)
                            return null;

                        string? role = String.Empty;
                        if (roleMapping.TryGetValue(AdminTB.Type, out var mappedRole))
                        {
                            role = mappedRole;
                        }
                        else
                        {
                            return null;
                        }
                        authClaims.Add(new Claim("AdminIdx", AdminTB.Id.ToString()));
                        authClaims.Add(new Claim("Role", role));
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    else
                    {
                        // 일반사용자
                        authClaims.Add(new Claim("UserType", "User"));
                        authClaims.Add(new Claim("Role", "User"));
                    }

                    PlaceTb? PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(placeid).ConfigureAwait(false);
                    if (PlaceTB is null)
                        return null;

                    authClaims.Add(new Claim("PlaceIdx", PlaceTB.Id!.ToString())); // 사업장 인덱스
                    authClaims.Add(new Claim("PlaceName", PlaceTB.Name!.ToString()));
                    authClaims.Add(new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")));


                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()},
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()},
                        { "UserPerm_Elec", UserTB.PermElec.ToString()},
                        { "UserPerm_Lift",UserTB.PermLift.ToString()},
                        { "UserPerm_Fire",UserTB.PermFire.ToString()},
                        { "UserPerm_Construct",UserTB.PermConstruct.ToString()},
                        { "UserPerm_Network",UserTB.PermNetwork.ToString()},
                        { "UserPerm_Beauty",UserTB.PermBeauty.ToString()},
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()},
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()},
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()},
                        { "UserPerm_User", UserTB.PermUser.ToString()},
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()}
                    };

                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /* VOC 권한 */
                    var vocPermission = new JObject
                    {
                          { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 처리권한
                          { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 처리권한
                          { "VocLift",UserTB.VocLift.ToString()}, // 승강민원 처리권한
                          { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 처리권한
                          { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 처리권한
                          { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 처리권한
                          { "VocBeauty", UserTB.VocBeauty.ToString()}, // 미화민원 처리권한
                          { "VocSecurity", UserTB.VocSecurity.ToString()}, // 보안민원 처리권한
                          { "VocDefault", UserTB.VocEtc.ToString()}, // 기타 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermission)));


                    /* 사업장 권한 */
                    var PlacePermission = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec",PlaceTB.PermElec.ToString()}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()}, // 사업장 소방메뉴 권한
                        {"PlacePerm_Construct", PlaceTB.PermConstruct.ToString()}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()} // 사업장 VOC 권한
                    };

                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(PlacePermission)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));
                
                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                    CreateBuilderLogger.ConsoleText(accessToken);
                    return accessToken;
                    // 일반모드
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        public async Task<ResponseUnit<string?>> LoginSelectPlaceService(int placeid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if(context is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? adminidx = Convert.ToString(context.Items["AdminIdx"]);
                if (adminidx is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<AdminPlaceTb>? adminplace = await AdminPlaceInfoRepository.GetMyWorksList(Convert.ToInt32(adminidx)).ConfigureAwait(false);
                if (adminplace is null || !adminplace.Any())
                    return new ResponseUnit<string?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };

                AdminPlaceTb? selectplace = adminplace.FirstOrDefault(m => m.PlaceTbId == placeid);
                if(selectplace is null)
                    return new ResponseUnit<string?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 404 };

                PlaceTb? placeInfo = await PlaceInfoRepository.GetByPlaceInfo(placeid).ConfigureAwait(false);
                if (placeInfo is null || placeInfo.Name is null)
                    return new ResponseUnit<string?>() { message = "사업장이 존재하지 않습니다.", data = null, code = 404 };

                /*
                 * 해약된 사업장 로그인못하게 
                 * Status(계약상태) true : 계약 / false : 해약
                 */
                if (placeInfo.Status == false)
                    return new ResponseUnit<string?>() { message = "해약된 사업장은 접근이 불가능합니다.", data = null, code = 200 };

                /* USER TOKEN 검사 */
                var checkUserToken = new[]
                {
                    "UserIdx", "Name","jti","AlarmYN","UserType","AdminIdx","Role"
                };

                foreach (var key in checkUserToken)
                {
                    if (String.IsNullOrWhiteSpace(context.Items[key]?.ToString()))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                var authClaims = new List<Claim>
                {
                    new Claim("UserIdx", context.Items["UserIdx"]!.ToString()!), // USER 인덱스
                    new Claim("Name", context.Items["Name"]!.ToString()!), // 이름
                    new Claim("jti", context.Items["jti"]!.ToString()!), // JTI 
                    new Claim("AlarmYN", context.Items["AlarmYN"]!.ToString()!), // 알람받을지 여부
                    new Claim("AdminYN", context.Items["AdminYN"]!.ToString()!), // 관리자 여부
                    new Claim("UserType", context.Items["UserType"]!.ToString()!), // 직책
                    new Claim("AdminIdx", context.Items["AdminIdx"]!.ToString()!), // 관리자 인덱스
                    new Claim("PlaceIdx", placeInfo.Id!.ToString()), // 사업장 인덱스
                    new Claim("PlaceName", placeInfo.Name!.ToString()), // 사업장 명
                    new Claim("PlaceCreateDT", placeInfo.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                };

                var roleMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "시스템관리자", "SystemManager" },
                    { "마스터", "Master" },
                    { "매니저", "Manager" },
                    { "SystemManager", "SystemManager" },
                    { "Master", "Master" },
                    { "Manager", "Manager" }
                };

                // Get the role from context
                string? role = Convert.ToString(context.Items["Role"]);

                // Try to map the role using the dictionary
                if (role != null && roleMapping.TryGetValue(role, out var mappedRole))
                {
                    role = mappedRole;
                }
                else
                {
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                authClaims.Add(new Claim("Role", role));
                authClaims.Add(new Claim(ClaimTypes.Role, role));
                
                /* USER 권한 TOKEN 검사 */
                var checkPermToken = new[]
                {
                    "UserPerm_Basic", 
                    "UserPerm_Machine",
                    "UserPerm_Elec",
                    "UserPerm_Lift",
                    "UserPerm_Fire",
                    "UserPerm_Construct",
                    "UserPerm_Network",
                    "UserPerm_Beauty",
                    "UserPerm_Security",
                    "UserPerm_Material",
                    "UserPerm_Energy",
                    "UserPerm_User",
                    "UserPerm_Voc"
                };

                foreach (var key in checkPermToken)
                {
                    if (String.IsNullOrWhiteSpace(context.Items[key]?.ToString()))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                var userPermissions = new JObject
                {
                    { "UserPerm_Basic", context.Items["UserPerm_Basic"]!.ToString() },
                    { "UserPerm_Machine", context.Items["UserPerm_Machine"]!.ToString() },
                    { "UserPerm_Elec", context.Items["UserPerm_Elec"]!.ToString() },
                    { "UserPerm_Lift", context.Items["UserPerm_Lift"]!.ToString() },
                    { "UserPerm_Fire", context.Items["UserPerm_Fire"]!.ToString() },
                    { "UserPerm_Construct", context.Items["UserPerm_Construct"]!.ToString() },
                    { "UserPerm_Network", context.Items["UserPerm_Network"]!.ToString() },
                    { "UserPerm_Beauty", context.Items["UserPerm_Beauty"]!.ToString() },
                    { "UserPerm_Security", context.Items["UserPerm_Security"]!.ToString() },
                    { "UserPerm_Material", context.Items["UserPerm_Material"]!.ToString() },
                    { "UserPerm_Energy", context.Items["UserPerm_Energy"]!.ToString() },
                    { "UserPerm_User", context.Items["UserPerm_User"]!.ToString() },
                    { "UserPerm_Voc", context.Items["UserPerm_Voc"]!.ToString() }
                };
                authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                /* USER VOC TOKEN 검사 */
                var checkVocPermToken = new[]
               {
                    "VocMachine",
                    "VocElec",
                    "VocLift",
                    "VocFire",
                    "VocConstruct",
                    "VocNetwork",
                    "VocBeauty",
                    "VocSecurity",
                    "VocDefault"
                };

                foreach (var key in checkVocPermToken)
                {
                    if (String.IsNullOrWhiteSpace(context.Items[key]?.ToString()))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                var vocPermissions = new JObject
                {
                    { "VocMachine", context.Items["VocMachine"]!.ToString() },
                    { "VocElec", context.Items["VocElec"]!.ToString() },
                    { "VocLift", context.Items["VocLift"]!.ToString() },
                    { "VocFire", context.Items["VocFire"]!.ToString() },
                    { "VocConstruct", context.Items["VocConstruct"]!.ToString() },
                    { "VocNetwork", context.Items["VocNetwork"]!.ToString() },
                    { "VocBeauty", context.Items["VocBeauty"]!.ToString() },
                    { "VocSecurity", context.Items["VocSecurity"]!.ToString() },
                    { "VocDefault", context.Items["VocDefault"]!.ToString() }
                };

                authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                foreach (var key in checkPermToken)
                {
                    if (String.IsNullOrWhiteSpace(context.Items[key]?.ToString()))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                var placePermissions = new JObject
                {
                    { "PlacePerm_Machine", placeInfo.PermMachine.ToString() },
                    { "PlacePerm_Elec", placeInfo.PermElec.ToString() },
                    { "PlacePerm_Lift", placeInfo.PermLift.ToString() },
                    { "PlacePerm_Fire", placeInfo.PermFire.ToString() },
                    { "PlacePerm_Construct", placeInfo.PermConstruct.ToString() },
                    { "PlacePerm_Network", placeInfo.PermNetwork.ToString() },
                    { "PlacePerm_Beauty", placeInfo.PermBeauty.ToString() },
                    { "PlacePerm_Security", placeInfo.PermSecurity.ToString() },
                    { "PlacePerm_Material", placeInfo.PermMaterial.ToString() },
                    { "PlacePerm_Energy", placeInfo.PermEnergy.ToString() },
                    { "PlacePerm_Voc", placeInfo.PermVoc.ToString() }
                };
                authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));


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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 일반페이지 유저 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<string?>> UserLoginService(LoginDTO dto)
        {
            try
            {
                UsersTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID!, dto.UserPassword!).ConfigureAwait(false);
                if (usertb is null)
                    return new ResponseUnit<string?>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 400 };

                bool? AdminYN = usertb.AdminYn;
                if(AdminYN == false) // 일반유저
                {
                    PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(usertb.PlaceTbId!.Value).ConfigureAwait(false);

                    if (placetb is null)
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    /*
                    * 해약된 사업장 로그인못하게 
                    * Status(계약상태) true : 계약 / false : 해약
                    */
                    if (placetb.Status == false)
                        return new ResponseUnit<string?>() { message = "해약된 사업장은 접근이 불가능합니다.", data = null, code = 200 };

                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", usertb.Id.ToString()), // USERID
                        new Claim("Name", usertb.Name!.ToString()), // USERNAME
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", usertb.AlarmYn!.ToString()), // 알람 받을지 여부
                        new Claim("AdminYN", usertb.AdminYn!.ToString()), // 관리자 여부
                        new Claim("UserType", "User"),
                        new Claim("Role", "User"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("PlaceIdx", placetb.Id!.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", placetb.Name!.ToString()), // 사업장 명칭
                        new Claim("PlaceCreateDT", placetb.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };


                    /* 메뉴 접근권한 */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", usertb.PermBasic.ToString()},
                        { "UserPerm_Machine", usertb.PermMachine.ToString()},
                        { "UserPerm_Elec", usertb.PermElec.ToString()},
                        { "UserPerm_Lift",usertb.PermLift.ToString()},
                        { "UserPerm_Fire",usertb.PermFire.ToString()},
                        { "UserPerm_Construct",usertb.PermConstruct.ToString()},
                        { "UserPerm_Network",usertb.PermNetwork.ToString()},
                        { "UserPerm_Beauty",usertb.PermBeauty.ToString()},
                        { "UserPerm_Security", usertb.PermSecurity.ToString()},
                        { "UserPerm_Material", usertb.PermMaterial.ToString()},
                        { "UserPerm_Energy", usertb.PermEnergy.ToString()},
                        { "UserPerm_User", usertb.PermUser.ToString()},
                        { "UserPerm_Voc", usertb.PermVoc.ToString()}
                    };

                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /* VOC 권한 */
                    var vocPermission = new JObject
                    {
                        { "VocMachine", usertb.VocMachine.ToString()}, // 기계민원 처리권한
                        { "VocElec", usertb.VocElec.ToString()}, // 전기민원 처리권한
                        { "VocLift",usertb.VocLift.ToString()}, // 승강민원 처리권한
                        { "VocFire", usertb.VocFire.ToString()}, // 소방민원 처리권한
                        { "VocConstruct", usertb.VocConstruct.ToString()}, // 건축민원 처리권한
                        { "VocNetwork", usertb.VocNetwork.ToString()}, // 통신민원 처리권한
                        { "VocBeauty", usertb.VocBeauty.ToString()}, // 미화민원 처리권한
                        { "VocSecurity", usertb.VocSecurity.ToString()}, // 보안민원 처리권한
                        { "VocDefault", usertb.VocEtc.ToString()}, // 기타 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermission)));


                    /* 사업장 권한 */
                    var PlacePermission = new JObject
                    {
                        { "PlacePerm_Machine", placetb.PermMachine.ToString()}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec",placetb.PermElec.ToString()}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", placetb.PermLift.ToString()}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", placetb.PermFire.ToString()}, // 사업장 소방메뉴 권한
                        {"PlacePerm_Construct", placetb.PermConstruct.ToString()}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", placetb.PermNetwork.ToString()}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", placetb.PermBeauty.ToString()}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", placetb.PermSecurity.ToString()}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", placetb.PermMaterial.ToString()}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", placetb.PermEnergy.ToString()}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", placetb.PermVoc.ToString()} // 사업장 VOC 권한
                    };

                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(PlacePermission)));

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
                else // 관리자
                {
                    // 위에만큼 담는데 (사업장 은 빼고)
                    AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);
                    if(admintb is null || String.IsNullOrWhiteSpace(admintb.Type))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", usertb.Id.ToString()), // 인덱스
                        new Claim("Name", usertb.Name!.ToString()), // 이름
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", usertb.AlarmYn!.ToString()), // 알람 받을지 여부
                        new Claim("AdminYN", usertb.AdminYn!.ToString()), // 관리자 여부
                        new Claim("UserType", "ADMIN"), // 사용자 타입
                        new Claim("AdminIdx", admintb.Id!.ToString()) // 관리자 인덱스
                    };

                    string? adminType = admintb.Type switch
                    {
                        "시스템관리자" => "SystemManager",
                        "마스터" => "Master",
                        "매니저" => "Manager",
                        _ => null
                    };
                    if(String.IsNullOrWhiteSpace(adminType))
                        return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    
                    authClaims.Add(new Claim("Role", admintb.Type));
                    authClaims.Add(new Claim(ClaimTypes.Role, adminType));

                    // 메뉴 접근권한
                    var UserPermissions = new JObject
                    {
                        { "UserPerm_Basic", usertb.PermBasic.ToString()},
                        { "UserPerm_Machine", usertb.PermMachine.ToString()},
                        { "UserPerm_Elec", usertb.PermElec.ToString()},
                        { "UserPerm_Lift", usertb.PermLift.ToString()},
                        { "UserPerm_Fire", usertb.PermFire.ToString()},
                        { "UserPerm_Construct", usertb.PermConstruct.ToString()},
                        { "UserPerm_Network", usertb.PermNetwork.ToString()},
                        {"UserPerm_Beauty" ,usertb.PermBeauty.ToString()},
                        { "UserPerm_Security", usertb.PermSecurity.ToString()},
                        { "UserPerm_Material", usertb.PermMaterial.ToString()},
                        { "UserPerm_Energy", usertb.PermEnergy.ToString()},
                        { "UserPerm_User", usertb.PermUser.ToString()},
                        { "UserPerm_Voc", usertb.PermVoc.ToString()}
                    };
                        
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(UserPermissions)));

                    // VOC 권한
                    var VocPermissions = new JObject
                    {
                        { "VocMachine", usertb.VocMachine.ToString()}, // 기계민원 처리권한
                        { "VocElec", usertb.VocElec.ToString()}, // 전기민원 처리권한
                        { "VocLift", usertb.VocLift.ToString()}, // 승강민원 처리권한
                        { "VocFire", usertb.VocFire.ToString()}, // 소방민원 처리권한
                        { "VocConstruct", usertb.VocConstruct.ToString()}, // 건축민원 처리권한
                        { "VocNetwork", usertb.VocNetwork.ToString()}, // 통신민원 처리권한
                        { "VocBeauty", usertb.VocBeauty.ToString()}, // 미화민원 처리권한
                        { "VocSecurity", usertb.VocSecurity.ToString()}, // 보안민원 처리권한
                        { "VocDefault", usertb.VocEtc.ToString()} // 기타 처리권한
                    };

                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(VocPermissions)));

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
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
        
        /// <summary>
        /// 로그인한 사업장의 사용자 LIST 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<ResponseList<ListUser>> GetPlaceUserList()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<ListUser>() { message = "잘못된 요청입니다.", data = new List<ListUser>(), code = 404 };

                List<UsersTb>? model = await UserInfoRepository.GetPlaceUserList(Convert.ToInt32(placeidx)).ConfigureAwait(false);

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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<ListUser>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<ListUser>(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 추가 서비스  0 : 퇴직 / 1 : 재직 / 2 :휴직
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<UsersDTO>> AddUserService(UsersDTO dto, IFormFile? files)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                DateTime ThisDate = DateTime.Now;
                
                if (String.IsNullOrWhiteSpace(Creater) || String.IsNullOrWhiteSpace(PlaceIdx) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx)).ConfigureAwait(false);
                if (TokenChk is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                if (TokenChk.PermUser != 2)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? CheckUserId = await UserInfoRepository.UserIdCheck(dto.USERID!).ConfigureAwait(false);
                if (CheckUserId is not null)
                    return new ResponseUnit<UsersDTO>() { message = "이미 존재하는 아이디입니다.", data = null, code = 204 };
                
                string NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                // 사용자 관련한 폴더 없으면 만들기
                PlaceFileFolderPath = Path.Combine(Common.FileServer, PlaceIdx.ToString(), "Users");

                di = new DirectoryInfo(PlaceFileFolderPath);
                if (!di.Exists) di.Create();

                UsersTb model = new UsersTb();
                model.UserId = dto.USERID!.ToLower(); // 사용자아이디
                model.Password = dto.PASSWORD!; // 비밀번호
                model.Name = dto.NAME; // 이름
                model.Email = dto.EMAIL; // 이메일
                model.Phone = dto.PHONE; // 전화번호
                model.PermBasic = dto.PERM_BASIC!.Value; // 기본정보메뉴 권한
                model.PermMachine = dto.PERM_MACHINE!.Value; // 기계메뉴 권한
                model.PermElec = dto.PERM_ELEC!.Value; // 전기메뉴 권한
                model.PermLift = dto.PERM_LIFT!.Value; // 승강메뉴 권한
                model.PermFire = dto.PERM_FIRE!.Value; // 소방메뉴 권한
                model.PermConstruct = dto.PERM_CONSTRUCT!.Value; // 건축메뉴 권한
                model.PermNetwork = dto.PERM_NETWORK!.Value; // 통신메뉴 권한
                model.PermBeauty = dto.PERM_BEAUTY!.Value; // 미화메뉴 권한
                model.PermSecurity = dto.PERM_SECURITY!.Value; // 보안메뉴 권한
                model.PermMaterial = dto.PERM_MATERIAL!.Value; // 자재메뉴 권한
                model.PermEnergy = dto.PERM_ENERGY!.Value; // 에너지메뉴 권한
                model.PermUser = dto.PERM_USER!.Value; // 사용자메뉴 권한
                model.PermVoc = dto.PERM_VOC!.Value; // VOC메뉴 권한
                model.AdminYn = false; // 관리자 아님
                model.AlarmYn = dto.ALRAM_YN!.Value; // 알람 여부
                model.Status = 2; // 재직여부
                model.CreateDt = ThisDate;
                model.CreateUser = Creater; // 생성자
                model.UpdateDt = ThisDate;
                model.UpdateUser = Creater; // 수정자
                model.Job = dto.JOB;
                model.VocMachine = dto.VOC_MACHINE!.Value; // VOC 기계권한
                model.VocElec = dto.VOC_ELEC!.Value; // VOC 전기권한
                model.VocLift = dto.VOC_LIFT!.Value; // VOC 승강권한
                model.VocFire = dto.VOC_FIRE!.Value; // VOC 소방권한
                model.VocConstruct = dto.VOC_CONSTRUCT!.Value; // VOC 건축권한
                model.VocNetwork = dto.VOC_NETWORK!.Value; // VOC 통신권한
                model.VocBeauty = dto.VOC_BEAUTY!.Value; // VOC 미화권한
                model.VocSecurity = dto.VOC_SECURITY!.Value; // VOC 보안권한
                
                if(dto.PERM_VOC > 0)
                    model.VocEtc = true; // VOC 미분류권한 있음
                else
                    model.VocEtc = false; // VOC 미분류권한 없음
                
                model.PlaceTbId = Int32.Parse(PlaceIdx);
                model.Image = NewFileName;

                bool? result = await UserInfoRepository.AddUserAsync(model).ConfigureAwait(false);
                if (result == true)
                {
                    if(files is not null)
                    {
                        // 파일 넣기
                        bool? AddFile = await FileService.AddResizeImageFile(NewFileName, PlaceFileFolderPath, files).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<UsersDTO>> GetUserDetails(int id, bool isMobile)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                UsersTb? TokenChk = await UserInfoRepository.GetUserIndexInfo(Convert.ToInt32(UserIdx)).ConfigureAwait(false);
                if (TokenChk is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                if (TokenChk.PermUser < 1)
                    return new ResponseUnit<UsersDTO>() { message = "접근 권한이 없습니다.", data = new UsersDTO(), code = 200 };

                UsersTb? model = await UserInfoRepository.GetUserIndexInfo(id).ConfigureAwait(false);

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
                    dto.PERM_LIFT = model.PermLift;
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
                    //dto.VOC_ETC = model.VocEtc;

                    string PlaceFileName = Path.Combine(Common.FileServer, placeid.ToString(), "Users");

                    di = new DirectoryInfo(PlaceFileName);
                    if (!di.Exists) di.Create();

                    if(isMobile)
                    {
#if DEBUG
                        CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                        if (!String.IsNullOrWhiteSpace(model.Image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(PlaceFileName, model.Image).ConfigureAwait(false);

                            if (ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, model.Image);
                                if (files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files).ConfigureAwait(false);

                                    if (ConvertFile is not null)
                                    {
                                        dto.ImageName = model.Image;
                                        dto.Image = ConvertFile;
                                    }
                                    else
                                    {
                                        dto.ImageName = null;
                                        dto.Image = null;
                                    }
                                }
                                else
                                {
                                    dto.ImageName = null;
                                    dto.Image = null;
                                }
                            }
                            else
                            {
                                dto.ImageName = null;
                                dto.Image = null;
                            }
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
#if DEBUG
                        CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                        if (!String.IsNullOrWhiteSpace(model.Image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(PlaceFileName, model.Image).ConfigureAwait(false);

                            if (ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, model.Image);
                                if (files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files).ConfigureAwait(false);

                                    if (ConvertFile is not null)
                                    {
                                        dto.ImageName = model.Image;
                                        dto.Image = ConvertFile;
                                    }
                                    else
                                    {
                                        dto.ImageName = null;
                                        dto.Image = null;
                                    }
                                }
                                else
                                {
                                    dto.ImageName = null;
                                    dto.Image = null;
                                }
                            }
                            else
                            {
                                dto.ImageName = null;
                                dto.Image = null;
                            }
                        }

                        return new ResponseUnit<UsersDTO>()
                        {
                            message = "요청이 정상 처리되었습니다.",
                            data = dto,
                            code = 200
                        };
                    }
                }
                else
                {
                    return new ResponseUnit<UsersDTO>() { message = "데이터가 존재하지 않습니다.", data = new UsersDTO(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UsersDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사용자 데이터 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> DeleteUserService(List<int> del)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 삭제체크
                foreach (int id in del) 
                {
                    bool? DelCheck = await UserInfoRepository.DelUserCheck(id).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "참조하고있는 하위 정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                bool? DeleteResult = await UserInfoRepository.DeleteUserInfo(del, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자 데이터 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<UsersDTO>> UpdateUserService(UsersDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;

                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };
                
                string? Name = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(Name) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                PlaceFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Users");

                di = new DirectoryInfo(PlaceFileFolderPath);
                if (!di.Exists) di.Create();

                UsersTb? model = await UserInfoRepository.GetUserIndexInfo(dto.ID!.Value).ConfigureAwait(false);
                if (model is null)
                    return new ResponseUnit<UsersDTO>() { message = "잘못된 요청입니다.", data = new UsersDTO(), code = 404 };

                model.UserId = dto.USERID!.ToLower()!;
                model.Password = dto.PASSWORD!;
                model.Name = dto.NAME;
                model.Email = dto.EMAIL;
                model.Phone = dto.PHONE;
                model.Job = dto.JOB;
                /* 메뉴권한 */
                model.PermBasic = dto.PERM_BASIC!.Value;
                model.PermMachine = dto.PERM_MACHINE!.Value;
                model.PermElec = dto.PERM_ELEC!.Value;
                model.PermLift = dto.PERM_LIFT!.Value;
                model.PermFire = dto.PERM_FIRE!.Value;
                model.PermConstruct = dto.PERM_CONSTRUCT!.Value;
                model.PermNetwork = dto.PERM_NETWORK!.Value;
                model.PermBeauty = dto.PERM_BEAUTY!.Value;
                model.PermSecurity = dto.PERM_SECURITY!.Value;
                model.PermMaterial = dto.PERM_MATERIAL!.Value;
                model.PermEnergy = dto.PERM_ENERGY!.Value;
                model.PermUser = dto.PERM_USER!.Value;
                model.PermVoc = dto.PERM_VOC!.Value;
                /* VOC권한 */
                model.VocMachine = dto.VOC_MACHINE!.Value;
                model.VocElec = dto.VOC_ELEC!.Value;
                model.VocLift = dto.VOC_LIFT!.Value;
                model.VocFire = dto.VOC_FIRE!.Value;
                model.VocConstruct = dto.VOC_CONSTRUCT!.Value;
                model.VocNetwork = dto.VOC_NETWORK!.Value;
                model.VocBeauty = dto.VOC_BEAUTY!.Value;
                model.VocSecurity = dto.VOC_SECURITY!.Value;

                if(dto.PERM_VOC > 0)
                    model.VocEtc = true;
                else
                    model.VocEtc = false;
                
                //model.VocEtc = dto.VOC_ETC!.Value;
                model.AlarmYn = dto.ALRAM_YN!.Value;
                model.Status = dto.STATUS!.Value;
                model.UpdateDt = ThisDate;
                model.UpdateUser = Name;
                
                if(files is not null) // 파일이 공백이 아닌경우
                {
                    if(files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다르면
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            deleteFileName = model.Image;
                        }

                        // 새로운 파일명 설정
                        string newFileName = FileService.SetNewFileName(UserIdx, files);
                        NewFileName = newFileName; // 파일명 리스트에 추가
                        model.Image = newFileName; // DB Image명칭 업데이트

                        RemoveTemp = newFileName; // 실패시 삭제명단에 넣어야함.
                    }
                }
                else // 파일이 공백인경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB의 이미지가 공백이 아니면
                    {
                        deleteFileName = model.Image; // 기존 파일 삭제 목록에 추가
                        model.Image = null; // 모델의 파일명 비우기
                    }
                }

                // 먼저 파일 삭제 처리
                // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                byte[]? ImageBytes = null;
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    ImageBytes = await FileService.GetImageFile(PlaceFileFolderPath, deleteFileName).ConfigureAwait(false);
                }

                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if(ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(PlaceFileFolderPath, deleteFileName);
                }
                
                // 새 파일 저장
                if(files is not null)
                {
                    if(String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, PlaceFileFolderPath, files).ConfigureAwait(false);
                    }
                }

                // 이후 데이터베이스 업데이트
                UsersTb? updatemodel = await UserInfoRepository.UpdateUserInfo(model).ConfigureAwait(false);
                if (updatemodel is not null)
                {
                    // 성공했으면 그걸로 끝
                    return new ResponseUnit<UsersDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을 원래대로 돌려놔야함.
                    if (AddTemp is not null)
                    {
                        try
                        {
                            if(FileService.IsFileExists(PlaceFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, PlaceFileFolderPath, files).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
                        }
                    }


                    if (!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(PlaceFileFolderPath, RemoveTemp);
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
                        }
                    }

                    return new ResponseUnit<UsersDTO>() { message = "요청이 처리되지 않았습니다.", data = dto, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<UsersDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UsersDTO(), code = 500 };
            }
        }

     

        /// <summary>
        /// 사업장 메뉴권한 리턴
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<PlacePermissionDTO?>> GetMenuPermService()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<PlacePermissionDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<PlacePermissionDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                PlaceTb? PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if(PlaceTB is not null)
                {
                    PlacePermissionDTO model = new PlacePermissionDTO
                    {
                        Id = PlaceTB.Id,
                        PermMachine = PlaceTB.PermMachine,
                        PermElec = PlaceTB.PermElec,
                        PermLift = PlaceTB.PermLift,
                        PermFire = PlaceTB.PermFire,
                        PermConstruct = PlaceTB.PermConstruct,
                        PermNetwork = PlaceTB.PermNetwork,
                        PermBeauty = PlaceTB.PermBeauty,
                        PermSecurity = PlaceTB.PermSecurity,
                        PermMaterial = PlaceTB.PermMaterial,
                        PermEnergy = PlaceTB.PermEnergy,
                        PermVoc = PlaceTB.PermVoc
                    };
                    return new ResponseUnit<PlacePermissionDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                    return new ResponseUnit<PlacePermissionDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<PlacePermissionDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 웹[일반 유저] 로그인 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<TokenDTOV2>?> WebUserLoginService(LoginDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserID) || String.IsNullOrWhiteSpace(dto.UserPassword))
                    return new ResponseUnit<TokenDTOV2>() { message = "필수항목을 확인해주세요.", data = null, code = 404 };

                var UserTB = await UserInfoRepository.GetUserInfo(dto.UserID, dto.UserPassword).ConfigureAwait(false);
                if (UserTB is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 404 };

                bool AdminYn = UserTB.AdminYn;
                if(AdminYn) // 관리자
                {
                    // 관리자는 사업장 Select를 해야하므로 사업장 뺸 AccessToken을 반환해준다.
                    // - Select 후 사업장 합친 Token을 AccessToken에 담아 보내주면됨.

                    var AdminTB = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);
                    if (AdminTB is null || String.IsNullOrWhiteSpace(AdminTB.Type))
                        return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name!.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN",UserTB.AlarmYn.ToString()), // 알람 받을지 여부
                        new Claim("AdminYN",UserTB.AdminYn.ToString()), // 관리자 여부
                        new Claim("UserType", "ADMIN"), // 사용자 타입
                        new Claim("AdminIdx", AdminTB.Id.ToString()) // 관리자 인덱스
                    };

                    string? adminType = AdminTB.Type switch
                    {
                        "시스템관리자" => "SystemManager",
                        "마스터" => "Master",
                        "매니저" => "Manager",
                        _ => null
                    };

                    if (String.IsNullOrWhiteSpace(adminType))
                        return new ResponseUnit<TokenDTOV2>() { message = "관리자 권한이 없습니다.", data = null, code = 403 };

                    authClaims.Add(new Claim("Role", AdminTB.Type));
                    authClaims.Add(new Claim(ClaimTypes.Role, adminType));

                    /*
                     * 메뉴 접근권한
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재관리 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자 관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()} // 민원관리 권한
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * Voc 권한
                     */
                    var vocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 권한
                        { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 권한
                        { "VocLift", UserTB.VocLift.ToString()}, // 승강민원 권한
                        { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 권한
                        { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 권한
                        { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 권한
                        {"VocBeauty", UserTB.VocBeauty.ToString()}, // 미화민원 권한
                        { "VocSecurity", UserTB.VocSecurity.ToString()}, // 보안민원 권한
                        { "VocDefault", UserTB.VocEtc.ToString()} // 기타민원 권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var SetRedisCache = await RedisService.SetWebUserpageAccessAsync(UserTB.Id, accessToken).ConfigureAwait(false);
                    if (SetRedisCache is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var (access, refresh, sessionId) = SetRedisCache.Value;

                    var returnToken = new TokenDTOV2
                    {
                        accessToken = access,
                        refreshToken = refresh,
                        sessionId = sessionId
                    };
                    return new ResponseUnit<TokenDTOV2>() { message = "로그인 성공(관리자)", data = returnToken, code = 200 };
                }
                else // 일반유저
                {
                    var PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(UserTB.PlaceTbId!.Value).ConfigureAwait(false);
                    if (PlaceTB is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    /*
                        해약된 사업장 로그인 안됨
                        Status(계약상태) true : 계약 / false : 해약
                     */
                    if (PlaceTB.Status == false)
                        return new ResponseUnit<TokenDTOV2>() { message = "해약된 사업장은 접속이 불가능합니다.", data = null, code = 403 };

                    /*
                     * 기초정보
                     */
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name!.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", UserTB.AlarmYn!.ToString()), // 알람 받을지 여부
                        new Claim("UserType", "User"),
                        new Claim("Role", "User"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("PlaceIdx", PlaceTB.Id!.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name!.ToString()), // 사업장 명칭
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };

                    /* 
                     * 메뉴 접근권한 
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()}, // 민원관리 권한
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * Voc 권한
                     */
                    var vocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 처리권한
                        { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 처리권한
                        { "VocLift", UserTB.VocLift.ToString()}, // 승강민원 처리권한
                        { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 처리권한
                        { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 처리권한
                        { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 처리권한
                        { "VocBeauty", UserTB.VocBeauty.ToString()}, // 미화민원 처리권한
                        {"VocSecurity", UserTB.VocSecurity.ToString()}, // 보안민원 처리권한
                        { "VocDefault", UserTB.VocEtc.ToString()} // 기타민원 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                    /*
                     * 사업장 권한
                     */
                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString()}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()}, // 사업장 소방메뉴 권한
                        { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString()}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()} // 사업장 Voc 권한
                    };

                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);


                    var SetRedisCache = await RedisService.SetWebUserpageAccessAsync(UserTB.Id, accessToken).ConfigureAwait(false);
                    if (SetRedisCache is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var (access, refresh, sessionId) = SetRedisCache.Value;

                    var returnToken = new TokenDTOV2
                    {
                        accessToken = access,
                        refreshToken = refresh,
                        sessionId = sessionId
                    };
                    return new ResponseUnit<TokenDTOV2>() { message = "로그인 성공(유저)", data = returnToken, code = 200 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<TokenDTOV2>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// [웹] - 관리자가 일반페이지 접속했을때 선택한 사업장 포함한 액세스 토큰 재생성
        /// </summary>
        /// <param name="context"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<TokenDTOV2?>> WebLoginSelectPlaceService(int placeid, string sessionId)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<TokenDTOV2?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (placeid is 0)
                    return new ResponseUnit<TokenDTOV2?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (String.IsNullOrWhiteSpace(sessionId))
                    return new ResponseUnit<TokenDTOV2?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? adminIdx = Convert.ToString(context.Items["AdminIdx"]);
                if (String.IsNullOrWhiteSpace(adminIdx))
                    return new ResponseUnit<TokenDTOV2?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                var AdminPlaceTbList = await AdminPlaceInfoRepository.GetMyWorksList(Convert.ToInt32(adminIdx)).ConfigureAwait(false);
                if (AdminPlaceTbList is null || !AdminPlaceTbList.Any())
                    return new ResponseUnit<TokenDTOV2?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 403 };

                AdminPlaceTb? SelectPlaceTB = AdminPlaceTbList.FirstOrDefault(m => m.PlaceTbId == placeid);
                if (SelectPlaceTB is null)
                    return new ResponseUnit<TokenDTOV2?>() { message = "해당 관리자는 선택된 사업장의 권한이 없습니다.", data = null, code = 403 };

                PlaceTb? PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(placeid).ConfigureAwait(false);
                if (PlaceTB is null || PlaceTB.Name is null)
                    return new ResponseUnit<TokenDTOV2?>() { message = "사업장이 존재하지 않습니다.", data = null, code = 404 };

                /*
                 * 해약된 사업장 로그인 못하게
                 * Status(계약상태) true : 계약 / false : 해약
                 */
                if (PlaceTB.Status == false)
                    return new ResponseUnit<TokenDTOV2?>() { message = "해약된 사업장은 접속이 불가능합니다.", data = null, code = 403 };

                string? userIdx = context.Items["UserIdx"]?.ToString();
                if(String.IsNullOrWhiteSpace(userIdx))
                    return new ResponseUnit<TokenDTOV2?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                /*
                 * 기본 정보
                 */
                var authClaim = new List<Claim>
                {
                    new Claim("UserIdx", userIdx), // UserID
                    new Claim("Name", context.Items["Name"]!.ToString()!), // UserName
                    new Claim("jti", context.Items["jti"]!.ToString()!), // JTI
                    new Claim("AlarmYN", context.Items["AlarmYN"]!.ToString()!), // 알람받을지 여부
                    new Claim("AdminYN", context.Items["AdminYN"]!.ToString()!), // 관리자 여부
                    new Claim("UserType", context.Items["AdminYN"]!.ToString()!), // 직책
                    new Claim("AdminIdx", context.Items["UserType"]!.ToString()!), // 관리자 인덱스
                    new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                    new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 명
                    new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                };

                var roleMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "시스템관리자", "SystemManager"},
                    { "마스터", "Master"},
                    { "매니저", "Manager"},
                    { "SystemManager", "SystemManager"},
                    { "Master", "Master"},
                    { "Manager", "Manager"}
                };
                string role = Convert.ToString(context.Items["Role"]!.ToString()!);

                if(role != null && roleMapping.TryGetValue(role, out var mappedRole))
                {
                    role = mappedRole;
                }
                else
                {
                    return new ResponseUnit<TokenDTOV2?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

                authClaim.Add(new Claim("Role", role));
                authClaim.Add(new Claim(ClaimTypes.Role, role));

                /*
                 * 메뉴 접근권한
                 */
                var userPermissions = new JObject
                {
                    { "UserPerm_Basic", context.Items["UserPerm_Basic"]!.ToString()}, // 기본정보 권한
                    { "UserPerm_Machine", context.Items["UserPerm_Machine"]!.ToString()}, // 기계관리 권한
                    { "UserPerm_Elec", context.Items["UserPerm_Elec"]!.ToString()}, // 전기관리 권한
                    { "UserPerm_Lift", context.Items["UserPerm_Lift"]!.ToString()}, // 승강관리 권한
                    { "UserPerm_Fire", context.Items["UserPerm_Fire"]!.ToString()}, // 소방관리 권한
                    { "UserPerm_Construct", context.Items["UserPerm_Construct"]!.ToString()}, // 건축관리 권한
                    { "UserPerm_Network", context.Items["UserPerm_Network"]!.ToString()}, // 통신관리 권한
                    { "UserPerm_Beauty", context.Items["UserPerm_Beauty"]!.ToString()}, // 미화 권한
                    { "UserPerm_Security", context.Items["UserPerm_Security"]!.ToString()}, // 보안 권한
                    { "UserPerm_Material", context.Items["UserPerm_Material"]!.ToString()}, // 자재관리 권한
                    { "UserPerm_Energy", context.Items["UserPerm_Energy"]!.ToString()}, // 에너지관리 권한
                    { "UserPerm_User", context.Items["UserPerm_User"]!.ToString()}, // 사용자 관리 권한
                    { "UserPerm_Voc", context.Items["UserPerm_Voc"]!.ToString()} // 민원관리 권한
                };
                authClaim.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                /*
                 * Voc 권한
                 */
                var vocPermissions = new JObject
                {
                    { "VocMachine", context.Items["VocMachine"]!.ToString()},
                    { "VocElec", context.Items["VocElec"]!.ToString()},
                    { "VocLift", context.Items["VocLift"]!.ToString()},
                    { "VocFire", context.Items["VocFire"]!.ToString()},
                    { "VocConstruct", context.Items["VocConstruct"]!.ToString()},
                    { "VocNetwork", context.Items["VocNetwork"]!.ToString()},
                    { "VocBeauty", context.Items["VocBeauty"]!.ToString()},
                    { "VocSecurity", context.Items["VocSecurity"]!.ToString()},
                    { "VocDefault", context.Items["VocDefault"]!.ToString()}
                };
                authClaim.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                /*
                 * 사업장 권한
                 */
                var placePermissions = new JObject
                {
                    { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()},
                    { "PlacePerm_Elec", PlaceTB.PermElec.ToString()},
                    { "PlacePerm_Lift", PlaceTB.PermLift.ToString()},
                    { "PlacePerm_Fire", PlaceTB.PermFire.ToString()},
                    { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString()},
                    { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()},
                    { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()},
                    { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()},
                    { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()},
                    { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()},
                    { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()}
                };
                authClaim.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                // JWT 인증 페이로드 사인 비밀키
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaim,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var SetRedisCache = await RedisService.SetWebUserpageAccessAsync(Convert.ToInt32(userIdx), accessToken, sessionId).ConfigureAwait(false);
                if (SetRedisCache is null)
                    return new ResponseUnit<TokenDTOV2?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                var (access, refresh, session) = SetRedisCache.Value;

                var returnToken = new TokenDTOV2
                {
                    accessToken = access,
                    refreshToken = refresh,
                    sessionId = session
                };

                return new ResponseUnit<TokenDTOV2?>() { message = "로그인 성공(관리자)", data = returnToken, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<TokenDTOV2?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<TokenDTOV2>?> WebLoginRefreshTokenService(RefreshTokenDTOV2 dto)
        {
            try
            {
                if (dto is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(String.IsNullOrWhiteSpace(dto.UserIdx) || String.IsNullOrWhiteSpace(dto.PlaceIdx) || String.IsNullOrWhiteSpace(dto.refreshToken) || String.IsNullOrWhiteSpace(dto.sessionId))
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int userIdx = Convert.ToInt32(dto.UserIdx);
                int placeIdx = Convert.ToInt32(dto.PlaceIdx);
                string oldRefreshToken = dto.refreshToken;

                var UserTB = await UserInfoRepository.GetUserIndexInfo(userIdx).ConfigureAwait(false);
                if (UserTB is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                var PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(placeIdx).ConfigureAwait(false);
                if(PlaceTB is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (PlaceTB.Status == false)
                    return new ResponseUnit<TokenDTOV2>() { message = "해약된 사업장은 접근불가능합니다.", data = null, code = 403 };

                bool AdminYn = UserTB.AdminYn;
                if (AdminYn) //관리자
                {
                    AdminTb? AdminTB = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);
                    if (AdminTB is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 404 };

                    /*
                    * 기본정보
                    */
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", UserTB.AlarmYn.ToString()), // 알람 받을지 여부
                        new Claim("AdminYN", UserTB.AdminYn.ToString()), // 관리자 여부
                        new Claim("UserType", "ADMIN"), // 사용자 타입
                        new Claim("AdminIdx", AdminTB.Id.ToString()), // 관리자 인덱스
                        new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 인덱스명
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };

                    string? adminType = AdminTB.Type switch
                    {
                        "시스템관리자" => "SystemManager",
                        "마스터" => "Master",
                        "매니저" => "Manager",
                        _ => null
                    };

                    if (String.IsNullOrWhiteSpace(adminType))
                        return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    authClaims.Add(new Claim("Role", AdminTB.Type));
                    authClaims.Add(new Claim(ClaimTypes.Role, adminType));

                    /*
                     * 메뉴 접근권한
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화관리 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재관리 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()} // 민원관리 권한
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * Voc 권한
                     */
                    var vocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 권한
                        { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 권한
                        { "VocLift", UserTB.VocLift.ToString()}, // 승강민원 권한
                        { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 권한
                        { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 권한
                        { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 권한
                        { "VocBeauty",UserTB.VocBeauty.ToString()}, // 미화민원 권한
                        { "VocSecurity",UserTB.VocSecurity.ToString()}, // 보안민원 권한
                        { "VocDefault", UserTB.VocEtc.ToString()}, // 기타민원 권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                    /*
                     * 사업장 권한
                     */
                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()},
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString()},
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()},
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()},
                        { "PlacePerm_Construct",PlaceTB.PermConstruct.ToString()},
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()},
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()},
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()},
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()},
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()},
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()}
                    };
                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var newRefreshToken = await RedisService.WebRotateUserpageRefreshTokenAsync(userIdx, dto.refreshToken, dto.sessionId);
                    if (String.IsNullOrWhiteSpace(newRefreshToken))
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var returnDto = new TokenDTOV2
                    {
                        accessToken = newAccessToken,
                        refreshToken = newRefreshToken,
                        sessionId = dto.sessionId
                    };
                    return new ResponseUnit<TokenDTOV2>() { message = "요청이 정상 처리되었습니다.", data = returnDto, code = 200 };
                }
                else // 일반사용자
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", UserTB.AlarmYn.ToString()), // 알람 받을지 여부
                        new Claim("UserType", "User"),
                        new Claim("Role", "User"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 명칭
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };

                    /*
                     * 메뉴 접근권한
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()}
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * Voc 권한
                     */
                    var vocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 처리권한
                        { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 처리권한
                        { "VocLift", UserTB.VocLift.ToString()}, // 승강민원 처리권한
                        { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 처리권한
                        { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 처리권한
                        { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 처리권한
                        { "VocBeauty", UserTB.VocBeauty.ToString()}, // 미화민원 처리권한
                        { "VocSecurity", UserTB.VocSecurity.ToString()}, // 보안민원 처리권한
                        { "VocDefault", UserTB.VocEtc.ToString()} // 기타민원 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                    /*
                     * 사업장 권한
                     */
                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString()}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()}, // 사업장 소방메뉴 권한
                        { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString()}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", PlaceTB.PermMachine.ToString()}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()}, // 사업장 Voc 권한
                    };
                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var newRefreshToken = await RedisService.WebRotateUserpageRefreshTokenAsync(userIdx, dto.refreshToken, dto.sessionId);
                    if (String.IsNullOrWhiteSpace(newRefreshToken))
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var returnDto = new TokenDTOV2
                    {
                        accessToken = newAccessToken,
                        refreshToken = newRefreshToken,
                        sessionId = dto.sessionId
                    };

                    return new ResponseUnit<TokenDTOV2>() { message = "요청이 정상 처리되었습니다.", data = returnDto, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<TokenDTOV2>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 웹 QR 로그인 [V2]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<TokenDTOV2>?> WebQRLoginService(QRLoginDTO dto)
        {
            try
            {
                if (dto is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto.placeid == 0)
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(String.IsNullOrWhiteSpace(dto.UserId) || String.IsNullOrWhiteSpace(dto.UserPassword))
                    return new ResponseUnit<TokenDTOV2>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                var UserTB = await UserInfoRepository.GetUserInfo(dto.UserId, dto.UserPassword).ConfigureAwait(false);
                if(UserTB is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 404 };

                var PlaceTB = await PlaceInfoRepository.GetByPlaceInfo(dto.placeid).ConfigureAwait(false);
                if (PlaceTB is null)
                    return new ResponseUnit<TokenDTOV2>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (PlaceTB.Status == false)
                    return new ResponseUnit<TokenDTOV2>() { message = "해약된 사업장은 접속이 불가능합니다.", data = null, code = 403 };

                if(UserTB.AdminYn)
                {
                    // 관리자
                    AdminTb? AdminTB = await AdminUserInfoRepository.GetAdminUserInfo(UserTB.Id).ConfigureAwait(false);

                    if (AdminTB is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "사용자 정보가 일치하지 않습니다.", data = null, code = 404 };

                    /*
                     * 기본정보
                     */
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN", UserTB.AlarmYn.ToString()), // 알람받을지 여부
                        new Claim("AdminYN", UserTB.AdminYn.ToString()), // 관리자 여부
                        new Claim("UserType","ADMIN"), // 사용자 타입
                        new Claim("AdminIdx", AdminTB.Id.ToString()), // 관리자 인덱스
                        new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 인덱스명
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };

                    string? adminType = AdminTB.Type switch
                    {
                        "시스템관리자" => "SystemManager",
                        "마스터" => "Master",
                        "매니저" => "Manager",
                        _ => null
                    };

                    if (String.IsNullOrWhiteSpace(adminType))
                        return new ResponseUnit<TokenDTOV2>() { message = "관리자 권한이 없습니다.", data = null, code = 403 };

                    authClaims.Add(new Claim("Role", AdminTB.Type));
                    authClaims.Add(new Claim(ClaimTypes.Role, adminType));

                    /*
                     * 메뉴 접근권한
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화관리 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재관리 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()} // 민원관리 권한
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * 사업장 권한
                     */
                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()},
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString()},
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()},
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()},
                        { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString()},
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()},
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()},
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()},
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()},
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()},
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()}
                    };
                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var SetRedisCache = await RedisService.SetWebUserpageAccessAsync(UserTB.Id, accessToken).ConfigureAwait(false);
                    if (SetRedisCache is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var (access, refresh, session) = SetRedisCache.Value;

                    var returnDto = new TokenDTOV2
                    {
                        accessToken = access,
                        refreshToken = refresh,
                        sessionId = session
                    };

                    return new ResponseUnit<TokenDTOV2>() { message = "로그인 성공(관리자).", data = returnDto, code = 200 };
                }
                else
                {
                    // 일반유저
                    /*
                     * 기초 정보
                     */
                    var authClaims = new List<Claim>
                    {
                        new Claim("UserIdx", UserTB.Id.ToString()), // UserID
                        new Claim("Name", UserTB.Name!.ToString()), // UserName
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("AlarmYN",UserTB.AlarmYn.ToString()), // 알람 받을지 여부
                        new Claim("UserType", "User"),
                        new Claim("Role", "User"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("PlaceIdx", PlaceTB.Id.ToString()), // 사업장 인덱스
                        new Claim("PlaceName", PlaceTB.Name.ToString()), // 사업장 명칭
                        new Claim("PlaceCreateDT", PlaceTB.CreateDt.ToString("yyyy-MM-dd")) // 사업장 생성일
                    };

                    /*
                     * 메뉴 접근권한
                     */
                    var userPermissions = new JObject
                    {
                        { "UserPerm_Basic", UserTB.PermBasic.ToString()}, // 기본정보 권한
                        { "UserPerm_Machine", UserTB.PermMachine.ToString()}, // 기계관리 권한
                        { "UserPerm_Elec", UserTB.PermElec.ToString()}, // 전기관리 권한
                        { "UserPerm_Lift", UserTB.PermLift.ToString()}, // 승강관리 권한
                        { "UserPerm_Fire", UserTB.PermFire.ToString()}, // 소방관리 권한
                        { "UserPerm_Construct", UserTB.PermConstruct.ToString()}, // 건축관리 권한
                        { "UserPerm_Network", UserTB.PermNetwork.ToString()}, // 통신관리 권한
                        { "UserPerm_Beauty", UserTB.PermBeauty.ToString()}, // 미화 권한
                        { "UserPerm_Security", UserTB.PermSecurity.ToString()}, // 보안 권한
                        { "UserPerm_Material", UserTB.PermMaterial.ToString()}, // 자재 권한
                        { "UserPerm_Energy", UserTB.PermEnergy.ToString()}, // 에너지관리 권한
                        { "UserPerm_User", UserTB.PermUser.ToString()}, // 사용자관리 권한
                        { "UserPerm_Voc", UserTB.PermVoc.ToString()} // 민원관리 권한
                    };
                    authClaims.Add(new Claim("UserPerms", JsonConvert.SerializeObject(userPermissions)));

                    /*
                     * Voc 권한
                     */
                    var vocPermissions = new JObject
                    {
                        { "VocMachine", UserTB.VocMachine.ToString()}, // 기계민원 처리권한
                        { "VocElec", UserTB.VocElec.ToString()}, // 전기민원 처리권한
                        { "VocLift", UserTB.VocLift.ToString()}, // 승강민원 처리권한
                        { "VocFire", UserTB.VocFire.ToString()}, // 소방민원 처리권한
                        { "VocConstruct", UserTB.VocConstruct.ToString()}, // 건축민원 처리권한
                        { "VocNetwork", UserTB.VocNetwork.ToString()}, // 통신민원 처리권한
                        { "VocBeauty", UserTB.VocBeauty.ToString()}, // 미화민원 처리권한
                        { "VocSecurity", UserTB.VocSecurity.ToString()}, // 보안민원 처리권한
                        { "VocDefault", UserTB.VocEtc.ToString()} // 기타민원 처리권한
                    };
                    authClaims.Add(new Claim("VocPerms", JsonConvert.SerializeObject(vocPermissions)));

                    /*
                     * 사업장 권한
                     */
                    var placePermissions = new JObject
                    {
                        { "PlacePerm_Machine", PlaceTB.PermMachine.ToString()}, // 사업장 기계메뉴 권한
                        { "PlacePerm_Elec", PlaceTB.PermElec.ToString()}, // 사업장 전기메뉴 권한
                        { "PlacePerm_Lift", PlaceTB.PermLift.ToString()}, // 사업장 승강메뉴 권한
                        { "PlacePerm_Fire", PlaceTB.PermFire.ToString()}, // 사업장 소방메뉴 권한
                        { "PlacePerm_Construct", PlaceTB.PermConstruct.ToString()}, // 사업장 건축메뉴 권한
                        { "PlacePerm_Network", PlaceTB.PermNetwork.ToString()}, // 사업장 통신메뉴 권한
                        { "PlacePerm_Beauty", PlaceTB.PermBeauty.ToString()}, // 사업장 미화메뉴 권한
                        { "PlacePerm_Security", PlaceTB.PermSecurity.ToString()}, // 사업장 보안메뉴 권한
                        { "PlacePerm_Material", PlaceTB.PermMaterial.ToString()}, // 사업장 자재메뉴 권한
                        { "PlacePerm_Energy", PlaceTB.PermEnergy.ToString()}, // 사업장 에너지메뉴 권한
                        { "PlacePerm_Voc", PlaceTB.PermVoc.ToString()} // 사업장 Voc 권한
                    };
                    authClaims.Add(new Claim("PlacePerms", JsonConvert.SerializeObject(placePermissions)));

                    // JWT 인증 페이로드 사인 비밀키
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: Configuration["JWT:Issuer"],
                        audience: Configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    string accessToken = new JwtSecurityTokenHandler().WriteToken(token);


                    var SetRedisCache = await RedisService.SetWebUserpageAccessAsync(UserTB.Id, accessToken).ConfigureAwait(false);
                    if (SetRedisCache is null)
                        return new ResponseUnit<TokenDTOV2>() { message = "양식이 잘못되었습니다.", data = null, code = 403 };

                    var (access, refresh, session) = SetRedisCache.Value;

                    var returnToken = new TokenDTOV2
                    {
                        accessToken = access,
                        refreshToken = refresh,
                        sessionId = session
                    };
                    return new ResponseUnit<TokenDTOV2>() { message = "로그인 성공(유저)", data = returnToken, code = 200 };

                }


            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<TokenDTOV2>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 웹 로그아웃
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> WebLogoutService(LogoutDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if(dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                if (String.IsNullOrWhiteSpace(dto.sessionId))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? userIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(userIdx))
                    return new ResponseUnit<bool>() { message = "요청이 잘못되었습니다.", data = false, code = 404 };

                int userId = Convert.ToInt32(userIdx);

                bool DelResult = await RedisService.DeleteRefreshTokenAsync(userId, dto.sessionId).ConfigureAwait(false);
                if (DelResult)
                    return new ResponseUnit<bool>() { message = "로그아웃 되었습니다.", data = true, code = 200 };
                else
                    return new ResponseUnit<bool>() { message = "이미 로그아웃 처리된 아이디입니다.", data = true, code = 403 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }
    }
}
 