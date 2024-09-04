using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Maintenence;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Services.Maintenance
{
    public class MaintanceService : IMaintanceService
    {
        private readonly IMaintanceRepository MaintanceRepository;
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private ILogService LogService;

        public MaintanceService(IMaintanceRepository _maintancerepository,
            IFacilityInfoRepository _facilityinforepository,
            ILogService _logservice)
        {
            this.MaintanceRepository = _maintancerepository;
            this.FacilityInfoRepository = _facilityinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 유지보수 출고등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext context, AddMaintanceDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);
                string? userid = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(userid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? OutResult = await MaintanceRepository.AddMaintanceAsync(dto, creater, userid, Convert.ToInt32(placeid), files);
                if (OutResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if (OutResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
        
        /// <summary>
        /// 해당 설비의 유지보수 이력 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeletemaintanceHistoryService(HttpContext context, DeleteMaintanceDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                string? deleter = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                bool? DeleteResult = await MaintanceRepository.DeleteHistoryInfo(dto, deleter);
                if(DeleteResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(HttpContext context, int facilityid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                // 여기 더 추가해야함
                
                FacilityTb? VaildFacility = await FacilityInfoRepository.GetFacilityInfo(facilityid);
                if(VaildFacility is null)
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceListDTO>? dto = await MaintanceRepository.GetFacilityHistoryList(facilityid, Int32.Parse(placeid));
                

                if (dto is not null && dto.Any())
                    return new ResponseList<MaintanceListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseList<MaintanceListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaintanceListDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaintanceHistoryDTO>?> GetDateHisotryList(HttpContext context, DateTime StartDate, DateTime EndDate, string category, int type)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceHistoryDTO>? model = await MaintanceRepository.GetDateHistoryList(Convert.ToInt32(placeid), StartDate, EndDate, category, type);

                if (model is not null && model.Any())
                    return new ResponseList<MaintanceHistoryDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<MaintanceHistoryDTO>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaintanceHistoryDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AllMaintanceHistoryDTO>?> GetAllHistoryList(HttpContext context, string category, int type)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<AllMaintanceHistoryDTO>? model = await MaintanceRepository.GetAllHistoryList(Convert.ToInt32(placeid), category, type);

                if (model is not null && model.Any())
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AllMaintanceHistoryDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<DetailMaintanceDTO?>> GetDetailService(HttpContext context, int MaintanceID)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DetailMaintanceDTO? model = await MaintanceRepository.DetailMaintanceList(MaintanceID, Int32.Parse(placeid));
                if (model is not null)
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailMaintanceDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
