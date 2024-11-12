using DocumentFormat.OpenXml.InkML;
using FamTec.Server.Middleware;
using FamTec.Server.Repository.Building;
using FamTec.Server.Services;
using FamTec.Server.Services.Building;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService BuildingService;
        
        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ICommService CommService;
        private readonly ConsoleLogService<BuildingController> CreateBuilderLogger;

        public BuildingController(IBuildingService _buildingservice,
            IBuildingInfoRepository _buildinginforepository,
            IFileService _fileservice,
            ILogService _logservice,
            ICommService _commservice,
            ConsoleLogService<BuildingController> _createbuilderlogger
        )
        {
            this.BuildingService = _buildingservice;

            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CommService = _commservice;
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
                
                byte[]? fileBytes = await BuildingService.DownloadBuildingForm(HttpContext);

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
        public async Task<IActionResult> ImportBuildingData([FromForm] IFormFile files)
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
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension); // 파일 확장자 검사
                    if(!extensioncheck)
                    {
                        return Ok(new ResponseUnit<bool>() { message = "지원하지 않는 파일 형식입니다.", data = false, code = 204 });
                    }
                }

                ResponseUnit<bool> model = await BuildingService.ImportBuildingService(HttpContext, files);
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
        public async Task<IActionResult> GetMaterialBuildings([FromQuery]int materialid)
        {
            try
            {
                //int placeid = 1;
                //int materialid = 1;
                if (HttpContext is null)
                    return BadRequest();

                if (materialid is 0)
                    return NoContent();

                ResponseList<PlaceBuildingNameDTO>? model = await BuildingService.GetPlaceAvailableBuildingList(HttpContext, materialid).ConfigureAwait(false);
                
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await BuildingService.TotalBuildingCount(HttpContext).ConfigureAwait(false);
                
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
                if (HttpContext is null)
                    return BadRequest();
                
                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuilidngListService(HttpContext).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

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
        public async Task<IActionResult> SelectMyBuildingPage([FromQuery]int skip, [FromQuery]int take)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuildingListPageService(HttpContext, skip, take).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PlaceBuildingNameDTO> model = await BuildingService.GetPlaceBuildingNameService(HttpContext).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

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
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PlaceBuildingListDTO> model = await BuildingService.GetPlaceBuildingService(HttpContext).ConfigureAwait(false);
                
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
                if (HttpContext is null)
                    return BadRequest();

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

                ResponseUnit<AddBuildingDTO> model = await BuildingService.AddBuildingService(HttpContext, dto, files).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

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
        public async Task<IActionResult> DetailBuilding([FromQuery] int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<DetailBuildingDTO> model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid, isMobile).ConfigureAwait(false);

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

        // 건물 삭제
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteBuilding")]
        public async Task<IActionResult> DeleteBuilding([FromBody] List<int> buildingidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (buildingidx is null)
                    return NoContent();
                
                if(buildingidx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BuildingService.DeleteBuildingService(HttpContext, buildingidx).ConfigureAwait(false);
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

        // 건물수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBuilding")]
        public async Task<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

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

                ResponseUnit<bool?> model = await BuildingService.UpdateBuildingService(HttpContext, dto, files).ConfigureAwait(false);

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

        /// <summary>
        /// 건물이름 조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBuildingName")]
        public async Task<IActionResult> GetBuildingName([FromQuery]int buildingid)
        {
            try
            {
                if (buildingid is 0)
                    return NoContent();

                ResponseUnit<string?> model = await BuildingService.GetBuildingName(buildingid).ConfigureAwait(false);
                
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
