using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Facility.ItemValue
{
    public class FacilityItemValueInfoRepository : IFacilityItemValueInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;
        
        public FacilityItemValueInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 아이템 KEY에 대한 VALUE 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemValueTb?> AddAsync(FacilityItemValueTb model)
        {
            try
            {
                await context.FacilityItemValueTbs.AddAsync(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
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
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// KEY에 해당하는 VALUE LIST 조회
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityItemValueTb>?> GetAllValueList(int keyid)
        {
            try
            {
                List<FacilityItemValueTb>? model = await context.FacilityItemValueTbs
                    .Where(m => m.FacilityItemKeyTbId == keyid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

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
        /// VALUE ID에 해당하는 Value 모델 반환
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemValueTb?> GetValueInfo(int valueid)
        {
            try
            {
                FacilityItemValueTb? model = await context.FacilityItemValueTbs
                    .FirstOrDefaultAsync(m => m.Id == valueid && m.DelYn != true);
                
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

        /// <summary>
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateValueInfo(FacilityItemValueTb model)
        {
            try
            {
                context.FacilityItemValueTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 아이템 Value 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteValueInfo(FacilityItemValueTb model)
        {
            try
            {
                context.FacilityItemValueTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


    
    }
}
