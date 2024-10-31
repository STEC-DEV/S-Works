using FamTec.Server.Middleware;
using FamTec.Server.Repository.Voc;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Voc
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class VocController : ControllerBase
    {
        private readonly IVocService VocService;
        private readonly IVocInfoRepository VocInfoRepository;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<VocController> CreateBuilderLogger;

        public VocController(IVocService _vocservice,
            ILogService _logservice,
            IVocInfoRepository _vocinforepository,
            ConsoleLogService<VocController> _createbuilderlogger)
        {
            this.VocService = _vocservice;
            this.VocInfoRepository = _vocinforepository;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocWeekCount")]
        public async Task<IActionResult> GetVocWeekCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<VocWeekCountDTO>? model = await VocService.GetVocDashBoardDataService(HttpContext).ConfigureAwait(false);
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
        /// 사업장 민원 전체보기 - 직원용 (월간)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocList")]
        //public async Task<IActionResult> GetVocList()
        public async Task<IActionResult> GetVocList([FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid)
        {
            try
            {
                //List<int> typesArray = type.Split(',').Select(int.Parse).ToList();
                //List<int> type = new List<int>() { 0,1, 7 };
                //List<int> status = new List<int>() { 0,1, 2 };
                //List<int> buildingid = new List<int>() { 1 };
                //List<int> division = new List<int>() { 1 };

                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllVocListDTO> model = await VocService.GetVocList(HttpContext, type, status, buildingid).ConfigureAwait(false);
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
        /// 사업장 민원 필터 전체보기 - 직원용 (기간)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocFilterList")]
        public async Task<IActionResult> GetVocFilterList([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate, [FromQuery] List<int> type, [FromQuery] List<int> status, [FromQuery] List<int> buildingid, [FromQuery] List<int> division)
        {
            try
            {
                //DateTime StartDate = new DateTime(2024,8,1);
                //DateTime EndDate = new DateTime(2024,8,8);
                //List<int> type = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
                //List<int> status = new List<int>() { 0, 1, 2 };
                //List<int> buildingid = new List<int>() { 1 };

                if (HttpContext is null)
                   return BadRequest();

                if (type is null)
                    return NoContent();
                if (type.Count == 0)
                    return NoContent();

                if (status is null)
                    return NoContent();
                if (status.Count == 0)
                    return NoContent();

                if (buildingid is null)
                    return NoContent();
                if (buildingid.Count == 0)
                    return NoContent();

                ResponseList<VocListDTO>? model = await VocService.GetVocFilterList(HttpContext, StartDate, EndDate, type, status, buildingid).ConfigureAwait(false);

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
        /// 민원 상세보기 - 직원용
        /// </summary>
        /// <param name="VocId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/VocInfo")]
        public async Task<IActionResult> GetDetailVoc([FromQuery] int VocId)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocEmployeeDetailDTO> model = await VocService.GetVocDetail(HttpContext, VocId).ConfigureAwait(false);

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
        /// 민원타입 변경
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateVocType")]
        public async Task<IActionResult> UpdateVocType([FromBody]UpdateVocDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.VocID is null)
                    return NoContent();

                if (dto.Type is null)
                    return NoContent();

                ResponseUnit<bool?> model = await VocService.UpdateVocTypeService(HttpContext, dto).ConfigureAwait(false);

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


    }
}
