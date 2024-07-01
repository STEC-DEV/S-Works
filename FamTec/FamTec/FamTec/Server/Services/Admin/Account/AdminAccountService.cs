using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Server.Repository.User;
using FamTec.Server.Tokens;
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
        private readonly IAdminPlacesInfoRepository AdminPlaceInfoRepository;


        private readonly IConfiguration Configuration;
        private ILogService LogService;


        public AdminAccountService(IUserInfoRepository _userinfoRepository,
            IAdminUserInfoRepository _admininfoRepository,
            IDepartmentInfoRepository _departmentinfoRepository,
            IAdminPlacesInfoRepository _adminplaceinforepository,
            IConfiguration _configuration,
            ILogService _logservice)
        {
            this.UserInfoRepository = _userinfoRepository;
            this.AdminUserInfoRepository = _admininfoRepository;
            this.DepartmentInfoRepository = _departmentinfoRepository;
            this.AdminPlaceInfoRepository = _adminplaceinforepository;


            this.Configuration = _configuration;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 접속화면 서비스
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string>> AdminLoginService(LoginDTO? dto)
        {
            List<Claim> authClaims = new List<Claim>();

            try
            {
                if (String.IsNullOrWhiteSpace(dto.UserID))
                    return new ResponseUnit<string>() { message = "아이디를 입력해주세요.", data = null, code = 200 };
                
                if (String.IsNullOrWhiteSpace(dto.UserPassword))
                    return new ResponseUnit<string>() { message = "비밀번호를 입력해주세요.", data = null, code = 200 };

                UserTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID, dto.UserPassword);

                if (usertb is not null)
                {
                    if (usertb.AdminYn == 1)
                    {
                        AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);

                        if (admintb is not null)
                        {
                            DepartmentTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId);
                            if (departmenttb is not null)
                            {
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
                                return new ResponseUnit<string>() { message = "관리자 로그인 성공.", data = accessToken, code = 200 };
                            }
                            else
                            {
                                // 정보가 잘못되었습니다.
                                return new ResponseUnit<string>() { message = "관리자 로그인 실패.", data = null, code = 401 };
                            }
                        }
                        else
                        {
                            // 관리자 아님
                            return new ResponseUnit<string>() { message = "로그인 실패 (해당 아이디는 관리자가 아닙니다.)", data = null, code = 401 };
                        }
                    }
                    else
                    {
                        // 관리자 아님
                        return new ResponseUnit<string>() { message = "로그인 실패 (해당 아이디는 관리자가 아닙니다.)", data = null, code = 401 };
                    }
                }
                else
                {
                    // 요청이 잘못됨
                    return new ResponseUnit<string>() { message = "로그인 실패 (요청 정보가 잘못되었습니다.)", data = null, code = 404 };
                }
            }
            catch (Exception ex) 
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string>() { message = "로그인 실패 (서버에서 요청을 처리하지 못하였습니다.)", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 관리자 아이디 생성 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> AdminRegisterService(HttpContext? context, AddManagerDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if(dto is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? useridx = Convert.ToString(context.Items["UserIdx"]);
                if(String.IsNullOrWhiteSpace(useridx))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserType = Convert.ToString(context.Items["UserType"]);
                if (String.IsNullOrWhiteSpace(UserType))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UserTb? usermodel = new UserTb
                {
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Password = dto.Password,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    PermBasic = 2,
                    PermMachine = 2,
                    PermLift = 2,
                    PermFire = 2,
                    PermConstruct = 2,
                    PermNetwork = 2,
                    PermBeauty = 2,
                    PermSecurity = 2,
                    PermMaterial = 2,
                    PermEnergy = 2,
                    PermUser = 2,
                    PermVoc = 2,
                    AdminYn = 1,
                    AlramYn = 1,
                    Status = 1,
                    CreateDt = DateTime.Now,
                    CreateUser = creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = creater
                };

                UserTb? userresult = await UserInfoRepository.AddAsync(usermodel);

                if (userresult is not null)
                {
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
                    adminmodel.DepartmentTbId = dto.DepartmentId;

                    AdminTb? adminresult = await AdminUserInfoRepository.AddAdminUserInfo(adminmodel);
                        
                    if (adminresult is not null)
                    {
                        return new ResponseUnit<int?> { message = "요청이 정상 처리되었습니다.", data = adminresult.Id, code = 200 };
                    }
                    else
                    {
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

        public async ValueTask<ResponseUnit<int>> DeleteAdminService(List<int> adminid)
        {
            int count = 0;
            
            try
            {
                if(adminid is [_, ..])
                {
                    for (int i=0;i<adminid.Count;i++)
                    {
                        AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(adminid[i]);

                        if (admintb is not null)
                        {
                            admintb.DelYn = true;
                            admintb.DelDt = DateTime.Now;

                            bool? deleteresult = await AdminUserInfoRepository.DeleteAdminInfo(admintb);
                            
                            if (deleteresult == true)
                            {
                                UserTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId);

                                if (usertb is not null)
                                {
                                    usertb.DelYn = true;
                                    usertb.DelDt = DateTime.Now;

                                    bool? delresult = await UserInfoRepository.DeleteUserInfo(usertb);

                                    if (delresult == true)
                                    {
                                        count++; // 삭제개수 카운팅

                                        List<AdminPlaceTb>? adminplacetb = await AdminPlaceInfoRepository.GetMyWorksList(admintb.Id);
                                        
                                        if (adminplacetb is [_, ..])
                                        {
                                            bool? result = await AdminPlaceInfoRepository.DeleteMyWorks(adminplacetb);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return new ResponseUnit<int> { message = "요청이 처리되지 않았습니다.", data = count, code = 404 };
                        }
                    }

                    return new ResponseUnit<int> { message = $"요청이 {count}건 정상 처리되었습니다.", data = count, code = 200 };
                }
                else
                {
                    return new ResponseUnit<int> { message = "잘못된 요청입니다.", data = count, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int> { message = "서버에서 요청을 처리하지 못하였습니다.", data = count, code = 500 };
            }
        }

      

        /// <summary>
        /// 매니저 상세보기 서비스
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DManagerDTO>> DetailAdminService(int? adminidx)
        {
            try
            {
                if(adminidx is null)
                    return new ResponseUnit<DManagerDTO>() { message = "잘못된 요청입니다.", data = new DManagerDTO(), code = 404 };

                DManagerDTO? dto = await AdminPlaceInfoRepository.GetManagerDetails(adminidx);
                    
                if(dto is not null)
                    return new ResponseUnit<DManagerDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<DManagerDTO>() { message = "요청이 처리되지 않았습니다.", data = new DManagerDTO(), code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DManagerDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DManagerDTO(), code = 500 };
            }
        }
    }
}
