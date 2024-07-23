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
            List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
            dto.Add(new InOutInventoryDTO
            {
                InOut = 1,
                MaterialID = 1,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now.AddDays(-10),
                    Num = 100,
                    RoomID = 1,
                    UnitPrice = 3000,
                    TotalPrice = 100*3000,
                    Note = "입고데이터_1"
                }
            });
            dto.Add(new InOutInventoryDTO
            {
                InOut = 1,
                MaterialID = 1,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now.AddDays(-20),
                    Num = 300,
                    RoomID = 1,
                    UnitPrice = 500,
                    TotalPrice = 300 * 500,
                    Note = "입고데이터_2"
                }
            });
            dto.Add(new InOutInventoryDTO
            {
                InOut = 1,
                MaterialID = 1,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now.AddDays(-20),
                    Num = 300,
                    RoomID = 2,
                    UnitPrice = 500,
                    TotalPrice = 300 * 500,
                    Note = "입고데이터_2_2"
                }
            });


            ResponseUnit<bool?> model = await InStoreService.AddInStoreService(HttpContext, dto);
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
        public async ValueTask<IActionResult> OutInventoryService()
        {

            List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
            
            dto.Add(new InOutInventoryDTO()
            {
                InOut = 0,
                MaterialID = 1,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now,
                    Note = "출고데이터_1",
                    Num = 180,
                    RoomID = 1,
                    UnitPrice = 300,
                    TotalPrice = 720 * 300
                }
            });
          
            dto.Add(new InOutInventoryDTO()
            {
                InOut = 0,
                MaterialID = 1,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now,
                    Note = "출고데이터_1",
                    Num = 300,
                    RoomID = 2,
                    UnitPrice = 100,
                    TotalPrice = 720 * 100
                }
            });
            

            ResponseList<bool?> model = await InStoreService.OutInventoryService(HttpContext, dto);
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

       


    }
}
