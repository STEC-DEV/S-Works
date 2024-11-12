using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Type.Lift;
using FamTec.Server.Services.Facility.Type.Machine;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class LiftFacilityController : ControllerBase
    {
        private readonly ILiftFacilityService LiftFacilityService;
        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ICommService CommService;

        private readonly ConsoleLogService<LiftFacilityController> CreateBuilderLogger;

        public LiftFacilityController(ILiftFacilityService _listfacilityservice,
            IFileService _fileservice,
            ILogService _logservice,
            ICommService _commservice,
            ConsoleLogService<LiftFacilityController> _createbuilderlogger)
        {
            this.LiftFacilityService = _listfacilityservice;
            
            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CommService = _commservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [HttpGet]
        [Route("sign/DownloadLiftFacilityForm")]
        public async Task<IActionResult> DownloadLiftFacilityForm()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                byte[]? ExcelForm = await LiftFacilityService.DownloadLiftFacilityForm(HttpContext);

                if (ExcelForm is not null)
                    return File(ExcelForm, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "기계설비정보.xlsx");
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportLiftFacility")]
        public async Task<IActionResult> ImportLiftFacilityForm([FromForm] IFormFile files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (files is null)
                    return NoContent();

                if (files.Length == 0)
                    return NoContent();

                string? extension = FileService.GetExtension(files);
                if (String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension);
                    if (!extensioncheck)
                    {
                        return Ok(new ResponseUnit<bool>() { message = "지원하지 않는 파일형식입니다.", data = false, code = 204 });
                    }
                }

                ResponseUnit<bool> model = await LiftFacilityService.ImportLiftFacilityService(HttpContext, files);
                if (model is null)
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddLiftFacility")]
        public async Task<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.RoomId is null)
                    return NoContent();


                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseUnit<FacilityDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseUnit<FacilityDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<FacilityDTO>? model = await LiftFacilityService.AddLiftFacilityService(HttpContext, dto, files).ConfigureAwait(false);
                
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllLiftFacility")]
        public async Task<IActionResult> GetAllLiftFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await LiftFacilityService.GetLiftFacilityListService(HttpContext).ConfigureAwait(false);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailLiftFacility")]
        public async Task<IActionResult> DetailLiftFacility([FromQuery]int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<FacilityDetailDTO> model = await LiftFacilityService.GetLiftDetailFacilityService(HttpContext, facilityid, isMobile).ConfigureAwait(false);
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


        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateLiftFacility")]
        public async Task<IActionResult> UpdateLiftFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.RoomId is null)
                    return NoContent();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseUnit<bool?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await LiftFacilityService.UpdateLiftFacilityService(HttpContext, dto, files).ConfigureAwait(false);

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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeletLiftFacility")]
        public async Task<IActionResult> DeleteLiftFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await LiftFacilityService.DeleteLiftFacilityService(HttpContext, delIdx).ConfigureAwait(false);
                
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
