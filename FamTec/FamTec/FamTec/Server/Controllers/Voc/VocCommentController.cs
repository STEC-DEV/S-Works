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

        public VocCommentController(IVocCommentService _voccommentservice)
        {
            this.VocCommentService = _voccommentservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddVocComment")]

        public async ValueTask<IActionResult> AddVocComment([FromBody]AddVocCommentDTO dto)
        {
            try
            {
                ResponseUnit<AddVocCommentDTO>? model = await VocCommentService.AddVocCommentService(HttpContext, dto);
                if (model is not null)
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
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocComment")]
        public async ValueTask<IActionResult> GetVocComment([FromQuery]int vocid)
        {
            ResponseList<VocCommentListDTO>? model = await VocCommentService.GetVocCommentList(vocid);
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
