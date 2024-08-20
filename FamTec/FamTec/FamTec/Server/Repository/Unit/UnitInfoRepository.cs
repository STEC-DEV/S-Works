using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Unit
{
    public class UnitInfoRepository : IUnitInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public UnitInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<UnitTb?> AddAsync(UnitTb model)
        {
            try
            {
                context.UnitTbs.Add(model);
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
        /// 사업장별 단위 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<UnitTb>?> GetUnitList(int placeid)
        {
            try
            {
                List<UnitTb>? model = await context.UnitTbs
                    .Where(m => m.PlaceTbId == null || m.PlaceTbId == placeid && m.DelYn != true)
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
        /// 단위정보 인덱스로 단위모델 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        public async ValueTask<UnitTb?> GetUnitInfo(int UnitIdx)
        {
            try
            {
                UnitTb? model = await context.UnitTbs
                    .FirstOrDefaultAsync(m => m.Id == UnitIdx && m.DelYn != true);

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
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteUnitInfo(UnitTb model)
        {
            try
            {
                context.UnitTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateUnitInfo(UnitTb model)
        {
            try
            {
                context.UnitTbs.Update(model);
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
