using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Voc
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class VocController : ControllerBase
    {
        private readonly IVocService VocService;
        private readonly ICommService CommService; /* 핼퍼클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<VocController> CreateBuilderLogger; /* 콘솔로그 */

        public VocController(IVocService _vocservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<VocController> _createbuilderlogger)
        {
            this.VocService = _vocservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 민원 IMPORT
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/v1/ImportVocData")]
        public async Task<IActionResult> ImportVocData([FromBody][Required] List<ImportVocData> dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                var model = await VocService.ImportVocServiceV2(dto).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else if (model.code == 401)
                    return Unauthorized();
                else if (model.code == 404)
                    return NotFound();
                else
                    return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        /// 민원 이전이력 엑셀 업로드용 양식 다운로드
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v1/DownloadVocForm")]
        public async Task<IActionResult> DownloadVocForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                byte[]? fileBytes = await VocService.DownloadVocForm().ConfigureAwait(false);

                if (fileBytes is not null)
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "민원(양식).xlsx");
                else
                    return Ok();
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
        /// 민원처리 내역 최신상태 알림톡으로 전송
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/v1/RecentVoc")]
        public async Task<IActionResult> RecentVoc([FromBody][Required]RecentVocDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.vocId == 0)
                    return NoContent();

                var model = await VocService.RecentVocSendService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return NoContent();
                else if (model.code == 401)
                    return Unauthorized();
                else
                    return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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


        /* ##################  */
        // [2]. 민원발생현황 - 금일
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetVocDaysStatusCount")]
        public async Task<IActionResult> GetVocDaysStatusCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<VocDaysStatusCountDTO>? model = await VocService.GetVocDaysStatusDataService().ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.",statusCode: 500);
            }
        }

        /// <summary>
        /// [2]. 민원발생현황 - 일주일
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetVocWeeksStatusCount")]
        public async Task<IActionResult> GetVocWeekStatusCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<VocWeekStatusCountDTO>>? model = await VocService.GetVocWeeksStatusDataService().ConfigureAwait(false);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.",statusCode: 500);
            }
        }


        /// <summary>
        /// DashBoad 하루치 유형별 발생건수
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetVocDaysCount")]
        public async Task<IActionResult> GetVocDaysCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<VocDaysCountDTO>? model = await VocService.GetVocDashBoardDaysDataService().ConfigureAwait(false);
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
        [Route("sign/v2/GetVocWeekCount")]
        public async Task<IActionResult> GetVocWeekCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<VocWeekCountDTO>>? model = await VocService.GetVocDashBoardWeeksDataService().ConfigureAwait(false);
                if (model is null)
                    return BadRequest();
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

        /* ##################  */


        /// <summary>
        /// 민원 리스트 조회 - v2
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
        [Route("sign/v2/GetVocSearchList")]
        //public async Task<IActionResult> GetVocSearchListV2()
        public async Task<IActionResult> GetVocSearchListV2([FromQuery][Required] int searchType, [FromQuery][Required] List<int> type, [FromQuery][Required] List<int> status, [FromQuery][Required] List<int> buildingid, [FromQuery][Required] List<int> division, [FromQuery] string? searchdate, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (searchType == 0) // 월간
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

                if (buildingid.Count == 0)
                    return NoContent();

                if (division.Count == 0)
                    return NoContent();

                if (searchType == 0)
                {
                    // 월간 Service API 호출
                    ResponseModel<List<VocListDTOV2>>? model = await VocService.GetMonthVocSearchListV2(type, status, buildingid, division, searchdate).ConfigureAwait(false);
                    if (model is null)
                        return BadRequest();
                    if (model.code == 200)
                        return Ok(model);
                    else
                        return BadRequest();
                }
                else if (searchType == 1)
                {
                    // 기간 Service API 호출
                    ResponseModel<List<VocListDTOV2>>? model = await VocService.GetDateVocSearchListV2(type, status, buildingid, division, StartDate!.Value, EndDate!.Value).ConfigureAwait(false);
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
        /// 민원 리스트 조회
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
        public async Task<IActionResult> GetVocSearchList([FromQuery][Required] int searchType, [FromQuery][Required] List<int> type, [FromQuery][Required] List<int> status, [FromQuery][Required] List<int> buildingid, [FromQuery][Required] List<int> division, [FromQuery]string? searchdate, [FromQuery]DateTime? StartDate, [FromQuery]DateTime? EndDate)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (searchType == 0) // 월간
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
                    ResponseModel<List<VocListDTO>>? model = await VocService.GetMonthVocSearchList(type, status, buildingid, division, searchdate).ConfigureAwait(false);
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
                    ResponseModel<List<VocListDTO>>? model = await VocService.GetDateVocSearchList(type, status, buildingid, division, StartDate!.Value, EndDate!.Value).ConfigureAwait(false);
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
        public async Task<IActionResult> GetVocList([FromQuery][Required] List<int> type, [FromQuery][Required] List<int> status, [FromQuery][Required] List<int> buildingid, [FromQuery][Required] List<int> division)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<AllVocListDTO>> model = await VocService.GetVocList(type, status, buildingid, division).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

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
        public async Task<IActionResult> GetVocFilterList([FromQuery][Required] DateTime StartDate, [FromQuery][Required] DateTime EndDate, [FromQuery][Required] List<int> type, [FromQuery][Required] List<int> status, [FromQuery][Required] List<int> buildingid, [FromQuery][Required] List<int> division)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
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

                ResponseModel<List<VocListDTO>>? model = await VocService.GetVocFilterList(StartDate, EndDate, type, status, buildingid, division).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();
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
        public async Task<IActionResult> GetDetailVoc([FromQuery][Required] int VocId)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<VocEmployeeDetailDTO> model = await VocService.GetVocDetail(VocId, isMobile).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();
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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.VocID is null)
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                ResponseModel<bool?> model = await VocService.UpdateVocTypeService(dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

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
