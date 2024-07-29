using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Maintenance;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        //private IMaintanceRepository MaintenenceRepository;
        private IMaintanceService MaintanceService;
        private ILogService LogService;


        public MaintenanceController(IMaintanceService _maintanceservice, ILogService _logservice)
        {
            this.MaintanceService = _maintanceservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/AddMaintenance")]
        public async ValueTask<IActionResult> AddMaintenence()
        {
            try
            {
                AddMaintanceDTO dto = new AddMaintanceDTO();
                dto.Name = "유지보수이력_1";
                dto.Type = 0;
                dto.Worker = "테스트";
                dto.UnitPrice = 500;
                dto.Num = 30;
                dto.TotalPrice = 30 * 500;
                dto.FacilityID = 1;

                dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                {
                    InOut = 0,
                    MaterialID = 5,
                    AddStore = new Shared.Server.DTO.Store.AddStoreDTO()
                    {
                        InOutDate = DateTime.Now,
                        RoomID = 1,
                        Num = 10,
                        UnitPrice = 100,
                        TotalPrice = 10 * 100,
                        Note = "출고등록"
                    }
                });

                dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                {
                    InOut = 0,
                    MaterialID = 6,
                    AddStore = new Shared.Server.DTO.Store.AddStoreDTO()
                    {
                        InOutDate = DateTime.Now,
                        RoomID = 2,
                        Num = 3,
                        UnitPrice = 200,
                        TotalPrice = 3 * 200,
                        Note = "출고등록"
                    }
                });

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

    }

}
