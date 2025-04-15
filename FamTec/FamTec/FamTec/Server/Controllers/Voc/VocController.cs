using DevExpress.Utils;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                if (HttpContext is null)
                    return BadRequest();

                byte[]? fileBytes = await VocService.DownloadVocForm(HttpContext);

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
        public async Task<IActionResult> RecentVoc([FromBody]RecentVocDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return Unauthorized();

                if (dto.vocId == 0)
                    return NoContent();

                var model = await VocService.RecentVocSendService(HttpContext, dto);
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocDaysStatusCountDTO>? model = await VocService.GetVocDaysStatusDataService(HttpContext).ConfigureAwait(false);
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<VocWeekStatusCountDTO>? model = await VocService.GetVocWeeksStatusDataService(HttpContext).ConfigureAwait(false);

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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocDaysCountDTO>? model = await VocService.GetVocDashBoardDaysDataService(HttpContext).ConfigureAwait(false);
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<VocWeekCountDTO>? model = await VocService.GetVocDashBoardWeeksDataService(HttpContext).ConfigureAwait(false);
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
        public async Task<IActionResult> GetVocSearchListV2([FromQuery] int searchType, [FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid, [FromQuery] List<int> division, [FromQuery] string? searchdate, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            try
            {
                //int searchType = 0;
                //DateTime? StartDate = null;
                //DateTime? EndDate = null;
                //string searchdate = "2025-04";
                //List<int> type = new List<int>
                //{
                //    0,1,2,3,4,5,6,7,8
                //};
                //List<int> status = new List<int>
                //{
                //    0,1,2
                //};
                //List<int> buildingid = new List<int>()
                //{
                //    1,2,3,4,5,10,23,28,29,30,31,32,34,35,43
                //};
                //List<int> division = new List<int>
                //{
                //    0,1
                //};

                if (HttpContext is null)
                    return BadRequest();

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
                    ResponseList<VocListDTOV2>? model = await VocService.GetMonthVocSearchListV2(HttpContext, type, status, buildingid, division, searchdate);
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
                    ResponseList<VocListDTOV2>? model = await VocService.GetDateVocSearchListV2(HttpContext, type, status, buildingid, division, StartDate!.Value, EndDate!.Value);
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
