using FamTec.Server.Repository.Building;
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
        public async ValueTask<IActionResult> GetTotalBuildingCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await BuildingService.TotalBuildingCount(HttpContext);
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
        public async ValueTask<IActionResult> SelectMyBuilding()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuilidngListService(HttpContext);

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
        public async ValueTask<IActionResult> SelectMyBuildingPage([FromQuery]int skip, [FromQuery]int take)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<BuildinglistDTO> model = await BuildingService.GetBuildingListPageService(HttpContext, skip, take);

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
        public async ValueTask<IActionResult> PlaceBuildingList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PlaceBuildingNameDTO> model = await BuildingService.GetPlaceBuildingNameService(HttpContext);

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
        public async ValueTask<IActionResult> SelectPlaceBuilding()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PlaceBuildingListDTO> model = await BuildingService.GetPlaceBuildingService(HttpContext);
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
        public async ValueTask<IActionResult> AddBuilding([FromForm] AddBuildingDTO dto, [FromForm] IFormFile? files)
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

                ResponseUnit<AddBuildingDTO> model = await BuildingService.AddBuildingService(HttpContext, dto, files);

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
        public async ValueTask<IActionResult> DetailBuilding([FromQuery] int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailBuildingDTO> model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid);

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
        [HttpPost]
        [Route("sign/DeleteBuilding")]
        public async ValueTask<IActionResult> DeleteBuilding([FromBody] List<int> buildingidx)
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

                ResponseUnit<bool?> model = await BuildingService.DeleteBuildingService(HttpContext, buildingidx);
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
        public async ValueTask<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Code))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

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

                ResponseUnit<bool?> model = await BuildingService.UpdateBuildingService(HttpContext, dto, files);

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
