using DocumentFormat.OpenXml.Spreadsheet;
using FamTec.Server.Services.Room;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Room
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IRoomService RoomService;
      

        public RoomController(IRoomService _roomservice)
        {
            RoomService = _roomservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddRoom")]
        public async ValueTask<IActionResult> AddRoom([FromBody] RoomDTO dto)
        {
            ResponseUnit<RoomDTO>? model = await RoomService.AddRoomService(HttpContext, dto);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllRoomList")]
        public async ValueTask<IActionResult> GetAllRoomList()
        {
            ResponseList<RoomListDTO>? model = await RoomService.GetRoomListService(HttpContext);

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/UpdateRoom")]
        public async ValueTask<IActionResult> UpdateRoom([FromBody] UpdateRoomDTO? dto)
        {
            ResponseUnit<bool?> model = await RoomService.UpdateRoomService(HttpContext, dto);
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

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/DeleteRoom")]
        public async ValueTask<IActionResult> DeleteRoom([FromBody]List<int> idx)
        {
            ResponseUnit<int?> model = await RoomService.DeleteRoomService(HttpContext, idx);
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



    }
}
