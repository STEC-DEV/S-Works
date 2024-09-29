using FamTec.Server.Services;
using FamTec.Server.Services.Building;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private IBuildingService BuildingService;
        private IFileService FileService;
        private ILogService LogService;

        public BuildingController(IBuildingService _buildingservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.BuildingService = _buildingservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
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

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
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

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
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

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
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

                if (String.IsNullOrWhiteSpace(dto.Code))
                    return NoContent();
                if (String.IsNullOrWhiteSpace(dto.Name))
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
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<AddBuildingDTO> model = await BuildingService.AddBuildingService(HttpContext, dto, files).ConfigureAwait(false);

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

                ResponseUnit<DetailBuildingDTO> model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid).ConfigureAwait(false);

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

        // 건물 삭제
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteBuilding")]
        public async Task<IActionResult> DeleteBuilding([FromBody] List<int> buildingidx)
        {
            try
            {
                //List<int> buildingidx = new List<int>() { 7, 9 };

                if (HttpContext is null)
                    return BadRequest();

                if (buildingidx is null)
                    return NoContent();
                
                if(buildingidx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BuildingService.DeleteBuildingService(HttpContext, buildingidx).ConfigureAwait(false);
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

        // 건물수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBuilding")]
        //public async Task<IActionResult> UpdateBuilding([FromForm] IFormFile? files)
        public async Task<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                //DetailBuildingDTO dto = new DetailBuildingDTO();
                //dto.ID = 12;
                //dto.Name = "C건물";

                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
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
                            return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await BuildingService.UpdateBuildingService(HttpContext, dto, files).ConfigureAwait(false);

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
