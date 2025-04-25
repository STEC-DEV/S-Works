using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Helpers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        public IActionResult FromService<T>(ResponseModel<T>? resp)
        {
            if (resp is null)
                return StatusCode(500, new { message = "서버에서 처리할 수 없는 데이터입니다." });

            var code = resp.code.GetValueOrDefault(500);
            return code switch
            {
                200 => Ok(resp),
                201 => Created(string.Empty, resp),
                204 => NoContent(),
                400 => BadRequest(resp),
                401 => Unauthorized(resp),
                403 => Forbid(),
                404 => NotFound(resp),
                _ => StatusCode(code, resp)
            };
        }
    }
}
