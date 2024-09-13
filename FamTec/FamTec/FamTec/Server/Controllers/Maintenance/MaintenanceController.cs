using FamTec.Server.Repository.Maintenence;
using FamTec.Server.Services;
using FamTec.Server.Services.Maintenance;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private IMaintanceService MaintanceService;
        private IFileService FileService;
        private ILogService LogService;
        private IMaintanceRepository MaintanceRepository;

        public MaintenanceController(IMaintanceService _maintanceservice,
            IMaintanceRepository _maintancerepository,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MaintanceService = _maintanceservice;
            this.MaintanceRepository = _maintancerepository;
            
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/temp2")]
        public async ValueTask<IActionResult> Temp2()
        {
            // 같은품목 + 같은공간 에는 한번만
            AddMaintanceMaterialDTO dto = new AddMaintanceMaterialDTO();
            dto.MaintanceID = 100;
            dto.MaterialList = new List<MaterialDTO>();
            dto.MaterialList.Add(new MaterialDTO
            {
                MaterialID = 11,
                Num = 145,
                RoomID = 3,
                UnitPrice = 200, // 프론트에서 계산해줘야함.
                TotalPrice = 2000 // 프론트에서 계산해줘야함.
            });
            dto.MaterialList.Add(new MaterialDTO
            {
                MaterialID = 10,
                Num = 5,
                RoomID = 3,
                UnitPrice = 200,
                TotalPrice = 1000
            });
            var temp = await MaintanceRepository.AddMaintanceMaterialAsync(dto, "용", 3);
            return Ok(temp);
        }

        /// <summary>
        /// 잠시보류
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/temp")]
        public async ValueTask<IActionResult> Temp()
        {
            UpdateMaintenanceMaterialDTO dto = new UpdateMaintenanceMaterialDTO()
            {
                MaterialID = 10,
                RoomID = 2,
                UseMaintanceID = 28,
                MaintanceID = 100,
                Num = 38,
                UnitPrice = 100,
                TotalPrice = 10000,
                Note = "테스트"
            };
        
            var temp = await MaintanceRepository.UpdateMaintenanceUseRecord(dto, 3, "용");
            return Ok(temp);
        }

        /// <summary>
        /// 유지보수 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpGet]
        [HttpPut]
        [Route("sign/UpdateMaintenance")]
        //public async ValueTask<IActionResult> UpdateMaintenance([FromForm] IFormFile? files)
        public async ValueTask<IActionResult> UpdateMaintenance([FromForm]UpdateMaintenanceDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (dto.Id == 0)
                    return BadRequest();
                    
                //UpdateMaintenanceDTO dto = new UpdateMaintenanceDTO();
                //dto.Id = 101;
                //dto.Name = "작업수정";
                //dto.Worker = "용용";

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await MaintanceService.UpdateMaintenanceService(HttpContext, dto, files);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaintenanceImage")]
        public async ValueTask<IActionResult> AddMaintenanceImage([FromForm] int id, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (id is 0)
                    return BadRequest();

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<bool?> model = await MaintanceService.AddMaintanceImageService(HttpContext, id, files);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
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
        public async ValueTask<IActionResult> AddMaintenence([FromBody]AddMaintenanceDTO dto)
        {
            try
            {
                // DTO의 Inventory에 역직렬화된 데이터를 할당
                //AddMaintenanceDTO dto = new AddMaintenanceDTO();
                //dto.Name = "유지보수이력_1";
                //dto.Type = 1;
                //dto.Worker = "테스트";
                //dto.TotalPrice = 30 * 500;
                //dto.FacilityID = 1;
                //dto.WorkDT = DateTime.Now;

                //dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                //{
                //    InOut = 0,
                //    MaterialID = 10,
                //    AddStore = new Shared.Server.DTO.Store.AddStoreDTO()
                //    {
                //        InOutDate = DateTime.Now,
                //        RoomID = 2,
                //        Num = 21,
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
                //        Num = 2,
                //        Note = "출고등록"
                //    }
                //});

                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.Worker))
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                if(dto.FacilityID == 0)
                    return NoContent();

                if (dto.Inventory is null || !dto.Inventory.Any())
                    return NoContent();

                ResponseUnit<FailResult?> model = await MaintanceService.AddMaintanceService(HttpContext, dto);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 422)
                    return Ok(model);
                else if (model.code == 409)
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
        public async ValueTask<IActionResult> GetMaintanceHistory([FromQuery]int facilityid)
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailMaintance")]
        public async ValueTask<IActionResult> GetDetailMaintance([FromQuery]int Maintanceid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailMaintanceDTO?> model = await MaintanceService.GetDetailService(HttpContext, Maintanceid);
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
        /// 유지보수 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        //[HttpPost]
        [Route("sign/DeleteMaintenanceList")]
        public async ValueTask<IActionResult> DeleteMaintenanceList()
        //public async ValueTask<IActionResult> DeleteMaintenanceList([FromBody] DeleteMaintanceDTO2 dto)
        {
            try
            {
                DeleteMaintanceDTO2 dto = new DeleteMaintanceDTO2();
                dto.Note = "테스트 유지보수삭제_20240913";
                dto.MaintanceID.Add(100);
                
                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is null || !dto.MaintanceID.Any())
                    return NoContent();

                ResponseUnit<bool?> model = await MaintanceService.DeleteMaintenanceRecordService(HttpContext, dto);
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
        /// 유지보수 내용 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/DeleteMaintenanceStore")]
        //public async ValueTask<IActionResult> DeleteMaintanceHistory()
        public async ValueTask<IActionResult> DeleteMaintenanceStore([FromBody]List<DeleteMaintanceDTO> DeleteList)
        {
            try
            {
                //List<DeleteMaintanceDTO> DeleteList = new List<DeleteMaintanceDTO>();
                //DeleteList.Add(new DeleteMaintanceDTO
                //{
                //    MaintanceID = 100,
                //    UseMaintenenceID = 7,
                //    Note = "출고취소_테스트1"
                //});

            

                if (HttpContext is null)
                    return BadRequest();
                foreach(DeleteMaintanceDTO dto in DeleteList)
                {
                    if (dto.MaintanceID is null)
                        return NoContent();

                    if (String.IsNullOrWhiteSpace(dto.Note))
                        return NoContent();
                }

                ResponseUnit<bool?> model = await MaintanceService.DeleteMaintenanceStoreRecordService(HttpContext, DeleteList);
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
        public async ValueTask<IActionResult> GetDateHistoryList()
        //public async ValueTask<IActionResult> GetDateHistoryList([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate, [FromQuery]string category, [FromQuery]int type)
        {
            try
            {
                DateTime StartDate = DateTime.Now.AddDays(-30);
                DateTime EndDate = DateTime.Now;
                string Category = "전체"; // 전체
                int type = 0; // 전체

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaintanceHistoryDTO>? model = await MaintanceService.GetDateHisotryList(HttpContext, StartDate, EndDate, Category, type);
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

                if (String.IsNullOrWhiteSpace(category))
                    return NoContent();

                ResponseList<AllMaintanceHistoryDTO>? model = await MaintanceService.GetAllHistoryList(HttpContext, category, 0);
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