using FamTec.Server.Helpers;
using FamTec.Shared.Server.DTO.Alarm;

namespace FamTec.Server.Services.Alarm
{
    public interface IAlarmService
    {
        /// <summary>
        /// 사용자의 안읽은 알람 전체조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<AlarmDTO>>> GetAllAlarmService();

        /// <summary>
        /// 사용자의 안읽음 알람 조회 - 2주 이전건 출력안됨
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<List<AlarmDTO>>> GetAllAlarmByDateService(DateTime StartDate);

        /// <summary>
        /// 사용자의 알람 전체 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> AllAlarmDelete();

        /// <summary>
        /// 알람 삭제
        /// </summary>
        /// <param name="alarmid"></param>
        /// <returns></returns>
        public Task<ResponseModel<bool?>> AlarmDelete(int? alarmid);
    }
}
