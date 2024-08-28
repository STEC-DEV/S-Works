using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Alarm;

namespace FamTec.Server.Repository.Alarm
{
    public interface IAlarmInfoRepository
    {
        /// <summary>
        /// 알람 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AlarmTb?> AddAsync(AlarmTb model);

        /// <summary>
        /// 사용자의 안읽은 알람 전체조회
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        ValueTask<List<AlarmDTO>?> GetAlarmList(int userid);

        /// <summary>
        /// 사용자의 안읽은 알람 조회 - 2주 이전건 출력안됨
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        ValueTask<List<AlarmDTO>?> GetAlarmListByDate(int userid, DateTime StartDate);

        /// <summary>
        /// 사용자의 알람 전체읽음 처리
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        ValueTask<bool?> AllAlarmDelete(int userid, string deleter);

        /// <summary>
        /// 알람 읽음처리
        /// </summary>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        ValueTask<bool?> AlarmDelete(int alarmId, string deleter);
    }
}
