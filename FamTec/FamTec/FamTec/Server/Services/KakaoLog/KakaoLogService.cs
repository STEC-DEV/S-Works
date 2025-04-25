using FamTec.Server.Helpers;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.KakaoLog;

namespace FamTec.Server.Services.KakaoLog
{
    public class KakaoLogService : IKakaoLogService
    {
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<KakaoLogService> CreateBuilderLogger; /* 콘솔로그 */

        public KakaoLogService(IKakaoLogInfoRepository _kakaologinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IHttpContextAccessor _httpcontextaccessor,
            ILogService _logservice,
            ConsoleLogService<KakaoLogService> _createbuilderlogger)
        {
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }


        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 기간 조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<KakaoLogListDTO>>> GetKakaoLogDateListService(DateTime StartDate, DateTime EndDate,int isSuccess)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (BuildingList is null || !BuildingList.Any())
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogList(Convert.ToInt32(placeid),isSuccess).ConfigureAwait(false);
                if (KakaoList is not null && KakaoList.Any())
                {
                    List<KakaoLogListDTO>? dto = (from LogTB in KakaoList
                                                  join BuildingTB in BuildingList
                                                  on LogTB.BuildingTbId equals BuildingTB.Id
                                                  select new KakaoLogListDTO
                                                  {
                                                      Id = LogTB.Id,
                                                      Message = LogTB.RsltMessage,
                                                      CreateDT = LogTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      BuildingName = BuildingTB.Name,
                                                      VocId = LogTB.VocTbId
                                                  })
                                                  .OrderByDescending(m => m.CreateDT)
                                                  .ToList();

                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<KakaoLogListDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<KakaoLogListDTO>>> GetKakaoLogListService(int isSuccess)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if(BuildingList is null || !BuildingList.Any())
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogList(Convert.ToInt32(placeid),isSuccess).ConfigureAwait(false);
                if (KakaoList is not null && KakaoList.Any())
                {
                    List<KakaoLogListDTO>? dto = (from LogTB in KakaoList
                                                  join BuildingTB in BuildingList
                                                  on LogTB.BuildingTbId equals BuildingTB.Id
                                                  select new KakaoLogListDTO
                                                  {
                                                      Id = LogTB.Id,
                                                      Message = LogTB.RsltMessage,
                                                      CreateDT = LogTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      BuildingName = BuildingTB.Name,
                                                      VocId = LogTB.VocTbId
                                                  })
                                                  .OrderByDescending(m => m.CreateDT)
                                                  .ToList();

                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<KakaoLogListDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장 카카오 로그 리스트 카운트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseModel<int?>> GetKakaoLogCountService()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int? count = await KakaoLogInfoRepository.GetKakaoLogCount(Int32.Parse(placeid)).ConfigureAwait(false);
                return new ResponseModel<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 카카오 로그 리스트 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<ResponseModel<List<KakaoLogListDTO>>> GetKakaoLogPageNationListService(int pagenumber, int pagesize)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (BuildingList is null || !BuildingList.Any())
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogPageNationList(Convert.ToInt32(placeid), pagenumber, pagesize).ConfigureAwait(false);
                if (KakaoList is not null && KakaoList.Any())
                {
                    List<KakaoLogListDTO>? dto = (from LogTB in KakaoList
                                                  join BuildingTB in BuildingList
                                                  on LogTB.BuildingTbId equals BuildingTB.Id
                                                  select new KakaoLogListDTO
                                                  {
                                                      Id = LogTB.Id,
                                                      Message = LogTB.Message,
                                                      CreateDT = LogTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      BuildingName = BuildingTB.Name,
                                                      VocId = LogTB.VocTbId
                                                  }).ToList();

                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<KakaoLogListDTO>>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<KakaoLogListDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }     
    }
}