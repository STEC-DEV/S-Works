using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Alarm
{
    public interface IAlarmInfoRepository
    {
        /// <summary>
        /// 알람 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AlarmTb?> AddAsync(AlarmTb? model);
    }
}
