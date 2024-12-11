using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Place;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Admin.AdminPlaces
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPlaceController : ControllerBase
    {
        private readonly IAdminPlaceService AdminPlaceService;
        private readonly ILogService LogService;

        // 콘솔로그
        private readonly ConsoleLogService<AdminPlaceController> CreateBuilderLogger;

        public AdminPlaceController(IAdminPlaceService _adminplaceservice,
            ILogService _logservice,
            ConsoleLogService<AdminPlaceController> _createbuilderlogger)
        {
            this.AdminPlaceService = _adminplaceservice;
            
            this.LogService = _logservice;
            // 콘솔로그
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 관리자 화면 가이드 PDF 다운로드
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/DownloadAdminGuideForm")]
        public async Task<IActionResult> DownloadAdminGuideForm()
        {
            try
            {
                byte[]? fileBytes = await AdminPlaceService.DownloadAdminGuidForm(HttpContext);

                if (fileBytes is not null)
                    return File(fileBytes, "application/pdf", "S-Works_관리자설명서_1.3_KO_241211.pdf");
                else
                    return Ok();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 전체 사업장 리스트 조회 [OK]
        /// [매니저는 본인이 할당된 것 만 출력]
        /// [토큰 적용완료]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllWorksList")]
        public async Task<IActionResult> GetAllWorksList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllPlaceDTO> model = await AdminPlaceService.GetAllWorksService(HttpContext).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);

                else
                    return Ok(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 관리자정보 전체조회 [OK]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllManagerList")]
        public async Task<IActionResult> GetAllManagerList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ManagerListDTO> model = await AdminPlaceService.GetAllManagerListService().ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 선택된 매니저가 관리하는 사업장 LIST반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/MyWorks")]
        public async Task<IActionResult> GetMyWorks([FromQuery] int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksService(adminid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }

        }

        /// <summary>
        /// 사업장 상세정보
        /// </summary>
        /// <param name="placeid">사업장ID</param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/DetailWorks")]
        public async Task<IActionResult> DetailWorks([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<PlaceDetailDTO> model = await AdminPlaceService.GetPlaceService(placeid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 등록
        /// </summary>
        /// <param name="dto">추가할 사업장정보 DTO</param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddWorks")]
        public async Task<IActionResult> AddWorks([FromBody]AddPlaceDTO dto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Tel))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.ContractNum))
                    return NoContent();

                if (dto.PermMachine == null)
                    return NoContent();

                if (dto.PermLift == null)
                    return NoContent();

                if (dto.PermFire == null)
                    return NoContent();

                if (dto.PermElec == null)
                    return NoContent();

                if (dto.PermConstruct == null)
                    return NoContent();

                if (dto.PermNetwork == null)
                    return NoContent();

                if (dto.PermBeauty == null)
                    return NoContent();

                if (dto.PermSecurity == null)
                    return NoContent();

                if (dto.PermMaterial == null)
                    return NoContent();

                if (dto.PermEnergy == null)
                    return NoContent();

                if (dto.PermVoc == null)
                    return NoContent();
                

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminPlaceService.AddPlaceService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                // 성공
                if (model.code == 200) 
                    return Ok(model);
                // 이미 해당 계약번호가 사용되었을경우
                else if (model.code == 202) 
                    return Ok(model);
                // 실패
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        //[HttpGet]
        [Route("sign/UpdatePlaceManager")]
        //public async Task<IActionResult> UpdatePlaceManager()
        public async Task<IActionResult> UpdatePlaceManager([FromBody]UpdatePlaceManagerDTO dto)
        {
            try
            {
                //UpdatePlaceManagerDTO dto = new UpdatePlaceManagerDTO();
                //dto.PlaceId = 1;
                //dto.AdminId.Add(1);
                //dto.AdminId.Add(3);
                //dto.AdminId.Add(4);

                if (dto.PlaceId is 0)
                    return NoContent();

                ResponseUnit<bool?> model = await AdminPlaceService.UpdatePlaceManagerService(HttpContext, dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }
        

        /// <summary>
        /// 사업장 삭제
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeleteWorks")]
        
        public async Task<IActionResult> DeleteWorks([FromBody]List<int> placeidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                if (placeidx is null)
                    return NoContent();
                if (placeidx.Count == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await AdminPlaceService.DeletePlaceService(HttpContext, placeidx).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        
        /// <summary>
        /// 사업장 정보 수정
        /// </summary>
        /// <param name="dto">
        ///     수정할 DTO
        ///         - dto.PlaceInfo.Id  * 필수값
        ///         - dto.PlaceInfo.PlaceCd * 필수값
        ///         - dto.PlaceInfo.Name * 필수값
        ///         - dto.PlaceInfo.Tel * 필수값
        ///         - dto.PlaceInfo.Status * 계약상태
        ///         - dto.PlaceInfo.DepartmentID * 부서ID
        ///         
        ///         - dto.PlacePerm * 필수값
        /// </param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateWorks")]
        public async Task<IActionResult> UpdateWorks([FromBody]UpdatePlaceDTO dto)
        {
            try
            {
                if (dto.PlaceInfo.Id is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Tel))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.ContractNum))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.PlaceInfo.ContractNum))
                    return NoContent();

                if (dto.PlaceInfo.Status is null)
                    return NoContent();

                if (dto.PlacePerm.PermMachine is null)
                    return NoContent();

                if(dto.PlacePerm.PermElec is null)
                    return NoContent();

                if(dto.PlacePerm.PermLift is null)
                    return NoContent();

                if(dto.PlacePerm.PermFire is null)
                    return NoContent();

                if(dto.PlacePerm.PermConstruct is null)
                    return NoContent();

                if(dto.PlacePerm.PermNetwork is null)
                    return NoContent();

                if(dto.PlacePerm.PermBeauty is null)
                    return NoContent();

                if(dto.PlacePerm.PermSecurity is null)
                    return NoContent();

                if(dto.PlacePerm.PermMaterial is null)
                    return NoContent();

                if(dto.PlacePerm.PermEnergy is null)
                    return NoContent();

                if(dto.PlacePerm.PermVoc is null)
                    return NoContent();

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UpdatePlaceDTO> model = await AdminPlaceService.UpdatePlaceService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainManagerList")]
        public async Task<IActionResult> NotContainManagerList([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (placeid is 0)
                    return NoContent();

                ResponseList<ManagerListDTO> model = await AdminPlaceService.NotContainManagerList(HttpContext, placeid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainPlaceList")]
        public async Task<IActionResult> NotContainPlaceList([FromQuery]int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (adminid is 0)
                    return NoContent();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.NotContainPlaceList(HttpContext, adminid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 관리자 추가
        /// </summary>
        /// <param name="placemanager"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddPlaceManager")]
        public async Task<IActionResult> AddPlaceManager([FromBody]AddPlaceManagerDTO<ManagerListDTO> placemanager)
        {
            try
            {
                if(placemanager.PlaceId is null)
                    return NoContent();
                
                if(placemanager.PlaceManager is null)
                    return NoContent();
                
                foreach (ManagerListDTO ManagerInfo in placemanager.PlaceManager)
                {
                    if(ManagerInfo.Id is null)
                        return NoContent();
                }

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.AddPlaceManagerService(HttpContext, placemanager).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");

#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 202) // 이미 포함되어있는 관리자
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에서 관리자 삭제
        /// </summary>
        /// <param name="DeletePlace"></param>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeletePlaceManager")]
        public async Task<IActionResult> DeleteWorks([FromBody] AddPlaceManagerDTO<ManagerListDTO> dto)
        {
            try
            {
                if (dto.PlaceId is null)
                    return NoContent();
                
                if(dto.PlaceManager is null)
                    return NoContent();
                
                foreach (ManagerListDTO ManagerList in dto.PlaceManager)
                {
                    if(ManagerList.Id is null)
                        return NoContent();
                }

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.DeleteManagerPlaceService(HttpContext, dto).ConfigureAwait(false);

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model is null)
                    return BadRequest(model);
                
                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
