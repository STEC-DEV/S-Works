using DocumentFormat.OpenXml.Spreadsheet;
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
        public async ValueTask<BuildingItemvalueTb?> AddAsync(BuildingItemvalueTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingItemvalueTbs.Add(model);
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
        /// 아이템 Value 리스트 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemvalueTb>?> GetAllValueList(int? keyid)
        {
            try
            {
                if(keyid is not null)
                {
                    List<BuildingItemvalueTb>? model = await context.BuildingItemvalueTbs.Where(m => m.ItemKeyId == keyid && m.DelYn != true).ToListAsync();

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
        /// 아이템 Value 상세검색 valueid로 검색
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemvalueTb?> GetValueInfo(int? valueid)
        {
            try
            {
                if(valueid is not null)
                {
                    BuildingItemvalueTb? model = await context.BuildingItemvalueTbs.FirstOrDefaultAsync(m => m.Id == valueid && m.DelYn != true);

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
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateValueInfo(BuildingItemvalueTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.BuildingItemvalueTbs.Update(model);
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

        /// <summary>
        /// 아이템 Value 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteValueInfo(BuildingItemvalueTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.BuildingItemvalueTbs.Update(model);
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

        public async ValueTask<List<BuildingItemvalueTb>?> ContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemvalueTb>? keytb = await context.BuildingItemvalueTbs.Where(e => KeyitemId.Contains(Convert.ToInt32(e.ItemKeyId)) && e.DelYn != true).ToListAsync();
                if (keytb is [_, ..])
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

        public async ValueTask<List<BuildingItemvalueTb>?> NotContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemvalueTb>? keytb = await context.BuildingItemvalueTbs.Where(e => !KeyitemId.Contains(Convert.ToInt32(e.ItemKeyId)) && e.DelYn != true).ToListAsync();
                if (keytb is [_, ..])
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
