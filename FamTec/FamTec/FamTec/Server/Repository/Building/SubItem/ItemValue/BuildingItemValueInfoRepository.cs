using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building.SubItem.ItemValue
{
    public class BuildingItemValueInfoRepository : IBuildingItemValueInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingItemValueInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 아이템 KEY에 대한 Value 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemValueTb?> AddAsync(BuildingItemValueTb model)
        {
            try
            {
                await context.BuildingItemValueTbs.AddAsync(model);
                
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
        /// 아이템 Value 리스트 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemValueTb>?> GetAllValueList(int keyid)
        {
            try
            {
                List<BuildingItemValueTb>? model = await context.BuildingItemValueTbs
                    .Where(m => m.BuildingKeyTbId == keyid && 
                                m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(model is not null && model.Any())
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
        /// 아이템 Value 상세검색 valueid로 검색
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemValueTb?> GetValueInfo(int valueid)
        {
            try
            {
                BuildingItemValueTb? model = await context.BuildingItemValueTbs
                    .FirstOrDefaultAsync(m => m.Id == valueid && 
                                              m.DelYn != true);

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
        public async ValueTask<bool?> UpdateValueInfo(BuildingItemValueTb model)
        {
            try
            {
                context.BuildingItemValueTbs.Update(model);
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
        public async ValueTask<bool?> DeleteValueInfo(BuildingItemValueTb model)
        {
            try
            {
                context.BuildingItemValueTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<List<BuildingItemValueTb>?> ContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemValueTb>? keytb = await context.BuildingItemValueTbs
                    .Where(e => KeyitemId.Contains(Convert.ToInt32(e.BuildingKeyTbId)) && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (keytb is not null && keytb.Any())
                    return keytb;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<List<BuildingItemValueTb>?> NotContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemValueTb>? keytb = await context.BuildingItemValueTbs
                    .Where(e => !KeyitemId.Contains(Convert.ToInt32(e.BuildingKeyTbId)) && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (keytb is not null && keytb.Any())
                    return keytb;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
