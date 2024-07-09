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
        /// 사업장에 건물 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddBuilding")]
        public async ValueTask<IActionResult> AddBuilding([FromForm] AddBuildingDTO dto, [FromForm]IFormFile files)
        {
            //AddBuildingDTO dto = new AddBuildingDTO();
            //dto.BuildingCD = "BCODE000004";
            //dto.Name = "TEST건물4";
            //dto.Address = "경기도";
            //dto.Tel = "02-123-4567";
            //dto.Usage = "주상복합";
            //dto.ConstComp = "ㅂㅂ업체";

            //AddGroupDTO groupdto1 = new AddGroupDTO();
            //groupdto1.Name = "주차장";

            //AddGroupItemKeyDTO groupkey = new AddGroupItemKeyDTO();
            //groupkey.Name = "실내";

            //AddGroupItemValueDTO groupValue = new AddGroupItemValueDTO();
            //groupValue.Values = "5";
            //groupValue.Unit = "대";
            //groupkey.ItemValues.Add(groupValue);

            
            //groupdto1.AddGroupKey.Add(groupkey);
            //groupkey = new AddGroupItemKeyDTO();
            //groupkey.Name = "실외";
            //groupValue = new AddGroupItemValueDTO();
            //groupValue.Values = "3";
            //groupValue.Unit = "대";
            //groupkey.ItemValues.Add(groupValue);
            //groupdto1.AddGroupKey.Add(groupkey);
            
            //// ====

            //AddGroupDTO groupdto2 = new AddGroupDTO();
            //groupdto2.Name = "전력량";

            //AddGroupItemKeyDTO groupkey2 = new AddGroupItemKeyDTO();
            //groupkey2.Name = "원자력";

            //AddGroupItemValueDTO groupValue2 = new AddGroupItemValueDTO();
            //groupValue2.Values = "30";
            //groupValue2.Unit = "%";
            //groupkey2.ItemValues.Add(groupValue2);


            //groupdto2.AddGroupKey.Add(groupkey2);
            //groupkey2 = new AddGroupItemKeyDTO();
            //groupkey2.Name = "화력";
            //groupValue2 = new AddGroupItemValueDTO();
            //groupValue2.Values = "70";
            //groupValue2.Unit = "%";
            //groupkey2.ItemValues.Add(groupValue2);
            //groupdto2.AddGroupKey.Add(groupkey2);


            //dto.SubItem.Add(groupdto1);
            //dto.SubItem.Add(groupdto2);
            //IFormFile files = null;

            ResponseUnit<bool> model = await BuildingService.AddBuildingService(HttpContext, dto, files);

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
            // 파일도 반환하는거 찾아야함.

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
        public async ValueTask<IActionResult> UpdateBuilding([FromForm] DetailBuildingDTO dto, [FromForm] IFormFile files)
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
