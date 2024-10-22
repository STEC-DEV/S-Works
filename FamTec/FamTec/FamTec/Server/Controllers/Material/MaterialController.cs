using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Material;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Material
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService MaterialService;
        private readonly IFileService FileService;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<MaterialController> CreateBuilderLogger;

        public MaterialController(IMaterialService _materialservice,
            IFileService _fileservice,
            ILogService _logservice,
            ConsoleLogService<MaterialController> _createbuilderlogger)
        {
            this.MaterialService = _materialservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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
        public async Task<IActionResult> AddMaterial([FromForm] AddMaterialDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Code))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(dto.RoomID is null)
                    return NoContent();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseUnit<AddMaterialDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseUnit<AddMaterialDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<AddMaterialDTO> model = await MaterialService.AddMaterialService(HttpContext, dto, files).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 201)
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
        public async Task<IActionResult> GetAllMaterial()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaterialListDTO> model = await MaterialService.GetPlaceMaterialListService(HttpContext).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력 - Search용
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllSearchMaterialList")]

        public async Task<IActionResult> GetAllSearchMaterialList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaterialSearchListDTO> model = await MaterialService.GetAllPlaecMaterialSearchService(HttpContext).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 총 개수 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllMaterialCount")]
        public async Task<IActionResult> GetAllMaterialCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await MaterialService.GetPlaceMaterialCountService(HttpContext).ConfigureAwait(false);
                
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        // 일반 게시판 1,2,3,4 페이지 구분있음
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllPageNationMaterial")]
        public async Task<IActionResult> GetAllPageNationMaterial([FromQuery]int pagenum, [FromQuery]int pagesize)
        {
            try
            {
                if (pagesize > 100)
                    return BadRequest(); // 사이즈 초과

                if (pagenum == 0)
                    return BadRequest(); // 잘못된 요청

                if (pagesize == 0)
                    return BadRequest(); // 잘못된 요청

                ResponseList<MaterialListDTO> model = await MaterialService.GetPlaceMaterialPageNationListService(HttpContext, pagenum, pagesize).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

                // Front 
                //1 페이지 25 ==> 0
                // 2 페이지 25 ==> 50
                //int offset = (pagenum - 1) * pagesize; // OFFSET 시작점
                //int limit = offset + pagesize; // LIMIT 끝점

                // 리턴 - LIst<Data> 
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        // CURSOR 기반 - NEXT ID 반환 (ex 쿠팡, 네이버) 페이지 1,2,3,4 구분없음 STACK 식으로 보여주는 구조
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllCursorPageNationMaterial")]
        public async Task<IActionResult> GetAllCursorPageNationMaterial([FromQuery] int id, [FromQuery] int pagesize)// 조회ID - pagesize / return: nextid
        {
            if (pagesize > 20)
                return BadRequest(); // 사이즈 초과

            
            if (pagesize == 0)
                return BadRequest(); // 잘못된 요청

            
            
            // return List<data> 와 next id를 주면됨
            return Ok();
        }


        /// <summary>
        /// 자재정보 상세조회
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailMaterial")]
        public async Task<IActionResult> DetailMaterial([FromQuery]int materialid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailMaterialDTO> model = await MaterialService.GetDetailMaterialService(HttpContext, materialid).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 자재정보 수정 --- 여기 확인
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateMaterial")]
        public async Task<IActionResult> UpdateMaterial([FromForm]UpdateMaterialDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.Id is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                //if (dto.RoomID is null)
                //    return NoContent();

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

                ResponseUnit<bool?> model = await MaterialService.UpdateMaterialService(HttpContext, dto, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 품목 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteMaterial")]
        public async Task<IActionResult> DeleteMateral([FromBody]List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (delIdx is null)
                    return NoContent();
                
                if(delIdx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await MaterialService.DeleteMaterialService(HttpContext, delIdx).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportMaterial")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (files is null)
                    return NoContent();

                if (files.Length == 0)
                    return NoContent();

                string? extension = FileService.GetExtension(files); // 파일 확장자 추출
                if(String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensionCheck = Common.XlsxAllowedExtensions.Contains(extension); // 파일 확장자 검사 xlsx or xlx 만 허용
                    if(!extensionCheck)
                    {
                        return Ok(new ResponseUnit<string?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<string?> model = await MaterialService.ImportMaterialService(HttpContext, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 품목검색
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/MaterialSearch")]
        public async Task<IActionResult> MaterialSearch([FromQuery]string searchData)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(searchData))
                    return NoContent();

                ResponseList<MaterialSearchListDTO>? model = await MaterialService.GetMaterialSearchService(HttpContext, searchData).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
            }
        }

    }
}
