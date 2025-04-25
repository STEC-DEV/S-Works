using FamTec.Server.Helpers;
using FamTec.Server.Repository.Unit;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Unit;

namespace FamTec.Server.Services.Unit
{
    public class UnitService : IUnitService
    {
        private readonly IUnitInfoRepository UnitInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<UnitService> CreateBuilderLogger; /* 콘솔로그 */

        public UnitService(IUnitInfoRepository _unitinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            ILogService _logservice,
            ConsoleLogService<UnitService> _createbuilderlogger)
        {
            this.UnitInfoRepository = _unitinforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.CreateBuilderLogger = _createbuilderlogger;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UnitsDTO>> AddUnitService(UnitsDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };


                string? creator = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(creator) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };

                bool? AddCheck = await UnitInfoRepository.AddUnitInfoCheck(dto.Unit!, Int32.Parse(placeidx)).ConfigureAwait(false);
                if(AddCheck != true)
                    return new ResponseModel<UnitsDTO>() { message = "이미 해당사업장에 생성한 적 있는 단위명칭 입니다.", data = new UnitsDTO(), code = 201 };

                UnitTb? model = new UnitTb()
                {
                    Unit = dto.Unit!,
                    CreateDt = ThisDate,
                    CreateUser = creator,
                    UpdateDt = ThisDate,
                    UpdateUser = creator,
                    PlaceTbId = Int32.Parse(placeidx)
                };

                UnitTb? result = await UnitInfoRepository.AddAsync(model).ConfigureAwait(false);
                if(result is null)
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };

                return new ResponseModel<UnitsDTO>()
                {
                    message = "데이터가 정상 처리되었습니다.",
                    data = new UnitsDTO()
                    {
                        Id = result.Id,
                        Unit = result.Unit,
                    },
                    code = 200
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UnitsDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장의 단위리스트 조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<UnitsDTO>>> GetUnitList()
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<List<UnitsDTO>>() { message = "잘못된 요청입니다.", data = new List<UnitsDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseModel<List<UnitsDTO>>() { message = "잘못된 요청입니다.", data = new List<UnitsDTO>(), code = 404 };

                List<UnitTb>? model = await UnitInfoRepository.GetUnitList(Int32.Parse(placeidx)).ConfigureAwait(false);

                if(model is not null && model.Any())
                {
                    var unitsDtoList = model.Select(e => new UnitsDTO
                    {
                        Id = e.Id,
                        Unit = e.Unit,
                        SystemCreate = e.PlaceTbId == null ? true : false
                    }).ToList();

                    return new ResponseModel<List<UnitsDTO>>() { message = "요청이 정상 처리되었습니다.", data = unitsDtoList, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<UnitsDTO>>() { message = "데이터가 존재하지 않습니다.", data = new List<UnitsDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<UnitsDTO>>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<UnitsDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> DeleteUnitService(List<int> unitid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if(unitid is null || unitid.Count == 0)
                    return new ResponseModel<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };


                foreach (int id in unitid)
                {
                    UnitTb? model = await UnitInfoRepository.GetUnitInfo(id).ConfigureAwait(false);
                    if(model is null)
                        return new ResponseModel<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                    if (model.PlaceTbId is null)
                        return new ResponseModel<bool?>() { message = "삭제할 수 없는 정보입니다.", data = null, code = 404 };
                }


                bool? result = await UnitInfoRepository.DeleteUnitInfo(unitid, creater).ConfigureAwait(false);

                return result switch
                {
                    true => new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UnitsDTO>> UpdateUnitService(UnitsDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                UnitTb? model = await UnitInfoRepository.GetUnitInfo(dto.Id!.Value).ConfigureAwait(false);
                if (model is null)
                    return new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
           
                model.Unit = dto.Unit!;
                model.UpdateDt = ThisDate;
                model.UpdateUser = creator;

                bool? result = await UnitInfoRepository.UpdateUnitInfo(model).ConfigureAwait(false);

                return result switch
                {
                    true => new ResponseModel<UnitsDTO>() { message = "요청을 처리하지 못하였습니다.", data = new UnitsDTO(), code = 200 },
                    false => new ResponseModel<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseModel<UnitsDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
