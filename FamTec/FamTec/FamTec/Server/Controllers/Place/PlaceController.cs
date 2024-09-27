using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Place;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Place
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private IAdminPlaceService AdminPlaceService;
        private ILogService LogService;

        public PlaceController(IAdminPlaceService _adminplaceservice,
            ILogService _logservice)
        {
            this.AdminPlaceService = _adminplaceservice;
            this.LogService = _logservice;
        }

        [HttpGet]
        [Route("GetPlaceName")]
        public async Task<IActionResult> GetPlaceName([FromQuery]int placeid)
        {
            try
            {
                if (placeid is 0)
                    return NoContent();

                ResponseUnit<string?> model = await AdminPlaceService.GetPlaceName(placeid).ConfigureAwait(false);
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
