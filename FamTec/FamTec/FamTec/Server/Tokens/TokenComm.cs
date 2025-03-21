using FamTec.Server.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamTec.Server.Tokens
{
    public class TokenComm : ITokenComm
    {
        private readonly string? _authSigningKey;
        private readonly ConsoleLogService<TokenComm> CreateBuilderLogger;
        private readonly ILogService LogService;

        public TokenComm(IConfiguration configuration,
            ILogService _logservice,
            ConsoleLogService<TokenComm> _createbuilderlogger)
        {
            this._authSigningKey = configuration["JWT:AuthSigningKey"];
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        public JObject? TokenConvert(HttpRequest? token)
        {
            try
            {
                if (token is not null && !String.IsNullOrWhiteSpace(_authSigningKey))
                {
                    string? accessToken = token.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                    var authSigningKey = Encoding.UTF8.GetBytes(_authSigningKey);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(authSigningKey),
                        ValidateIssuer = false,
                        ValidateAudience = false,
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }
    }
}

