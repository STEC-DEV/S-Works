using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamTec.Server.Tokens
{
    public class TokenComm : ITokenComm
    {
        public JObject? TokenConvert(HttpRequest? token)
        {
            if (token is not null)
            {
                string? accessToken = token.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var authSigningKey = Encoding.UTF8.GetBytes("DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4");

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(authSigningKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                int split = validatedToken.ToString()!.IndexOf('.') + 1;

                string payload = validatedToken.ToString()!.Substring(split, validatedToken.ToString()!.Length - split);
                JObject? jobj = JObject.Parse(payload.ToString());
                return jobj;
            }
            else
            {
                return null;
            }
        }
    }
}

