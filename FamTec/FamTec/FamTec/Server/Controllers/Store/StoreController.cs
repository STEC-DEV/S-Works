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
        private IInVentoryService InStoreService;

        public StoreController(IInVentoryService _instoreservice)
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
            AddInventoryDTO dto = new AddInventoryDTO();
            dto.MaterialID = 1; // 품목코드
            dto.StoreList.Add(new InventoryDTO()
            {
                InOut = 1, // 입고
                InOutDate = DateTime.Now.AddDays(-10), // 입고날짜
                Num = 100, // 입고수량
                RoomID = 1, // 공간정보
                UnitPrice = 3000, // 입고금액
                Note = "입고데이터_1"
            });
            dto.StoreList.Add(new InventoryDTO()
            {
                InOut = 1,
                InOutDate = DateTime.Now.AddDays(-20),
                Num = 300,
                RoomID = 1,
                UnitPrice = 3500,
                Note = "입고데이터_2"
            });

            ResponseUnit<AddInventoryDTO>? model = await InStoreService.AddInStoreService(HttpContext, dto);
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
        [HttpGet]
        [Route("sign/GetHistory")]
        public async ValueTask<IActionResult> GetInoutHistory()
        {
            ResponseList<InOutHistoryListDTO>? model = await InStoreService.GetInOutHistoryService(HttpContext);
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
        [HttpGet]
        [Route("sign/GetMaterialCount")]
        public async ValueTask<IActionResult> GetMaterialCount([FromQuery]int materialid, [FromQuery]int roomid)
        {

            ResponseUnit<int?> model = await InStoreService.GetOutCountService(HttpContext, materialid, roomid);
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
        [HttpGet]
        [Route("sign/OutInventory")]
        public async ValueTask<IActionResult> OutInventoryService([FromQuery]int materialid, int roomid)
        {
            ResponseList<bool?> model = await InStoreService.OutInventoryService(HttpContext, materialid, roomid);
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
