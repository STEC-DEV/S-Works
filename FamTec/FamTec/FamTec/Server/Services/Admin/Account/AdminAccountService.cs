using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Server.Repository.User;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Login;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamTec.Server.Services.Admin.Account
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly IAdminUserInfoRepository AdminUserInfoRepository;
        private readonly IDepartmentInfoRepository DepartmentInfoRepository;
        private IFileService FileService;

        private readonly IConfiguration Configuration;
        private readonly ILogService LogService;

        private readonly ConsoleLogService<AdminAccountService> CreateBuilderLogger;
        DirectoryInfo? di;

        public AdminAccountService(IUserInfoRepository _userinfoRepository,
            IAdminUserInfoRepository _admininfoRepository,
            IDepartmentInfoRepository _departmentinfoRepository,
            IFileService _fileservice,
            IConfiguration _configuration,
            ILogService _logservice,
            ConsoleLogService<AdminAccountService> _createbuilderlogger)
        {
            this.UserInfoRepository = _userinfoRepository;
            this.AdminUserInfoRepository = _admininfoRepository;
            this.DepartmentInfoRepository = _departmentinfoRepository;

            this.FileService = _fileservice;
            this.Configuration = _configuration;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 관리자 이미지 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="adminid"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateAdminImageService(HttpContext context, int adminid, IFormFile? files)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? ImageAddResult = await AdminUserInfoRepository.UpdateAdminImageInfo(adminid, files).ConfigureAwait(false);

                return ImageAddResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 매니저 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateAdminService(HttpContext context, UpdateManagerDTO dto)
        {
            try
            {
                string? creater = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(dto.AdminIndex!.Value).ConfigureAwait(false);
                if (admintb is null) // 받아온 dto의 관리자ID에 해당하는 관리자가 없을때
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 계정정보 변경을 위해 UserTB 조회
                UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId).ConfigureAwait(false);
                if (usertb is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                dto.UserId = dto.UserId!.ToLower(); // UserID 소문자로 변환
                bool? UpdateResult = await AdminUserInfoRepository.UpdateAdminInfo(dto, UserIdx, creater).ConfigureAwait(false);
                return UpdateResult switch
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
        /// 관리자 접속화면 서비스
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<string?>> AdminLoginService(LoginDTO dto)
        {
            try
            {
                UsersTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID!, dto.UserPassword!).ConfigureAwait(false);
                
                if(usertb is null)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (로그인 정보가 올바르지 않습니다.)", data = null, code = 402 };

                if(usertb.AdminYn != true)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (해당 사용자는 관리자가 아닙니다.)", data = null, code = 403 };

                AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id).ConfigureAwait(false);
                if(admintb is null)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (해당 사용자는 관리자가 아닙니다.)", data = null, code = 401 };

                DepartmentsTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId).ConfigureAwait(false);
                if(departmenttb is null)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (로그인 정보가 올바르지 않습니다.)", data = null, code = 200 };
                
                List<Claim> authClaims = new List<Claim>();

                // 로그인성공
                authClaims.Add(new Claim("UserIdx", usertb.Id.ToString()));
                authClaims.Add(new Claim("Name", usertb.Name!.ToString()));
                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                authClaims.Add(new Claim("UserType", "ADMIN"));
                authClaims.Add(new Claim("AdminIdx", admintb.Id.ToString()));
                authClaims.Add(new Claim("DepartIdx", admintb.DepartmentTbId.ToString()));
                authClaims.Add(new Claim("DepartmentName",  departmenttb.Name.ToString()));

                switch(admintb.Type.Trim())
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
                    //expires: DateTime.Now.AddSeconds(10),
                    expires: DateTime.Now.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                // 로그인 성공
                return new ResponseUnit<string?>() { message = "관리자 로그인 성공.", data = accessToken, code = 200 };
            }
            catch (Exception ex) 
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<string?>() { message = "로그인 실패 (서버에서 요청을 처리하지 못하였습니다.)", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 관리자 아이디 생성 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> AdminRegisterService(HttpContext context, AddManagerDTO dto, IFormFile? files)
        {
            try
            {
                string? useridx = Convert.ToString(context.Items["UserIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);
                string? UserType = Convert.ToString(context.Items["Role"]);

                if (String.IsNullOrWhiteSpace(useridx) || String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(UserType))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 새로운 파일명칭 생성 - 없으면 String.Empty;
                string NewFileName = files is not null ? FileService.SetNewFileName(useridx, files) : String.Empty;


                // 관리자 관련한 폴더가 없으면 만듬
                //string AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);
                string AdminFileFolderPath = Path.Combine(Common.FileServer, "Administrator");

                di = new DirectoryInfo(AdminFileFolderPath);
                if (!di.Exists) di.Create();

                UsersTb? AlreadyCheck = await UserInfoRepository.UserIdCheck(!String.IsNullOrWhiteSpace(dto.UserId) ? dto.UserId.Trim() : dto.UserId!).ConfigureAwait(false);
                if (AlreadyCheck is not null)
                    return new ResponseUnit<int?>() { message = "이미 존재하는 아이디입니다.", data = null, code = 204 };

                DateTime ThisTime = DateTime.Now;

                UsersTb model = new UsersTb();
                model.UserId = !String.IsNullOrWhiteSpace(dto.UserId) ? dto.UserId.ToLower().Trim() : dto.UserId!;
                model.Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name;
                model.Password = !String.IsNullOrWhiteSpace(dto.Password) ? dto.Password.Trim() : dto.Password!;
                model.Email = !String.IsNullOrWhiteSpace(dto.Email) ? dto.Email.Trim() : dto.Email;
                model.Phone = !String.IsNullOrWhiteSpace(dto.Phone) ? dto.Phone.Trim() : dto.Phone;
                model.PermBasic = 2;
                model.PermMachine = 2;
                model.PermLift = 2;
                model.PermElec = 2;
                model.PermFire = 2;
                model.PermConstruct = 2;
                model.PermNetwork = 2;
                model.PermBeauty = 2;
                model.PermSecurity = 2;
                model.PermMaterial = 2;
                model.PermEnergy = 2;
                model.PermUser = 2;
                model.PermVoc = 2;
                model.VocMachine = true;
                model.VocElec = true;
                model.VocLift = true;
                model.VocConstruct = true;
                model.VocNetwork = true;
                model.VocBeauty = true;
                model.VocSecurity = true;
                model.VocEtc = true;
                model.Job = "관리자";
                model.AdminYn = true;
                model.AlarmYn = true;
                model.Status = 2;
                model.CreateDt = ThisTime;
                model.CreateUser = !String.IsNullOrWhiteSpace(creater) ? creater.Trim() : creater;
                model.UpdateDt = ThisTime;
                //model.UpdateUser = creater;
                model.UpdateUser = !String.IsNullOrWhiteSpace(creater) ? creater.Trim() : creater;
                model.Image = files is not null ? NewFileName : null;
                

                UsersTb? userresult = await UserInfoRepository.AddAsync(model).ConfigureAwait(false);
                if (userresult is null)
                    return new ResponseUnit<int?> { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };
                
                if (files is not null)
                {
                    // 파일넣기
                    bool? AddFile = await FileService.AddResizeImageFile(NewFileName, AdminFileFolderPath, files).ConfigureAwait(false);
                }

                var adminModel = new AdminTb
                {
                    Type = UserType.Trim() switch
                    {
                        "시스템관리자" => "마스터",
                        "마스터" => "매니저",
                        _ => string.Empty
                    },
                    CreateDt = ThisTime,
                    CreateUser = !String.IsNullOrWhiteSpace(creater) ? creater.Trim() : creater,
                    UpdateDt = ThisTime,
                    UpdateUser = !String.IsNullOrWhiteSpace(creater) ? creater.Trim() : creater,
                    DelYn = false,
                    UserTbId = userresult.Id,
                    DepartmentTbId = dto.DepartmentId!.Value
                };

                AdminTb? adminresult = await AdminUserInfoRepository.AddAdminUserInfo(adminModel).ConfigureAwait(false);
                    
                // 요청이 정상 처리되었을 경우
                if (adminresult is not null)
                {
                    return new ResponseUnit<int?> { message = "요청이 정상 처리되었습니다.", data = adminresult.Id, code = 200 };
                }
                else
                {
                    // 넣었던 이미지가 있으면 삭제
                    if (!String.IsNullOrWhiteSpace(model.Image)) 
                    {
                        FileService.DeleteImageFile(AdminFileFolderPath, model.Image);
                    }

                    return new ResponseUnit<int?> { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> DeleteAdminService(HttpContext context, List<int> adminidx)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                foreach(int AdminID in adminidx)
                {
                    AdminTb? adminTB = await AdminUserInfoRepository.GetAdminIdInfo(AdminID).ConfigureAwait(false);
                    if(adminTB is null)
                        return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                    
                    UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(adminTB.UserTbId).ConfigureAwait(false);
                    if(UserTB is null)
                        return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                    
                    if (UserTB.Job!.Trim().Equals("시스템관리자")) // 시스템 관리자 삭제못하게 막음.
                    {
                        return new ResponseUnit<bool?>() { message = "시스템관리자는 삭제 불가능합니다.", data = null, code = 404 };
                    }
                }

                bool? result = await AdminUserInfoRepository.DeleteAdminsInfo(adminidx, creater).ConfigureAwait(false);
                return result switch
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
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<DManagerDTO>> DetailAdminService(int adminidx)
        {
            try
            {
                AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(adminidx).ConfigureAwait(false);
                if (admintb is null)
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };

                DepartmentsTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId).ConfigureAwait(false);
                if (departmenttb is null)
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };

                UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId).ConfigureAwait(false);
                if(usertb is null)
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };

                DManagerDTO? dto = (from admin in Enumerable.Repeat(admintb, 1) // List 대신 반복(Repeat)을 사용하여 JOIN
                                    join department in Enumerable.Repeat(departmenttb, 1)
                                    on admin.DepartmentTbId equals department.Id
                                    join user in Enumerable.Repeat(usertb, 1)
                                        on admin.UserTbId equals user.Id
                                    select new DManagerDTO
                                    {
                                        DepartmentId = department.Id, // 부서 인덱스
                                        Department = departmenttb.Name, // 부서명
                                        AdminId = admintb.Id, // 관리자테이블 인덱스
                                        UserId = usertb.UserId, // 로그인 ID
                                        Name = usertb.Name, // 사용자 명
                                        Password = usertb.Password, // 사용자 비밀번호
                                        Phone = usertb.Phone, // 전화번호
                                        Email = usertb.Email, // 비밀번호
                                        Type = admintb.Type // 관리자 유형
                                    }).FirstOrDefault();

                if(dto is null)
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };

                //string AdminFileName = String.Format(@"{0}\\Administrator", Common.FileServer);
                string AdminFileName = Path.Combine(Common.FileServer, "Administrator");

                di = new DirectoryInfo(AdminFileName);
                if (!di.Exists) di.Create();

                if(!String.IsNullOrWhiteSpace(usertb.Image))
                {
                    dto.ImageName = usertb.Image; // 이미지명
                    dto.Image = await FileService.GetImageFile(AdminFileName, usertb.Image); // 이미지 Byte[]
                }
                
                return new ResponseUnit<DManagerDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<DManagerDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DManagerDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UserIdCheckService(string userid)
        {
            try
            {
                UsersTb? UserIdCheck = await UserInfoRepository.UserIdCheck(userid).ConfigureAwait(false);
                if (UserIdCheck is not null)
                {
                    // 이미 사용중인 아이디
                    return new ResponseUnit<bool?>() { message = "이미 사용중인 아이디입니다.", data = false, code = 200 };
                }
                else
                {
                    // 가능
                    return new ResponseUnit<bool?>() { message = "사용가능한 아이디입니다..", data = true, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

   
    }
}
