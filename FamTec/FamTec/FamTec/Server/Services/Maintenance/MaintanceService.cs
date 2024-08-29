using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Maintenence;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using System;

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
        public async ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext context, AddMaintanceDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                /*
                string? GUID = Guid.NewGuid().ToString();

                bool? SetOccupantResult = await MaintanceRepository.SetOccupantToken(Convert.ToInt32(placeid), dto, GUID);
                if (SetOccupantResult == false)
                {
                    // 다른곳에서 사용중인 품목
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "다른곳에서 이미 사용중인 품목입니다.", data = null, code = 200 };
                }
                if (SetOccupantResult == null)
                {
                    // 조회결과가 없을때
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "조회결과가 없습니다.", data = null, code = 200 };
                }
                */
                //bool? OutResult = await MaintanceRepository.AddMaintanceAsync(dto, creater, Convert.ToInt32(placeid), GUID);
                bool? OutResult = await MaintanceRepository.AddMaintanceAsync(dto, creater, Convert.ToInt32(placeid));
                if (OutResult == true)
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
                }
                else if (OutResult == false)
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID);
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

                /*
                string GUID = Guid.NewGuid().ToString();

                // 동시성 검사 TOKEN 넣기
                bool? SetOccupantResult = await MaintanceRepository.SetOccupantToken(dto.PlaceTBID!.Value, dto.RoomTBID!.Value, dto.MaterialTBID!.Value, GUID);
                if(SetOccupantResult == false)
                {
                    // 다른곳에서 사용중인 품목
                    await MaintanceRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 200 };
                }
                if (SetOccupantResult == null)
                {
                    // 조회결과가 없을때
                    await MaintanceRepository.RoolBackOccupant(GUID);
                    return new ResponseUnit<bool?>() { message = "조회결과가 없습니다.", data = null, code = 200 };
                }
                */
                // 여기서 취소 하면됨.
                bool? DeleteResult = await MaintanceRepository.DeleteHistoryInfo(dto, deleter);
                if(DeleteResult == true)
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
                    return new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 };
                }
                else
                {
                    //await MaintanceRepository.RoolBackOccupant(GUID); // 토큰 돌려놓기
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

                FacilityTb? VaildFacility = await FacilityInfoRepository.GetFacilityInfo(facilityid);
                if(VaildFacility is null)
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceListDTO>? dto = await MaintanceRepository.GetFacilityHistoryList(facilityid);
                
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
    }
}
