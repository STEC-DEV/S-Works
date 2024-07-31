using DocumentFormat.OpenXml.Office.Word;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Voc
{
    [Route("api/[controller]")]
    [ApiController]
    public class VocCommentController : ControllerBase
    {
        private IVocCommentService VocCommentService;
        private ILogService LogService;

        public VocCommentController(IVocCommentService _voccommentservice,
            ILogService _logservice)
        {
            this.VocCommentService = _voccommentservice;
            this.LogService = _logservice;
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

        public async ValueTask<IActionResult> AddVocComment([FromForm]AddVocCommentDTO dto, [FromForm] List<IFormFile>? files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if(files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        string? FileName = file.Name;
                        string? FileExtenstion = Path.GetExtension(file.FileName);

                        // VOC 이미지는 2MB 제한
                        if(file.Length > 2097152)
                        {
                            return Ok(new ResponseUnit<AddVocCommentDTO?>() { message = "이미지 업로드는 2MB 이하만 가능합니다.", data = null, code = 200 });
                        }
                        
                        // 파일형식 검사
                        if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "올바르지 않은 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                // 밑에 추가로 작성
                ResponseUnit<AddVocCommentDTO>? model = await VocCommentService.AddVocCommentService(HttpContext, dto, files);
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
                return Problem("서버에서 처리할수 없습니다.", statusCode: 500);
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
        public async ValueTask<IActionResult> GetVocComment([FromQuery]int vocid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<VocCommentListDTO>? model = await VocCommentService.GetVocCommentList(HttpContext, vocid);
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
                return Problem("서버에서 처리할수 없음", statusCode: 500);
            }
        }

        /// <summary>
        /// VOC 댓글 상세보기
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/VocCommetDetail")]
        public async ValueTask<IActionResult> GetVocCommentDetail([FromQuery] int? commentid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocCommentDetailDTO?> model = await VocCommentService.GetVocCommentDetail(HttpContext, commentid);
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
                return Problem("서버에서 처리할 수 없음", statusCode: 500);
            }
        }


        // VocCommentUpdate

        //public async ValueTask<IActionResult> UpdateVocComment()
    }
}
