using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Facility.ItemKey
{
    public class FacilityItemKeyInfoRepository : IFacilityItemKeyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;
        
        public FacilityItemKeyInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹의 Key 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemKeyTb?> AddAsync(FacilityItemKeyTb model)
        {
            try
            {
                context.FacilityItemKeyTbs.Add(model);
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
        /// 그룹ID에 포함되어있는 KEY 리스트 전체 반환
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityItemKeyTb>?> GetAllKeyList(int groupitemid)
        {
            try
            {
                List<FacilityItemKeyTb>? model = await context.FacilityItemKeyTbs
                    .Where(m => m.FacilityItemGroupTbId == groupitemid && m.DelYn != true)
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
        /// KeyID에 해당하는 KEY모델 반환
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemKeyTb?> GetKeyInfo(int keyid)
        {
            try
            {
                FacilityItemKeyTb? model = await context.FacilityItemKeyTbs
                    .FirstOrDefaultAsync(m => m.Id == keyid && m.DelYn != true);

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
        /// Key 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateKeyInfo(FacilityItemKeyTb model)
        {
            try
            {
                context.FacilityItemKeyTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Key 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteKeyInfo(FacilityItemKeyTb model)
        {
            try
            {
                context.FacilityItemKeyTbs.Update(model);
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
