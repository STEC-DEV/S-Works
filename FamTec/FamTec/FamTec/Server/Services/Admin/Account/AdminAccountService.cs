using DocumentFormat.OpenXml.Office2013.Excel;
using FamTec.Server.Repository.Admin.AdminPlaces;
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

                UsersTb? usertb = await UserInfoRepository.GetUserInfo(dto.UserID, dto.UserPassword);

                if (usertb is not null)
                {
                    if (usertb.AdminYn == true)
                    {
                        AdminTb? admintb = await AdminUserInfoRepository.GetAdminUserInfo(usertb.Id);

                        if (admintb is not null)
                        {
                            DepartmentsTb? departmenttb = await DepartmentInfoRepository.GetDepartmentInfo(admintb.DepartmentTbId);
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
        public async ValueTask<ResponseUnit<int?>> AdminRegisterService(HttpContext? context, AddManagerDTO? dto, IFormFile? files)
        {
            try
            {
                DirectoryInfo? di;
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

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

                string? UserType = Convert.ToString(context.Items["Role"]);
                if (String.IsNullOrWhiteSpace(UserType))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                // 관리자 관련한 폴더가 없으면 만듬
                string AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);

                di = new DirectoryInfo(AdminFileFolderPath);
                if (!di.Exists) di.Create();



                UsersTb? model = new UsersTb();

                model.UserId = dto.UserId;
                model.Name = dto.Name;
                model.Password = dto.Password;
                model.Email = dto.Email;
                model.Phone = dto.Phone;
                
                /* 메뉴관련 권한 */
                model.PermBasic = 2;
                model.PermMachine = 2;
                model.PermLift = 2;
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
                
                if(files is not null)
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if(!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<int?>() { message = "올바르지 않은 파일형식입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        // 이미지경로
                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}";
                        string? newFilePath = Path.Combine(AdminFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                }
                else
                {
                    model.Image = null;
                }

                UsersTb? userresult = await UserInfoRepository.AddAsync(model);

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

        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="useridx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> DeleteAdminService(HttpContext? context,List<int>? adminidx)
        {
            int delcount = 0;
            
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (adminidx is null)
                    return new ResponseUnit<int?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };


                for (int i=0;i< adminidx.Count();i++)
                {
                    AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(adminidx[i]);

                    if (admintb is null)
                        return new ResponseUnit<int?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                    
                    admintb.DelYn = true;
                    admintb.DelDt = DateTime.Now;
                    admintb.DelUser = creater;

                    bool? deladmintb = await AdminUserInfoRepository.DeleteAdminInfo(admintb);
                    if(deladmintb != true)
                        return new ResponseUnit<int?>() { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };

                    UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId);
                    usertb!.DelYn = true;
                    usertb!.DelDt = DateTime.Now;
                    usertb.DelUser = creater;

                    bool? delusertb = await UserInfoRepository.DeleteUserInfo(usertb);

                    if (delusertb == true)
                    {
                        List<AdminPlaceTb>? adminplacetb = await AdminPlaceInfoRepository.GetMyWorksList(admintb.Id);
                                        
                        if (adminplacetb is [_, ..])
                        {
                            for (int j = 0; j < adminplacetb.Count(); j++)
                            {
                                adminplacetb[j].DelYn = true;
                                adminplacetb[j].DelDt = DateTime.Now;
                                adminplacetb[j].DelUser = creater;

                                bool? result = await AdminPlaceInfoRepository.DeleteMyWorks(adminplacetb[j]);
                            }
                        }
                        delcount++;
                    }
                }
                return new ResponseUnit<int?> { message = $"요청이 {delcount}건 정상 처리되었습니다.", data = delcount, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
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

                            string? Image = usertb.Image;
                            if(!String.IsNullOrWhiteSpace(Image))
                            {
                                string AdminFileName = String.Format(@"{0}\\Administrator", Common.FileServer);
                                string[] FileList = Directory.GetFiles(AdminFileName);

                                if(FileList is [_, ..])
                                {
                                    foreach(var file in FileList)
                                    {
                                        if(file.Contains(Image))
                                        {
                                            byte[] ImageBytes = File.ReadAllBytes(file);
                                            dto.Image = Convert.ToBase64String(ImageBytes);
                                        }
                                    }
                                }
                                else
                                {
                                    dto.Image = usertb.Image;
                                }
                            }
                            else
                            {
                                dto.Image = usertb.Image;
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
        public async ValueTask<ResponseUnit<bool?>> UserIdCheckService(string? userid)
        {
            try
            {
                if (userid is not null)
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
                else
                {
                    // 잘못된 요청입니다.
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
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
        public async ValueTask<ResponseUnit<int?>> UpdateAdminService(HttpContext? context, UpdateManagerDTO? dto, IFormFile? files)
        {
            try
            {
                int updatecount = 0;
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;
                string? AdminFileFolderPath = String.Empty;

                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                AdminTb? admintb = await AdminUserInfoRepository.GetAdminIdInfo(dto.AdminIndex);
                if(admintb is null) // 받아온 dto의 관리자ID에 해당하는 관리자가 없을때
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 계정정보 변경을 위해 UserTB 조회
                UsersTb? usertb = await UserInfoRepository.GetUserIndexInfo(admintb.UserTbId);
                if(usertb is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UsersTb? UserIdcheck = await UserInfoRepository.UserIdCheck(dto.UserId);
                if (UserIdcheck is not null)
                    return new ResponseUnit<int?>() { message = "이미 사용중인 아이디입니다.", data = null, code = 200 };


                usertb.Name = dto.Name; // 이름
                usertb.Phone = dto.Phone; // 전화번호
                usertb.Email = dto.Email; // 이메일
                usertb.UserId = dto.UserId; // 로그인 ID
                usertb.Password = dto.Password; // 비밀번호
                
                if(files is not null) // 파일이 공백이 아닌경우 - 삭제 - 업데이트 or insert
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if(!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<int?>() { message = "이미지의 형식이 올바르지 않습니다.", data = null, code = 404 };
                    }

                    string? filePath = usertb.Image;
                    AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);

                    if(!String.IsNullOrWhiteSpace(filePath))
                    {
                        FileName = String.Format("{0}\\{1}", AdminFileFolderPath, filePath);
                        if(File.Exists(FileName))
                        {
                            File.Delete(FileName);
                        }
                    }

                    string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}";
                    string? newFilePath = Path.Combine(AdminFileFolderPath, newFileName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await files.CopyToAsync(fileStream);
                        usertb.Image = newFileName;
                    }
                }
                else // 파일이 공백인경우 db에 해당 데이터 값이 있으면 삭제
                {
                    string? filePath = usertb.Image;
                    if(!String.IsNullOrWhiteSpace(filePath))
                    {
                        AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);
                        FileName = String.Format("{0}\\{1}", AdminFileFolderPath, filePath);
                        if(File.Exists(FileName))
                        {
                            File.Delete(FileName);
                            usertb.Image = null;
                        }
                    }
                }


                // 사용자 정보 수정
                UsersTb? updateusertb = await UserInfoRepository.UpdateUserInfo(usertb);
                if (updateusertb is null)
                {
                    // 사용자 테이블 업데이트 실패했을 경우
                    return new ResponseUnit<int?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 200 };
                }

                admintb.DepartmentTbId = dto.DepartmentId; // 부서
                // 관리자 정보 수정
                AdminTb? updateadmintb = await AdminUserInfoRepository.UpdateAdminInfo(admintb);
                if (updateadmintb is null)
                {
                    // 관리자 테이블 업데이트 실패했을 경우
                    return new ResponseUnit<int?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 200 };
                }

                updatecount++;

                List<int?> placeidx =  dto.PlaceList.Select(m => m.Id).ToList();

                // 수정 - 삭제 튜플반환
                (List<int?>? insert, List<int?>? delete)? temp = await AdminPlaceInfoRepository.DisassembleUpdateAdminInfo(admintb.Id, placeidx);
                
                // 튜플 분해
                List<int?>? insertidx = (((List<int?>?,List<int?>?))temp).Item1;
                List<int?>? deleteidx = (((List<int?>?, List<int?>?))temp).Item2;

                // insert 할것이 있으면
                if (insertidx is [_, ..])
                {
                    for (int i = 0; i < insertidx.Count; i++)
                    {
                        AdminPlaceTb inserttb = new AdminPlaceTb()
                        {
                            AdminTbId = dto.AdminIndex,
                            CreateDt = DateTime.Now,
                            CreateUser = creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = creater,
                            PlaceTbId = insertidx[i]
                        };

                        AdminPlaceTb? insertresult = await AdminPlaceInfoRepository.AddAdminPlaceInfo(inserttb);
                        if(insertresult is not null)
                        {
                            updatecount++;
                        }
                    }
                }

                // delete 할것이 있으면
                if (deleteidx is [_, ..])
                {
                    // delete 할것
                    for (int i = 0; i < deleteidx.Count; i++)
                    {
                        AdminPlaceTb? deletetb = await AdminPlaceInfoRepository.GetPlaceAdminInfo(admintb.Id, deleteidx[i]);
                        if(deletetb is not null) // 모델이 있으면 삭제
                        {
                            bool? deleteresult = await AdminPlaceInfoRepository.DeleteAdminPlaceInfo(deletetb);
                            if(deleteresult == true)
                            {
                                updatecount++;
                            }
                        }
                        else
                        {
                            return new ResponseUnit<int?>() { message = "이미 처리된 요청입니다.", data = null, code = 200 };
                        }
                    }
                }

                return new ResponseUnit<int?>() { message = $"요청을 {updatecount}건 처리하였습니다.", data = updatecount, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
