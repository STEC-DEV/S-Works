using FamTec.Server.Services.Building.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupKeyController : ControllerBase
    {
        private IBuildingKeyService BuildingKeyService;

        public BuildingGroupKeyController(IBuildingKeyService _buildingkeyservice)
        {
            this.BuildingKeyService = _buildingkeyservice;
        }

        // 추가
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async ValueTask<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            //AddKeyDTO dto = new AddKeyDTO();
            //dto.GroupID = 1;
            //dto.Name = "추가키";
            //dto.ItemValues.Add(new Shared.Server.DTO.Building.Group.AddGroupItemValueDTO()
            //{
            //    Values = "값1",
            //    Unit = "단위1"
            //});
            //dto.ItemValues.Add(new Shared.Server.DTO.Building.Group.AddGroupItemValueDTO()
            //{
            //    Values = "값2",
            //    Unit = "단위2"
            //});

            ResponseUnit<AddKeyDTO?> model = await BuildingKeyService.AddKeyService(HttpContext, dto);

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

        // 수정 - 삭제 확인

        // 수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateKey")]
        public async ValueTask<IActionResult> UpdateGroupKey([FromBody]UpdateKeyDTO dto)
        {
            //UpdateKeyDTO dto = new UpdateKeyDTO();
            //dto.ID = 3;
            //dto.Itemkey = "추가_수정1";
            //dto.ValueList.Add(new Shared.Server.DTO.Building.GroupValueListDTO()
            //{
            //    ID = 4,
            //    ItemValue = "수정값1",
            //    Unit = "수정단위1"
            //});
            //dto.ValueList.Add(new Shared.Server.DTO.Building.GroupValueListDTO()
            //{
            //    ID = 5,
            //    ItemValue = "수정값2",
            //    Unit = "수정단위2"
            //});

            ResponseUnit<UpdateKeyDTO?> model = await BuildingKeyService.UpdateKeyService(HttpContext, dto);

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

        // 삭제
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async ValueTask<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            ResponseUnit<bool?> model = await BuildingKeyService.DeleteKeyService(HttpContext, keyid);
            
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



    }
}
