using FamTec.Server.Repository.Inventory;
using FamTec.Server.Services;
using FamTec.Server.Services.Store;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;

namespace FamTec.Server.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IInVentoryService InStoreService;
        private ILogService LogService;
        private IInventoryInfoRepository InventoryInfoRepository;

        public StoreController(IInVentoryService _instoreservice,
            IInventoryInfoRepository _inventoryInfoRepository,
            ILogService _logservice)
        {
            this.InStoreService = _instoreservice;
            this.InventoryInfoRepository = _inventoryInfoRepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 입고 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpGet]
        [HttpPost]
        [Route("sign/AddInStore")]
        //public async ValueTask<IActionResult> AddInStore()
        public async ValueTask<IActionResult> AddInStore([FromBody] List<InOutInventoryDTO> dto)
        {
            try
            {
                //List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
                //dto.Add(new InOutInventoryDTO
                //{
                //    InOut = 1,
                //    MaterialID = 10,
                //    AddStore = new AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now.AddDays(-10),
                //        Num = 100,
                //        RoomID = 2,
                //        UnitPrice = 3000,
                //        TotalPrice = 100*3000,
                //        Note = "입고데이터_1"
                //    }
                //});
                //dto.Add(new InOutInventoryDTO
                //{
                //    InOut = 1,
                //    MaterialID = 11,
                //    AddStore = new AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now.AddDays(-20),
                //        Num = 135,
                //        RoomID = 3,
                //        UnitPrice = 500,
                //        TotalPrice = 300 * 500,
                //        Note = "입고데이터_2"
                //    }
                //});

                if (HttpContext is null)
                    return BadRequest();

                foreach(InOutInventoryDTO InOutDTO in dto)
                {
                    if (InOutDTO.InOut is null)
                        return NoContent();
                    if (InOutDTO.MaterialID is null)
                        return NoContent();
                    if(InOutDTO.AddStore!.RoomID is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.UnitPrice is null)
                        return NoContent();
                    if(InOutDTO.AddStore!.Num is null)
                        return NoContent();
                }

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 출고 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpGet]
        [HttpPost]
        [Route("sign/OutInventory")]
        //public async ValueTask<IActionResult> OutInventoryService()
        public async ValueTask<IActionResult> OutInventoryService(List<InOutInventoryDTO> dto)
        {
            try
            {
                //List<InOutInventoryDTO> dto = new List<InOutInventoryDTO>();
                //dto.Add(new InOutInventoryDTO()
                //{
                //InOut = 0,
                //MaterialID = 10,
                //AddStore = new AddStoreDTO()
                //{
                //InOutDate = DateTime.Now,
                //Note = "출고데이터_1",
                //Num = 10,
                //RoomID = 2,
                //UnitPrice = 300,
                //TotalPrice = 3000
                //}
                //});


                //dto.Add(new InOutInventoryDTO()
                //{
                //InOut = 0,
                //MaterialID = 11,
                //AddStore = new AddStoreDTO()
                //{
                //InOutDate = DateTime.Now,
                //Note = "출고데이터_1",
                //Num = 10,
                //RoomID = 3,
                //UnitPrice = 100,
                //TotalPrice = 1000
                //}
                //});

                if (HttpContext is null)
                    return BadRequest();

                foreach (InOutInventoryDTO InOutDTO in dto)
                {
                    if (InOutDTO.InOut is null)
                        return NoContent();
                    if (InOutDTO.MaterialID is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.RoomID is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.UnitPrice is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.Num is null)
                        return NoContent();
                }

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }

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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 입출고 이력 페이지네이션 조회
        /// </summary>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPageNationHistory")]
        public async ValueTask<IActionResult> GetInoutPageNationHistory([FromQuery] int pagenum, [FromQuery] int pagesize)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (pagenum == 0)
                    return NoContent();

                if(pagesize == 0 || pagesize > 100)
                    return NoContent();

                ResponseList<InOutHistoryListDTO>? model = await InStoreService.GetInoutPageNationHistoryService(HttpContext, pagenum, pagesize);
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

        /// <summary>
        /// 사업장의 입-출고 이력 개수 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllInoutPlaceCount")]
        public async ValueTask<IActionResult> GetAllInoutPlaceCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await InStoreService.GetPlaceInOutCountService(HttpContext);
                
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

        /// <summary>
        /// 사업장 별 폼목별 재고 현황 
        ///     - true (재고가 없는것도) : false: 재고가 하나라도 있는것만
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceInventoryStatus")]
        //public async ValueTask<IActionResult> GetPlaceInventoryStatus()
        public async ValueTask<IActionResult> GetPlaceInventoryStatus([FromQuery]List<int> materialid, [FromQuery]bool type)
        {
            try
            {
                //List<int> materialid = new List<int>() { 10,11 };
                //bool type = false;

                if (HttpContext is null)
                    return BadRequest();

                if (materialid is null)
                    return NoContent();

                if(materialid.Count() == 0)
                    return NoContent();

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> PeriodicRecord([FromQuery] int materialid, DateTime Startdate, DateTime EndDate)
        {
            try
            {
                //int materialid = 10;
                //DateTime Startdate = DateTime.Now.AddDays(-30);
                //DateTime EndDate = DateTime.Now;


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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 공간의 재고수량 Return
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetLocationMaterial")]
        public async ValueTask<IActionResult> GetLocationMaterial([FromQuery]int materialid, [FromQuery]int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (materialid is 0)
                    return NoContent();

                ResponseList<InOutLocationDTO> model = await InStoreService.GetMaterialRoomNumService(HttpContext, materialid, buildingid);
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

        /// <summary>
        /// 출고 리스트에 담음 - FRONT 용
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/AddOutStoreList")]
        public async ValueTask<IActionResult> AddOutStoreList([FromQuery]int roomid, [FromQuery]int materialid, [FromQuery]int outcount)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (roomid is 0)
                    return NoContent();
                if(materialid is 0)
                    return NoContent();
                if(outcount is 0)
                    return NoContent();

                ResponseList<InOutInventoryDTO>? model = await InStoreService.AddOutStoreList(HttpContext, roomid, materialid, outcount);
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
