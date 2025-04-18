﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
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
    public class MachineFacilityController : ControllerBase
    {
        private readonly IMachineFacilityService MachineFacilityService;
        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ICommService CommService;

        private readonly ConsoleLogService<MachineFacilityController> CreateBuilderLogger;

        public MachineFacilityController(IMachineFacilityService _machinefacilityservice,
            IFileService _fileservice,
            ILogService _logservice,
            ICommService _commservice,
            ConsoleLogService<MachineFacilityController> _createbuilderlogger)
        {
            this.MachineFacilityService = _machinefacilityservice;
            
            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CommService = _commservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [HttpGet]
        [Route("sign/DownloadMachineFacilityForm")]
        public async Task<IActionResult> DownloadMachineFacilityForm()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                byte[]? ExcelForm = await MachineFacilityService.DownloadMachineFacilityForm(HttpContext);

                if (ExcelForm is not null)
                    return File(ExcelForm, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "기계설비정보(양식).xlsx");
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
        [Route("sign/ImportMachineFacility")]
        public async Task<IActionResult> ImportMachineFacilityForm([FromForm] IFormFile files)
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
                if(String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension);
                    if(!extensioncheck)
                    {
                        return Ok(new ResponseUnit<bool>() { message = "지원하지 않는 파일형식입니다.", data = false, code = 204 });
                    }
                }

                if (files.Length > Common.MEGABYTE_10)
                    return Ok(new ResponseUnit<bool>() { message = "파일의 용량은 10MB까지 가능합니다.", data = false, code = 204 });

                ResponseUnit<bool> model = await MachineFacilityService.ImportMachineFacilityService(HttpContext, files);
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
        [Route("sign/AddMachineFacility")]
        public async Task<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                //FacilityDTO dto = new FacilityDTO();
                //dto.Category = "기계";
                //dto.Name = "기계설비A_1";
                //dto.Type = "형식1";
                //dto.Num = 30;
                //dto.Unit = "개";
                //dto.RoomTbId = 10;

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

                ResponseUnit<FacilityDTO> model = await MachineFacilityService.AddMachineFacilityService(HttpContext, dto, files).ConfigureAwait(false);

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
        [Route("sign/GetAllMachineFacility")]
        public async Task<IActionResult> GetAllMachineFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await MachineFacilityService.GetMachineFacilityListService(HttpContext).ConfigureAwait(false);

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
        [Route("sign/DetailMachineFacility")]
        public async Task<IActionResult> DetailMachineFacility([FromQuery]int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<FacilityDetailDTO> model = await MachineFacilityService.GetMachineDetailFacilityService(HttpContext, facilityid, isMobile).ConfigureAwait(false);
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
        [Route("sign/UpdateMachineFacility")]
        public async Task<IActionResult> UpdateMachineFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                //FacilityDTO dto = new FacilityDTO();
                //dto.ID = 4;
                //dto.Category = "기계";
                //dto.Name = "기계수정설비A_1";
                //dto.Type = "형식1";
                //dto.RoomTbId = 10;

                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
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

                ResponseUnit<bool?> model = await MachineFacilityService.UpdateMachineFacilityService(HttpContext, dto, files).ConfigureAwait(false);
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
        [Route("sign/DeleteMachineFacility")]
        public async Task<IActionResult> DeleteMachineFacility([FromBody] List<int> delIdx)
        {
            //List<int> delIdx = new List<int>() { 3, 4 };

            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await MachineFacilityService.DeleteMachineFacilityService(HttpContext, delIdx).ConfigureAwait(false);
                
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
