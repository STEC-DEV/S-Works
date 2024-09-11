using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace FamTec.Server.Repository.Building.SubItem.ItemKey
{
    public class BuildingItemKeyInfoRepository : IBuildingItemKeyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingItemKeyInfoRepository(WorksContext _context,
            ILogService _logservice)
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
        public async ValueTask<BuildingItemKeyTb?> AddAsync(BuildingItemKeyTb model)
        {
            try
            {
                await context.BuildingItemKeyTbs.AddAsync(model);
             
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
        /// 그룹의 KEY 리스트 상세검색 groupitemid로 검색
        /// </summary>
        /// <param name="groupitemid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingItemKeyTb>?> GetAllKeyList(int groupitemid)
        {
            try
            {
                List<BuildingItemKeyTb>? model = await context.BuildingItemKeyTbs
                    .Where(m => m.BuildingGroupTbId == groupitemid && m.DelYn != true)
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
        /// 그룹 KEY 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async ValueTask<BuildingItemKeyTb?> GetKeyInfo(int keyid)
        {
            try
            {
                BuildingItemKeyTb? model = await context.BuildingItemKeyTbs
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
        /// 그룹 KEY 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateKeyInfo(BuildingItemKeyTb model)
        {
            try
            {
                context.BuildingItemKeyTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹 KEY - VALUE 업데이트
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateKeyInfo(UpdateKeyDTO dto, string updater)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool? result = await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 디버깅 포인트를 강제로 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        BuildingItemKeyTb? KeyTB = await context.BuildingItemKeyTbs
                            .FirstOrDefaultAsync(m => m.Id == dto.ID && m.DelYn != true);

                        if (KeyTB is not null)
                        {
                            KeyTB.Name = dto.Itemkey!;
                            KeyTB.Unit = dto.Unit!;

                            context.BuildingItemKeyTbs.Update(KeyTB);
                            bool KeyUpdate = await context.SaveChangesAsync() > 0 ? true : false;

                            if (!KeyUpdate)
                            {
                                // KEY 업데이트 실패 시 rollback
                                await transaction.RollbackAsync();
                                return false;
                            }

                            // SELECT VALUE 정보 반환
                            List<BuildingItemValueTb>? ValueList = await context.BuildingItemValueTbs
                                .Where(m => m.BuildingKeyTbId == dto.ID &&
                                            m.DelYn != true)
                                .ToListAsync();

                            // NULL 인값 INSERT OR UPDATE OR DELETE
                            if (dto.ValueList is [_, ..])
                            {
                                List<GroupValueListDTO> INSERTLIST = dto.ValueList.Where(m => m.ID == null)
                                                                    .ToList();


                                // DTO IDList 중 NULL이 아닌것 -- 수정대상
                                List<GroupValueListDTO> UPDATELIST = dto.ValueList!.Where(m => m.ID != null)
                                                                    .ToList();

                                // DB IDList
                                List<int> db_valueidx = ValueList.Select(m => m.Id).ToList();

                                List<int> updateidx = UPDATELIST.Select(m => m.ID!.Value).ToList();
                                // 삭제대상 (디비 인덱스 - DTO 인덱스 = 남는 DB 인덱스)
                                List<int> delIdx = db_valueidx.Except(updateidx).ToList(); // list1에만 있는 값 -- DB에만있는값 (삭제할값)

                                //추가작업
                                foreach (GroupValueListDTO InsertInfo in INSERTLIST)
                                {
                                    BuildingItemValueTb InsertTB = new BuildingItemValueTb();
                                    InsertTB.ItemValue = InsertInfo.ItemValue!;
                                    InsertTB.CreateDt = DateTime.Now;
                                    InsertTB.CreateUser = updater;
                                    InsertTB.UpdateDt = DateTime.Now;
                                    InsertTB.UpdateUser = updater;
                                    InsertTB.BuildingKeyTbId = dto.ID!.Value;
                                    context.BuildingItemValueTbs.Add(InsertTB);
                                }

                                // 업데이트 작업
                                foreach (GroupValueListDTO UpdateInfo in UPDATELIST)
                                {
                                    BuildingItemValueTb? UpdateTB = await context.BuildingItemValueTbs
                                        .FirstOrDefaultAsync(m => m.Id == UpdateInfo.ID && m.DelYn != true);

                                    if (UpdateTB is not null)
                                    {
                                        UpdateTB.ItemValue = UpdateInfo.ItemValue!;
                                        UpdateTB.UpdateDt = DateTime.Now;
                                        UpdateTB.UpdateUser = updater;
                                        context.BuildingItemValueTbs.Update(UpdateTB);
                                    }
                                    else
                                    {
                                        await transaction.RollbackAsync();
                                        return false;
                                    }

                                }

                                // 삭제작업
                                foreach (int DelID in delIdx)
                                {
                                    BuildingItemValueTb? DeleteTB = await context.BuildingItemValueTbs
                                        .FirstOrDefaultAsync(m => m.Id == DelID && m.DelYn != true);

                                    if (DeleteTB is not null)
                                    {
                                        DeleteTB.DelDt = DateTime.Now;
                                        DeleteTB.DelYn = true;
                                        DeleteTB.DelUser = updater;
                                        context.BuildingItemValueTbs.Update(DeleteTB);
                                    }
                                    else
                                    {
                                        await transaction.RollbackAsync();
                                        return false;
                                    }
                                }

                            }
                            else // DELETE
                            {
                                if (ValueList is [_, ..])
                                {
                                    foreach (BuildingItemValueTb ValueTB in ValueList)
                                    {
                                        ValueTB.DelDt = DateTime.Now;
                                        ValueTB.DelYn = true;
                                        ValueTB.DelUser = updater;
                                        context.BuildingItemValueTbs.Update(ValueTB);
                                    }
                                }
                            }

                            bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                            if (DeleteResult)
                            {
                                await transaction.CommitAsync();
                                return true;
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }
                        else
                        {
                            return (bool?)null;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 그룹 KEY 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteKeyInfo(BuildingItemKeyTb model)
        {
            try
            {
                context.BuildingItemKeyTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹 KEY 리스트 삭제 - Value 까지 삭제됨
        /// </summary>
        /// <param name="KeyList"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteKeyList(List<int> KeyList, string deleter)
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            bool? result = await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 디버깅 포인트를 강제로 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (int KeyId in KeyList)
                        {
                            BuildingItemKeyTb? KeyTB = await context.BuildingItemKeyTbs.FirstOrDefaultAsync(m => m.Id == KeyId && m.DelYn != true);
                            if (KeyTB is null)
                                return (bool?)null;

                            KeyTB.DelDt = DateTime.Now;
                            KeyTB.DelUser = deleter;
                            KeyTB.DelYn = true;

                            context.BuildingItemKeyTbs.Update(KeyTB);

                            List<BuildingItemValueTb>? ValueList = await context.BuildingItemValueTbs.Where(m => m.BuildingKeyTbId == KeyTB.Id && m.DelYn != true).ToListAsync();
                            if (ValueList is [_, ..])
                            {
                                foreach (BuildingItemValueTb ValueTB in ValueList)
                                {
                                    ValueTB.DelDt = DateTime.Now;
                                    ValueTB.DelUser = deleter;
                                    ValueTB.DelYn = true;

                                    context.BuildingItemValueTbs.Update(ValueTB);
                                }
                            }
                        }

                        bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;

                        if (DeleteResult)
                        {
                            await transaction.CommitAsync();
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw new ArgumentNullException();
                    }
                }
            });
            return result;
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
                List<BuildingItemKeyTb>? keytb = await context.BuildingItemKeyTbs
                    .Where(e => GroupItemId.Contains(Convert.ToInt32(e.BuildingGroupTbId)) && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(keytb is not null && keytb.Any())
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
                List<BuildingItemKeyTb>? keytb = await context.BuildingItemKeyTbs
                    .Where(e => !GroupItemId.Contains(Convert.ToInt32(e.BuildingGroupTbId)) && e.DelYn != true)
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
