using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Meter
{
    public class MeterInfoRepository : IMeterInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public MeterInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 검침기 추가 - 전기일 경우 / 외래키 ContractTypeTB_ID 넣어야함.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<MeterItemTb?> AddAsync(MeterItemTb model)
        {
            try
            {
                await context.MeterItemTbs.AddAsync(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;

                if (AddResult)
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
        /// 사업장에 속한 검침기 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<MeterItemTb>?> GetAllMeterList(int placeid)
        {
            try
            {
                List<MeterItemTb>? model = await context.MeterItemTbs
                    .Where(m => m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .ToListAsync();

                if (model is not null && model.Any())
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
        /// 사업장에 속한 해당 카테고리 검침기 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async ValueTask<List<MeterItemTb>?> GetAllCategoryMeterList(int placeid, string category)
        {
            try
            {
                List<MeterItemTb>? model = await context.MeterItemTbs
                   .Where(m => m.PlaceTbId == placeid &&
                               m.Category.Equals(category) &&
                               m.DelYn != true)
                   .ToListAsync();

                if (model is not null && model.Any())
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
        /// 검침기 명칭으로 검색
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async ValueTask<MeterItemTb?> GetMeterName(int placeid, string name)
        {
            try
            {
                MeterItemTb? model = await context.MeterItemTbs.FirstOrDefaultAsync(m => m.PlaceTbId == placeid && m.Name == name && m.DelYn != true);

                if (model is not null)
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

    }
}
