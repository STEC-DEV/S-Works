using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

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
                await context.UnitTbs.AddAsync(model).ConfigureAwait(false);

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
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
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
        /// 단위정보 인덱스로 단위모델 조회
        /// </summary>
        /// <param name="UnitIdx"></param>
        /// <returns></returns>
        public async ValueTask<UnitTb?> GetUnitInfo(int UnitIdx)
        {
            try
            {
                UnitTb? model = await context.UnitTbs
                    .FirstOrDefaultAsync(m => m.Id == UnitIdx && m.DelYn != true)
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
        /// 해당사업장에 단위 추가되는지 여부
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddUnitInfoCheck(string unit, int placeid)
        {
            try
            {
                UnitTb? UnitInfo = await context.UnitTbs
                    .FirstOrDefaultAsync(m => m.Unit == unit && 
                                              m.PlaceTbId == placeid && 
                                              m.DelYn != true)
                    .ConfigureAwait(false);

                if (UnitInfo is not null)
                    return false;
                else
                    return true;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteUnitInfo(List<int> idx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
            
            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        foreach (int unitid in idx)
                        {
                            UnitTb? UnitModel = await context.UnitTbs
                                .FirstOrDefaultAsync(m => m.Id == unitid &&
                                                            m.DelYn != true)
                                .ConfigureAwait(false);

                            if (UnitModel is null)
                                return (bool?)null;

                            // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                            UnitModel.Unit = $"{UnitModel.Unit}_{UnitModel.Id}";
                            UnitModel.DelDt = ThisDate;
                            UnitModel.DelUser = deleter;
                            UnitModel.DelYn = true;

                            context.UnitTbs.Update(UnitModel);
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
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
                        throw;
                    }
                }
            });
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
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

     
    }
}
