using FamTec.Server.Services.Facility.Type.Beauty;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamTec.Server.Services;

namespace FamTec.Server.Controllers.Facility
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeautyFacilityController : ControllerBase
    {
        private IBeautyFacilityService BeautyFacilityService;
        private IFileService FileService;
        private ILogService LogService;

        public BeautyFacilityController(IBeautyFacilityService _beautyfacilityservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.BeautyFacilityService = _beautyfacilityservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 미화 설비 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBeautyFacility")]
        public async Task<IActionResult> AddFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if(HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.RoomTbId is null)
                    return NoContent();

                if (files is not null)
                {
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

                ResponseUnit<FacilityDTO>? model = await BeautyFacilityService.AddBeautyFacilityService(HttpContext, dto, files).ConfigureAwait(false);

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

        /// <summary>
        /// 사업장에 속한 미화설비 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllBeautyFacility")]
        public async Task<IActionResult> GetAllBeautyFacility()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<FacilityListDTO>? model = await BeautyFacilityService.GetBeautyFacilityListService(HttpContext).ConfigureAwait(false);

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

        /// <summary>
        /// 미화설비 상세조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailBeautyFacility")]
        public async Task<IActionResult> DetailBeautyFacility([FromQuery] int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<FacilityDetailDTO> model = await BeautyFacilityService.GetBeautyDetailFacilityService(HttpContext, facilityid).ConfigureAwait(false);
                
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

        /// <summary>
        /// 미화설비 정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateBeautyFacility")]
        public async Task<IActionResult> UpdateBeautyFacility([FromForm] FacilityDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Category))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.RoomTbId is null)
                    return NoContent();

                if (files is not null) // 파일이 있으면 1MB 제한
                {
                    string? extension = FileService.GetExtension(files);
                    if(String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if(!extensioncheck)
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await BeautyFacilityService.UpdateBeautyFacilityService(HttpContext, dto, files).ConfigureAwait(false);
                
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

        /// <summary>
        /// 미화설비 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteBeautyFacility")]
        public async Task<IActionResult> DeleteBeautyFacility([FromBody] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();

                if(delIdx.Count() == 0)
                    return NoContent();


                ResponseUnit<bool?> model = await BeautyFacilityService.DeleteBeautyFacilityService(HttpContext, delIdx).ConfigureAwait(false);
                
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

    }
}
