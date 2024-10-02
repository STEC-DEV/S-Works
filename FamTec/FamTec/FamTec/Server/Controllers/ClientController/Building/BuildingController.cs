using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.ClientController.Building
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        [HttpPut]
        [Route("updatebuilding")]
        public async Task<IActionResult> UpdateBuilding([FromBody] List<int> adminIds)
        {
            try
            {
                Console.WriteLine("[건물 수정 컨트롤러 시작]");




                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Building][Controller][Put] 건물 수정 에러!!\n " + ex);
                return Problem("[Building][Controller][Put] 건물 수정 에러!!\n" + ex);
            }
        }

    }
}
