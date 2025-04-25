using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc.Hub;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Hubs
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubService HubService;
        private readonly ICommService CommService; /* 핼퍼클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<HubController> CreateBuilderLogger; /* 콘솔로그 */
        
        public HubController(
            IHubService _hubservice,
            ILogService _logservice,
            ICommService _commservice,
            ConsoleLogService<HubController> _createbuilderlogger)
        {
            this.HubService = _hubservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 인증코드 발급 [V2]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v2/AddAuthCode")]
        [Route("AddAuthCode")]
        public async Task<IActionResult> AddAuthCode([FromQuery][Required]int PlaceId, [FromQuery][Required]int BuildingId, [FromQuery][Required]string PhoneNumber)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (PlaceId is 0)
                    return NoContent();

                if(BuildingId is 0)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(PhoneNumber))
                    return NoContent();

                ResponseModel<bool> model = await HubService.AddAuthCodeService(PlaceId, BuildingId, PhoneNumber).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 인증코드 검사 [V2]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v2/GetVerifyAuthCode")]
        [Route("GetVerifyAuthCode")]
        public async Task<IActionResult> GetVerifyAuthCode([FromQuery][Required] string PhoneNumber, [FromQuery][Required]string AuthCode)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(PhoneNumber) || String.IsNullOrWhiteSpace(AuthCode))
                    return NoContent();

                ResponseModel<bool> model = await HubService.GetVerifyAuthCodeService(PhoneNumber, AuthCode).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 민원접수 V2 [일반사용자] / 직원용이랑 같이쓰는듯
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("v2/AddVoc")]
        public async Task<IActionResult> AddVocV2([FromForm] AddVocDTOV2 dto, [FromForm] List<IFormFile>? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.title)) // 민원 제목 NULL CHECK
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.contents)) // 민원 내용 NULL CEHCK
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.name)) // 작성자 이름 NULL CHECK
                    return NoContent();

                // 확장자 검사
                if (files is [_, ..])
                {
                    foreach (IFormFile file in files)
                    {
                        if (file.Length > Common.MEGABYTE_10)
                            return Ok(new ResponseModel<AddVocReturnDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = Path.GetExtension(file.FileName);
                        if (String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }

                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseModel<AddVocReturnDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseModel<AddVocReturnDTO?> model = await HubService.AddVocServiceV2(dto, files).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();
                if (model.code == 200) // OK
                    return Ok(model);
                else if (model.code == 204) // 내용잘못됨
                    return NoContent();
                else if (model.code == 401) // 해약된 사업장.
                    return Unauthorized();
                else if (model.code == 501) // 클라이언트가 재시도 해야함.
                    return Ok(model);
                else // 완전 서버에러
                    return Problem("서버에서 처리하지 못함", statusCode: 500);
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
        /// 민원접수 [일반사용자] / 직원용이랑 같이쓰는듯
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddVoc")]
        public async Task<IActionResult> AddVoc([FromForm] AddVocDTO dto, [FromForm] List<IFormFile>? files)
        {
            try 
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
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
                            return Ok(new ResponseModel<AddVocReturnDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = Path.GetExtension(file.FileName);
                        if(String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }

                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseModel<AddVocReturnDTO?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseModel<AddVocReturnDTO?> model = await HubService.AddVocService(dto, files).ConfigureAwait(false);
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
        public async Task<IActionResult> GetVocInfo([FromQuery][Required]string voccode)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(voccode))
                    return NoContent();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<VocUserDetailDTO?> model = await HubService.GetVocRecord(voccode, isMobile).ConfigureAwait(false);
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
        public async Task<IActionResult> GetVocComment([FromQuery][Required]string voccode)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(voccode))
                    return NoContent();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<List<VocCommentListDTO>>? model = await HubService.GetVocCommentList(voccode, isMobile).ConfigureAwait(false);
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
        public async Task<IActionResult> GetVocCommentDetail([FromQuery][Required] int commentid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif

                if (commentid is 0)
                    return NoContent();

                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<VocCommentDetailDTO?> model = await HubService.GetVocCommentDetail(commentid, isMobile).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

    }
}
