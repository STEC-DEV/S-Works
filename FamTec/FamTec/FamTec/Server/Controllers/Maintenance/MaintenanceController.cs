using FamTec.Server.Repository.Maintenence;
using FamTec.Server.Services;
using FamTec.Server.Services.Maintenance;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FamTec.Server.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private IMaintanceService MaintanceService;
        private ILogService LogService;

        public MaintenanceController(IMaintanceService _maintanceservice,
            ILogService _logservice)
        {
            this.MaintanceService = _maintanceservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 유지보수 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/AddMaintenance")]
        //public async ValueTask<IActionResult> AddMaintenence()
        public async ValueTask<IActionResult> AddMaintenence([FromBody]AddMaintanceDTO dto)
        {
            try
            {
                //AddMaintanceDTO dto = new AddMaintanceDTO();
                //dto.Name = "유지보수이력_1";
                //dto.Type = 0;
                //dto.Worker = "테스트";
                //dto.UnitPrice = 500;
                //dto.Num = 30;
                //dto.TotalPrice = 30 * 500;
                //dto.FacilityID = 1;
                
                //dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                //{
                //    InOut = 0,
                //    MaterialID = 10,
                //    AddStore = new Shared.Server.DTO.Store.AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now,
                //        RoomID = 2,
                //        Num = 10,
                //        UnitPrice = 100,
                //        TotalPrice = 10 * 100,
                //        Note = "출고등록"
                //    }
                //});
                
                //dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                //{
                //    InOut = 0,
                //    MaterialID = 11,
                //    AddStore = new Shared.Server.DTO.Store.AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now,
                //        RoomID = 3,
                //        Num = 3,
                //        UnitPrice = 200,
                //        TotalPrice = 3 * 200,
                //        Note = "출고등록"
                //    }
                //});
                
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                if (dto.UnitPrice is null)
                    return NoContent();

                if(dto.Num is null)
                    return NoContent();



                ResponseUnit<bool?> model = await MaintanceService.AddMaintanceService(HttpContext, dto);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetMaintanceHistory")]
        public async ValueTask<IActionResult> GetMaintanceHistory(int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaintanceListDTO> model = await MaintanceService.GetMaintanceHistoryService(HttpContext, facilityid);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }

        }

        /// <summary>
        /// 유지보수 이력 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteMaintanceHistory")]
        public async ValueTask<IActionResult> DeleteMaintanceHistory([FromBody]DeleteMaintanceDTO dto)
        {
            try
            {
                //DeleteMaintanceDTO dto = new DeleteMaintanceDTO();
                //dto.ID = 65;
                //dto.StoreID = 472;
                //dto.RoomTBID = 1;
                //dto.PlaceTBID = 3;
                //dto.MaterialTBID = 5;
                //dto.Note = "테스트";

                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null)
                    return NoContent();

                if (dto.StoreID is null)
                    return NoContent();

                if (dto.RoomTBID is null)
                    return NoContent();

                if (dto.PlaceTBID is null)
                    return NoContent();

                if(dto.MaterialTBID is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Note))
                    return NoContent();

                ResponseUnit<bool?> model = await MaintanceService.DeletemaintanceHistoryService(HttpContext, dto);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDateHistoryList")]
        public async ValueTask<IActionResult> GetDateHistoryList([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate, [FromQuery]string category, [FromQuery]int type)
        {
            try
            {
                //DateTime StartDate = DateTime.Now.AddDays(-30);
                //DateTime EndDate = DateTime.Now;
                //int Category = "전체"; // 전체
                //int type = 0; // 전체

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaintanceHistoryDTO>? model = await MaintanceService.GetDateHisotryList(HttpContext, StartDate, EndDate, category, type);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 이력 사업장별 전체
        /// </summary>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllHistoryList")]

        public async ValueTask<IActionResult> GetAllHistoryList([FromQuery]string category, [FromQuery]int type)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                ResponseList<AllMaintanceHistoryDTO>? model = await MaintanceService.GetAllHistoryList(HttpContext, "전체", 0);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

    }
}