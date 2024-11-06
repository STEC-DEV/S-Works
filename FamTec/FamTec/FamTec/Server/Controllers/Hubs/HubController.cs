using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc.Hub;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Hubs
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubService HubService;
        
        private readonly ILogService LogService;
        private readonly ICommService CommService;
        private readonly ConsoleLogService<HubController> CreateBuilderLogger;
        

        public HubController(
            IHubService _hubservice,
            ILogService _logservice,
            ICommService _commservice,
            ConsoleLogService<HubController> _createbuilderlogger)
        {
            this.HubService = _hubservice;
            
            this.LogService = _logservice;
            this.CommService = _commservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 민원접수 [일반사용자]
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddVoc")]
        public async Task<IActionResult> AddVoc([FromForm] AddVocDTO dto, [FromForm] List<IFormFile>? files)
        {
            try 
            {
                if (String.IsNullOrWhiteSpace(dto.Title)) // 민원 제목 NULL CHECK
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Contents)) // 민원 내용 NULL CHECK
                    return NoContent();

                if (dto.Placeid is null)
                    return NoContent();

                if(dto.Buildingid is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name)) // 작성자 이름 NULL CHECK
                    return NoContent();

                // 확장자 검사
                if (files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        if (file.Length > Common.MEGABYTE_10)
                            return Ok(new ResponseUnit<AddVocReturnDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = Path.GetExtension(file.FileName);
                        if(String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }

                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<AddVocReturnDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<AddVocReturnDTO?> model = await HubService.AddVocService(dto, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 민원 조회 [일반사용자]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("VocInfo")]
        public async Task<IActionResult> GetVocInfo([FromQuery]string voccode)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (String.IsNullOrWhiteSpace(voccode))
                    return NoContent();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<VocUserDetailDTO?> model = await HubService.GetVocRecord(voccode, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }


        /// <summary>
        /// 민원에 대한 댓글 조회 [일반사용자]
        /// </summary>
        /// <param name="voccode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetVocCommentList")]
        public async Task<IActionResult> GetVocComment([FromQuery]string voccode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(voccode))
                    return NoContent();

                if (HttpContext is null)
                    return BadRequest();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseList<VocCommentListDTO>? model = await HubService.GetVocCommentList(voccode, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// VOC 댓글 상세보기
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("VocCommentDetail")]
        public async Task<IActionResult> GetVocCommentDetail([FromQuery] int commentid)
        {
            try
            {
                if(commentid is 0)
                    return NoContent();

                if (HttpContext is null)
                    return BadRequest();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck(HttpContext);

                ResponseUnit<VocCommentDetailDTO?> model = await HubService.GetVocCommentDetail(commentid, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

    }
}
