using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;

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
        public async ValueTask<AlarmTb?> AddAsync(AlarmTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.AlarmTbs.Add(model);
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
    }
}
