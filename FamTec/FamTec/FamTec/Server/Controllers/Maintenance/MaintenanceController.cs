using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Maintenance;
using FamTec.Server.Services.UseMaintenence;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Maintenance
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintanceService MaintanceService;
        private readonly IUseMaintenenceService UseMaintenenceService;
                
        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ILogger<MaintenanceController> BuilderLogger;

        public MaintenanceController(IMaintanceService _maintanceservice,
            IUseMaintenenceService _usermaintenenceservice,
            IFileService _fileservice,
            ILogService _logservice,
            ILogger<MaintenanceController> _builderlogger)
        {
            this.MaintanceService = _maintanceservice;
            this.UseMaintenenceService = _usermaintenenceservice;
            
            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor(); // 색상 초기화
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/UpdateSupMaintenance")]
        //public async Task<IActionResult> UpdateSupMaintenance()
        public async Task<IActionResult> UpdateSupMaintenance([FromBody]UpdateMaintancematerialDTO dto)
        {
            try
            {
                //Logger.LogInformation(dto.ToString());
                //Logger.LogWarning("테스트-------------------------");
                //return Ok("asdgasdg");

                //UpdateMaintancematerialDTO dto = new UpdateMaintancematerialDTO();
                //dto.MaintanceID = 1;

                ////추가 출고
                //dto.UpdateUsematerialDTO.Add(new UpdateUseMaterialDTO
                //{
                //    UseID = 20,
                //    RoomID = 3,
                //    Num = 400,
                //    MaterialID = 1
                //});

                // 재입고
                //dto.UpdateUsematerialDTO.Add(new UpdateUseMaterialDTO
                //{ 
                //    UseID = 21,
                //    RoomID = 3,
                //    Num = 95,
                //    MaterialID = 1,
                //});

                //dto.UpdateUsematerialDTO.Add(new UpdateUseMaterialDTO
                //{
                //    UseID = 1,
                //    Num = 100,
                //});

                // 쌩출고
                //dto.UpdateUsematerialDTO.Add(new UpdateUseMaterialDTO
                //{
                //    Num = 100,
                //    RoomID = 3,
                //    MaterialID = 1,
                //});

                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.UpdateUsematerialDTO is [_, ..])
                {
                    foreach (var item in dto.UpdateUsematerialDTO)
                    {
                        if (item.RoomID is 0)
                            return NoContent();
                        if (item.MaterialID is 0)
                            return NoContent();
                    }
                }

                ResponseUnit<bool?> model = await UseMaintenenceService.UpdateUseMaintanceService(HttpContext, dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();
                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else if (model.code == 401)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 출고 - 추가출고
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        //[HttpGet]
        [Route("sign/AddSupMaintenance")]
        //public async Task<IActionResult> AddSupMaintenance()
        public async Task<IActionResult> AddSupMaintenance([FromBody]AddMaintanceMaterialDTO dto)
        {
            try
            {
                // 같은품목 + 같은공간 에는 한번만
                //AddMaintanceMaterialDTO dto = new AddMaintanceMaterialDTO();
                //dto.MaintanceID = 142;
                //dto.MaterialList = new List<MaterialDTO>();
                //dto.MaterialList.Add(new MaterialDTO
                //{
                //    MaterialID = 22,
                //    RoomID = 24,
                //    Num = 5,
                //});
                
                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is 0)
                    return NoContent();

                if (dto.MaterialList.Count == 0 || dto.MaterialList is null)
                    return NoContent();

                foreach (MaterialDTO MaterialInfo in dto.MaterialList)
                {
                    if (MaterialInfo.MaterialID is 0)
                        return NoContent();
                    if (MaterialInfo.Num is 0)
                        return NoContent();
                    if (MaterialInfo.RoomID is 0)
                        return NoContent();
                }

                bool hasDuplicates = dto.MaterialList
                .GroupBy(m => new { m.MaterialID, m.RoomID })
                .Any(g => g.Count() > 1);

                if (hasDuplicates)
                {
                    // MaterialID와 RoomID가 2개이상 있는지 검사
                    return BadRequest();
                }

                ResponseUnit<FailResult?> model = await MaintanceService.AddSupMaintanceService(HttpContext, dto).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
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
        public async Task<IActionResult> UpdateMaintenance([FromForm]UpdateMaintenanceDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                if (dto.MaintanceID == 0)
                    return BadRequest();
                    
                //UpdateMaintenanceDTO dto = new UpdateMaintenanceDTO();
                //dto.Id = 101;
                //dto.Name = "작업수정";
                //dto.Worker = "용용";

                if (files is not null)
                {
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

                ResponseUnit<bool?> model = await MaintanceService.UpdateMaintenanceService(HttpContext, dto, files).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaintenanceImage")]
        public async Task<IActionResult> AddMaintenanceImage([FromForm] int id, [FromForm] IFormFile? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (id is 0)
                    return BadRequest();

                if (files is not null)
                {
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

                ResponseUnit<bool?> model = await MaintanceService.AddMaintanceImageService(HttpContext, id, files).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
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
        //public async Task<IActionResult> AddMaintenence()
        public async Task<IActionResult> AddMaintenence([FromBody]AddMaintenanceDTO dto)
        {
            try
            {
                // DTO의 Inventory에 역직렬화된 데이터를 할당
                //AddMaintenanceDTO dto = new AddMaintenanceDTO();
                //dto.Name = "유지보수이력_1";
                //dto.Type = 0;
                //dto.Worker = "테스트";
                ////dto.TotalPrice = 30 * 500;
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
                //        Num = 2,
                //        Note = "출고등록"
                //    }
                //});
                //dto.Inventory.Add(new Shared.Server.DTO.Store.InOutInventoryDTO
                //{
                //    InOut = 0,
                //    MaterialID = 10,
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

                // 자체 작업
                if (dto.Type == 0) 
                {
                    if (dto.Inventory is null || !dto.Inventory.Any())
                        return NoContent();

                    bool hasDuplicates = dto.Inventory
                    .GroupBy(i => new { i.MaterialID, RoomID = i.AddStore?.RoomID })
                    .Any(g => g.Count() > 1);
                
                    if (hasDuplicates)
                    {
                        // MaterialID와 RoomID가 2개이상 있는지 검사
                        return BadRequest();
                    }
                }

                ResponseUnit<FailResult?> model = await MaintanceService.AddMaintanceService(HttpContext, dto).ConfigureAwait(false);
                
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
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
        public async Task<IActionResult> GetMaintanceHistory([FromQuery]int facilityid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaintanceListDTO> model = await MaintanceService.GetMaintanceHistoryService(HttpContext, facilityid).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetDetailMaintance")]
        public async Task<IActionResult> GetDetailMaintance([FromQuery]int Maintanceid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<DetailMaintanceDTO?> model = await MaintanceService.GetDetailService(HttpContext, Maintanceid).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpGet]
        [HttpPost]
        [Route("sign/DeleteMaintenanceList")]
        //public async Task<IActionResult> DeleteMaintenanceList()
        public async Task<IActionResult> DeleteMaintenanceList([FromBody] DeleteMaintanceDTO2 dto)
        {
            try
            {
                //DeleteMaintanceDTO2 dto = new DeleteMaintanceDTO2();
                //dto.Note = "테스트 유지보수삭제_20240913";
                //dto.MaintanceID.Add(100);
                
                if (HttpContext is null)
                    return BadRequest();

                if (dto.MaintanceID is null || !dto.MaintanceID.Any())
                    return NoContent();

                ResponseUnit<bool?> model = await MaintanceService.DeleteMaintenanceRecordService(HttpContext, dto).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
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
        //public async Task<IActionResult> DeleteMaintanceHistory()
        public async Task<IActionResult> DeleteMaintenanceStore([FromBody]DeleteMaintanceDTO delInfo)
        {
            try
            {
                //DeleteMaintanceDTO delInfo = new DeleteMaintanceDTO();
                //delInfo.MaintanceID = 113;
                //delInfo.Note = "출고취소_테스트2";
                //delInfo.UseMaintenenceIDs.Add(62);
                //delInfo.UseMaintenenceIDs.Add(63);


                if (HttpContext is null)
                    return BadRequest();
                
                if (delInfo.MaintanceID is 0)
                    return NoContent();
                if(!delInfo.UseMaintenenceIDs.Any())
                    return NoContent();


                ResponseUnit<bool?> model = await MaintanceService.DeleteMaintenanceStoreRecordService(HttpContext, delInfo).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
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
        //public async Task<IActionResult> GetDateHistoryList()
        public async Task<IActionResult> GetDateHistoryList([FromQuery]DateTime StartDate, [FromQuery]DateTime EndDate, [FromQuery]List<string> category, [FromQuery]List<int> type)
        {
            try
            {
                //DateTime StartDate = DateTime.Now.AddDays(-30);
                //DateTime EndDate = DateTime.Now;
                //List<string> category = new List<string>() { "기계", "전기", "승강", "소방", "건축", "통신", "미화","보안","기타" };
                //List<int> type = new List<int>() { 0,1}; // 전체

                if (HttpContext is null)
                    return BadRequest();

                if (category is null || !category.Any())
                    return NoContent();

                if (type is null || !type.Any())
                    return NoContent();

                category.ForEach(s => s = s.Trim());

                ResponseList<MaintanceHistoryDTO>? model = await MaintanceService.GetDateHisotryList(HttpContext, StartDate, EndDate, category, type).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 401)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
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
        //public async Task<IActionResult> GetAllHistoryList()
        public async Task<IActionResult> GetAllHistoryList([FromQuery]List<string> category, [FromQuery]List<int> type)
        {
            try
            {
                //List<string> category = new List<string>() { "기계", "전기", "승강", "소방", "건축", "통신", "미화", "보안", "기타" };
                //List<int> type = new List<int>() {  0,1 };
                
                if (HttpContext is null)
                    return BadRequest();

                if (category is null || !category.Any())
                    return NoContent();

                if (type is null || !type.Any())
                    return NoContent();

                category.ForEach(s => s = s.Trim());

                ResponseList<AllMaintanceHistoryDTO>? model = await MaintanceService.GetAllHistoryList(HttpContext, category, type).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 401)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return Problem("서버에서 처리할 수 없는 작업입니다", statusCode: 500);
            }
        }

    }
}