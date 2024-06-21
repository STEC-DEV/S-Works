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
        public async ValueTask<UnitTb?> AddAsync(UnitTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.UnitTbs.Add(model);
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
        /// 사업장별 단위 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<UnitTb>?> GetUnitList(int? placeid)
        {
            try
            {
                if(placeid is not null)
                {
                    List<UnitTb>? model = await context.UnitTbs.Where(m => m.PlaceTbId == null || m.PlaceTbId == placeid && m.DelYn != 1).ToListAsync();

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
        /// 단위정보 인덱스로 단위모델 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        public async ValueTask<UnitTb?> GetUnitInfo(int? UnitIdx)
        {
            try
            {
                if(UnitIdx is not null)
                {
                    UnitTb? model = await context.UnitTbs.FirstOrDefaultAsync(m => m.Id == UnitIdx && m.DelYn != 1);

                    if (model is not null)
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
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteUnitInfo(UnitTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.UnitTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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


 
    }
}
