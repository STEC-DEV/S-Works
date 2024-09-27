using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Alarm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace FamTec.Server.Repository.Alarm
{
    public class AlarmInfoRepository : IAlarmInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public AlarmInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 알람 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<AlarmTb?> AddAsync(AlarmTb model)
        {
            try
            {
                await context.AlarmTbs.AddAsync(model).ConfigureAwait(false);

                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 알람추가 리스트
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="Creater"></param>
        /// <param name="AlarmType"></param>
        /// <param name="VocTBId"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddAlarmList(List<UsersTb>? userlist, string Creater, int AlarmType, int VocTBId)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        foreach (UsersTb UserTB in userlist)
                        {
                            AlarmTb AlarmTB = new AlarmTb
                            {
                                Type = AlarmType,
                                UsersTbId = UserTB.Id,
                                VocTbId = VocTBId,
                                CreateDt = ThisDate,
                                CreateUser = Creater,
                                UpdateDt = ThisDate,
                                UpdateUser = Creater
                            };

                            await context.AlarmTbs.AddAsync(AlarmTB).ConfigureAwait(false);
                        }

                        bool Result = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (Result)
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
                        return false;
                    }
                }
            });
        }

        /// <summary>
        /// 사용자의 안읽은 알람 전체조회
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<List<AlarmDTO>?> GetAlarmList(int userid)
        {
            try
            {
                List<AlarmTb>? AlarmTB = await context.AlarmTbs
                    .Where(m => m.DelYn != true && m.UsersTbId == userid)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (AlarmTB is null || !AlarmTB.Any())
                    return null;

                List<AlarmDTO?> dto = (from Alarm in AlarmTB
                                       join VocTB in context.VocTbs.Where(m => m.DelYn != true)
                                       on Alarm.VocTbId equals VocTB.Id
                                       join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true)
                                       on  VocTB.BuildingTbId equals BuildingTB.Id
                                       select new AlarmDTO
                                       {
                                           AlarmID = Alarm.Id,
                                           Type = Alarm.Type,
                                           VocTitle = VocTB.Title!.Length > 8 ? VocTB.Title.Substring(0, 8) + "..." : VocTB.Title,
                                           UserID = Alarm.UsersTbId,
                                           VocID = Alarm.VocTbId,
                                           BuildingName = BuildingTB.Name,
                                           CODE = VocTB.Code
                                       }).ToList();

                if (dto is [_, ..])
                    return dto;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 사용자의 안읽은 알람 조회 - 2주 이전건 출력안됨
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public async ValueTask<List<AlarmDTO>?> GetAlarmListByDate(int userid, DateTime StartDate)
        {
            try
            {
                List<AlarmTb>? AlarmTB = await context.AlarmTbs
                   .Where(m => m.DelYn != true &&
                               m.UsersTbId == userid &&
                               m.CreateDt >= StartDate.AddDays(-14))
                   .OrderBy(m => m.CreateDt)
                   .ToListAsync()
                   .ConfigureAwait(false);

                if (AlarmTB is null || !AlarmTB.Any())
                    return null;

                List<AlarmDTO> dto = (from Alarm in AlarmTB
                                      join VocTB in context.VocTbs.Where(m => m.DelYn != true)
                                      on Alarm.VocTbId equals VocTB.Id
                                      join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true)
                                       on VocTB.BuildingTbId equals BuildingTB.Id
                                      select new AlarmDTO
                                      {
                                          AlarmID = Alarm.Id,
                                          Type = Alarm.Type,
                                          VocTitle = VocTB.Title!.Length > 8 ? VocTB.Title.Substring(0, 8) + "..." : VocTB.Title,
                                          UserID = Alarm.UsersTbId,
                                          VocID = Alarm.VocTbId,
                                          BuildingName = BuildingTB.Name,
                                          CODE = VocTB.Code
                                      }).ToList();

                if (dto is [_, ..])
                    return dto;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 사용자의 알람 전체읽음처리
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AllAlarmDelete(int userid, string deleter)
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

                        List<AlarmTb>? AlarmList = await context.AlarmTbs
                            .Where(m => m.DelYn != true && m.UsersTbId == userid)
                            .OrderBy(m => m.CreateDt)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        if (!AlarmList.Any())
                            return true;

                        foreach (AlarmTb AlarmTB in AlarmList)
                        {
                            AlarmTB.DelYn = true;
                            AlarmTB.DelDt = ThisDate;
                            AlarmTB.DelUser = deleter;

                            context.AlarmTbs.Update(AlarmTB);
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
                        return false;
                    }
                }
            });
        }

        /// <summary>
        /// 알람 읽음처리
        /// </summary>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<bool?> AlarmDelete(int alarmId, string deleter)
        {
            try
            {
                AlarmTb? AlarmTB = await context.AlarmTbs
                    .FirstOrDefaultAsync(m => m.Id == alarmId && m.DelYn != true)
                    .ConfigureAwait(false);

                if (AlarmTB is null)
                    return null;

                AlarmTB.DelYn = true;
                AlarmTB.DelDt = DateTime.Now;
                AlarmTB.DelUser = deleter;

                context.AlarmTbs.Update(AlarmTB);

                bool DeleteResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                if (DeleteResult)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw;
            }
        }
    

   
    }
}
