using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Maintenance;
using FamTec.Server.Services.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Maintenance
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintanceService MaintanceService;
        private readonly IUseMaintenenceService UseMaintenenceService;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ICommService CommService; /* 핼퍼 클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<MaintenanceController> CreateBuilderLogger; /* 콘솔로그 */

        public MaintenanceController(IMaintanceService _maintanceservice,
            IUseMaintenenceService _usermaintenenceservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<MaintenanceController> _createbuilderlogger)
        {
            this.MaintanceService = _maintanceservice;
            this.UseMaintenenceService = _usermaintenenceservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        #region 대시보드

        /// <summary>
        /// 금일 유지보수 이력
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetMaintenanceDaysList")]
        public async Task<IActionResult> GetMaintenanceDaysList()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<MaintenanceDaysDTO>>? model = await MaintanceService.GetMaintenanceDaysList().ConfigureAwait(false);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 대시보드용 유지보수비용추이(1년)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetMaintenanceYearPrice")]
        public async Task<IActionResult> GetMaintenanceYearPrice()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<MaintanceYearPriceDTO>>? model = await MaintanceService.GetMaintenanceYearPriceList().ConfigureAwait(false);
                if (model == null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(new ResponseModel<List<MaintanceYearPriceDTO>>() { message = "값이 존재하지 않습니다.", data = null, code = 200 });
                else
                    return BadRequest();
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
        /// 설비 유지보수 건수 (7일)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetMaintenanceCount")]
        public async Task<IActionResult> GetMaintenanceCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<MaintanceWeekCount>>? model = await MaintanceService.GetMaintanceDashBoardDataService().ConfigureAwait(false);

                if (model == null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        #endregion


        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateSupMaintenance")]
        public async Task<IActionResult> UpdateSupMaintenance([FromBody]UpdateMaintancematerialDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.UpdateUsematerialDTO is [_, ..])
                {
                    foreach (var item in dto.UpdateUsematerialDTO)
                    {
                        if (item.RoomID is 0)
                            return NoContent();
                        if (item.MaterialID is 0)
                            return NoContent();
                    }
                }

                ResponseModel<bool?> model = await UseMaintenenceService.UpdateUseMaintanceService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else if (model.code == 401)
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 출고 - 추가출고
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddSupMaintenance")]
        public async Task<IActionResult> AddSupMaintenance([FromBody]AddMaintanceMaterialDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.MaterialList.Count == 0 || dto.MaterialList is null)
                    return NoContent();

                foreach (MaterialDTO MaterialInfo in dto.MaterialList)
                {
                    if (MaterialInfo.MaterialID is 0)
                        return NoContent();
                    if (MaterialInfo.Num is 0)
                        return NoContent();
                    if (MaterialInfo.RoomID is 0)
                        return NoContent();
                }

                bool hasDuplicates = dto.MaterialList
                .GroupBy(m => new { m.MaterialID, m.RoomID })
                .Any(g => g.Count() > 1);

                if (hasDuplicates)
                {
                    // MaterialID와 RoomID가 2개이상 있는지 검사
                    return BadRequest();
                }

                ResponseModel<FailResult?> model = await MaintanceService.AddSupMaintanceService(dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 422)
                    return Ok(model);
                else if (model.code == 409)
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

     
        /// <summary>
        /// 유지보수 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateMaintenance")]
        public async Task<IActionResult> UpdateMaintenance([FromForm]UpdateMaintenanceDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.MaintanceID == 0)
                    return BadRequest();
              
                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseModel<bool?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseModel<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseModel<bool?> model = await MaintanceService.UpdateMaintenanceService(dto, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaintenanceImage")]
        public async Task<IActionResult> AddMaintenanceImage([FromForm][Required] int id, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (id is 0)
                    return BadRequest();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseModel<bool?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseModel<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseModel<bool?> model = await MaintanceService.AddMaintanceImageService(id, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaintenance")]
        public async Task<IActionResult> AddMaintenence([FromBody]AddMaintenanceDTO dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Worker))
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                if(dto.FacilityID == 0)
                    return NoContent();

                // 자체 작업
                if (dto.Type == 0) 
                {
                    if (dto.Inventory is null || !dto.Inventory.Any())
                        return NoContent();

                    bool hasDuplicates = dto.Inventory
                    .GroupBy(i => new { i.MaterialID, RoomID = i.AddStore?.RoomID })
                    .Any(g => g.Count() > 1);
                
                    if (hasDuplicates)
                    {
                        // MaterialID와 RoomID가 2개이상 있는지 검사
                        return BadRequest();
                    }
                }

                ResponseModel<FailResult?> model = await MaintanceService.AddMaintanceService(dto).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 422)
                    return Ok(model);
                else if (model.code == 409)
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMaintanceHistory")]
        public async Task<IActionResult> GetMaintanceHistory([FromQuery][Required]int facilityid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseModel<List<MaintanceListDTO>> model = await MaintanceService.GetMaintanceHistoryService(facilityid).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailMaintance")]
        public async Task<IActionResult> GetDetailMaintance([FromQuery][Required]int Maintanceid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<DetailMaintanceDTO?> model = await MaintanceService.GetDetailService(Maintanceid, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteMaintenanceList")]
        public async Task<IActionResult> DeleteMaintenanceList([FromBody] DeleteMaintanceDTO2 dto)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.MaintanceID is null || !dto.MaintanceID.Any())
                    return NoContent();

                ResponseModel<bool?> model = await MaintanceService.DeleteMaintenanceRecordService(dto).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 내용 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteMaintenanceStore")]
        public async Task<IActionResult> DeleteMaintenanceStore([FromBody]DeleteMaintanceDTO delInfo)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (delInfo.MaintanceID is 0)
                    return NoContent();
                if(!delInfo.UseMaintenenceIDs.Any())
                    return NoContent();


                ResponseModel<bool?> model = await MaintanceService.DeleteMaintenanceStoreRecordService(delInfo).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDateHistoryList")]
        public async Task<IActionResult> GetDateHistoryList([FromQuery][Required]DateTime StartDate, [FromQuery][Required]DateTime EndDate, [FromQuery][Required]List<string> category, [FromQuery][Required]List<int> type)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (category is null || !category.Any())
                    return NoContent();

                if (type is null || !type.Any())
                    return NoContent();

                category.ForEach(s => s = s.Trim());

                ResponseModel<List<MaintanceHistoryDTO>>? model = await MaintanceService.GetDateHistoryList(StartDate, EndDate, category, type).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 401)
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 이력 사업장별 전체
        /// </summary>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllHistoryList")]
        public async Task<IActionResult> GetAllHistoryList([FromQuery][Required]List<string> category, [FromQuery][Required]List<int> type)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (category is null || !category.Any())
                    return NoContent();

                if (type is null || !type.Any())
                    return NoContent();

                category.ForEach(s => s = s.Trim());

                ResponseModel<List<AllMaintanceHistoryDTO>>? model = await MaintanceService.GetAllHistoryList(category, type).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 401)
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
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetHistorySearchList")]
        public async Task<IActionResult> GetHistorySearchList([FromQuery][Required] int searchType, [FromQuery][Required] List<string> category, [FromQuery][Required] List<int> type, [FromQuery] string? searchdate, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
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
                else // 기간
                {
                    if (StartDate is null)
                        return NoContent();
                    if (EndDate is null)
                        return NoContent();
                }

                if (category.Count == 0)
                    return NoContent();

                if (type.Count == 0)
                    return NoContent();

                if(searchType == 0)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("월간조회");
#endif
                    // 월간 Service API 호출
                    ResponseModel<List<MaintanceHistoryDTO>>? model = await MaintanceService.GetMonthHistoryList(searchdate, category, type);

                    if (model is null)
                        return BadRequest();
                    if (model.code == 200)
                        return Ok(model);
                    else if (model.code == 401)
                        return Ok(model);
                    else
                        return BadRequest();
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("기간조회");
#endif
                    // 기간 Service API 호출
                    ResponseModel<List<MaintanceHistoryDTO>>? model = await MaintanceService.GetDateHistoryList(StartDate!.Value, EndDate!.Value, category, type).ConfigureAwait(false);

                    if (model is null)
                        return BadRequest();
                    if (model.code == 200)
                        return Ok(model);
                    else if (model.code == 401)
                        return Ok(model);
                    else
                        return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

    }
}