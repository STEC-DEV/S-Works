using FamTec.Server.Repository.Inventory;
using FamTec.Server.Services;
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
        private ILogService LogService;
        

        public StoreController(IInVentoryService _instoreservice,
            ILogService _logservice)
        {
            this.InStoreService = _instoreservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 입출고 이력 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetHistory")]
        public async ValueTask<IActionResult> GetInoutHistory()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<InOutHistoryListDTO>? model = await InStoreService.GetInOutHistoryService(HttpContext);
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
                return Problem("서버에서 처리하지 못하였습니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 입고등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddInStore")]
        public async ValueTask<IActionResult> AddInStore([FromBody] List<InOutInventoryDTO> dto)
        {
            try
            {
                //List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
                //dto.Add(new InOutInventoryDTO
                //{
                //    InOut = 1,
                //    MaterialID = 5,
                //    AddStore = new AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now.AddDays(-10),
                //        Num = 100,
                //        RoomID = 1,
                //        UnitPrice = 3000,
                //        TotalPrice = 100*3000,
                //        Note = "입고데이터_1"
                //    }
                //});
                //dto.Add(new InOutInventoryDTO
                //{
                //    InOut = 1,
                //    MaterialID = 6,
                //    AddStore = new AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now.AddDays(-20),
                //        Num = 135,
                //        RoomID = 2,
                //        UnitPrice = 500,
                //        TotalPrice = 300 * 500,
                //        Note = "입고데이터_2"
                //    }
                //});

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await InStoreService.AddInStoreService(HttpContext, dto);
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
                return Problem("서버에서 처리하지 못하였습니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 출고
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/OutInventory")]
        public async ValueTask<IActionResult> OutInventoryService(List<InOutInventoryDTO> dto)
        {
            try
            {
                //List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
                //dto.Add(new InOutInventoryDTO()
                //{
                //    InOut = 0,
                //    MaterialID = 5,
                //    AddStore = new AddStoreDTO()
                //    {
                //    InOutDate = DateTime.Now,
                //    Note = "출고데이터_1",
                //    Num = 10,
                //    RoomID = 1,
                //    UnitPrice = 300,
                //    TotalPrice = 10 * 300
                //    }
                //});


                //dto.Add(new InOutInventoryDTO()
                //{
                //    InOut = 0,
                //    MaterialID = 6,
                //    AddStore = new AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now,
                //        Note = "출고데이터_1",
                //        Num = 10,
                //        RoomID = 2,
                //        UnitPrice = 100,
                //        TotalPrice = 100 * 10
                //    }
                //});

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<bool?> model = await InStoreService.OutInventoryService(HttpContext, dto);
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
                return Problem("서버에서 처리하지 못하였습니다.", statusCode: 500);
            }

        }


        /// <summary>
        /// 사업장 별 폼목별 재고 현황
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceInventoryStatus")]
        //public async ValueTask<IActionResult> GetPlaceInventoryStatus()
        public async ValueTask<IActionResult> GetPlaceInventoryStatus([FromQuery]List<int>? materialid, [FromQuery]bool? type)
        {
            try
            {
                //List<int> materialId = new List<int>() { 3, 4, 5, 6 };
                //bool type = false;

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaterialHistory>? model = await InStoreService.GetPlaceInventoryRecordService(HttpContext, materialid, type);

                if (model is null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리하지 못하였습니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 기간별 입출고 내역
        /// </summary>
        /// <param name="materialid"></param>
        /// <param name="Startdate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPeriodicRecord")]
        public async ValueTask<IActionResult> PeriodicRecord([FromQuery]int? materialid, DateTime? Startdate, DateTime? EndDate)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PeriodicInventoryRecordDTO>? model = await InStoreService.PeriodicInventoryRecordService(HttpContext, materialid, Startdate, EndDate);

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
                return Problem("서버에서 처리하지 못하였습니다.", statusCode: 500);
            }
        }


    }
}
