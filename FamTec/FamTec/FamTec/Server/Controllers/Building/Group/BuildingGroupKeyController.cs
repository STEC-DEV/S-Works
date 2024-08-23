using FamTec.Server.Services;
using FamTec.Server.Services.Building.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Building.Group;
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

        public BuildingGroupKeyController(IBuildingKeyService _buildingkeyservice,
            ILogService _logservice)
        {
            this.BuildingKeyService = _buildingkeyservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// Key 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async ValueTask<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            try
            {
                //AddKeyDTO dto = new AddKeyDTO();
                //dto.GroupID = 10;
                //dto.Name = "추가키";
                //dto.Unit = "단위1";
                //dto.ItemValues.Add(new Shared.Server.DTO.Building.Group.AddGroupItemValueDTO()
                //{
                //    Values = "값1",
                    
                //});
                //dto.ItemValues.Add(new Shared.Server.DTO.Building.Group.AddGroupItemValueDTO()
                //{
                //    Values = "값2"
                //});

                if (HttpContext is null)
                    return BadRequest();

                if (dto.GroupID is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                if(dto.ItemValues is [_, ..])
                {
                    foreach(AddGroupItemValueDTO ValueDTO in dto.ItemValues)
                    {
                        if (String.IsNullOrWhiteSpace(ValueDTO.Values))
                            return NoContent();
                    }
                }

                ResponseUnit<AddKeyDTO> model = await BuildingKeyService.AddKeyService(HttpContext, dto);

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
                //dto.ID = 14;
                //dto.Itemkey = "추가_수정1";
                //dto.Unit = "수정";
                //dto.ValueList.Add(new Shared.Server.DTO.Building.GroupValueListDTO()
                //{
                //    ID = 38,
                //    ItemValue = "수정값11",
                //});
                //dto.ValueList.Add(new Shared.Server.DTO.Building.GroupValueListDTO()
                //{
                //    ItemValue = "추가값1",
                //});
                

                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();
                
                if (String.IsNullOrWhiteSpace(dto.Itemkey))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Unit))
                    return NoContent();

                ResponseUnit<UpdateKeyDTO> model = await BuildingKeyService.UpdateKeyService(HttpContext, dto);
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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteKeyList")]
        public async ValueTask<IActionResult> DeleteGroupKeyList([FromQuery]List<int> keylist)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                if (keylist is null)
                    return NoContent();
                if (keylist.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await BuildingKeyService.DeleteKeyListService(HttpContext, keylist);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 키 삭제
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteKey")]
        public async ValueTask<IActionResult> DeleteGroupKey([FromQuery]int keyid)
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
