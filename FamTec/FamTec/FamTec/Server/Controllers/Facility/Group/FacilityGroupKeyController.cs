using FamTec.Server.Services.Facility.Key;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Facility.Group
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityGroupKeyController : ControllerBase
    {
        private IFacilityKeyService FacilityKeyService;

        public FacilityGroupKeyController(IFacilityKeyService _facilitykeyservice)
        {
            this.FacilityKeyService = _facilitykeyservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddKey")]
        public async ValueTask<IActionResult> AddGroupKey([FromBody]AddKeyDTO dto)
        {
            //AddKeyDTO dto = new AddKeyDTO();
            //dto.GroupID = 1;
            //dto.Name = "부하축베어링";
            //dto.ItemValues.Add(new AddGroupItemValueDTO()
            //{
            //    Values = "6309ZZ"
            //});

            ResponseUnit<AddKeyDTO?> model = await FacilityKeyService.AddKeyService(HttpContext, dto);
            
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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateKey")]
        public async ValueTask<IActionResult> UpdateGroupKey([FromBody]UpdateKeyDTO dto)
        {
            //UpdateKeyDTO dto = new UpdateKeyDTO();
            //dto.ID = 3;
            //dto.Itemkey = "수정_부하축베어링";
            //dto.ValueList.Add(new GroupValueListDTO()
            //{
            //    ID = 3,
            //    ItemValue = "6333XX"
            //});

            ResponseUnit<UpdateKeyDTO?> model = await FacilityKeyService.UpdateKeyService(HttpContext, dto);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteKey")]
        public async ValueTask<IActionResult> DeleteGroupKey([FromBody]int keyid)
        {
            ResponseUnit<bool?> model = await FacilityKeyService.DeleteKeyService(HttpContext, keyid);

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
