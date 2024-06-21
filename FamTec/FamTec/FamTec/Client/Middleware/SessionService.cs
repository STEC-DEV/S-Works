using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json.Linq;


namespace FamTec.Client.Middleware
{
    public class SessionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _key = "DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4";
        public SessionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetClaimValue(string token, string claimType)
        {
        

            if(token == null)
            {
                return null;
            }
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var encryptToken = handler.ReadJwtToken(token);

            var claim = encryptToken.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

        public JObject GetConvertToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4")),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            int split = validatedToken.ToString()!.IndexOf('.') + 1;

            string payload = validatedToken.ToString()!.Substring(split, validatedToken.ToString()!.Length - split);
            JObject jobj = JObject.Parse(payload.ToString());

            return jobj;
        }


        public class SessionStatus
        {
            public bool IsActive { get; set; }
        }
    }
}
