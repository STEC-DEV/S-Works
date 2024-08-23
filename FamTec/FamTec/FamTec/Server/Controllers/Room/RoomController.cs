using FamTec.Server.Services;
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
        private ILogService LogService;

        public RoomController(IRoomService _roomservice,
            ILogService _logservice)
        {
            this.RoomService = _roomservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 공간정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddRoom")]
        public async ValueTask<IActionResult> AddRoom([FromBody] RoomDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.FloorID is null)
                    return NoContent();

                ResponseUnit<RoomDTO> model = await RoomService.AddRoomService(HttpContext, dto);

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
        /// 공간정보 전체조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllRoomList")]
        public async ValueTask<IActionResult> GetAllRoomList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<RoomListDTO> model = await RoomService.GetRoomListService(HttpContext);

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
        /// 공간정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateRoom")]
        public async ValueTask<IActionResult> UpdateRoom([FromBody] UpdateRoomDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.RoomId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<bool?> model = await RoomService.UpdateRoomService(HttpContext, dto);
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
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/DeleteRoom")]
        public async ValueTask<IActionResult> DeleteRoom([FromBody]List<int> idx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                if (idx is null)
                    return NoContent();
                
                if (idx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await RoomService.DeleteRoomService(HttpContext, idx);
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
