using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

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
                await context.FacilityItemGroupTbs.AddAsync(model).ConfigureAwait(false);

                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                    .FirstOrDefaultAsync(m => m.Id == groupid && m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
                context.FacilityItemGroupTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
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
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        FacilityItemGroupTb? GroupTB = await context.FacilityItemGroupTbs
                            .FirstOrDefaultAsync(m => m.Id == groupid)
                            .ConfigureAwait(false);

                        if (GroupTB is null)
                            return (bool?)null;

                        GroupTB.DelDt = ThisDate;
                        GroupTB.DelUser = deleter;
                        GroupTB.DelYn = true;

                        context.FacilityItemGroupTbs.Update(GroupTB);

                        List<FacilityItemKeyTb>? KeyTB = await context.FacilityItemKeyTbs
                        .Where(m => m.FacilityItemGroupTbId == groupid)
                        .ToListAsync()
                        .ConfigureAwait(false);

                        if (KeyTB is [_, ..])
                        {
                            foreach (FacilityItemKeyTb KeyModel in KeyTB)
                            {
                                KeyModel.DelDt = ThisDate;
                                KeyModel.DelUser = deleter;
                                KeyModel.DelYn = true;

                                context.FacilityItemKeyTbs.Update(KeyModel);

                                List<FacilityItemValueTb>? ValueTB = await context.FacilityItemValueTbs
                                .Where(m => m.FacilityItemKeyTbId == KeyModel.Id)
                                .ToListAsync()
                                .ConfigureAwait(false);

                                if (ValueTB is [_, ..])
                                {
                                    foreach (FacilityItemValueTb ValueModel in ValueTB)
                                    {
                                        ValueModel.DelDt = ThisDate;
                                        ValueModel.DelUser = deleter;
                                        ValueModel.DelYn = true;

                                        context.FacilityItemValueTbs.Update(ValueModel);
                                    }
                                }
                            }
                        }

                        bool DeleteResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (DeleteResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
        }
    }
}
