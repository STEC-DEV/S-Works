using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;


namespace FamTec.Server.Repository.Admin.AdminPlaces
{
    public class AdminPlaceInfoRepository : IAdminPlacesInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public AdminPlaceInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 관리자 사업장 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> AddAsync(List<AdminPlaceTb> model)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        await context.AdminPlaceTbs.AddRangeAsync(model).ConfigureAwait(false);

                        bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (AddResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 관리자에 해당하는 사업장리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async Task<List<AdminPlaceTb>?> GetMyWorksList(int adminid)
        {
            try
            {
                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                    .Where(m => m.AdminTbId == adminid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (adminplacetb is [_, ..])
                    return adminplacetb;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 관리자에 해당하는 사업장 리스트 출력
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async Task<List<AdminPlaceDTO>?> GetMyWorks(int adminid)
        {
            try
            {
                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                    .Where(m => m.AdminTbId == adminid && m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (!adminplacetb.Any())
                    return null;

               
                List<PlaceTb>? placetb = await context.PlaceTbs
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (!placetb.Any())
                    return null;

                List<AdminPlaceDTO>? result = (from admin in adminplacetb
                                        join place in placetb
                                        on admin.PlaceTbId equals place.Id
                                        where place.DelYn != true
                                        select new AdminPlaceDTO
                                        {
                                            Id = place.Id,
                                            Name = place.Name,
                                            ContractNum = place.ContractNum,
                                            ContractDt = place.ContractDt,
                                            Status = place.Status
                                        }).ToList();
              
                if (result is [_, ..])
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 매니저 상세보기
        /// </summary>
        /// <param name="adminidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<DManagerDTO?> GetManagerDetails(int adminidx)
        {
            try
            {
                AdminTb? admintb = await context.AdminTbs
                    .FirstOrDefaultAsync(m => m.Id == adminidx && m.DelYn != true)
                    .ConfigureAwait(false);

                if (admintb is null)
                    return null;

                DepartmentsTb? departmenttb = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id == admintb.DepartmentTbId && 
                                              m.DelYn != true)
                    .ConfigureAwait(false);

                if (departmenttb is null)
                    return null;

                UsersTb? usertb = await context.UsersTbs
                    .FirstOrDefaultAsync(m => m.Id == admintb.UserTbId && m.DelYn != true)
                    .ConfigureAwait(false);

                if (usertb is null)
                    return null;

                DManagerDTO dto = new DManagerDTO
                {
                    UserId = usertb.UserId,
                    Name = usertb.Name,
                    Password = usertb.Password,
                    Phone = usertb.Phone,
                    Email = usertb.Email,
                    Type = admintb.Type,
                    Department = departmenttb.Name
                };

                return dto;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// AdminPlaceTb 사업장 삭제 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool?> DeleteMyWorks(AdminPlaceTb model)
        {
            try
            {
                context.AdminPlaceTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 사업장번호로 사업장 상세정보조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<PlaceDetailDTO?> GetWorksInfo(int placeid)
        {
            try
            {
                
                PlaceTb? placetb = await context.PlaceTbs
                    .FirstOrDefaultAsync(m => m.Id == placeid && m.DelYn != true)
                    .ConfigureAwait(false);

                if (placetb is null )
                    return null;

                List<ManagerListDTO>? ManagerDTO = (from admintb in context.AdminTbs.ToList()
                                                    join adminplacetb in context.AdminPlaceTbs.Where(m => m.PlaceTbId == placeid).ToList()
                                                    on admintb.Id equals adminplacetb.AdminTbId
                                                    join usertb in context.UsersTbs.ToList()
                                                    on admintb.UserTbId equals usertb.Id
                                                    join departmenttb in context.DepartmentsTbs.ToList()
                                                    on admintb.DepartmentTbId equals departmenttb.Id
                                                    where (admintb.DelYn != true && adminplacetb.DelYn != true)
                                                    select new ManagerListDTO
                                                    {
                                                        Id = admintb.Id,
                                                        UserId = usertb.UserId,
                                                        Name = usertb.Name,
                                                        Department = departmenttb.Name
                                                    }).ToList();

                PlaceDetailDTO PlaceDetail = new PlaceDetailDTO();
                PlaceDetail.PlaceInfo!.Id = placetb.Id;
                PlaceDetail.PlaceInfo.Address = placetb.Address; // 주소
                PlaceDetail.PlaceInfo.Name = placetb.Name;
                PlaceDetail.PlaceInfo.Tel = placetb.Tel;
                PlaceDetail.PlaceInfo.ContractNum = placetb.ContractNum;
                PlaceDetail.PlaceInfo.ContractDt = placetb.ContractDt;
                PlaceDetail.PlaceInfo.CancelDt = placetb.CancelDt;
                PlaceDetail.PlaceInfo.Status = placetb.Status;

                DepartmentsTb? DepartmentTB = await context.DepartmentsTbs
                    .FirstOrDefaultAsync(m => m.Id == placetb.DepartmentTbId).ConfigureAwait(false);
                        
                if (DepartmentTB is not null)
                {
                    PlaceDetail.PlaceInfo.DepartmentID = DepartmentTB.Id;
                    PlaceDetail.PlaceInfo.DepartmentName = DepartmentTB.Name;
                }

                PlaceDetail.PlacePerm!.Id = placetb.Id;
                PlaceDetail.PlacePerm.PermMachine = placetb.PermMachine;
                PlaceDetail.PlacePerm.PermElec = placetb.PermElec;
                PlaceDetail.PlacePerm.PermLift = placetb.PermLift;
                PlaceDetail.PlacePerm.PermFire = placetb.PermFire;
                PlaceDetail.PlacePerm.PermConstruct = placetb.PermConstruct;
                PlaceDetail.PlacePerm.PermNetwork = placetb.PermNetwork;
                PlaceDetail.PlacePerm.PermBeauty = placetb.PermBeauty;
                PlaceDetail.PlacePerm.PermSecurity = placetb.PermSecurity;
                PlaceDetail.PlacePerm.PermMaterial = placetb.PermMaterial;
                PlaceDetail.PlacePerm.PermEnergy = placetb.PermEnergy;
                PlaceDetail.PlacePerm.PermVoc = placetb.PermVoc;

                PlaceDetail.ManagerList = ManagerDTO;
                        
                return PlaceDetail;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 관리자 사업장 조회 - 사업장 INDEX
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<AdminPlaceTb?> GetWorksModelInfo(int placeid)
        {
            try
            {
                AdminPlaceTb? adminplacetb = await context.AdminPlaceTbs
                    .FirstOrDefaultAsync(m => m.DelYn != true && m.PlaceTbId == placeid)
                    .ConfigureAwait(false);

                if (adminplacetb is not null)
                    return adminplacetb;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 해당 사업장에서 관리자 삭제
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<bool?> RemoveAdminPlace(List<int> adminid, int placeid)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
            
            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트를 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        foreach (int id in adminid)
                        {
                            AdminPlaceTb? admintb = await context.AdminPlaceTbs
                                .FirstOrDefaultAsync(m => m.AdminTbId == id && m.PlaceTbId == placeid)
                                .ConfigureAwait(false);

                            if (admintb is null)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return false;
                            }
                            else
                            {
                                context.AdminPlaceTbs.Remove(admintb);
                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return true;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 해당 사업장에 할당된 관리자 삭제 - DelYN 삭제함.
        /// </summary>
        /// <param name="admintbid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<AdminPlaceTb?> GetPlaceAdminInfo(int admintbid, int placeid)
        {
            try
            {
                AdminPlaceTb? model = await context.AdminPlaceTbs
                    .FirstOrDefaultAsync(m => m.AdminTbId == admintbid && m.PlaceTbId == placeid)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 해당 사업장의 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool?> DeleteAdminPlaceManager(AdminPlaceTb model)
        {
            try
            {
                context.AdminPlaceTbs.Remove(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 선택된 사업장에 포함되어있는 AdminPlaceTB 리스트 반환 
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<AdminPlaceTb>?> SelectPlaceAdminList(List<int> placeidx)
        {
            try
            {
                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs
                    .Where(m => placeidx.Contains(Convert.ToInt32(m.PlaceTbId)) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (adminplacetb is not null && adminplacetb.Any())
                {
                    return adminplacetb;
                }
                else
                {
                    return null;
                }
               
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<(List<int>? insert, List<int>? delete)?> DisassembleUpdateAdminInfo(int adminid, List<int> placeidx)
        {
            try
            {
                List<int>? selectplaceidx = null;
                List<int>? insertplaceidx = null;
                List<int>? deleteplaceidx = null;
                
                List<int> allplaceidx = await context.AdminPlaceTbs
                    .Where(m => m.AdminTbId == adminid && m.DelYn != true)
                    .Select(m => m.PlaceTbId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (placeidx is [_, ..])
                {
                    // AdminID에 해당하는 사업장 전체출력
                    if (allplaceidx is [_, ..])
                    {
                        // 넘어온 PlaceID중에서 내가 갖고 있는것.
                        selectplaceidx = await context.AdminPlaceTbs.Where(m => placeidx.Contains(Convert.ToInt32(m.PlaceTbId)) && m.DelYn != true && m.AdminTbId == adminid).Select(m => m.PlaceTbId).ToListAsync().ConfigureAwait(false);
                        if (selectplaceidx is [_, ..]) // 가지고 있는게 있으면
                        {
                            // 추가사업장 구하기
                            insertplaceidx = placeidx.Except(selectplaceidx).ToList();

                            // 빠진 사업장 allplacetb - selectplacetb = "빠진사업장"
                            deleteplaceidx = allplaceidx.Except(selectplaceidx).ToList();
                        }
                        // 검색후 내것중 가진게 없으면
                        else
                        {
                            insertplaceidx = placeidx;
                            deleteplaceidx = allplaceidx;
                        }
                    }
                    else // 가지고있는 사업장이 아에 없을때
                    {
                        insertplaceidx = placeidx;
                        deleteplaceidx = null;
                    }
                }
                else
                {
                    deleteplaceidx = allplaceidx;
                }

                return (insertplaceidx, deleteplaceidx);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 관리자 사업장 단일모델 저장
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AdminPlaceTb?> AddAdminPlaceInfo(AdminPlaceTb model)
        {
            try
            {
                await context.AdminPlaceTbs.AddAsync(model).ConfigureAwait(false);
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                if (AddResult)
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
                throw;
            }
        }

        /// <summary>
        /// 관리자 사업장 단일모델 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteAdminPlaceInfo(AdminPlaceTb model)
        {
            try
            {
                context.AdminPlaceTbs.Remove(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 관리자가 포함안된 사업장 리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async Task<List<AdminPlaceDTO>?> GetNotContainsPlaceList(int adminid)
        {
            try
            {
                // 관리자 있는지 Check
                AdminTb? AdminCheck = await context.AdminTbs.FirstOrDefaultAsync(m => m.Id == adminid && m.DelYn != true).ConfigureAwait(false);
                if (AdminCheck is null)
                    return null;


                List<AdminPlaceTb>? adminplacetb = await context.AdminPlaceTbs.Where(m => m.AdminTbId == adminid && m.DelYn != true).ToListAsync().ConfigureAwait(false);
                if (adminplacetb is [_, ..])
                {
                    List<int> adminplacetbid = adminplacetb.Select(m => m.PlaceTbId).ToList();

                    List<PlaceTb>? placetb = await context.PlaceTbs.Where(e => !adminplacetbid.Contains(e.Id) && e.DelYn != true).ToListAsync().ConfigureAwait(false);
                    if (placetb is [_, ..])
                    {
                        List<AdminPlaceDTO> model = placetb.Select(e => new AdminPlaceDTO
                        {
                            Id = e.Id, // 사업장ID
                            Name = e.Name, // 사업장이름
                            ContractNum = e.ContractNum, // 계약번호
                            ContractDt = e.ContractDt, // 계약일자
                            Status = e.Status // 상태
                        }).ToList();

                        return model;
                    }
                    else
                    {
                        return null;
                    }
                }
                else // 이사람은 아무 사업장도 없음
                {
                    List<PlaceTb>? placetb = await context.PlaceTbs.Where(m => m.DelYn != true).ToListAsync().ConfigureAwait(false); ;
                    if(placetb is [_, ..])
                    {
                        List<AdminPlaceDTO> model = placetb.Select(e => new AdminPlaceDTO
                        {
                            Id = e.Id, // 사업장ID
                            Name = e.Name, // 사업장이름
                            ContractNum = e.ContractNum, // 계약번호
                            ContractDt = e.ContractDt, // 계약일자
                            Status = e.Status // 상태
                        }).ToList();

                        return model;
                    }
                    else // 사업장 자체가 아무것도 없음
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 로그인용 관리자 사업장 리스트 반환
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public async Task<List<AdminPlaceDTO>?> LoginSelectPlaceList(int adminid)
        {
            try
            {
                List<AdminPlaceDTO>? model = await (from AdminPlaceTB in context.AdminPlaceTbs.Where(m => m.DelYn != true && m.AdminTbId == adminid)
                                              join PlaceTB in context.PlaceTbs.Where(m => m.DelYn != true && m.Status == true)
                                              on AdminPlaceTB.PlaceTbId equals PlaceTB.Id
                                              select new AdminPlaceDTO
                                              {
                                                  Id = PlaceTB.Id,
                                                  Name = PlaceTB.Name,
                                                  Status = PlaceTB.Status,
                                                  ContractDt = PlaceTB.ContractDt,
                                                  ContractNum = PlaceTB.ContractNum
                                              }).ToListAsync()
                                              .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 데드락 감지코드
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
