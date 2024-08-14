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
        private ILogService LogService;
        
        DirectoryInfo? di;

        public AdminAccountService(IUserInfoRepository _userinfoRepository,
            IAdminUserInfoRepository _admininfoRepository,
            IDepartmentInfoRepository _departmentinfoRepository,
            IFileService _fileservice,
            IConfiguration _configuration,
            ILogService _logservice)
        {
            this.UserInfoRepository = _userinfoRepository;
            this.AdminUserInfoRepository = _admininfoRepository;
            this.DepartmentInfoRepository = _departmentinfoRepository;

            this.FileService = _fileservice;
            this.Configuration = _configuration;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 접속화면 서비스
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> AdminLoginService(LoginDTO dto)
        {
            try
            {
                UsersTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID!, dto.UserPassword!);
                
                if(usertb is null)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (로그인 정보가 올바르지 않습니다.)", data = null, code = 200 };

                if(usertb.AdminYn != true)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (해당 사용자는 관리자가 아닙니다.)", data = null, code = 200 };

                AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);
                if(admintb is null)
                    return new ResponseUnit<string?>() { message = "로그인 실패 (해당 사용자는 관리자가 아닙니다.)", data = null, code = 401 };

                DepartmentsTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId);
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


                // JWT 인증 페이로드 사인 비밀키
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:authSigningKey"]!));

                var token = new JwtSecurityToken(
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
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
                return new ResponseUnit<string?>() { message = "로그인 실패 (서버에서 요청을 처리하지 못하였습니다.)", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 관리자 아이디 생성 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> AdminRegisterService(HttpContext context, AddManagerDTO dto, IFormFile? files)
        {
            try
            {
                string? useridx = Convert.ToString(context.Items["UserIdx"]);
                if(String.IsNullOrWhiteSpace(useridx))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserType = Convert.ToString(context.Items["Role"]);
                if (String.IsNullOrWhiteSpace(UserType))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(useridx, files);
                }

                // 관리자 관련한 폴더가 없으면 만듬
                string AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);
                di = new DirectoryInfo(AdminFileFolderPath);
                if (!di.Exists) di.Create();

                UsersTb? AlreadyCheck = await UserInfoRepository.UserIdCheck(dto.UserId!);
                if(AlreadyCheck is not null)
                    return new ResponseUnit<int?>() { message = "이미 존재하는 아이디입니다.", data = null, code = 204 };

                UsersTb? model = new UsersTb();
                model.UserId = dto.UserId!;
                model.Name = dto.Name;
                model.Password = dto.Password!;
                model.Email = dto.Email;
                model.Phone = dto.Phone;
                
                /* 메뉴관련 권한 */
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

                /* VOC관련 권한 */
                model.VocMachine = true;
                model.VocElec = true;
                model.VocLift = true;
                model.VocFire = true;
                model.VocConstruct = true;
                model.VocNetwork = true;
                model.VocBeauty = true;
                model.VocSecurity = true;
                model.VocEtc = true;
                model.Job = "관리자";

                model.AdminYn = true;
                model.AlarmYn = true;
                model.Status = 2;
                model.CreateDt = DateTime.Now;
                model.CreateUser = creater;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;
                
                if (files is not null)
                {
                    model.Image = NewFileName;
                }
                else
                {
                    model.Image = null;
                }

                UsersTb? userresult = await UserInfoRepository.AddAsync(model);
                if (userresult is not null)
                {
                    if(files is not null)
                    {
                        // 파일넣기
                        bool? AddFile = await FileService.AddImageFile(NewFileName, AdminFileFolderPath, files);
                    }

                    AdminTb? adminmodel = new AdminTb();
                        
                    if (UserType == "시스템관리자")
                        adminmodel.Type = "마스터";
                    if (UserType == "마스터")
                        adminmodel.Type = "매니저";
                        
                    adminmodel.CreateDt = DateTime.Now;
                    adminmodel.CreateUser = creater;
                    adminmodel.UpdateDt = DateTime.Now;
                    adminmodel.UpdateUser = creater;
                    adminmodel.DelYn = false;
                    adminmodel.UserTbId = userresult.Id;
                    adminmodel.DepartmentTbId = dto.DepartmentId!.Value;

                    AdminTb? adminresult = await AdminUserInfoRepository.AddAdminUserInfo(adminmodel);
                    
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
                else
                {
                    return new ResponseUnit<int?> { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteAdminService(HttpContext context, List<int> adminidx)
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
                    AdminTb? adminTB = await AdminUserInfoRepository.GetAdminIdInfo(AdminID);
                    if(adminTB is null)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                    }
                    else
                    {
                        UsersTb? UserTB = await UserInfoRepository.GetUserIndexInfo(adminTB.UserTbId);
                        if(UserTB is null)
                        {
                            return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                        }
                        else
                        {
                            // 시스템 관리자 삭제못하게 막음.
                            if (UserTB.Job!.Equals("시스템관리자"))
                            {
                                return new ResponseUnit<bool?>() { message = "시스템관리자는 삭제 불가능합니다.", data = null, code = 404 };
                            }
                        }
                    }
                }

                bool? result = await AdminUserInfoRepository.DeleteAdminsInfo(adminidx, creater);
                if (result == true)
                {
                    return new ResponseUnit<bool?> { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if (result == false)
                {
                    return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
                else
                {
                    return new ResponseUnit<bool?> { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      

        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DManagerDTO>> DetailAdminService(int adminidx)
        {
            try
            {
                AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(adminidx);
                if(admintb is not null)
                {
                    DepartmentsTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId);
                    if(departmenttb is not null)
                    {
                        UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId);
                        if(usertb is not null)
                        {
                            DManagerDTO dto = new DManagerDTO();
                            dto.UserId = usertb.UserId;
                            dto.Name = usertb.Name;
                            dto.Password = usertb.Password;
                            dto.Phone = usertb.Phone;
                            dto.Email = usertb.Email;
                            dto.Type = admintb.Type;
                            dto.Department = departmenttb.Name;

                            string AdminFileName = String.Format(@"{0}\\Administrator", Common.FileServer);
                            if(!String.IsNullOrWhiteSpace(usertb.Image))
                            {
                                dto.Image = await FileService.GetImageFile(AdminFileName, usertb.Image);
                            }
                            return new ResponseUnit<DManagerDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                        }
                        else
                        {
                            return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DManagerDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DManagerDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 아이디 중복검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UserIdCheckService(string userid)
        {
            try
            {
                UsersTb? UserIdCheck = await UserInfoRepository.UserIdCheck(userid);
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
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 매니저 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateAdminService(HttpContext context, UpdateManagerDTO dto, IFormFile? files)
        {
            try
            {
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (string.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(dto.AdminIndex!.Value);
                if(admintb is null) // 받아온 dto의 관리자ID에 해당하는 관리자가 없을때
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 계정정보 변경을 위해 UserTB 조회
                UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId);
                if(usertb is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };



                bool? UpdateResult = await AdminUserInfoRepository.UpdateAdminInfo(dto, UserIdx, creater, files);

                if(UpdateResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if(UpdateResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
