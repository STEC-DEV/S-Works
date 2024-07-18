using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Maintenence
{
    /// <summary>
    /// 유지보수 이력을 하려면 자재부터 해야함.
    /// </summary>
    public class MaintenenceRepository : IMaintenenceRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public MaintenenceRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 유지보수이력 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<MaintenenceHistoryTb?> AddAsync(MaintenenceHistoryTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.MaintenenceHistoryTbs.Add(model);
                    await context.SaveChangesAsync();
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
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async ValueTask<List<MaintenenceHistoryTb>?> GetFacilityHistoryList(int? facilityid)
        {
            try
            {
                if(facilityid is not null)
                {
                    List<MaintenenceHistoryTb>? model = await context.MaintenenceHistoryTbs
                        .Where(m => m.FacilityTbId == facilityid && m.DelYn != true)
                        .ToListAsync();

                    if (model is [_, ..])
                        return model;
                    else
                        return null;
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
        /// 유지보수 이력 사업장별 전체 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<List<MaintenenceHistoryTb>?> GetPlaceHistoryList(int? placeid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ValueTask<List<MaintenenceHistoryTb>?> GetDateHistoryList(int? placeid, string? date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유지보수이력 ID에 해당하는 상세정보 검색
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? GetDetailHistoryInfo(int? id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 유지보수이력 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? UpdateHistoryInfo(MaintenenceHistoryTb? model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유지보수이력 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? DeleteHistoryInfo(MaintenenceHistoryTb? model)
        {
            throw new NotImplementedException();
        }
    }
}
