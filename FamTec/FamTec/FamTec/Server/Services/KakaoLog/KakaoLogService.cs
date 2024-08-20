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
        private ILogService LogService;

        public KakaoLogService(IKakaoLogInfoRepository _kakaologinforepository,
            IBuildingInfoRepository _buildinginforepository,
            ILogService _logservice)
        {
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 해당 사업장의 카카오 로그 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<KakaoLogListDTO>?> GetKakaoLogListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<KakaoLogListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeid));
                if(BuildingList is not [_, ..])
                    return new ResponseList<KakaoLogListDTO>() { message = "데이터 조회결과가 없습니다.", data = null, code = 200 };

                List<KakaoLogTb>? KakaoList = await KakaoLogInfoRepository.GetKakaoLogList(Convert.ToInt32(placeid));
                if (KakaoList is [_, ..])
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
                return new ResponseList<KakaoLogListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
