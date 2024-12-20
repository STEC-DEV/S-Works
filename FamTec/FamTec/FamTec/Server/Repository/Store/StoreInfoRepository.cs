using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Material;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.Store
{
    public class StoreInfoRepository : IStoreInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<StoreInfoRepository> CreateBuilderLogger;
        
        public StoreInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<StoreInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 대쉬보드용 금일 입출고 내역 조회
        /// </summary>
        /// <param name="ThisDate"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<InOutListDTO?> GetDashBoardInOutData(DateTime ThisDate, int placeid)
        {
            try
            {
                // StoreTb를 가져오며 RoomTb와 MaterialTb를 포함 (Eager Loading)
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Include(m => m.RoomTb)       // RoomTb를 포함
                    .Include(m => m.MaterialTb)   // MaterialTb를 포함
                    .Where(m => m.DelYn != true &&
                                m.PlaceTbId == placeid &&
                                m.CreateDt.Date == ThisDate)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var model = new InOutListDTO
                {
                    InPutList = StoreList
                        .Where(m => m.Inout == 1) // 입고 데이터 필터링
                        .Select(m => new InOutDataDTO
                        {
                            Id = m.Id,
                            Num = m.Num,
                            UnitPrice = m.UnitPrice,
                            TotalPrice = m.TotalPrice,
                            RoomId = m.RoomTbId,
                            RoomName = m.RoomTb?.Name,       // RoomTb의 Name 사용
                            MaterialID = m.MaterialTbId,
                            MaterialName = m.MaterialTb?.Name // MaterialTb의 Name 사용
                        }).ToList(),

                    OutPutList = StoreList
                        .Where(m => m.Inout == 0) // 출고 데이터 필터링
                        .Select(m => new InOutDataDTO
                        {
                            Id = m.Id,
                            Num = m.Num,
                            UnitPrice = m.UnitPrice,
                            TotalPrice = m.TotalPrice,
                            RoomId = m.RoomTbId,
                            RoomName = m.RoomTb?.Name,
                            MaterialID = m.MaterialTbId,
                            MaterialName = m.MaterialTb?.Name
                        }).ToList(),

                    InOutCount = StoreList.Count // 입출고 총 카운트
                };

                return model;
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

        public async Task<StoreTb?> AddAsync(StoreTb model)
        {
            try
            {
                await context.StoreTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 입출고 이력 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<InOutHistoryListDTO>?> GetInOutList(int placeid)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => m.PlaceTbId == placeid && 
                                m.MaintenenceHistoryTbId == null)
                    .OrderByDescending(m => m.InoutDate)
                    .ToListAsync()
                    .ConfigureAwait(false);


                List<InOutHistoryListDTO> model = (from Store in StoreList
                                                   join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                   on Store.MaterialTbId equals Material.Id
                                                   join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                   on Store.RoomTbId equals Room.Id
                                                   select new InOutHistoryListDTO
                                                   {
                                                       ID = Store.Id,
                                                       INOUT = Store.Inout,
                                                       InOutDate = Store.InoutDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       MaterialCode = Material.Code, // 품목코드
                                                       RoomID = Room.Id,
                                                       RoomName = Room.Name,
                                                       MaterialID = Store.MaterialTbId,
                                                       MaterialName = Material.Name,
                                                       MaterialUnit = Material.Unit,
                                                       Num = Store.Num,
                                                       UnitPrice = Store.UnitPrice,
                                                       ToTalPrice = Store.TotalPrice,
                                                       Note = Store.Note
                                                   }).ToList();
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
        /// 입출고 이력 조회 - 페이지 네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<InOutHistoryListDTO>?> GetInOutPageNationList(int placeid, int pagenumber, int pagesize)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .OrderByDescending(m => m.InoutDate)
                    .ToListAsync()
                    .ConfigureAwait(false);

                List<InOutHistoryListDTO> model = (from Store in StoreList
                                                   join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                   on Store.MaterialTbId equals Material.Id
                                                   join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                   on Store.RoomTbId equals Room.Id
                                                   select new InOutHistoryListDTO
                                                   {
                                                       ID = Store.Id,
                                                       INOUT = Store.Inout,
                                                       InOutDate = Store.InoutDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       MaterialCode = Material.Code, // 품목코드
                                                       RoomID = Room.Id,
                                                       RoomName = Room.Name,
                                                       MaterialID = Store.MaterialTbId,
                                                       MaterialName = Material.Name,
                                                       MaterialUnit = Material.Unit,
                                                       Num = Store.Num,
                                                       UnitPrice = Store.UnitPrice,
                                                       ToTalPrice = Store.TotalPrice,
                                                       Note = Store.Note
                                                   }).ToList();
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
        /// 사업장의 입출고 총 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int> GetPlaceInOutCount(int placeid)
        {
            try
            {
                int count = await context.StoreTbs
                    .Where(m => m.PlaceTbId == placeid &&
                           m.DelYn != true)
                    .CountAsync()
                    .ConfigureAwait(false);

                return count;
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
        /// 자재별 입출고 카운트 (대쉬보드)
        /// </summary>
        /// <param name="startOfWeek"></param>
        /// <param name="EndOfWeek"></param>
        /// <param name="MaterialId"></param>
        /// <returns></returns>
        public async Task<List<MaterialWeekCountDTO>?> GetDashBoardData(DateTime startOfWeek, DateTime EndOfWeek, List<int> MaterialId)
        {
            try
            {
                /* 입출고 이력 있는 ID만 조회 */

                // 조회 기간의 모든 날짜 생성
                var allDates = Enumerable.Range(0, 1 + EndOfWeek.AddDays(-1).Subtract(startOfWeek).Days)
                                .Select(offset => startOfWeek.AddDays(offset).Date)
                                .ToList();

                // StoreTb와 MaterialTb 조인 데이터 가져오기
                var inOutList = await context.StoreTbs
                    .Where(m => m.DelYn != true &&
                                MaterialId.Contains(m.MaterialTbId) &&
                                m.CreateDt >= startOfWeek &&
                                m.CreateDt <= EndOfWeek)
                    .Join(
                        context.MaterialTbs,
                        store => store.MaterialTbId,
                        material => material.Id,
                        (store, material) => new
                        {
                            store.MaterialTbId,
                            MaterialName = material.Name,
                            store.CreateDt,
                            store.Inout, // 1: Input, 2: Output
                            store.Num    // 수량
                        }
                    )
                    .ToListAsync()
                    .ConfigureAwait(false);
        
                // inOutList 결과에서 MaterialTbId 목록만 가져오기
                var materialIdsFromResult = inOutList
                    .Select(x => x.MaterialTbId)
                    .Distinct()
                    .ToList();

                // MaterialTbId 기준으로 데이터 그룹화 및 날짜 매핑
                List<MaterialWeekCountDTO> model = materialIdsFromResult
                    .Select(materialId => new MaterialWeekCountDTO
                    {
                        MaterialId = materialId,
                        MaterialName = inOutList
                            .First(x => x.MaterialTbId == materialId).MaterialName, // inOutList에서 이름 가져오기
                        WeekCountList = allDates.Select(date => new WeekInOutCountDTO
                        {
                            Date = date,
                            InputCount = inOutList
                                .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date && x.Inout == 1)
                                .Sum(x => x.Num),
                            OutputCount = inOutList
                                .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date && x.Inout == 0)
                                .Sum(x => x.Num),
                            TotalCount = inOutList
                                .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date)
                                .Sum(x => x.Num)
                        }).ToList()
                    })
                    .ToList();

                if (model is [_, ..])
                    return model;
                else
                    return null;

                /* 내역 유무와 상관없이 자재별로 모든 입출력 데이터 1주일치 */
                /*
                // MaterialId 기준으로 데이터 그룹화 및 날짜 매핑
                

                // 조회 기간의 모든 날짜 생성
                var allDates = Enumerable.Range(0, 1 + EndOfWeek.AddDays(-1).Subtract(startOfWeek).Days)
                                .Select(offset => startOfWeek.AddDays(offset).Date)
                                .ToList();

                // StoreTb와 MaterialTb 조인 데이터 가져오기
                var inOutList = await context.StoreTbs
                    .Where(m => m.DelYn != true &&
                                MaterialId.Contains(m.MaterialTbId) &&
                                m.CreateDt >= startOfWeek &&
                                m.CreateDt <= EndOfWeek)
                    .Join(
                        context.MaterialTbs,
                        store => store.MaterialTbId,
                        material => material.Id,
                        (store, material) => new
                        {
                            store.MaterialTbId,
                            MaterialName = material.Name,
                            store.CreateDt,
                            store.Inout, // 1: Input, 2: Output
                            store.Num    // 수량
                        }
                    )
                    .ToListAsync()
                    .ConfigureAwait(false);

                

                List<MaterialWeekCountDTO> model = MaterialId
                     .Select(materialId => new MaterialWeekCountDTO
                     {
                         MaterialId = materialId,
                         MaterialName = context.MaterialTbs.First(m => m.Id == materialId).Name, // MaterialTb에서 이름을 가져옴
                         WeekCountList = allDates.Select(date => new WeekInOutCountDTO
                         {
                             Date = date,
                             InputCount = inOutList
                                 .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date && x.Inout == 1)
                                 .Sum(x => x.Num),
                             OutputCount = inOutList
                                 .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date && x.Inout == 0)
                                 .Sum(x => x.Num),
                             TotalCount = inOutList
                                 .Where(x => x.MaterialTbId == materialId && x.CreateDt.Date == date)
                                 .Sum(x => x.Num)
                         }).ToList()
                     })
                     .ToList();
                if (model is [_, ..])
                    return model;
                else
                    return null;
               */

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

   
    }
}
