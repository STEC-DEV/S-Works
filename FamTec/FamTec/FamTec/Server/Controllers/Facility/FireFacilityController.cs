using FamTec.Server.Services.Facility.Type.Fire;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;
using FamTec.Server.Middleware;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Facility
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FireFacilityController : ControllerBase
    {
        private readonly IFireFacilityService FireFacilityService;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ICommService CommService; /* 핼퍼 클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<FireFacilityController> CreateBuilderLogger; /* 콘솔로그 */

        public FireFacilityController(IFireFacilityService _firefacilityservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<FireFacilityController> _createbuilderlogger)
        {
            this.FireFacilityService = _firefacilityservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DownloadFireFacilityForm")]
        public async Task<IActionResult> DownloadFireFacilityForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                byte[]? ExcelForm = await FireFacilityService.DownloadFireFacilityForm().ConfigureAwait(false);

                if (ExcelForm is not null)
                    return File(ExcelForm, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "소방설비정보(양식).xlsx");
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
        [Route("sign/ImportFireFacility")]
        public async Task<IActionResult> ImportFireFacilityForm([FromForm][Required] IFormFile files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (files is null)
                    return NoContent();

                if (files.Length == 0)
                    return NoContent();

                string? extenstion = FileService.GetExtension(files);
                if (String.IsNullOrWhiteSpace(extenstion))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extenstion);
                    if (!extensioncheck)
                    {
                        return Ok(new ResponseUnit<bool>() { message = "지원하지 않는 파일형식입니다.", data = false, code = 204 });
                    }
                }

                if (files.Length > Common.MEGABYTE_10)
                    return Ok(new ResponseUnit<bool>() { message = "파일의 용량은 10MB까지 가능합니다.", data = false, code = 204 });

                ResponseUnit<bool> model = await FireFacilityService.ImportFireFacilityService(files).ConfigureAwait(false);
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
        [Route("sign/AddFireFacility")]
        public async Task<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.RoomId is null)
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

                ResponseUnit<FacilityDTO>? model = await FireFacilityService.AddFireFacilityService(dto, files).ConfigureAwait(false);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllFireFacility")]
        public async Task<IActionResult> GetAllFireFacility()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<FacilityListDTO>? model = await FireFacilityService.GetFireFacilityListService().ConfigureAwait(false);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailFireFacility")]
        public async Task<IActionResult> DetailFireFacility([FromQuery][Required] int facilityid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseUnit<FacilityDetailDTO> model = await FireFacilityService.GetFireDetailFacilityService(facilityid, isMobile).ConfigureAwait(false);
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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateFireFacility")]
        public async Task<IActionResult> UpdateFireFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.RoomId is null)
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

                ResponseUnit<bool?> model = await FireFacilityService.UpdateFireFacilityService(dto, files).ConfigureAwait(false);
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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteFireFacility")]
        public async Task<IActionResult> DeleteFireFacility([FromBody][Required] List<int> delIdx)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (delIdx is null)
                    return NoContent();

                if(delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await FireFacilityService.DeleteFireFacilityService(delIdx).ConfigureAwait(false);
                
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
