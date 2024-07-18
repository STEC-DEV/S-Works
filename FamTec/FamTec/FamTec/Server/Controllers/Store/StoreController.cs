using FamTec.Server.Services.Store;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IInStoreService InStoreService;

        public StoreController(IInStoreService _instoreservice)
        {
            this.InStoreService = _instoreservice;
        }

        [AllowAnonymous]
        //[HttpPost]
        [HttpGet]
        [Route("sign/AddInStore")]
        public async ValueTask<IActionResult> AddInStore()
        //public async ValueTask<IActionResult> AddInStore([FromBody]AddStoreDTO dto)
        {
            AddStoreDTO dto = new AddStoreDTO();
            dto.MaterialID = 1; // 품목코드
            dto.StoreList.Add(new StoreDTO()
            {
                InOut = 1, // 입고
                InOutDate = DateTime.Now.AddDays(-10), // 입고날짜
                Num = 100, // 입고수량
                RoomID = 1, // 공간정보
                UnitPrice = 3000 // 입고금액
            });
            dto.StoreList.Add(new StoreDTO()
            {
                InOut = 1,
                InOutDate = DateTime.Now.AddDays(-20),
                Num = 300,
                RoomID = 1,
                UnitPrice = 3500
            });

            ResponseUnit<AddStoreDTO>? model = await InStoreService.AddInStoreService(HttpContext, dto);
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
