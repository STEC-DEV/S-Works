using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;

namespace FamTec.Server.Services.KakaoLog
{
    public class KakaoLogService : IKakaoLogService
    {
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly ILogService LogService;
        private readonly ILogger<KakaoLogService> BuilderLogger;

        public KakaoLogService(IKakaoLogInfoRepository _kakaologinforepository,
            IBuildingInfoRepository _buildinginforepository,
            ILogService _logservice,
            ILogger<KakaoLogService> _builderlogger)
        {
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        /// <summary>
        /// ASP - 빌드로그
        /// </summary>
        /// <param name="ex"></param>
        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<KakaoLogListDTO>> GetKakaoLogListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if(BuildingList is null || !BuildingList.Any())
                    return new ResponseList<KakaoLogListDTO>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (KakaoList is not null && KakaoList.Any())
                {
                    List<KakaoLogListDTO>? dto = (from LogTB in KakaoList
                                                  join BuildingTB in BuildingList
                                                  on LogTB.BuildingTbId equals BuildingTB.Id
                                                  select new KakaoLogListDTO
                                                  {
                                                      Id = LogTB.Id,
                                                      Code = LogTB.Code,
                                                      Message = LogTB.Message,
                                                      CreateDT = LogTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      BuildingName = BuildingTB.Name,
                                                      VocId = LogTB.VocTbId
                                                  }).ToList();

                    return new ResponseList<KakaoLogListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseList<KakaoLogListDTO>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseList<KakaoLogListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장 카카오 로그 리스트 카운트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> GetKakaoLogCountService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int? count = await KakaoLogInfoRepository.GetKakaoLogCount(Int32.Parse(placeid)).ConfigureAwait(false);
                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 카카오 로그 리스트 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<ResponseList<KakaoLogListDTO>> GetKakaoLogPageNationListService(HttpContext context, int pagenumber, int pagesize)
        {
            try
            {
                if (context is null)
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid)).ConfigureAwait(false);
                if (BuildingList is null || !BuildingList.Any())
                    return new ResponseList<KakaoLogListDTO>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogPageNationList(Convert.ToInt32(placeid), pagenumber, pagesize).ConfigureAwait(false);
                if (KakaoList is not null && KakaoList.Any())
                {
                    List<KakaoLogListDTO>? dto = (from LogTB in KakaoList
                                                  join BuildingTB in BuildingList
                                                  on LogTB.BuildingTbId equals BuildingTB.Id
                                                  select new KakaoLogListDTO
                                                  {
                                                      Id = LogTB.Id,
                                                      Code = LogTB.Code,
                                                      Message = LogTB.Message,
                                                      CreateDT = LogTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                                      BuildingName = BuildingTB.Name,
                                                      VocId = LogTB.VocTbId
                                                  }).ToList();

                    return new ResponseList<KakaoLogListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseList<KakaoLogListDTO>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseList<KakaoLogListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
