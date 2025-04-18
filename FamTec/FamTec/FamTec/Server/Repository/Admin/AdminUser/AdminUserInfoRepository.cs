using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Admin.AdminUser
{
    public class AdminUserInfoRepository : IAdminUserInfoRepository
    {
        private readonly WorksContext context;
        private DirectoryInfo di = null;
        
        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<AdminUserInfoRepository> CreateBuilderLogger;

        public AdminUserInfoRepository(WorksContext _context, 
            IFileService _fileservice,
            ILogService _logservice,
            ConsoleLogService<AdminUserInfoRepository> _createbuilderlogger)
        {
            this.context = _context;

            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 해당 사업장에 해당하는 전체 관리자들 리턴
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<PlaceUserDTO>> GetAdminPlaceList(int placeid)
        {
            try
            {
                IQueryable<PlaceUserDTO> adminUsers =
                    from adp in context.AdminPlaceTbs
                    join ad in context.AdminTbs
                        on adp.AdminTbId equals ad.Id
                    join ut in context.UsersTbs
                        on ad.UserTbId equals ut.Id
                    where adp.PlaceTbId == 1
                    select new PlaceUserDTO
                    {
                        userIdx = ut.Id,
                        userName = ut.Name
                    };


                IQueryable<PlaceUserDTO> Users = context.UsersTbs
                    .Where(ut => ut.PlaceTbId == 1)
                    .Select(ut => new PlaceUserDTO
                    {
                        userIdx = ut.Id,
                        userName = ut.Name
                    });

                if(adminUsers is null)
                {
                    return await Users.ToListAsync();
                }

                if(Users is null)
                {
                    return await adminUsers.ToListAsync();
                }

                IQueryable<PlaceUserDTO> combined = adminUsers.Union(Users);
                return await combined.ToListAsync();
            }
            catch(Exception ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        /// <summary>
        /// 관리자 추가
        /// </summary>
        /// <returns></returns>
        public async Task<AdminTb?> AddAdminUserInfo(AdminTb model)
        {
            try
            {
                await context.AdminTbs.AddAsync(model).ConfigureAwait(false);

                bool Addresult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
             
                if(Addresult)
                    return model;
                else
                    return null;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteAdminInfo(AdminTb model)
        {
            try
            {
                context.AdminTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        public async Task<bool?> DeleteAdminsInfo(List<int> idx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        foreach (int adminid in idx)
                        {
                            AdminTb? admintb = await context.AdminTbs
                            .FirstOrDefaultAsync(m => m.Id == adminid && 
                                                      m.DelYn != true)
                            .ConfigureAwait(false);

                            if (admintb is not null)
                            {
                                admintb.DelYn = true;
                                admintb.DelDt = ThisDate;
                                admintb.DelUser = deleter;
                                context.AdminTbs.Update(admintb);
                                bool AdminTbResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                                if (!AdminTbResult) // 실패하면
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                    return false;
                                }

                                UsersTb? usertb = await context.UsersTbs
                                .FirstOrDefaultAsync(m => m.Id == admintb.UserTbId && m.DelYn != true)
                                .ConfigureAwait(false);

                                if (usertb is not null)
                                {
                                    // 해당 UserId 재사용을 위함
                                    usertb.UserId = $"{usertb.UserId}_{usertb.Id}";
                                    usertb.DelYn = true;
                                    usertb.DelDt = ThisDate;
                                    usertb.DelUser = deleter;
                                    context.UsersTbs.Update(usertb);
                                    bool UserTbResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!UserTbResult) // 실패하면
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                        return false;
                                    }

                                    List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                                    .Where(m => m.AdminTbId == admintb.Id && m.DelYn != true)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                                    if (adminplacetb is [_, ..])
                                    {
                                        foreach (AdminPlaceTb AdminPlace in adminplacetb)
                                        {
                                            context.AdminPlaceTbs.Remove(AdminPlace);
                                            bool AdminPlaceResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                            if (!AdminPlaceResult) // 실패하면
                                            {
                                                await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                                return false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                    return false;
                                }
                            }
                            else
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false); // 롤백
                                return false;
                            }
                        }
                        await transaction.CommitAsync().ConfigureAwait(false); // 커밋
                        return true;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                }
            });
        }

   

        /// <summary>
        /// 매개변수의 ADMINID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async Task<AdminTb?> GetAdminIdInfo(int adminid)
        {
            try
            {
                AdminTb? model = await context.AdminTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(adminid) && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 매개변수의 USERID에 해당하는 관리자모델 모델 조회
        /// </summary>
        /// <param name="adminuseridx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AdminTb?> GetAdminUserInfo(int usertbid)
        {
            try
            {
                AdminTb? model = await context.AdminTbs
                    .FirstOrDefaultAsync(m => m.UserTbId.Equals(usertbid) && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 관리자 DTO 반환
        /// </summary>
        /// <returns></returns>
        public async Task<List<ManagerListDTO>?> GetAllAdminUserList()
        {
            try
            {
                List<ManagerListDTO>? model = await context.AdminTbs
                    .Where(m => m.DelYn != true)
                    .Include(m => m.UserTb)
                    .Include(m => m.DepartmentTb)
                    .Select(m => new ManagerListDTO
                    {
                        Id = m.Id,
                        UserId = m.UserTb!.UserId!,
                        Name = m.UserTb.Name!,
                        Department = m.DepartmentTb!.Name!,
                        Type = m.Type
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 해당 사업장 관리자로 선택가능한 사업장 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<ManagerListDTO>?> GetNotContainsAdminList(int placeid)
        {
            try
            {
                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                    .Where(m => m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (adminplacetb is [_, ..])
                {
                    List<int> admintbid = adminplacetb.Select(m => m.AdminTbId).ToList();

                    List<AdminTb>? admintb = await context.AdminTbs
                        .Where(e => !admintbid.Contains(e.Id) && e.DelYn != true)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    if (admintb is [_, ..])
                    {
                        List<ManagerListDTO>? model = (from Admin in admintb
                                                        join User in context.UsersTbs.Where(m => m.DelYn != true).ToList()
                                                        on Admin.UserTbId equals User.Id
                                                        join Department in context.DepartmentsTbs.Where(m => m.DelYn != true).ToList()
                                                        on Admin.DepartmentTbId equals Department.Id
                                                        select new ManagerListDTO()
                                                        {
                                                            Id = Admin.Id,
                                                            Name = User.Name,
                                                            UserId = User.UserId,
                                                            Department = Department.Name
                                                        }).ToList();

                        return model;
                    }
                    else
                    {
                        return null; // 전체 관리자 다 이사업장에 포함됨.
                    }
                }
                else
                {
                    // 이사업장에 아무도 포함되어있지 않음
                    List<ManagerListDTO>? model = (from Admin in context.AdminTbs.Where(m => m.DelYn != true).ToList()
                                                    join User in context.UsersTbs.Where(m => m.DelYn != true).ToList()
                                                    on Admin.UserTbId equals User.Id
                                                    join Department in context.DepartmentsTbs.Where(m => m.DelYn != true).ToList()
                                                    on Admin.DepartmentTbId equals Department.Id
                                                    select new ManagerListDTO()
                                                    {
                                                        Id = Admin.Id,
                                                        Name = User.Name,
                                                        UserId = User.UserId,
                                                        Department = Department.Name
                                                    }).ToList();

                    return model;
                }
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AdminTb?> UpdateAdminInfo(AdminTb model)
        {
            try
            {
                context.AdminTbs.Update(model);
                bool UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (UpdateResult)
                    return model;
                else
                    return null;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 업데이트 이미지
        /// </summary>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateAdminImageInfo(int adminid, IFormFile? files)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // 파일처리 준비
                        string NewFileName = String.Empty;
                        string deleteFileName = String.Empty;

                        // 수정실패 시 돌려놓을 FormFile
                        IFormFile? AddTemp = default;
                        string RemoveTemp = String.Empty;

                        AdminTb? AdminInfo = await context.AdminTbs
                        .FirstOrDefaultAsync(m => m.Id == adminid && m.DelYn != true)
                        .ConfigureAwait(false);

                        if (AdminInfo is null)
                            return false;

                        UsersTb? UserInfo = await context.UsersTbs
                        .FirstOrDefaultAsync(m => m.Id == AdminInfo.UserTbId &&
                                                  m.DelYn != true)
                        .ConfigureAwait(false);

                        if (UserInfo is null)
                            return false;

                        //string AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);
                        string AdminFileFolderPath = Path.Combine(Common.FileServer, "Administrator");

                        di = new DirectoryInfo(AdminFileFolderPath);
                        if (!di.Exists) di.Create();

                        // 파일이 공백이 아닌 경우
                        if (files is not null)
                        {
                            if (files.FileName != UserInfo.Image)
                            {
                                if (!String.IsNullOrWhiteSpace(UserInfo.Image))
                                {
                                    deleteFileName = UserInfo.Image;
                                }

                                // 새로운 파일명 설정
                                string newFileName = FileService.SetNewFileName(UserInfo.Id.ToString(), files);
                                NewFileName = newFileName; // 파일명 리스트에 추가
                                UserInfo.Image = newFileName; // DB Image명칭 업데이트

                                RemoveTemp = newFileName;
                            }
                        }
                        else // 파일이 공백인 경우
                        {
                            if (!String.IsNullOrWhiteSpace(UserInfo.Image))
                            {
                                deleteFileName = UserInfo.Image; // 기존 파일 삭제 목록에 추가
                                UserInfo.Image = null; // 모델의 파일명 비우기
                            }
                        }

                        // 먼저 파일 삭제 처리
                        // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                        byte[]? ImageBytes = await FileService.GetImageFile(AdminFileFolderPath, deleteFileName).ConfigureAwait(false);
                        // - DB 실패했을경우 IFormFile을 바이트로 변환해서 DB의 해당명칭으로 다시저장해야함.
                        if (ImageBytes is not null)
                        {
                            AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                        }

                        // 파일삭제
                        if (!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            FileService.DeleteImageFile(AdminFileFolderPath, deleteFileName);
                        }

                        // 새 파일 저장
                        if (files is not null)
                        {
                            if (String.IsNullOrWhiteSpace(UserInfo.Image) || files.FileName != UserInfo.Image)
                            {
                                // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                                await FileService.AddResizeImageFile(UserInfo.Image!, AdminFileFolderPath, files).ConfigureAwait(false);
                            }
                        }

                        // 이후 데이터베이스 업데이트
                        context.UsersTbs.Update(UserInfo);

                        bool UpdateImageResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (UpdateImageResult)
                        {
                            // 성공했으면 그걸로 끝.
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            // 실패했으면 파일을 원래대로 돌려놔야함.
                            if (AddTemp is not null)
                            {
                                try
                                {
                                    if (FileService.IsFileExists(AdminFileFolderPath, AddTemp.FileName) == false)
                                    {
                                        // 파일을 저장하는 로직 (AddResizeImageFile)
                                        await FileService.AddResizeImageFile(AddTemp.FileName, AdminFileFolderPath, AddTemp).ConfigureAwait(false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogService.LogMessage($"파일 복원실패 : {ex.Message}");
#if DEBUG
                                    CreateBuilderLogger.ConsoleLog(ex);
#endif
                                }
                            }

                            if (!String.IsNullOrWhiteSpace(RemoveTemp))
                            {
                                try
                                {
                                    FileService.DeleteImageFile(AdminFileFolderPath, RemoveTemp);
                                }
                                catch (Exception ex)
                                {
                                    LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
#if DEBUG
                                    CreateBuilderLogger.ConsoleLog(ex);
#endif
                                }
                            }

                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return false;
                    }
                }
            });
        }

        /// <summary>
        /// 업데이트 수정로직
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateAdminInfo(UpdateManagerDTO dto, string UserIdx, string creater)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        AdminTb? adminTB = await context.AdminTbs
                            .FirstOrDefaultAsync(m => m.Id == dto.AdminIndex && 
                                                      m.DelYn != true)
                            .ConfigureAwait(false);

                        if (adminTB is null)
                            return (bool?)null;

                        // 계정정보 변경을 위해 조회
                        UsersTb? userTB = await context.UsersTbs
                            .FirstOrDefaultAsync(m => m.Id == adminTB.UserTbId && 
                                                      m.DelYn != true)
                            .ConfigureAwait(false);

                        if (userTB is null)
                            return (bool?)null;

                        if (!adminTB.Type.Equals("시스템관리자"))
                        {
                            if (dto.Name != userTB.Name)
                            {
                                userTB.Name = dto.Name; // 이름
                            }
                            userTB.Phone = dto.Phone; // 전화번호
                            userTB.Email = dto.Email; // 이메일
                            userTB.UserId = dto.UserId!; // 로그인ID
                            userTB.Password = dto.Password!; // 비밀번호
                            userTB.UpdateDt = ThisDate; // 수정시간
                            userTB.UpdateUser = creater; // 수정자

                            context.UsersTbs.Update(userTB);

                            bool UserUpdate = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if (!UserUpdate) // 업데이트 실패 - 롤백
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                        }

                        if (!adminTB.Type.Equals("시스템관리자"))
                        {
                            adminTB.DepartmentTbId = dto.DepartmentId!.Value; // 부서
                            adminTB.UpdateDt = ThisDate;
                            adminTB.UpdateUser = creater;
                            context.AdminTbs.Update(adminTB);

                            bool AdminUpdate = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if (!AdminUpdate) // 업데이트 실패 - 롤백
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                        }

                        // DTO의 PlaceID -- 최종 결과물이 되야할 것들
                        List<int> DTOPlaceIdx = dto.PlaceList!.Select(m => m.Id!.Value).ToList();

                        // 이 관리자의 관리하고있는 사업장ID 조회
                        List<int> AdminPlaceIdx = await context.AdminPlaceTbs
                            .Where(m => m.AdminTbId == adminTB.Id && 
                                        m.DelYn != true)
                            .Select(m => m.PlaceTbId)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        List<int> insertplaceidx = new List<int>();
                        List<int> deleteplacecidx = new List<int>();

                        if (DTOPlaceIdx is [_, ..]) // 넘어온 사업장이 있으면
                        {
                            if (AdminPlaceIdx is [_, ..]) // 해당 관리자의 사업장이 있으면
                            {
                                // 내가갖고 있는것
                                List<int>? MineList = await context.AdminPlaceTbs
                                    .Where(m => DTOPlaceIdx.Contains(Convert.ToInt32(m.PlaceTbId)) &&
                                                m.DelYn != true &&
                                                m.AdminTbId == adminTB.Id)
                                    .Select(m => m.PlaceTbId)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                                if (MineList is [_, ..]) // 가지고 있는게 있으면
                                {
                                    // 추가해야할 것
                                    insertplaceidx = DTOPlaceIdx.Except(AdminPlaceIdx).ToList();
                                    // 삭제해야할것
                                    deleteplacecidx = AdminPlaceIdx.Except(MineList).ToList();

                                }
                                else // 검색 후 내가 가진게 없으면
                                {
                                    // 전부다 ISERT
                                    insertplaceidx = DTOPlaceIdx;
                                    deleteplacecidx = AdminPlaceIdx;
                                }
                            }
                            else // 가지고있는게 아에 없으면
                            {
                                // 들어온거 INSERT
                                insertplaceidx = DTOPlaceIdx;
                            }
                        }
                        else
                        {
                            // 없으면 -->
                            // AllPlaceIdx --> 내용 모두 삭제
                            deleteplacecidx = AdminPlaceIdx;
                        }


                        // INSERT 할게 있으면
                        if (insertplaceidx is [_, ..])
                        {
                            foreach (int InsertPlaceID in insertplaceidx)
                            {
                                AdminPlaceTb InsertTB = new AdminPlaceTb();
                                InsertTB.AdminTbId = adminTB.Id;
                                InsertTB.PlaceTbId = InsertPlaceID;
                                InsertTB.CreateDt = ThisDate;
                                InsertTB.CreateUser = creater;
                                InsertTB.UpdateDt = ThisDate;
                                InsertTB.UpdateUser = creater;

                                context.AdminPlaceTbs.Add(InsertTB);
                                bool Addresult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!Addresult)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return false;
                                }
                            }
                        }

                        // DELETE 할게 있으면
                        if (deleteplacecidx is [_, ..])
                        {
                            foreach (int DeletePlaceID in deleteplacecidx)
                            {
                                AdminPlaceTb? deleteTB = await context.AdminPlaceTbs
                                    .FirstOrDefaultAsync(m => m.AdminTbId == adminTB.Id &&
                                                              m.PlaceTbId == DeletePlaceID &&
                                                              m.DelYn != true)
                                    .ConfigureAwait(false);

                                if (deleteTB is not null)
                                {
                                    context.AdminPlaceTbs.Remove(deleteTB);
                                    bool deleteResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!deleteResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
                                        return false;
                                    }
                                }
                                else
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return false;
                                }
                            }
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return true;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return false;
                    }
                }
            });
        }

        /// <summary>
        /// 데드락 감지
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsDeadlockException(Exception ex)
        {
            // MySqlException 및 MariaDB의 교착 상태 오류 코드는 일반적으로 1213입니다.
            if (ex is MySqlException mysqlEx && mysqlEx.Number == 1213)
                return true;

            // InnerException에도 동일한 확인 로직을 적용
            if (ex.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1213)
                return true;

            return false;
        }

        
    }
}
