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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceRoomGroup")]
        public async Task<IActionResult> GetPlaceRoomGroup()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PlaceRoomListDTO>? model = await RoomService.GetPlaceAllGroupRoomInfo(HttpContext).ConfigureAwait(false);
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
        /// 룸 명칭 조회
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetRoomName")]
        public async Task<IActionResult> GetRoomName([FromQuery]int roomid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (roomid is 0)
                    return NoContent();

                ResponseUnit<string?> model = await RoomService.GetRoomNameService(HttpContext, roomid).ConfigureAwait(false);
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
        /// 공간정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddRoom")]
        public async Task<IActionResult> AddRoom([FromBody] RoomDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (dto.FloorID is null)
                    return NoContent();

                ResponseUnit<RoomDTO> model = await RoomService.AddRoomService(HttpContext, dto).ConfigureAwait(false);

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
        public async Task<IActionResult> GetAllRoomList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<RoomListDTO> model = await RoomService.GetRoomListService(HttpContext).ConfigureAwait(false);

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
        public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.RoomId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<bool?> model = await RoomService.UpdateRoomService(HttpContext, dto).ConfigureAwait(false);
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
        public async Task<IActionResult> DeleteRoom([FromBody]List<int> idx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                if (idx is null)
                    return NoContent();
                
                if (idx.Count() == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await RoomService.DeleteRoomService(HttpContext, idx).ConfigureAwait(false);
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
