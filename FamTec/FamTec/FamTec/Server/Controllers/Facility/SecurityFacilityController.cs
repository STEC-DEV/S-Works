using FamTec.Server.Services.Facility.Type.Security;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;
using FamTec.Server.Middleware;
using FamTec.Server.Services.Facility.Type.Beauty;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Facility
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityFacilityController : ControllerBase
    {
        private readonly ISecurityFacilityService SecurityFacilityService;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ICommService CommService; /* 핼퍼 클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<SecurityFacilityController> CreateBuilderLogger; /* 콘솔로그 */

        public SecurityFacilityController(ISecurityFacilityService _securityfacilityservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<SecurityFacilityController> _createbuilderlogger)
        {
            this.SecurityFacilityService = _securityfacilityservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DownloadSecurityFacilityForm")]
        public async Task<IActionResult> DownloadSecurityFacilityForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                byte[]? ExcelForm = await SecurityFacilityService.DownloadSecurityFacilityForm().ConfigureAwait(false);

                if (ExcelForm is not null)
                    return File(ExcelForm, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "보안설비정보(양식).xlsx");
                else
                    return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
            catch (Exception ex)
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
        [Route("sign/ImportSecurityFacility")]
        public async Task<IActionResult> ImportSecurityFacilityForm([FromForm][Required] IFormFile files)
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

                ResponseUnit<bool> model = await SecurityFacilityService.ImportSecurityFacilityService(files).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
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
        [Route("sign/AddSecurityFacility")]
        public async Task<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
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

                ResponseUnit<FacilityDTO> model = await SecurityFacilityService.AddSecurityFacilityService(dto, files).ConfigureAwait(false);

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
        [Route("sign/GetAllSecurityFacility")]
        public async Task<IActionResult> GetAllSecurityFacility()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<FacilityListDTO> model = await SecurityFacilityService.GetSecurityFacilityListService().ConfigureAwait(false);

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
        [Route("sign/DetailSecurityFacility")]
        public async Task<IActionResult> DetailSecurityFacility([FromQuery][Required] int facilityid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseUnit<FacilityDetailDTO> model = await SecurityFacilityService.GetSecurityDetailFacilityService(facilityid, isMobile).ConfigureAwait(false);
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
        [Route("sign/UpdateSecurityFacility")]
        public async Task<IActionResult> UpdateSecurityFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
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

                ResponseUnit<bool?> model = await SecurityFacilityService.UpdateSecurityFacilityService(dto, files).ConfigureAwait(false);
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
        [Route("sign/DeleteSecurityFacility")]
        public async Task<IActionResult> DeleteFireFacility([FromBody][Required] List<int> delIdx)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await SecurityFacilityService.DeleteSecurityFacilityService(delIdx).ConfigureAwait(false);
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
