using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Building
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService BuildingService;
        private readonly IFileService FileService; /* 이미지 공통 서비스 */
        private readonly ICommService CommService; /* 핼퍼클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<BuildingController> CreateBuilderLogger; /* 콘솔로그 */

        public BuildingController(IBuildingService _buildingservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<BuildingController> _createbuilderlogger)
        {
            this.BuildingService = _buildingservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 건물 엑셀 양식 다운로드
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadBuildingForm")]
        public async Task<IActionResult> DownloadBuildingForm()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                byte[]? fileBytes = await BuildingService.DownloadBuildingForm();

                if (fileBytes is not null)
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "건물정보(양식).xlsx");
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportBuilding")]
        public async Task<IActionResult> ImportBuildingData([FromForm][Required] IFormFile files)
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

                string? extension = FileService.GetExtension(files); // 파일 확장자 추출
                if(String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension); // 파일 확장자 검사
                    if(!extensioncheck)
                    {
                        return Ok(new ResponseUnit<bool>() { message = "지원하지 않는 파일 형식입니다.", data = false, code = 204 });
                    }
                }

                if (files.Length > Common.MEGABYTE_10)
                    return Ok(new ResponseUnit<bool>() { message = "파일의 용량은 10MB까지 가능합니다.", data = false, code = 204 });

                ResponseUnit<bool> model = await BuildingService.ImportBuildingService(files);
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


        /// <summary>
        /// 해당 자재가 존재하는 건물들 반환
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMaterialBuildings")]
        public async Task<IActionResult> GetMaterialBuildings([FromQuery][Required]int materialid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (materialid is 0)
                    return NoContent();

                ResponseList<PlaceBuildingNameDTO>? model = await BuildingService.GetPlaceAvailableBuildingList(materialid).ConfigureAwait(false);
                
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/TotalBuildingCount")]
        public async Task<IActionResult> GetTotalBuildingCount()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseUnit<int?> model = await BuildingService.TotalBuildingCount().ConfigureAwait(false);
                
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

        /// <summary>
        /// 사업장에 해당하는 건물리스트 출력 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/MyBuildings")]
        public async Task<IActionResult> SelectMyBuilding()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuilidngListService().ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
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
        /// 사업장에 해당하는 건물리스트 출력 - 서버 페이지 네이션
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/MyBuildingPage")]
        public async Task<IActionResult> SelectMyBuildingPage([FromQuery][Required]int skip, [FromQuery][Required]int take)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuildingListPageService(skip, take).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
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
        /// 로그인한 아이디의 사업장의 건물명 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceBuilding")]
        public async Task<IActionResult> PlaceBuildingList()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<PlaceBuildingNameDTO> model = await BuildingService.GetPlaceBuildingNameService().ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/PlaceBuildingList")]
        public async Task<IActionResult> SelectPlaceBuilding()
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                ResponseList<PlaceBuildingListDTO> model = await BuildingService.GetPlaceBuildingService().ConfigureAwait(false);
                
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

        /// <summary>
        /// 사업장에 건물 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBuilding")]
        public async Task<IActionResult> AddBuilding([FromForm] AddBuildingDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_10)
                        return Ok(new ResponseUnit<AddBuildingDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

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
                            return Ok(new ResponseUnit<AddBuildingDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<AddBuildingDTO> model = await BuildingService.AddBuildingService(dto, files).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
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
        /// 건물 상세정보 조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailBuilding")]
        public async Task<IActionResult> DetailBuilding([FromQuery][Required] int buildingid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                bool isMobile = CommService.MobileConnectCheck();

                ResponseUnit<DetailBuildingDTO> model = await BuildingService.GetDetailBuildingService(buildingid, isMobile).ConfigureAwait(false);

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

        // 건물 삭제
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteBuilding")]
        public async Task<IActionResult> DeleteBuilding([FromBody][Required] List<int> buildingidx)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (buildingidx is null)
                    return NoContent();
                
                if(buildingidx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BuildingService.DeleteBuildingService(buildingidx).ConfigureAwait(false);
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

        // 건물수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBuilding")]
        public async Task<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
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

                ResponseUnit<bool?> model = await BuildingService.UpdateBuildingService(dto, files).ConfigureAwait(false);

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

        /// <summary>
        /// 건물이름 조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBuildingName")]
        public async Task<IActionResult> GetBuildingName([FromQuery][Required]int buildingid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (buildingid is 0)
                    return NoContent();

                ResponseUnit<string?> model = await BuildingService.GetBuildingName(buildingid).ConfigureAwait(false);
                
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
