using FamTec.Server.Repository.Alarm;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Alarm;

namespace FamTec.Server.Services.Alarm
{
    public class AlarmService : IAlarmService
    {
        private readonly IAlarmInfoRepository AlarmInfoRepository;
        private ILogService LogService;

        public AlarmService(IAlarmInfoRepository _alarminforepository,
            ILogService _logservice)
        {
            this.AlarmInfoRepository = _alarminforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사용자의 안읽은 알람 전체조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AlarmDTO>> GetAllAlarmService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AlarmDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if(String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseList<AlarmDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<AlarmDTO>? model = await AlarmInfoRepository.GetAlarmList(Convert.ToInt32(UserIdx));
                if (model is not null && model.Any())
                    return new ResponseList<AlarmDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<AlarmDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AlarmDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사용자의 안읽음 알람 조회 - 2주 이전건 출력안됨
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AlarmDTO>> GetAllAlarmByDateService(HttpContext? context, DateTime StartDate)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AlarmDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseList<AlarmDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<AlarmDTO>? model = await AlarmInfoRepository.GetAlarmListByDate(Convert.ToInt32(UserIdx), StartDate);
                if (model is not null && model.Any())
                    return new ResponseList<AlarmDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<AlarmDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AlarmDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 알람 전체삭제
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AllAlarmDelete(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                string? deleter = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(UserIdx) || String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 여기수정
                bool? result = await AlarmInfoRepository.AllAlarmDelete(Convert.ToInt32(UserIdx), deleter);
                
                return result switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 알람 삭제
        /// </summary>
        /// <param name="alarmid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AlarmDelete(HttpContext? context, int? alarmid)
        {
            try
            {
                if (context is null || alarmid is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? deleter = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? result = await AlarmInfoRepository.AlarmDelete(alarmid.Value, deleter);
                return result switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 }, // 성공
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 }, // 실패
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 } // 잘못된 요청
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

     
    }
}
