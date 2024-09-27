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
        Task<AlarmTb?> AddAsync(AlarmTb model);

        /// <summary>
        /// 알람 추가 리스트
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="Creater"></param>
        /// <param name="AlarmType"></param>
        /// <param name="VocTBId"></param>
        /// <returns></returns>
        Task<bool?> AddAlarmList(List<UsersTb>? userlist, string Creater, int AlarmType, int VocTBId);

        /// <summary>
        /// 사용자의 안읽은 알람 전체조회
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<List<AlarmDTO>?> GetAlarmList(int userid);

        /// <summary>
        /// 사용자의 안읽은 알람 조회 - 2주 이전건 출력안됨
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        Task<List<AlarmDTO>?> GetAlarmListByDate(int userid, DateTime StartDate);

        /// <summary>
        /// 사용자의 알람 전체읽음 처리
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<bool?> AllAlarmDelete(int userid, string deleter);

        /// <summary>
        /// 알람 읽음처리
        /// </summary>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        Task<bool?> AlarmDelete(int alarmId, string deleter);
    }
}
