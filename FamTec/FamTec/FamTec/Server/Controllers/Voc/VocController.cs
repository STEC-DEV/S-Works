using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace FamTec.Server.Controllers.Voc
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class VocController : ControllerBase
    {
        private readonly IVocService VocService;
        private readonly ILogService LogService;
        private readonly IFileService FileService;
        private readonly ICommService CommService;

        private readonly ConsoleLogService<VocController> CreateBuilderLogger;

        public VocController(IVocService _vocservice,
            ILogService _logservice,
            ConsoleLogService<VocController> _createbuilderlogger,
            ICommService _commservice,
            IFileService _fileservice)
        {
            this.VocService = _vocservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;

            this.CommService = _commservice;
            this.FileService = _fileservice;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocDaysStatusCount")]
        public async Task<IActionResult> GetVocDaysStatusCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocDaysStatusCountDTO>? model = await VocService.GetVocDaysStatusDataService(HttpContext).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.");
            }
        }

        /// <summary>
        /// DashBoad 하루치 유형별 발생건수
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocDaysCount")]
        public async Task<IActionResult> GetVocDaysCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocWeekCountDTO>? model = await VocService.GetVocDashBoardDaysDataService(HttpContext).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.");
            }
        }

        /// <summary>
        /// DashBoad 일주일간의 유형별 발생건수
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocWeekCount")]
        public async Task<IActionResult> GetVocWeekCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocWeekCountDTO>? model = await VocService.GetVocDashBoardWeeksDataService(HttpContext).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="type">0,1,2,3,4,5,6,7 : 민원유형</param>
        /// <param name="status">민원상태 : 미처리, 처리, 처리완료</param>
        /// <param name="buildingid">민원위치</param>
        /// <param name="division">모바일-웹</param>
        /// <param name="searchdate">월간용 - 날짜</param>
        /// <param name="StartDate">기간용 - 시작날짜</param>
        /// <param name="EndDate">기간용 - 종료날짜</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocSearchList")]
        //public async Task<IActionResult> GetVocSearchList()
        public async Task<IActionResult> GetVocSearchList([FromQuery] int searchType, [FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid, [FromQuery] List<int> division, [FromQuery]string? searchdate, [FromQuery]DateTime? StartDate, [FromQuery]DateTime? EndDate)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(searchType == 0) // 월간
                {
                    if (String.IsNullOrWhiteSpace(searchdate))
                        return NoContent();
                }
                else  // 기간
                {
                    if (StartDate is null)
                        return NoContent();
                    if (EndDate is null)
                        return NoContent();
                }

                if (status.Count == 0)
                    return NoContent();

                if(buildingid.Count == 0)
                    return NoContent();

                if(division.Count == 0)
                    return NoContent();

                if(searchType == 0)
                {
                    // 월간 Service API 호출
                    ResponseList<VocListDTO>? model = await VocService.GetMonthVocSearchList(HttpContext, type, status, buildingid, division, searchdate);
                    if (model is null)
                        return BadRequest();
                    if (model.code == 200)
                        return Ok(model);
                    else
                        return BadRequest();
                }
                else if(searchType == 1)
                {
                    // 기간 Service API 호출
                    ResponseList<VocListDTO>? model = await VocService.GetDateVocSearchList(HttpContext, type, status, buildingid, division, StartDate!.Value, EndDate!.Value);
                    if (model is null)
                        return BadRequest();
                    if (model.code == 200)
                        return Ok(model);
                    else
                        return BadRequest();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


        /// <summary>
        /// 사업장 민원 전체보기 - 직원용 (월간)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocList")]
        // SearchDate = string 값
        public async Task<IActionResult> GetVocList([FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid, [FromQuery] List<int> division)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllVocListDTO> model = await VocService.GetVocList(HttpContext, type, status, buildingid, division).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 민원 필터 전체보기 - 직원용 (기간)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocFilterList")]
        public async Task<IActionResult> GetVocFilterList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate, [FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid, [FromQuery] List<int> division)
        {
            try
            {
                if (HttpContext is null)
                   return BadRequest();

                if (type is null)
                    return NoContent();
                if (type.Count == 0)
                    return NoContent();

                if (status is null)
                    return NoContent();
                if (status.Count == 0)
                    return NoContent();

                if (buildingid is null)
                    return NoContent();
                if (buildingid.Count == 0)
                    return NoContent();

                ResponseList<VocListDTO>? model = await VocService.GetVocFilterList(HttpContext, StartDate, EndDate, type, status, buildingid, division).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 민원 상세보기 - 직원용
        /// </summary>
        /// <param name="VocId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/VocInfo")]
        public async Task<IActionResult> GetDetailVoc([FromQuery] int VocId)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<VocEmployeeDetailDTO> model = await VocService.GetVocDetail(HttpContext, VocId, isMobile).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }



        /// <summary>
        /// 민원타입 변경
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateVocType")]
        public async Task<IActionResult> UpdateVocType([FromBody]UpdateVocDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.VocID is null)
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                ResponseUnit<bool?> model = await VocService.UpdateVocTypeService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }


    }
}
