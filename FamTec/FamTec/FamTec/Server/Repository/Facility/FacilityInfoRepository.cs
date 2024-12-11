using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Facility
{
    public class FacilityInfoRepository : IFacilityInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<FacilityInfoRepository> CreateBuilderLogger;

        public FacilityInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<FacilityInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="Facilityid"></param>
        /// <returns></returns>
        public async Task<bool?> DelFacilityCheck(int Facilityid)
        {
            try
            {
                bool MaintenenceCheck = await context.MaintenenceHistoryTbs
                    .AnyAsync(m => m.FacilityTbId == Facilityid && m.DelYn != true)
                    .ConfigureAwait(false);

                return MaintenenceCheck;
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
        /// 설비추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FacilityTb?> AddAsync(FacilityTb model)
        {
            try
            {
                await context.FacilityTbs.AddAsync(model).ConfigureAwait(false);

                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
               
                if (AddResult)
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
        /// 설비 리스트 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddFacilityList(List<FacilityTb> model)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

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
                        await context.FacilityTbs.AddRangeAsync(model).ConfigureAwait(false);

                        bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(AddResult)
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
                    catch(Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch(DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch(MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch(Exception ex)
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
        /// 공간ID에 포함되어있는 전체 설비LIST 조회
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async Task<List<FacilityTb>?> GetAllFacilityList(int roomid)
        {
            try
            {
                List<FacilityTb>? model = await context.FacilityTbs
                    .Where(m => m.RoomTbId == roomid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
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
        /// 설비 ID로 단일 설비모델 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FacilityTb?> GetFacilityInfo(int id)
        {
            try
            {
                FacilityTb? model = await context.FacilityTbs.
                    FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true)
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
        /// 설비 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateFacilityInfo(FacilityTb model)
        {
            try
            {
                context.FacilityTbs.Update(model);
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

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteFacilityInfo(List<int> facilityid, string deleter)
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
                        foreach (int fcid in facilityid)
                        {
                            FacilityTb? FacilityTB = await context.FacilityTbs.FirstOrDefaultAsync(m => m.Id == fcid).ConfigureAwait(false);

                            if (FacilityTB is null)
                                return (bool?)null;

                            FacilityTB.DelYn = true;
                            FacilityTB.DelDt = ThisDate;
                            FacilityTB.DelUser = deleter;

                            context.FacilityTbs.Update(FacilityTB);

                            List<FacilityItemGroupTb>? GroupList = await context.FacilityItemGroupTbs
                                .Where(m => m.FacilityTbId == FacilityTB.Id && m.DelYn != true)
                                .ToListAsync()
                                .ConfigureAwait(false);

                            if (GroupList is [_, ..])
                            {
                                foreach (FacilityItemGroupTb GroupTB in GroupList)
                                {
                                    // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                    GroupTB.Name = $"{GroupTB.Name}_{GroupTB.Id}";
                                    GroupTB.DelYn = true;
                                    GroupTB.DelDt = ThisDate;
                                    GroupTB.DelUser = deleter;

                                    context.FacilityItemGroupTbs.Update(GroupTB);

                                    List<FacilityItemKeyTb>? KeyList = await context.FacilityItemKeyTbs
                                        .Where(m => m.FacilityItemGroupTbId == GroupTB.Id && m.DelYn != true)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                                    if (KeyList is [_, ..])
                                    {
                                        foreach (FacilityItemKeyTb KeyTB in KeyList)
                                        {
                                            KeyTB.DelYn = true;
                                            KeyTB.DelDt = ThisDate;
                                            KeyTB.DelUser = deleter;

                                            context.FacilityItemKeyTbs.Update(KeyTB);

                                            List<FacilityItemValueTb>? ValueList = await context.FacilityItemValueTbs
                                                .Where(m => m.FacilityItemKeyTbId == KeyTB.Id && m.DelYn != true)
                                                .ToListAsync()
                                                .ConfigureAwait(false);

                                            if (ValueList is [_, ..])
                                            {
                                                foreach (FacilityItemValueTb ValueTB in ValueList)
                                                {
                                                    ValueTB.DelYn = true;
                                                    ValueTB.DelDt = ThisDate;
                                                    ValueTB.DelUser = deleter;

                                                    context.FacilityItemValueTbs.Update(ValueTB);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        bool DeleteResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (DeleteResult)
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
        /// 사업장에 속한 기계설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<FacilityListDTO>?> GetPlaceMachineFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> machinelist = await (from building in context.BuildingTbs
                                                           where building.PlaceTbId == placeid && building.DelYn != true
                                                           join floortb in context.FloorTbs
                                                           on building.Id equals floortb.BuildingTbId
                                                           where floortb.DelYn != true
                                                           join roomtb in context.RoomTbs
                                                           on floortb.Id equals roomtb.FloorTbId
                                                           where roomtb.DelYn != true
                                                           join facilitytb in context.FacilityTbs
                                                           on roomtb.Id equals facilitytb.RoomTbId
                                                           where facilitytb.Category.Trim() == "기계" && facilitytb.DelYn != true
                                                           select new FacilityListDTO
                                                           {
                                                               Id = facilitytb.Id, /* 설비인덱스 */
                                                               Name = facilitytb.Name, /* 설비명칭 */
                                                               Type = facilitytb.Type, /* 설비타입 */
                                                               Num = facilitytb.Num, /* 수량 */
                                                               RoomName = roomtb.Name, /* 공간위치 이름 */
                                                               BuildingName = building.Name, /* 건물명칭 */
                                                               StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                               EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                               LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                               ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                           }).OrderBy(m => m.Id)
                                            .ToListAsync()
                                            .ConfigureAwait(false);


                if (machinelist is [_, ..])
                {
                    return machinelist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 전기설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceElectronicFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> electlist = await (from building in context.BuildingTbs
                                                           where building.PlaceTbId == placeid && building.DelYn != true
                                                           join floortb in context.FloorTbs
                                                           on building.Id equals floortb.BuildingTbId
                                                           where floortb.DelYn != true
                                                           join roomtb in context.RoomTbs
                                                           on floortb.Id equals roomtb.FloorTbId
                                                           where roomtb.DelYn != true
                                                           join facilitytb in context.FacilityTbs
                                                           on roomtb.Id equals facilitytb.RoomTbId
                                                           where facilitytb.Category.Trim() == "전기" && facilitytb.DelYn != true
                                                           select new FacilityListDTO
                                                           {
                                                               Id = facilitytb.Id, /* 설비인덱스 */
                                                               Name = facilitytb.Name, /* 설비명칭 */
                                                               Type = facilitytb.Type, /* 설비타입 */
                                                               Num = facilitytb.Num, /* 수량 */
                                                               RoomName = roomtb.Name, /* 공간위치 이름 */
                                                               BuildingName = building.Name, /* 건물명칭 */
                                                               StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                               EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                               LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                               ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                           }).OrderBy(m => m.Id)
                                          .ToListAsync()
                                          .ConfigureAwait(false);

                if (electlist is [_, ..])
                {
                    return electlist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 승강설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceLiftFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> liftlist = await (from building in context.BuildingTbs
                                                         where building.PlaceTbId == placeid && building.DelYn != true
                                                         join floortb in context.FloorTbs
                                                         on building.Id equals floortb.BuildingTbId
                                                         where floortb.DelYn != true
                                                         join roomtb in context.RoomTbs
                                                         on floortb.Id equals roomtb.FloorTbId
                                                         where roomtb.DelYn != true
                                                         join facilitytb in context.FacilityTbs
                                                         on roomtb.Id equals facilitytb.RoomTbId
                                                         where facilitytb.Category.Trim() == "승강" && facilitytb.DelYn != true
                                                         select new FacilityListDTO
                                                         {
                                                             Id = facilitytb.Id, /* 설비인덱스 */
                                                             Name = facilitytb.Name, /* 설비명칭 */
                                                             Type = facilitytb.Type, /* 설비타입 */
                                                             Num = facilitytb.Num, /* 수량 */
                                                             RoomName = roomtb.Name, /* 공간위치 이름 */
                                                             BuildingName = building.Name, /* 건물명칭 */
                                                             StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                             EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                             LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                             ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                         }).OrderBy(m => m.Id)
                                           .ToListAsync()
                                           .ConfigureAwait(false); 

                if (liftlist is [_, ..])
                {
                    return liftlist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 소방설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceFireFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> firelist = await (from building in context.BuildingTbs
                                                        where building.PlaceTbId == placeid && building.DelYn != true
                                                        join floortb in context.FloorTbs
                                                        on building.Id equals floortb.BuildingTbId
                                                        where floortb.DelYn != true
                                                        join roomtb in context.RoomTbs
                                                        on floortb.Id equals roomtb.FloorTbId
                                                        where roomtb.DelYn != true
                                                        join facilitytb in context.FacilityTbs
                                                        on roomtb.Id equals facilitytb.RoomTbId
                                                        where facilitytb.Category.Trim() == "소방" && facilitytb.DelYn != true
                                                        select new FacilityListDTO
                                                        {
                                                            Id = facilitytb.Id, /* 설비인덱스 */
                                                            Name = facilitytb.Name, /* 설비명칭 */
                                                            Type = facilitytb.Type, /* 설비타입 */
                                                            Num = facilitytb.Num, /* 수량 */
                                                            RoomName = roomtb.Name, /* 공간위치 이름 */
                                                            BuildingName = building.Name, /* 건물명칭 */
                                                            StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                            EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                            LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                            ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                        }).OrderBy(m => m.Id)
                                           .ToListAsync()
                                           .ConfigureAwait(false);

                if (firelist is [_, ..])
                {
                    return firelist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 건축설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceConstructFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> Constructlist = await (from building in context.BuildingTbs
                                                        where building.PlaceTbId == placeid && building.DelYn != true
                                                        join floortb in context.FloorTbs
                                                        on building.Id equals floortb.BuildingTbId
                                                        where floortb.DelYn != true
                                                        join roomtb in context.RoomTbs
                                                        on floortb.Id equals roomtb.FloorTbId
                                                        where roomtb.DelYn != true
                                                        join facilitytb in context.FacilityTbs
                                                        on roomtb.Id equals facilitytb.RoomTbId
                                                        where facilitytb.Category.Trim() == "건축" && facilitytb.DelYn != true
                                                        select new FacilityListDTO
                                                        {
                                                            Id = facilitytb.Id, /* 설비인덱스 */
                                                            Name = facilitytb.Name, /* 설비명칭 */
                                                            Type = facilitytb.Type, /* 설비타입 */
                                                            Num = facilitytb.Num, /* 수량 */
                                                            RoomName = roomtb.Name, /* 공간위치 이름 */
                                                            BuildingName = building.Name, /* 건물명칭 */
                                                            StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                            EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                            LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                            ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                        }).OrderBy(m => m.Id)
                                       .ToListAsync()
                                       .ConfigureAwait(false);

                if (Constructlist is [_, ..])
                {
                    return Constructlist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 통신설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceNetworkFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> Networklist = await (from building in context.BuildingTbs
                                                             where building.PlaceTbId == placeid && building.DelYn != true
                                                             join floortb in context.FloorTbs
                                                             on building.Id equals floortb.BuildingTbId
                                                             where floortb.DelYn != true
                                                             join roomtb in context.RoomTbs
                                                             on floortb.Id equals roomtb.FloorTbId
                                                             where roomtb.DelYn != true
                                                             join facilitytb in context.FacilityTbs
                                                             on roomtb.Id equals facilitytb.RoomTbId
                                                             where facilitytb.Category.Trim() == "통신" && facilitytb.DelYn != true
                                                             select new FacilityListDTO
                                                             {
                                                                 Id = facilitytb.Id, /* 설비인덱스 */
                                                                 Name = facilitytb.Name, /* 설비명칭 */
                                                                 Type = facilitytb.Type, /* 설비타입 */
                                                                 Num = facilitytb.Num, /* 수량 */
                                                                 RoomName = roomtb.Name, /* 공간위치 이름 */
                                                                 BuildingName = building.Name, /* 건물명칭 */
                                                                 StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                                 EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                                 LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                                 ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                             }).OrderBy(m => m.Id)
                                   .ToListAsync()
                                   .ConfigureAwait(false);

                if (Networklist is [_, ..])
                {
                    return Networklist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 미화설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceBeautyFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> Beautylist = await (from building in context.BuildingTbs
                                                          where building.PlaceTbId == placeid && building.DelYn != true
                                                          join floortb in context.FloorTbs
                                                          on building.Id equals floortb.BuildingTbId
                                                          where floortb.DelYn != true
                                                          join roomtb in context.RoomTbs
                                                          on floortb.Id equals roomtb.FloorTbId
                                                          where roomtb.DelYn != true
                                                          join facilitytb in context.FacilityTbs
                                                          on roomtb.Id equals facilitytb.RoomTbId
                                                          where facilitytb.Category.Trim() == "미화" && facilitytb.DelYn != true
                                                          select new FacilityListDTO
                                                          {
                                                              Id = facilitytb.Id, /* 설비인덱스 */
                                                              Name = facilitytb.Name, /* 설비명칭 */
                                                              Type = facilitytb.Type, /* 설비타입 */
                                                              Num = facilitytb.Num, /* 수량 */
                                                              RoomName = roomtb.Name, /* 공간위치 이름 */
                                                              BuildingName = building.Name, /* 건물명칭 */
                                                              StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                              EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                              LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                              ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                          }).OrderBy(m => m.Id)
                                                          .ToListAsync()
                                                          .ConfigureAwait(false);

                if (Beautylist is [_, ..])
                {
                    return Beautylist;
                }
                else
                {
                    return null;
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
        /// 사업장에 속한 보안설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<FacilityListDTO>?> GetPlaceSecurityFacilityList(int placeid)
        {
            try
            {
                List<FacilityListDTO> Securitylist = await (from building in context.BuildingTbs
                                                          where building.PlaceTbId == placeid && building.DelYn != true
                                                          join floortb in context.FloorTbs
                                                          on building.Id equals floortb.BuildingTbId
                                                          where floortb.DelYn != true
                                                          join roomtb in context.RoomTbs
                                                          on floortb.Id equals roomtb.FloorTbId
                                                          where roomtb.DelYn != true
                                                          join facilitytb in context.FacilityTbs
                                                          on roomtb.Id equals facilitytb.RoomTbId
                                                          where facilitytb.Category.Trim() == "보안" && facilitytb.DelYn != true
                                                          select new FacilityListDTO
                                                          {
                                                              Id = facilitytb.Id, /* 설비인덱스 */
                                                              Name = facilitytb.Name, /* 설비명칭 */
                                                              Type = facilitytb.Type, /* 설비타입 */
                                                              Num = facilitytb.Num, /* 수량 */
                                                              RoomName = roomtb.Name, /* 공간위치 이름 */
                                                              BuildingName = building.Name, /* 건물명칭 */
                                                              StandardCapacity = facilitytb.StandardCapacity, /* 규격용량 */
                                                              EquipDT = facilitytb.EquipDt, /* 설치년월 */
                                                              LifeSpan = facilitytb.Lifespan, /* 내용연수 */
                                                              ChangeDT = facilitytb.ChangeDt /* 교체년월 */
                                                          }).OrderBy(m => m.Id)
                                                    .ToListAsync()
                                                    .ConfigureAwait(false);

                if (Securitylist is [_, ..])
                {
                    return Securitylist;
                }
                else
                {
                    return null;
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
