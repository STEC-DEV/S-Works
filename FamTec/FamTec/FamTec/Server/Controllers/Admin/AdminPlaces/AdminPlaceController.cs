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
                ResponseList<AllPlaceDTO>? model = await AdminPlaceService.GetAllWorksService(HttpContext);
                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return Ok(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
                ResponseList<ManagerListDTO>? model = await AdminPlaceService.GetAllManagerListService();

                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
                ResponseList<AdminPlaceDTO>? model = await AdminPlaceService.GetMyWorksService(adminid);

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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
                ResponseUnit<PlaceDetailDTO>? model = await AdminPlaceService.GetPlaceService(placeid);

                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
                ResponseUnit<int?> model = await AdminPlaceService.AddPlaceService(HttpContext, dto);

                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
            ResponseUnit<bool>? model = await AdminPlaceService.DeletePlaceService(HttpContext, placeidx);

            if(model is not null)
            {
                if(model.code == 200)
                {
                    return Ok(model);
                }
                else if(model.code == 204)
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

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateWorks")]
        public async ValueTask<IActionResult> UpdateWorks([FromBody]UpdatePlaceDTO dto)
        {
            ResponseUnit<UpdatePlaceDTO>? model = await AdminPlaceService.UpdatePlaceService(HttpContext, dto);

            if (model is not null)
            {
                if (model.code == 200)
                {
                    return Ok(model);
                }
                else if (model.code == 204)
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

        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainManagerList")]
        public async ValueTask<IActionResult> NotContainManagerList([FromQuery]int placeid)
        {
            ResponseList<ManagerListDTO>? model = await AdminPlaceService.NotContainManagerList(HttpContext, placeid);

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
                ResponseUnit<bool>? model = await AdminPlaceService.AddPlaceManagerService(HttpContext, placemanager);

                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else if(model.code == 401)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
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
        public async ValueTask<IActionResult> DeleteWorks([FromBody] AddPlaceManagerDTO<ManagerListDTO>? dto)
        {
            try
            {
                ResponseUnit<int?> model = await AdminPlaceService.DeleteManagerPlaceService(HttpContext, dto);

                if (model is not null)
                {
                    if (model.code == 200)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest(model);
                    }
                }
                else
                {
                    return BadRequest(model);
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return StatusCode(500);
            }
        }

      

        

    }
}
