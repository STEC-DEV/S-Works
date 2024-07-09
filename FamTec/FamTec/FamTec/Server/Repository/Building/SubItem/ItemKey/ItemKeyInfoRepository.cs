using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building.SubItem.ItemKey
{
    public class ItemKeyInfoRepository : IItemKeyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public ItemKeyInfoRepository(WorksContext _context, ILogService _logservice)
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
        public async ValueTask<ItemkeyTb?> AddAsync(ItemkeyTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.ItemkeyTbs.Add(model);
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
        public async ValueTask<List<ItemkeyTb>?> GetAllKeyList(int? groupitemid)
        {
            try
            {
                if(groupitemid is not null)
                {
                    List<ItemkeyTb>? model = await context.ItemkeyTbs.Where(m => m.GroupItemId == groupitemid && m.DelYn != true).ToListAsync();

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
        public async ValueTask<ItemkeyTb?> GetKeyInfo(int? keyid)
        {
            try
            {
                if(keyid is not null)
                {
                    ItemkeyTb? model = await context.ItemkeyTbs.FirstOrDefaultAsync(m => m.Id == keyid && m.DelYn != true);

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
        public async ValueTask<bool?> UpdateKeyInfo(ItemkeyTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.ItemkeyTbs.Update(model);
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
        public async ValueTask<bool?> DeleteKeyInfo(ItemkeyTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.ItemkeyTbs.Update(model);
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
        public async ValueTask<List<ItemkeyTb>?> ContainsKeyList(List<int> GroupItemId)
        {
            try
            {
                List<ItemkeyTb>? keytb = await context.ItemkeyTbs.Where(e => GroupItemId.Contains(Convert.ToInt32(e.GroupItemId)) && e.DelYn != true).ToListAsync();
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
        public async ValueTask<List<ItemkeyTb>?> NotContainsKeyList(List<int> GroupItemId)
        {
            try
            {
                List<ItemkeyTb>? keytb = await context.ItemkeyTbs.Where(e => !GroupItemId.Contains(Convert.ToInt32(e.GroupItemId)) && e.DelYn != true).ToListAsync();
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
