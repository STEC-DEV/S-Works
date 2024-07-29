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
        private ILogService LogService;

        public BuildingController(IBuildingService _buildingservice,
            ILogService _logservice)
        {
            this.BuildingService = _buildingservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장에 해당하는 건물리스트 출력 [수정완료]
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

                ResponseList<BuildinglistDTO>? model = await BuildingService.GetBuilidngListService(HttpContext);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> AddBuilding([FromForm] AddBuildingDTO? dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is not null) // 파일이 있으면 1MB 제한
                {
                    if(files.Length > 1048576)
                    {
                        return Ok(new ResponseUnit<AddBuildingDTO>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<AddBuildingDTO?> model = await BuildingService.AddBuildingService(HttpContext, dto, files);

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
                return Problem("서버에서 처리할 수 없는 작업입니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> DetailBuilding([FromQuery] int? buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailBuildingDTO>? model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid);

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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
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
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await BuildingService.DeleteBuildingService(HttpContext, buildingidx);
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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }

        // 건물수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBuilding")]
        public async ValueTask<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO? dto, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                // 파일크기 1MB 지정
                if(files is not null)
                {
                    if(files.Length > 1048576)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
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
                return Problem("서버에서 처리할수 없는 작업입니다.", statusCode: 500);
            }
        }




    }
}
