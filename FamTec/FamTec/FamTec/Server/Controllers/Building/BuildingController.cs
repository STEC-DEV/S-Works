using FamTec.Server.Services.Building;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
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
        /// 사업장에 건물 추가 [수정완료]
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sign/AddBuilding")]
        public async ValueTask<IActionResult> InsertBuilding([FromBody] BuildingsDTO dto)
        {
            ResponseUnit<bool> model = await BuildingService.AddBuildingService(HttpContext, dto);

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

        // 건물 디테일
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailBuilding")]
        public async ValueTask<IActionResult> DetailBuilding([FromQuery]int? buildingid)
        {
            ResponseUnit<DetailBuildingDTO>? model = await BuildingService.GetDetailBuildingService(HttpContext, buildingid);

            if(model is not null)
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
        [HttpPost]
        [Route("sign/UpdateBuilding")]
        public async ValueTask<IActionResult> UpdateBuilding([FromBody]DetailBuildingDTO dto)
        {
            ResponseUnit<DetailBuildingDTO>? model = await BuildingService.UpdateBuildingService(HttpContext, dto);
            
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

        // 건물 삭제
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DeleteBuilding")]
        public async ValueTask<IActionResult> DeleteBuilding()
        {
            List<int> temp = new List<int>() { 4, 5 };

            ResponseUnit<int?> model = await BuildingService.DeleteBuildingService(HttpContext, temp);
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
