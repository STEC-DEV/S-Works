using FamTec.Server.Tokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamTec.Server.Middleware
{
    public class AdminMiddleware
    {
        private readonly RequestDelegate Next;
        private ITokenComm TokenComm;

        public AdminMiddleware(RequestDelegate _next, ITokenComm _tokencomm)
        {
            this.Next = _next;
            this.TokenComm = _tokencomm;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // API 키 Configuration 수정
            if (!context.Request.Headers.TryGetValue("Authorization", out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Api Key was not provided. (Using ApiKeyMiddleware)");
                return;
            }

            string? accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (accessToken is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("jwt token validation failed");
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var authSigningKey = Encoding.UTF8.GetBytes("DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4");

            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(authSigningKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;


            // 토큰분해
            JObject? jobj = TokenComm.TokenConvert(context.Request);

            if (jobj is null)
                return;

            context.Items.Add("UserIdx", jobj["UserIdx"]!.ToString());
            context.Items.Add("Name", jobj["Name"]!.ToString());
            context.Items.Add("jti", jobj["jti"]!.ToString());
            context.Items.Add("UserType", jobj["UserType"]!.ToString());
            context.Items.Add("AdminIdx", jobj["AdminIdx"]!.ToString());
            context.Items.Add("DepartIdx", jobj["DepartIdx"]!.ToString());
            context.Items.Add("DepartmentName", jobj["DepartmentName"]!.ToString());
            context.Items.Add("Role", jobj["Role"]!.ToString());
            await Next(context);

            return;
        }


    }
}
