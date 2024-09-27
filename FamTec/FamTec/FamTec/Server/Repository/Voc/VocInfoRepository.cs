using FamTec.Client.Pages.Normal.User.UserAdd;
using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.Voc
{
    public class VocInfoRepository : IVocInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public VocInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// DashBoard 용 일주일치 각 타입별 카운트
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public async Task<List<VocWeekCountDTO>?> GetDashBoardData(DateTime StartDate, DateTime EndDate)
        {
            // Step 1: 날짜 범위 생성
            var allDates = Enumerable.Range(0, 1 + EndDate.AddDays(-1).Subtract(StartDate).Days)
                                     .Select(offset => StartDate.AddDays(offset).Date)
                                     .ToList();

            // Step 2: 데이터베이스에서 두 가지 유형의 데이터를 동시에 가져오기
            var groupedReceipts = await context.VocTbs
                .Where(m => m.DelYn != true &&
                            (m.Type == 0 || // 미분류
                            m.Type == 1 ||  // 기계
                            m.Type == 2 || // 전기
                            m.Type == 3 || // 승강
                            m.Type == 4 || // 소방
                            m.Type == 5 || // 건축
                            m.Type == 6 || // 통신
                            m.Type == 7 || // 미화
                            m.Type == 8 ) &&  // 보안
                            m.CreateDt >= StartDate && m.CreateDt <= EndDate)
                .GroupBy(m => new { m.CreateDt.Date, m.Type }) // CreateDt의 Date와 Type을 기준으로 그룹화
                .ToListAsync()
                .ConfigureAwait(false);

            // Step 3: 날짜별 데이터와 모든 날짜를 조인하여 결과 구성
            List<VocWeekCountDTO> model = allDates
                .Select(date => new VocWeekCountDTO
                {
                    Date = date,
                    DefaultType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 0)
                        .SelectMany(g => g)
                        .Count(),
                    MachineType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 1)
                        .SelectMany(g => g)
                        .Count(),
                    ElecType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 2)
                        .SelectMany(g => g)
                        .Count(),
                    liftType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 3)
                        .SelectMany(g => g)
                        .Count(),
                    ConstructType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 4)
                        .SelectMany(g => g)
                        .Count(),
                    FireType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 5)
                        .SelectMany(g => g)
                        .Count(),
                    NetWorkType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 6)
                        .SelectMany(g => g)
                        .Count(),
                    BeautyType = groupedReceipts
                        .Where(g => g.Key.Date == date && g.Key.Type == 7)
                        .SelectMany(g => g)
                        .Count(),
                    SecurityType = groupedReceipts
                        .Where(g=>g.Key.Date == date && g.Key.Type == 8)
                        .SelectMany(g => g)
                        .Count()
                }).ToList();

            return model;
        }

        /// <summary>
        /// VOC 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<VocTb?> AddAsync(VocTb model)
        {
            try
            {
                await context.VocTbs.AddAsync(model);
             
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch (DbUpdateException dbEx)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                throw;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 사업장별 민원 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<AllVocListDTO>?> GetVocList(int placeid, List<int> type, List<int> status, List<int> buildingid)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs.Where(m => type.Contains(m.Type) &&
                    status.Contains(m.Status) && 
                    buildingid.Contains(m.BuildingTbId))
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if(VocList is [_, ..])
                {
                    var grouplist = VocList.GroupBy(voc => new
                    {
                        voc.CreateDt.Year,
                        voc.CreateDt.Month
                    }).Select(group => new
                    {
                        Year = group.Key.Year,
                        Month = group.Key.Month,
                        VocList = group.ToList()
                    }).ToList();

                    if (grouplist is not [_, ..])
                        return null;

                    List<AllVocListDTO> AllVocList = new List<AllVocListDTO>();
                    foreach(var Group in grouplist)
                    {
                        AllVocListDTO VocItem = new AllVocListDTO();
                        VocItem.Years = Group.Year;
                        VocItem.Month = Group.Month;
                        VocItem.Dates = $"{Group.Year}-{Group.Month}";

                        List<VocListDTO> dto = (from VocTB in Group.VocList
                                                join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                                on VocTB.BuildingTbId equals BuildingTB.Id
                                                select new VocListDTO
                                                {
                                                    Id = VocTB.Id, // VOCID
                                                    BuildingName = BuildingTB.Name, // 건물명
                                                    Type = VocTB.Type, // 유형
                                                    Title = VocTB.Title, // 제목
                                                    Status = VocTB.Status, // 처리상태
                                                    CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 민원 요청시간
                                                    CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 민원처리 완료시간 -- .ToString() 에러
                                                    DurationDT = VocTB.DurationDt // 민원처리 소요시간
                                                }).OrderBy(m => m.CreateDT)
                                                .ToList();

                        VocItem.VocList = dto;
                        AllVocList.Add(VocItem);
                    }

                    if (AllVocList is [_, ..])
                        return AllVocList;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 조건별 민원 리스트 조회
        /// </summary>
        /// <param name="buildinglist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status,List<int> BuildingID)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => Type.Contains(m.Type) &&
                    status.Contains(m.Status) && 
                    BuildingID.Contains(m.BuildingTbId) &&
                    m.CreateDt >= StartDate && m.CreateDt <= EndDate.AddDays(1))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (VocList is [_, ..])
                {
                    List<VocListDTO>? dto = (from VocTB in VocList
                                            join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                            on VocTB.BuildingTbId equals BuildingTB.Id
                                            select new VocListDTO
                                            {
                                                CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                Id = VocTB.Id, // VOC ID
                                                BuildingName = BuildingTB.Name, // 건물명
                                                Type = VocTB.Type, // 유형
                                                Title = VocTB.Title, // 민원제목
                                                Status = VocTB.Status, // 민원상태
                                                CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                DurationDT = VocTB.DurationDt // 소요시간
                                            }).OrderBy(m => m.CreateDT)
                                            .ToList();

                    return dto;
                }
                else
                {
                    // VocList 조회결과가 없음.
                    return null;
                }
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// ID로 민원 상세조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async Task<VocTb?> GetVocInfoById(int vocid)
        {
            try
            {
                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Id == vocid && m.DelYn != true)
                    .ConfigureAwait(false);

                if(model is not null)
                    return model;
                else
                    return null;
            }
            catch (MySqlException mysqlEx)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

       

        /// <summary>
        /// Code로 민원 상세조회
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<VocTb?> GetVocInfoByCode(string code)
        {
            try
            {
                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Code == code && m.DelYn != true)
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

        public async Task<bool> UpdateVocInfo(VocTb model)
        {
            try
            {
                context.VocTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

    
    }
}
