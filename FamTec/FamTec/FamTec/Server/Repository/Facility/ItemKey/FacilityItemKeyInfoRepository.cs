using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Facility.ItemKey
{
    public class FacilityItemKeyInfoRepository : IFacilityItemKeyInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;
        
        public FacilityItemKeyInfoRepository(WorksContext _context,
            ILogService _logservice)
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
                await context.FacilityItemKeyTbs.AddAsync(model);
                
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
        /// 그룹 KEY - VALUE 업데이트
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateKeyInfo(UpdateKeyDTO dto, string updater)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    FacilityItemKeyTb? KeyTB = await context.FacilityItemKeyTbs
                        .FirstOrDefaultAsync(m => m.Id == dto.ID && m.DelYn != true);

                    if (KeyTB is null)
                        return null;
                    
                    KeyTB.Name = dto.Itemkey!;
                    KeyTB.Unit = dto.Unit!;

                    context.FacilityItemKeyTbs.Update(KeyTB);
                    bool KeyUpdate = await context.SaveChangesAsync() > 0 ? true : false;

                    if(!KeyUpdate)
                    {
                        // KEY 업데이트 실패시 Rollback
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // SELECT VALUE 정보 반환
                    List<FacilityItemValueTb>? ValueList = await context.FacilityItemValueTbs
                        .Where(m => m.FacilityItemKeyTbId == dto.ID && m.DelYn != true)
                        .ToListAsync();

                    // NULL 인값 INSERT OR UPDATE OR DELETE
                    if(dto.ValueList is [_, ..])
                    {
                        List<GroupValueListDTO> INSERTLIST = dto.ValueList.Where(m => m.ID == null).ToList();

                        // DTO IDList중 NULL이 아닌것 -- 수정대상
                        List<GroupValueListDTO> UPDATELIST = dto.ValueList.Where(m => m.ID != null).ToList();

                        // DB IDList
                        List<int> db_valueidx = ValueList.Select(m => m.Id).ToList();

                        List<int> updateidx = UPDATELIST.Select(m => m.ID!.Value).ToList();
                        // 삭제대상 (디비 인덱스 - DTO 인덱스 = 남는 DTO 인덱스)
                        List<int> delIdx = db_valueidx.Except(updateidx).ToList(); // list1에만 있는 값 -- DB에만 있는값 (삭제할값)

                        // 추가작업
                        foreach(GroupValueListDTO InsertInfo in INSERTLIST)
                        {
                            FacilityItemValueTb InsertTB = new FacilityItemValueTb();
                            InsertTB.ItemValue = InsertInfo.ItemValue!;
                            InsertTB.CreateDt = DateTime.Now;
                            InsertTB.CreateUser = updater;
                            InsertTB.UpdateDt = DateTime.Now;
                            InsertTB.UpdateUser = updater;
                            InsertTB.FacilityItemKeyTbId = dto.ID!.Value;
                            context.FacilityItemValueTbs.Add(InsertTB);
                        }

                        // 업데이트 작업
                        foreach(GroupValueListDTO UpdateInfo in UPDATELIST)
                        {
                            FacilityItemValueTb? UpdateTB = await context.FacilityItemValueTbs.
                                FirstOrDefaultAsync(m => m.Id == UpdateInfo.ID && m.DelYn != true);

                            if(UpdateTB is not null)
                            {
                                UpdateTB.ItemValue = UpdateInfo.ItemValue!;
                                UpdateTB.UpdateDt = DateTime.Now;
                                UpdateTB.UpdateUser = updater;
                                context.FacilityItemValueTbs.Update(UpdateTB);
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }

                        // 삭제작업
                        foreach(int DelID in delIdx)
                        {
                            FacilityItemValueTb? DeleteTB = await context.FacilityItemValueTbs
                                .FirstOrDefaultAsync(m => m.Id == DelID && m.DelYn != true);

                            if(DeleteTB is not null)
                            {
                                DeleteTB.DelDt = DateTime.Now;
                                DeleteTB.DelYn = true;
                                DeleteTB.DelUser = updater;
                                context.FacilityItemValueTbs.Update(DeleteTB);
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
                        if(ValueList is [_, ..])
                        {
                            foreach(FacilityItemValueTb ValueTB in ValueList)
                            {
                                ValueTB.DelDt = DateTime.Now;
                                ValueTB.DelYn = true;
                                ValueTB.DelUser = updater;
                                context.FacilityItemValueTbs.Update(ValueTB);
                            }
                        }
                    }

                    bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(DeleteResult)
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
                catch(Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
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

        /// <summary>
        /// 그룹 Key 리스트 삭제 - Value 까지 삭제됨
        /// </summary>
        /// <param name="KeyList"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteKeyList(List<int> KeyList, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (int KeyId in KeyList)
                    {
                        FacilityItemKeyTb? KeyTB = await context.FacilityItemKeyTbs.FirstOrDefaultAsync(m => m.Id == KeyId && m.DelYn != true);
                        if (KeyTB is null)
                            return null;

                        KeyTB.DelDt = DateTime.Now;
                        KeyTB.DelUser = deleter;
                        KeyTB.DelYn = true;

                        context.FacilityItemKeyTbs.Update(KeyTB);

                        List<FacilityItemValueTb>? ValueList = await context.FacilityItemValueTbs.Where(m => m.Id == KeyTB.Id && m.DelYn != true).ToListAsync();
                        if (ValueList is [_, ..])
                        {
                            foreach(FacilityItemValueTb ValueTB in ValueList)
                            {
                                ValueTB.DelDt = DateTime.Now;
                                ValueTB.DelUser = deleter;
                                ValueTB.DelYn = true;

                                context.FacilityItemValueTbs.Update(ValueTB);
                            }
                        }
                    }

                    bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(DeleteResult)
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
                catch(Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
