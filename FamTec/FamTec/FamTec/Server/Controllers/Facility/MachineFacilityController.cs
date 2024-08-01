using FamTec.Server.Services;
using FamTec.Server.Services.Facility.Type.Machine;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineFacilityController : ControllerBase
    {
        private IMachineFacilityService MachineFacilityService;
        private IFileService FileService;
        private ILogService LogService;

        public MachineFacilityController(IMachineFacilityService _machinefacilityservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MachineFacilityService = _machinefacilityservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMachineFacility")]
        public async ValueTask<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

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
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<FacilityDTO>? model = await MachineFacilityService.AddMachineFacilityService(HttpContext, dto, files);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllMachineFacility")]
        public async ValueTask<IActionResult> GetAllMachineFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await MachineFacilityService.GetMachineFacilityListService(HttpContext);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailMachineFacility")]
        public async ValueTask<IActionResult> DetailMachineFacility([FromQuery]int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO?> model = await MachineFacilityService.GetMachineDetailFacilityService(HttpContext, facilityid);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateMachineFacility")]
        public async ValueTask<IActionResult> UpdateMachineFacility([FromForm] FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

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
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await MachineFacilityService.UpdateMachineFacilityService(HttpContext, dto, files);
                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteMachineFacility")]
        public async ValueTask<IActionResult> DeleteMachineFacility([FromBody] List<int> delIdx)
        {
            ResponseUnit<int?> model = await MachineFacilityService.DeleteMachineFacilityService(HttpContext, delIdx);
            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
