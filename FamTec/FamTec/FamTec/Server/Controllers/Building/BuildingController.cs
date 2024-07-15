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


        public BuildingController(IBuildingService _buildingservice)
        {
            BuildingService = _buildingservice;
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
            ResponseList<BuildinglistDTO>? model = await BuildingService.GetBuilidngListService(HttpContext);

            if (model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
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
            ResponseUnit<AddBuildingDTO?> model = await BuildingService.AddBuildingService(HttpContext, dto, files);

            if (model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
            }
            else
            {
                return BadRequest(model);
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
            ResponseUnit<DetailBuildingDTO>? model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid);

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

        // 건물 삭제
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteBuilding")]
        public async ValueTask<IActionResult> DeleteBuilding([FromBody] List<int> buildingidx)
        {
            
            ResponseUnit<int?> model = await BuildingService.DeleteBuildingService(HttpContext, buildingidx);
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

        // 건물수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateBuilding")]
        public async ValueTask<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO? dto, [FromForm] IFormFile? files)
        {
            ResponseUnit<bool?> model = await BuildingService.UpdateBuildingService(HttpContext, dto, files);

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




    }
}
