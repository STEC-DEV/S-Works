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
        /// 전체 사업장 리스트 조회 [OK]
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

                ResponseList<AllPlaceDTO> model = await AdminPlaceService.GetAllWorksService(HttpContext);
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
        /// 관리자정보 전체조회 [OK]
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

                ResponseList<ManagerListDTO> model = await AdminPlaceService.GetAllManagerListService();

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

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksService(adminid);

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
        /// <param name="placeid">사업장ID</param>
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

                ResponseUnit<PlaceDetailDTO> model = await AdminPlaceService.GetPlaceService(placeid);

                if (model is null)
                    return BadRequest(model);

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
        /// 사업장 등록
        /// </summary>
        /// <param name="dto">추가할 사업장정보 DTO</param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddWorks")]
        public async ValueTask<IActionResult> AddWorks([FromBody]AddPlaceDTO dto)
        {
            try
            {
                
                //AddPlaceDTO dto = new AddPlaceDTO();
                //dto.PlaceCd = "AB000000004"; // 사업장코드
                //dto.Name = "C사업장"; // 사업장명
                //dto.Tel = "02-0000-0000"; // 사업장 전화번호
                //dto.Address = "서울시 강서구"; // 사업장 주소
                //dto.ContractNum = "00054487"; // 계약번호
                //dto.ContractDT = DateTime.Now; // 계약일자
                //dto.PermMachine = true; // 설비메뉴 권한
                //dto.PermLift = true; // 승강메뉴 권한
                //dto.PermFire = true; // 소방메뉴 권한
                //dto.PermConstruct = true; // 건축메뉴 권한
                //dto.PermNetwork = true; // 통신메뉴 권한
                //dto.PermBeauty = false; // 미화메뉴 권한
                //dto.PermSecurity = false; // 보안메뉴 권한
                //dto.PermMaterial = false; // 자재메뉴 권한
                //dto.PermEnergy = false; // 에너지 메뉴 권한
                //dto.PermVoc = false; // VOC 권한
                //dto.Status = true; // 계약상태
                //dto.Note = "테스트데이터";
                

                if (String.IsNullOrWhiteSpace(dto.PlaceCd))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Tel))
                    return NoContent();

                if (dto.PermMachine == null)
                    return NoContent();

                if (dto.PermLift == null)
                    return NoContent();

                if (dto.PermFire == null)
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

                ResponseUnit<int?> model = await AdminPlaceService.AddPlaceService(HttpContext, dto);

                if (model is null)
                    return BadRequest();
                
                // 성공
                if (model.code == 200) 
                    return Ok(model);
                // 이미 해당 코드가 사용한 이력이 있을때
                else if (model.code == 202) 
                    return Ok(model);
                // 실패
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
                if (placeidx is null)
                    return NoContent();
                if (placeidx.Count == 0)
                    return NoContent();

                ResponseUnit<bool> model = await AdminPlaceService.DeletePlaceService(HttpContext, placeidx);

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
        public async ValueTask<IActionResult> UpdateWorks([FromBody]UpdatePlaceDTO dto)
        {
            try
            {
                /*
                UpdatePlaceDTO dto = new UpdatePlaceDTO();
                dto.PlaceInfo.Id = 3;
                dto.PlaceInfo.Name = "A수정사업장";
                dto.PlaceInfo.PlaceCd = "P001";
                dto.PlaceInfo.ContractNum = "ABC123";
                dto.PlaceInfo.Tel = "02-123-1234";
                dto.PlaceInfo.DepartmentID = 5;
                dto.PlaceInfo.Status = true;

                dto.PlacePerm.PermMachine = true;
                dto.PlacePerm.PermElec = true;
                dto.PlacePerm.PermLift = true;
                dto.PlacePerm.PermFire = true;
                dto.PlacePerm.PermConstruct = true;
                dto.PlacePerm.PermNetwork = true;
                dto.PlacePerm.PermBeauty = true;
                dto.PlacePerm.PermSecurity = true;
                dto.PlacePerm.PermMaterial = true;
                dto.PlacePerm.PermEnergy = true;
                dto.PlacePerm.PermVoc = true;
                */

                if (dto.PlaceInfo.Id is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.PlaceCd))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Tel))
                    return NoContent();

                if(dto.PlaceInfo.Status is null)
                    return NoContent();

                if(dto.PlacePerm.PermMachine is null)
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

                ResponseUnit<UpdatePlaceDTO> model = await AdminPlaceService.UpdatePlaceService(HttpContext, dto);

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

        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainManagerList")]
        public async ValueTask<IActionResult> NotContainManagerList([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ManagerListDTO> model = await AdminPlaceService.NotContainManagerList(HttpContext, placeid);

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
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainPlaceList")]
        public async ValueTask<IActionResult> NotContainPlaceList([FromQuery]int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.NotContainPlaceList(HttpContext, adminid);

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
        /// 사업장에 관리자 추가
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
                //AddPlaceManagerDTO<ManagerListDTO> placemanager = new AddPlaceManagerDTO<ManagerListDTO>();
                //placemanager.PlaceId = 3;
                //placemanager.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 15
                //});
                //placemanager.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 16
                //});

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

                ResponseUnit<bool?> model = await AdminPlaceService.AddPlaceManagerService(HttpContext, placemanager);

                if (model is null)
                    return BadRequest(model);

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
                //AddPlaceManagerDTO<ManagerListDTO> dto = new AddPlaceManagerDTO<ManagerListDTO>();
                //dto.PlaceId = 3;
                //dto.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 15
                //});
                //dto.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 16
                //});

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
