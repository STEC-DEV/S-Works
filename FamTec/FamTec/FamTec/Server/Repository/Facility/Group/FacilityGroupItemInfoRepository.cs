using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Facility.Group
{
    public class FacilityGroupItemInfoRepository : IFacilityGroupItemInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public FacilityGroupItemInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemGroupTb?> AddAsync(FacilityItemGroupTb model)
        {
            try
            {
                await context.FacilityItemGroupTbs.AddAsync(model);
             
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
        /// 설비ID에 포함되어있는 그룹List 조회
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public async ValueTask<List<FacilityItemGroupTb>?> GetAllGroupList(int facilityId)
        {
            try
            {
                List<FacilityItemGroupTb>? model = await context.FacilityItemGroupTbs
                    .Where(m => m.FacilityTbId == facilityId && m.DelYn != true)
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
        /// 그룹ID로 모델 검색
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async ValueTask<FacilityItemGroupTb?> GetGroupInfo(int groupid)
        {
            try
            {
                FacilityItemGroupTb? model = await context.FacilityItemGroupTbs
                    .FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true);

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
        /// 그룹 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateGroupInfo(FacilityItemGroupTb model)
        {
            try
            {
                context.FacilityItemGroupTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(FacilityItemGroupTb model)
        {
            try
            {
                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                model.Name = $"{model.Name}_{model.Id}";
                context.FacilityItemGroupTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 그룹삭제 -2
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteGroupInfo(int groupid, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    FacilityItemGroupTb? GroupTB = await context.FacilityItemGroupTbs
                        .FirstOrDefaultAsync(m => m.Id == groupid);

                    if(GroupTB is null)
                        return null;

                    // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                    GroupTB.Name = $"{GroupTB.Name}_{GroupTB.Id}";
                    GroupTB.DelDt = DateTime.Now;
                    GroupTB.DelUser = deleter;
                    GroupTB.DelYn = true;

                    context.FacilityItemGroupTbs.Update(GroupTB);

                    List<FacilityItemKeyTb>? KeyTB = await context.FacilityItemKeyTbs.Where(m => m.FacilityItemGroupTbId == groupid).ToListAsync();
                        
                    if(KeyTB is [_, ..])
                    {
                        foreach(FacilityItemKeyTb KeyModel in KeyTB)
                        {
                            // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                            KeyModel.Name = $"{KeyModel.Name}_{KeyModel.Id}";
                            KeyModel.DelDt = DateTime.Now;
                            KeyModel.DelUser = deleter;
                            KeyModel.DelYn = true;

                            context.FacilityItemKeyTbs.Update(KeyModel);

                            List<FacilityItemValueTb>? ValueTB = await context.FacilityItemValueTbs.Where(m => m.FacilityItemKeyTbId == KeyModel.Id).ToListAsync();
                            if(ValueTB is [_, ..])
                            {
                                foreach(FacilityItemValueTb ValueModel in ValueTB)
                                {
                                    // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                    ValueModel.ItemValue = $"{ValueModel.ItemValue}_{ValueModel.Id}";
                                    ValueModel.DelDt = DateTime.Now;
                                    ValueModel.DelUser = deleter;
                                    ValueModel.DelYn = true;

                                    context.FacilityItemValueTbs.Update(ValueModel);
                                }
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
