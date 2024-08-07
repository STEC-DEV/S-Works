using FamTec.Server.Hubs;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc.Hub;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FamTec.Server.Controllers.Hubs
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubContext<BroadcastHub> HubContext;
        private IHubService HubService;
        private ILogService LogService;
       

        public HubController(IHubContext<BroadcastHub> _hubcontext,
            IHubService _hubservice,
            ILogService _logservice)
        {
            this.HubContext = _hubcontext;
            this.HubService = _hubservice;
            this.LogService = _logservice;
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
                // 확장자 검사
                if(files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        // VOC 이미지는 2MB 제한
                        if(file.Length > Common.MEGABYTE_2)
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 2MB 이하만 가능합니다.", data = null, code = 200 });
                        }

                        string? extension = Path.GetExtension(file.FileName);
                        if(String.IsNullOrWhiteSpace(extension))
                        {
                            return BadRequest();
                        }

                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<string?> model = await HubService.AddVocService(dto, files);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                {
                    // 미분류로 알람전송
                    await HubContext.Clients.Group($"{dto.Placeid}_ETCRoom").SendAsync("ReceiveVoc", dto.Title);
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

        /// <summary>
        /// 민원 조회 [일반사용자]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("VocInfo")]
        public async ValueTask<IActionResult> GetVocInfo([FromQuery]string voccode)
        {
            try
            {
                ResponseUnit<VocUserDetailDTO?> model = await HubService.GetVocRecord(voccode);
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
        public async ValueTask<IActionResult> GetVocComment([FromQuery]string voccode)
        {
            try
            {
                ResponseList<VocCommentListDTO>? model = await HubService.GetVocCommentList(voccode);
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
        public async ValueTask<IActionResult> GetVocCommentDetail([FromQuery] int commentid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<VocCommentDetailDTO?> model = await HubService.GetVocCommentDetail(commentid);
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
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

    }
}
