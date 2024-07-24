using FamTec.Server.Repository.Inventory;
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
        
        // Temp
        private IInventoryInfoRepository InventoryInfoRepository;

        public StoreController(IInVentoryService _instoreservice, IInventoryInfoRepository _inven)
        {
            this.InStoreService = _instoreservice;
            this.InventoryInfoRepository = _inven;
        }

        // 기간별 입출고 내역 뽑는 로직 짜야함.
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/Temp")]
        public async ValueTask<IActionResult> GetList()
        {
            InventoryInfoRepository.GetInventoryRecord(3,1);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/Temp2")]
        public async ValueTask<IActionResult> GetList2()
        {
            InventoryInfoRepository.GetInventoryRecord2(3);
            return Ok();
        }

        // 입고
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
                MaterialID = 5,
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
                MaterialID = 6,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now.AddDays(-20),
                    Num = 135,
                    RoomID = 2,
                    UnitPrice = 500,
                    TotalPrice = 300 * 500,
                    Note = "입고데이터_2"
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

        // 출고
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/OutInventory")]
        public async ValueTask<IActionResult> OutInventoryService()
        {

            List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
            
            dto.Add(new InOutInventoryDTO()
            {
                InOut = 0,
                MaterialID = 5,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now,
                    Note = "출고데이터_1",
                    Num = 125,
                    RoomID = 1,
                    UnitPrice = 300,
                    TotalPrice = 720 * 300
                }
            });
          
        
            dto.Add(new InOutInventoryDTO()
            {
                InOut = 0,
                MaterialID = 6,
                AddStore = new AddStoreDTO()
                {
                    InOutDate = DateTime.Now,
                    Note = "출고데이터_1",
                    Num = 155,
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

        // 입출고 이력
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
