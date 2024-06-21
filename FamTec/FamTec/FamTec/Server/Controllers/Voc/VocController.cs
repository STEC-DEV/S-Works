using FamTec.Server.Services.Voc;
using FamTec.Server.Tokens;
using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Controllers.Voc
{
    [Route("api/[controller]")]
    [ApiController]
    public class VocController : ControllerBase
    {
        private IVocService VocService;

        public VocController(IVocService _vocservice)
        {
            this.VocService = _vocservice;
        }

     


        // placeidx
        // date = 2024
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetVocList")]
        public async ValueTask<IActionResult> GetVocList([FromQuery]string date)
        {
            try
            {
                ResponseList<ListVoc>? model = await VocService.GetVocList(HttpContext, date);

                if (model is not null)
                {
                    if (model.code == 200)
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
                return BadRequest(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
