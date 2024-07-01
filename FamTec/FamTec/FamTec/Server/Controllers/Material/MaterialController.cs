using FamTec.Server.Services.Material;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private IMaterialService MaterialService;

        public MaterialController(IMaterialService _materialservice)
        {
            this.MaterialService = _materialservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddMaterial")]
        public async ValueTask<IActionResult> AddMaterial([FromBody] AddMaterialDTO dto)
        {
            ResponseUnit<AddMaterialDTO>? model = await MaterialService.AddMaterialService(HttpContext, dto);

            if(model is not null)
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllMaterial")]
        public async ValueTask<IActionResult> GetAllMaterial()
        {
            ResponseList<MaterialListDTO>? model = await MaterialService.GetPlaceMaterialListService(HttpContext);
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
