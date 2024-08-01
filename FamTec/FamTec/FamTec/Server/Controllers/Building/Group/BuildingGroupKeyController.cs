using FamTec.Server.Services;
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
        private ILogService LogService;

        public BuildingGroupKeyController(IBuildingKeyService _buildingkeyservice, ILogService _logservice)
        {
            this.BuildingKeyService = _buildingkeyservice;
            this.LogService = _logservice;
        }

        // 추가
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async ValueTask<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            try
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

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<AddKeyDTO?> model = await BuildingKeyService.AddKeyService(HttpContext, dto);

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

        // 수정 - 삭제 확인

        // 수정
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateKey")]
        public async ValueTask<IActionResult> UpdateGroupKey([FromBody]UpdateKeyDTO dto)
        {
            try
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

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UpdateKeyDTO?> model = await BuildingKeyService.UpdateKeyService(HttpContext, dto);

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

        // 삭제
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async ValueTask<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await BuildingKeyService.DeleteKeyService(HttpContext, keyid);

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
