using FamTec.Server.Repository.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;

namespace FamTec.Server.Services.UseMaintenence
{
    public class UseMaintenenceService : IUseMaintenenceService
    {
        private readonly IUseMaintenenceInfoRepository UseMaintenenceInfoRepository;
        private ILogService LogService;

        public UseMaintenenceService(IUseMaintenenceInfoRepository _usemaintenenceinforepository, ILogService _logservice)
        {
            this.UseMaintenenceInfoRepository = _usemaintenenceinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사용자재 상세 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseUnit<UseMaterialDetailDTO>> GetDetailUseMaterialService(HttpContext context, int usematerialid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UseMaterialDetailDTO? model = await UseMaintenenceInfoRepository.GetDetailUseStoreList(usematerialid, Int32.Parse(placeid));
                if (model is not null)
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseUnit<UseMaterialDetailDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UseMaterialDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
