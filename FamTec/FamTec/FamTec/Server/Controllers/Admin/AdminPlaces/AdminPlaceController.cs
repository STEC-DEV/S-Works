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
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPlaceController : ControllerBase
    {
        private IAdminPlaceService AdminPlaceService;
        private ILogService LogService;


        public AdminPlaceController(IAdminPlaceService _adminplaceservice,
            ILogService _logservice)
        {
            this.AdminPlaceService = _adminplaceservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 전체 사업장 리스트 조회
        /// [매니저는 본인이 할당된 것 만 출력]
        /// [토큰 적용완료]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllWorksList")]
        public async ValueTask<IActionResult> GetAllWorksList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllPlaceDTO>? model = await AdminPlaceService.GetAllWorksService(HttpContext);
                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return Ok(model);

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 매니저리스트 전체 반환 [수정완료]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllManagerList")]
        public async ValueTask<IActionResult> GetAllManagerList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ManagerListDTO>? model = await AdminPlaceService.GetAllManagerListService();

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
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
        public async ValueTask<IActionResult> GetMyWorks([FromQuery] int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO>? model = await AdminPlaceService.GetMyWorksService(adminid);

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
        /// 사업장 상세정보
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/DetailWorks")]
        public async ValueTask<IActionResult> DetailWorks([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<PlaceDetailDTO>? model = await AdminPlaceService.GetPlaceService(placeid);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 생성
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddWorks")]
        public async ValueTask<IActionResult> AddWorks([FromBody]AddPlaceDTO dto)
        {
            try
            {
                /*
                AddPlaceDTO dto = new AddPlaceDTO();
                dto.PlaceCd = "AB000000002"; // 사업장코드
                dto.Name = "B사업장"; // 사업장명
                dto.Tel = "02-0000-0000"; // 사업장 전화번호
                dto.Address = "서울시 강서구"; // 사업장 주소
                dto.ContractNum = "00054487"; // 계약번호
                dto.ContractDT = DateTime.Now; // 계약일자
                dto.PermMachine = true; // 설비메뉴 권한
                dto.PermLift = true; // 승강메뉴 권한
                dto.PermFire = true; // 소방메뉴 권한
                dto.PermConstruct = true; // 건축메뉴 권한
                dto.PermNetwork = true; // 통신메뉴 권한
                dto.PermBeauty = false; // 미화메뉴 권한
                dto.PermSecurity = false; // 보안메뉴 권한
                dto.PermMaterial = false; // 자재메뉴 권한
                dto.PermEnergy = false; // 에너지 메뉴 권한
                dto.PermVoc = false; // VOC 권한
                dto.Status = true; // 계약상태
                dto.Note = "테스트데이터";
                */
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminPlaceService.AddPlaceService(HttpContext, dto);

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
        public async ValueTask<IActionResult> DeleteWorks([FromBody]List<int> placeidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool>? model = await AdminPlaceService.DeletePlaceService(HttpContext, placeidx);

                if (model is null)
                    return BadRequest();

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateWorks")]
        public async ValueTask<IActionResult> UpdateWorks([FromBody]UpdatePlaceDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UpdatePlaceDTO>? model = await AdminPlaceService.UpdatePlaceService(HttpContext, dto);

                if (model is null)
                    return BadRequest();

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainManagerList")]
        public async ValueTask<IActionResult> NotContainManagerList([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ManagerListDTO>? model = await AdminPlaceService.NotContainManagerList(HttpContext, placeid);

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

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainPlaceList")]
        public async ValueTask<IActionResult> NotContainPlaceList([FromQuery]int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO>? model = await AdminPlaceService.NotContainPlaceList(HttpContext, adminid);

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
        /// 사업장에 매니저 할당
        /// </summary>
        /// <param name="placemanager"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddPlaceManager")]
        public async ValueTask<IActionResult> AddPlaceManager([FromBody]AddPlaceManagerDTO<ManagerListDTO> placemanager)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool>? model = await AdminPlaceService.AddPlaceManagerService(HttpContext, placemanager);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 401)
                    return Ok(model);
                else
                    return BadRequest(model);

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
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
        public async ValueTask<IActionResult> DeleteWorks([FromBody] AddPlaceManagerDTO<ManagerListDTO> dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.DeleteManagerPlaceService(HttpContext, dto);

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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

      

        

    }
}
