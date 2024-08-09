using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Admin.AdminUser
{
    public class AdminUserInfoRepository : IAdminUserInfoRepository
    {
        private readonly WorksContext context;
        private readonly IFileService FileService;
        private ILogService LogService;

        public AdminUserInfoRepository(WorksContext _context, 
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.context = _context;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> AddAdminUserInfo(AdminTb model)
        {
            try
            {
                context.AdminTbs.Add(model);
                bool Addresult = await context.SaveChangesAsync() > 0 ? true : false;
                if(Addresult)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteAdminInfo(AdminTb model)
        {
            try
            {
                context.AdminTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

       
        public async ValueTask<bool?> DeleteAdminsInfo(List<int> idx, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int adminid in idx)
                    {
                        AdminTb? admintb = await context.AdminTbs.FirstOrDefaultAsync(m => m.Id == adminid && m.DelYn != true);
                        if(admintb is not null)
                        {
                            admintb.DelYn = true;
                            admintb.DelDt = DateTime.Now;
                            admintb.DelUser = deleter;
                            context.AdminTbs.Update(admintb);
                            bool AdminTbResult = await context.SaveChangesAsync() > 0 ? true : false;
                                
                            if(!AdminTbResult) // 실패하면
                            {
                                await context.Database.RollbackTransactionAsync(); // 롤백
                                return false;
                            }

                            UsersTb? usertb = await context.UsersTbs.FirstOrDefaultAsync(m => m.Id == admintb.UserTbId && m.DelYn != true);
                            if(usertb is not null)
                            {
                                usertb.DelYn = true;
                                usertb.DelDt = DateTime.Now;
                                usertb.DelUser = deleter;
                                context.UsersTbs.Update(usertb);
                                bool UserTbResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!UserTbResult) // 실패하면
                                {
                                    await context.Database.RollbackTransactionAsync(); // 롤백
                                    return false;
                                }

                                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs.Where(m => m.AdminTbId == admintb.Id && m.DelYn != true).ToListAsync();
                                if(adminplacetb is [_, ..])
                                {
                                    foreach(AdminPlaceTb AdminPlace in adminplacetb)
                                    {
                                        context.AdminPlaceTbs.Remove(AdminPlace);
                                        bool AdminPlaceResult = await context.SaveChangesAsync() > 0 ? true : false;
                                        if (!AdminPlaceResult) // 실패하면
                                        {
                                            await context.Database.RollbackTransactionAsync(); //롤백
                                            return false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await context.Database.RollbackTransactionAsync(); // 롤백
                                return false;
                            }
                        }
                        else
                        {
                            await context.Database.RollbackTransactionAsync(); // 롤백
                            return false;
                        }
                    }
                    await context.Database.CommitTransactionAsync(); // 커밋
                    return true;
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 매개변수의 ADMINID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> GetAdminIdInfo(int adminid)
        {
            try
            {
                AdminTb? model = await context.AdminTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(adminid) && m.DelYn != true);
                
                if (model is not null)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



        /// <summary>
        /// 매개변수의 USERID에 해당하는 관리자모델 모델 조회
        /// </summary>
        /// <param name="adminuseridx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<AdminTb?> GetAdminUserInfo(int usertbid)
        {
            try
            {
                AdminTb? model = await context.AdminTbs
                    .FirstOrDefaultAsync(m => m.UserTbId.Equals(usertbid) && m.DelYn != true);
                
                if (model is not null)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



        /// <summary>
        /// 관리자 DTO 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<ManagerListDTO>?> GetAllAdminUserList()
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
                        Department = m.DepartmentTb!.Name!
                    }).ToListAsync();
                
                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 해당 사업장 관리자로 선택가능한 사업장 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<ManagerListDTO>?> GetNotContainsAdminList(int placeid)
        {
            try
            {
                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .ToListAsync();

                if (adminplacetb is [_, ..])
                {
                    List<int> admintbid = adminplacetb.Select(m => m.AdminTbId).ToList();

                    List<AdminTb>? admintb = await context.AdminTbs
                        .Where(e => !admintbid.Contains(e.Id) && e.DelYn != true)
                        .ToListAsync();

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
                    //List<AdminTb>? alladmin = await context.AdminTbs.Where(m => m.DelYn != true).ToListAsync();

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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<AdminTb?> UpdateAdminInfo(AdminTb model)
        {
            try
            {
                context.AdminTbs.Update(model);
                bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                
                if (UpdateResult)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        /// <summary>
        /// 업데이트 수정로직
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateAdminInfo(UpdateManagerDTO dto, string creater, IFormFile? files)
        {
            string AdminFileFolderPath = String.Format(@"{0}\\Administrator", Common.FileServer);

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    AdminTb? adminTB = await context.AdminTbs.FirstOrDefaultAsync(m => m.Id == dto.AdminIndex && m.DelYn != true);
                    if (adminTB is null)
                        return null;

                    // 계정정보 변경을 위해 조회
                    UsersTb? userTB = await context.UsersTbs.FirstOrDefaultAsync(m => m.Id == adminTB.UserTbId && m.DelYn != true);
                    if(userTB is null)
                        return null;

                    if (!adminTB.Type.Equals("시스템관리자"))
                    {
                        userTB.Name = dto.Name; // 이름
                        userTB.Phone = dto.Phone; // 전화번호
                        userTB.Email = dto.Email; // 이메일
                        userTB.UserId = dto.UserId!; // 로그인ID
                        userTB.Password = dto.Password!; // 비밀번호
                        userTB.UpdateDt = DateTime.Now;
                        userTB.UpdateUser = creater;
                        context.UsersTbs.Update(userTB);

                        bool UserUpdate = await context.SaveChangesAsync() > 0 ? true : false;

                        if (!UserUpdate) // 업데이트 실패 - 롤백
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    if (!adminTB.Type.Equals("시스템관리자"))
                    {
                        adminTB.DepartmentTbId = dto.DepartmentId!.Value; // 부서
                        adminTB.UpdateDt = DateTime.Now;
                        adminTB.UpdateUser = creater;
                        context.AdminTbs.Update(adminTB);

                        bool AdminUpdate = await context.SaveChangesAsync() > 0 ? true : false;
                        if (!AdminUpdate) // 업데이트 실패 - 롤백
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    // DTO의 PlaceID -- 최종 결과물이 되야할 것들
                    List<int> DTOPlaceIdx = dto.PlaceList!.Select(m => m.Id!.Value).ToList();

                    // 이 관리자의 관리하고있는 사업장ID 조회
                    List<int> AdminPlaceIdx = await context.AdminPlaceTbs
                        .Where(m => m.AdminTbId == adminTB.Id && m.DelYn != true)
                        .Select(m => m.PlaceTbId).ToListAsync();

                    List<int> insertplaceidx = new List<int>();
                    List<int> deleteplacecidx = new List<int>();

                    if (DTOPlaceIdx is [_, ..]) // 넘어온 사업장이 있으면
                    {
                        
                        if(AdminPlaceIdx is [_, ..]) // 해당 관리자의 사업장이 있으면
                        {
                            // 내가갖고 있는것
                            List<int>? MineList = await context.AdminPlaceTbs
                                .Where(m => DTOPlaceIdx.Contains(Convert.ToInt32(m.PlaceTbId)) && 
                                m.DelYn != true && 
                                m.AdminTbId == adminTB.Id)
                                .Select(m => m.PlaceTbId)
                                .ToListAsync();

                            if(MineList is [_, ..]) // 가지고 있는게 있으면
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
                            InsertTB.CreateDt = DateTime.Now;
                            InsertTB.CreateUser = creater;
                            InsertTB.UpdateDt = DateTime.Now;
                            InsertTB.UpdateUser = creater;

                            context.AdminPlaceTbs.Add(InsertTB);
                            bool Addresult = await context.SaveChangesAsync() > 0 ? true : false;
                            if(!Addresult)
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                    }

                    // DELETE 할게 있으면
                    if (deleteplacecidx is [_, ..])
                    {
                        foreach(int DeletePlaceID in deleteplacecidx)
                        {
                            AdminPlaceTb? deleteTB = await context.AdminPlaceTbs
                                .FirstOrDefaultAsync(m => m.AdminTbId == adminTB.Id && 
                                m.PlaceTbId == DeletePlaceID &&
                                m.DelYn != true);

                            if(deleteTB is not null)
                            {
                                context.AdminPlaceTbs.Remove(deleteTB);
                                bool deleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!deleteResult)
                                {
                                    await transaction.RollbackAsync();
                                    return false;
                                }
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                    }

                    
                    // 파일 갈아끼우기 작업.
                    if (files is not null) // 파일이 있는데
                    {
                        if(!String.IsNullOrWhiteSpace(userTB.Image)) // DB에는 있다
                        {
                            // DB삭제 + 파일삭제
                            bool result = FileService.DeleteImageFile(AdminFileFolderPath, userTB.Image);
                            if (result)
                                userTB.Image = null;
                        }
                        // 파일추가
                        userTB.Image = await FileService.AddImageFile(AdminFileFolderPath, files);
                    }
                    else // 파일이 없다
                    {
                        if(!String.IsNullOrWhiteSpace(userTB.Image)) // DB엔 있다
                        {
                            // DB삭제 + 파일삭제
                            bool result = FileService.DeleteImageFile(AdminFileFolderPath, userTB.Image);
                            if (result)
                                userTB.Image = null;
                        }
                    }

                    context.UsersTbs.Update(userTB);
                    bool UpdateUser = await context.SaveChangesAsync() > 0 ? true : false;
                    if(UpdateUser)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        // Rollback시 파일삭제
                        if (!String.IsNullOrWhiteSpace(userTB.Image))
                        {
                            FileService.DeleteImageFile(AdminFileFolderPath, userTB.Image);
                        }

                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

    }
}
