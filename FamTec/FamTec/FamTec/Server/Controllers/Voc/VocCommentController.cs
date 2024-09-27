using FamTec.Server.Middleware;
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
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    public class VocCommentController : ControllerBase
    {
        private IVocCommentService VocCommentService;
        private IFileService FileService;
        private ILogService LogService;

        public VocCommentController(IVocCommentService _voccommentservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.VocCommentService = _voccommentservice;
            this.FileService = _fileservice;
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
                        //if (file.Length > Common.MEGABYTE_2)
                        //{
                        //    return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 2MB 이하만 가능합니다.", data = null, code = 200 });
                        //}

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
                                return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                            }
                        }
                    }
                }

                // 밑에 추가로 작성
                ResponseUnit<AddVocCommentDTO?> model = await VocCommentService.AddVocCommentService(HttpContext, dto, files);
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
        [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
        [Route("sign/VocCommentDetail")]
        public async ValueTask<IActionResult> GetVocCommentDetail([FromQuery] int commentid)
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
        //public async ValueTask<IActionResult> UpdateVocComment([FromForm] List<IFormFile>? files)
        public async ValueTask<IActionResult> UpdateVocComment([FromForm] VocCommentDetailDTO dto, [FromForm] List<IFormFile>? files)
        {
            try
            {

                //VocCommentDetailDTO dto = new VocCommentDetailDTO();
                //dto.VocCommentId = 15;
                //dto.Content = "수저된내용";
                //dto.Status = 1;

                //files = files.OrderBy(file => Path.GetFileName(file.FileName)).ToList();
                if (HttpContext is null) // NULL CHECK
                    return BadRequest();

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
                        //if (file.Length > Common.MEGABYTE_2)
                        //{
                        //    return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 2MB 이하만 가능합니다.", data = null, code = 200 });
                        //}

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

                ResponseUnit<bool?> model = await VocCommentService.UpdateCommentService(HttpContext, dto, files);
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
    }
}
