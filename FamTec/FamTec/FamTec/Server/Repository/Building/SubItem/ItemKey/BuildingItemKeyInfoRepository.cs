using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building.SubItem.ItemKey
{
    public class BuildingItemKeyInfoRepository : IBuildingItemKeyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingItemKeyInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹의 KEY 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<BuildingItemKeyTb?> AddAsync(BuildingItemKeyTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingItemKeyTbs.Add(model);
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
        /// 그룹의 KEY 리스트 상세검색 groupitemid로 검색
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemKeyTb>?> GetAllKeyList(int? groupitemid)
        {
            try
            {
                if(groupitemid is not null)
                {
                    List<BuildingItemKeyTb>? model = await context.BuildingItemKeyTbs.Where(m => m.BuildingGroupTbId == groupitemid && m.DelYn != true).ToListAsync();

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
        /// 그룹 KEY 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemKeyTb?> GetKeyInfo(int? keyid)
        {
            try
            {
                if(keyid is not null)
                {
                    BuildingItemKeyTb? model = await context.BuildingItemKeyTbs.FirstOrDefaultAsync(m => m.Id == keyid && m.DelYn != true);

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
        /// 그룹 KEY 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateKeyInfo(BuildingItemKeyTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.BuildingItemKeyTbs.Update(model);
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
        /// 그룹 KEY 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteKeyInfo(BuildingItemKeyTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.BuildingItemKeyTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 넘어온 GroupItemId에 포함되어있는 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemKeyTb>?> ContainsKeyList(List<int> GroupItemId)
        {
            try
            {
                List<BuildingItemKeyTb>? keytb = await context.BuildingItemKeyTbs.Where(e => GroupItemId.Contains(Convert.ToInt32(e.BuildingGroupTbId)) && e.DelYn != true).ToListAsync();
                if (keytb is [_, ..])
                    return keytb;
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
        /// 넘어온 GroupItemId에 포함되어있지 않은 KeyTb 반환
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemKeyTb>?> NotContainsKeyList(List<int> GroupItemId)
        {
            try
            {
                List<BuildingItemKeyTb>? keytb = await context.BuildingItemKeyTbs.Where(e => !GroupItemId.Contains(Convert.ToInt32(e.BuildingGroupTbId)) && e.DelYn != true).ToListAsync();
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
