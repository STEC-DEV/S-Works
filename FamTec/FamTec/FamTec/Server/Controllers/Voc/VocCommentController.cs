using FamTec.Server.Helpers;
using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FamTec.Server.Controllers.Voc
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class VocCommentController : ControllerBase
    {
        private readonly IVocCommentService VocCommentService;
        private readonly IFileService FileService; /* 이미지 서비스 */
        private readonly ICommService CommService; /* 핼퍼클래스 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<VocCommentController> CreateBuilderLogger; /* 콘솔로그 */

        public VocCommentController(IVocCommentService _voccommentservice,
            IFileService _fileservice,
            ICommService _commservice,
            ILogService _logservice,
            ConsoleLogService<VocCommentController> _createbuilderlogger)
        {
            this.VocCommentService = _voccommentservice;
            this.FileService = _fileservice;
            this.CommService = _commservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/v2/AddVocComment")]
        public async Task<IActionResult> AddVocCommandV2([FromForm]AddVocCommentDTOV2 dto, [FromForm] List<IFormFile>? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.Content))
                    return NoContent();

                if (dto.Status is null)
                    return NoContent();

                if (dto.VocTbId is null)
                    return NoContent();

                if(files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        if (file.Length > Common.MEGABYTE_10)
                            return Ok(new ResponseModel<AddVocCommentDTOV2?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = FileService.GetExtension(file);
                        if(String.IsNullOrEmpty(extension))
                        {
                            return BadRequest();
                        }
                        else
                        {
                            bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                            if(!extensioncheck)
                            {
                                return Ok(new ResponseModel<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                            }
                        }
                    }
                }


                // 밑에 추가로 작성
                ResponseModel<AddVocCommentDTOV2?> model = await VocCommentService.AddVocCommentServiceV2(dto, files).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return NoContent();
                else if (model.code == 401)
                    return Unauthorized();
                else if (model.code == 500)
                    return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
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
        /// 조치사항 입력 - VOC 댓글
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddVocComment")]

        public async Task<IActionResult> AddVocComment([FromForm]AddVocCommentDTO dto, [FromForm] List<IFormFile>? files)
       {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (String.IsNullOrWhiteSpace(dto.Content))
                    return NoContent();

                if(dto.Status is null)
                    return NoContent();

                if (dto.VocTbId is null)
                    return NoContent();

                if (files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        if (file.Length > Common.MEGABYTE_10)
                            return Ok(new ResponseModel<AddVocCommentDTO?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = FileService.GetExtension(file);
                        if (String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }
                        else
                        {
                            bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                            if (!extensioncheck)
                            {
                                return Ok(new ResponseModel<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                            }
                        }
                    }
                }

                // 밑에 추가로 작성
                ResponseModel<AddVocCommentDTO?> model = await VocCommentService.AddVocCommentService(dto, files).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// Voc Comment List
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocCommentList")]
        public async Task<IActionResult> GetVocComment([FromQuery][Required]int vocid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<List<VocCommentListDTO>>? model = await VocCommentService.GetVocCommentList(vocid, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// VOC 댓글 상세보기
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/VocCommentDetail")]
        public async Task<IActionResult> GetVocCommentDetail([FromQuery][Required] int commentid)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                // 모바일 여부
                bool isMobile = CommService.MobileConnectCheck();

                ResponseModel<VocCommentDetailDTO?> model = await VocCommentService.GetVocCommentDetail(commentid, isMobile).ConfigureAwait(false);
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
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 민원 댓글 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/VocCommentUpdate")]
        public async Task<IActionResult> UpdateVocComment([FromForm] VocCommentDetailDTO dto, [FromForm] List<IFormFile>? files)
        {
            try
            {
#if DEBUG
                CreateBuilderLogger.ConsoleText($"{HttpContext.Request.Path.Value}");
#endif
                if (dto.VocCommentId is null) // NULL CHECK
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Content)) // NULL CHECK
                    return NoContent();

                if (dto.Status is null) // NULL CHECK
                    return NoContent();


                if (files is [_, ..])
                {
                    foreach (IFormFile file in files)
                    {
                        if (file.Length > Common.MEGABYTE_10)
                            return Ok(new ResponseModel<bool?>() { message = "파일의 용량은 10MB까지 가능합니다.", data = null, code = 403 });

                        string? extension = FileService.GetExtension(file);
                        if (String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }
                        else
                        {
                            bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                            if (!extensioncheck)
                            {
                                return Ok(new ResponseUnit<bool?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                            }
                        }
                    }
                }

                ResponseModel<bool?> model = await VocCommentService.UpdateCommentService(dto, files).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }
    }
}
