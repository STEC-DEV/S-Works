using FamTec.Server.Services;
using FamTec.Server.Services.Material;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private IMaterialService MaterialService;
        private IFileService FileService;
        private ILogService LogService;


        public MaterialController(IMaterialService _materialservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MaterialService = _materialservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 자재 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaterial")]
        public async ValueTask<IActionResult> AddMaterial([FromForm] AddMaterialDTO dto, [FromForm]IFormFile? files)
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

                ResponseUnit<AddMaterialDTO>? model = await MaterialService.AddMaterialService(HttpContext, dto, files);

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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 속한 전체 자재LIST 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllMaterial")]
        public async ValueTask<IActionResult> GetAllMaterial()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaterialListDTO>? model = await MaterialService.GetPlaceMaterialListService(HttpContext);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 자재정보 상세조회
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailMaterial")]
        public async ValueTask<IActionResult> DetailMaterial([FromQuery]int materialid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailMaterialDTO>? model = await MaterialService.GetDetailMaterialService(HttpContext, materialid);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateMaterial")]
        public async ValueTask<IActionResult> UpdateMaterial([FromForm]UpdateMaterialDTO dto, [FromForm]IFormFile? files)
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

                ResponseUnit<bool?> model = await MaterialService.UpdateMaterialService(HttpContext, dto, files);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }


    }
}
