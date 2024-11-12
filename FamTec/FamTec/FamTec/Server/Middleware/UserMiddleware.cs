using FamTec.Server.Tokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FamTec.Server.Middleware
{
    public class UserMiddleware
    {
        private readonly RequestDelegate Next;
        private ITokenComm TokenComm;
        private readonly string? _authSigningKey;


        public UserMiddleware(RequestDelegate _next, ITokenComm _tokencomm, IConfiguration configuration)
        {
            this.Next = _next;
            this.TokenComm = _tokencomm;
            this._authSigningKey = configuration["JWT:AuthSigningKey"];
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

            if (String.IsNullOrWhiteSpace(_authSigningKey))
                return;

            var authSigningKey = Encoding.UTF8.GetBytes(_authSigningKey);

            try {
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
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                bool AdminYN = Boolean.Parse(jobj["AdminYN"]!.ToString());

                if (AdminYN == true) // 관리자
                {
                    if (jobj?["UserIdx"] == null ||
                        jobj?["Name"] == null ||
                        jobj?["AlarmYN"] == null ||
                        jobj?["AdminYN"] == null ||
                        jobj?["UserType"] == null ||
                        jobj?["AdminIdx"] == null ||
                        jobj?["jti"] == null ||
                        jobj?["Role"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    context.Items.Add("UserIdx", jobj["UserIdx"]!.ToString()); // 사용자테이블 인덱스
                    context.Items.Add("Name", jobj["Name"]!.ToString()); // 사용자이름
                    context.Items.Add("AlarmYN", jobj["AlarmYN"]!.ToString()); // 알람유무
                    context.Items.Add("AdminYN", jobj["AdminYN"]!.ToString()); // 관리자 유무
                    context.Items.Add("UserType", jobj["UserType"]!.ToString()); // 사용자 타입(유저 or 관리자)
                    context.Items.Add("AdminIdx", jobj["AdminIdx"]!.ToString()); // 관리자인덱스
                    context.Items.Add("jti", jobj["jti"]!.ToString());
                    context.Items.Add("Role", jobj["Role"]!.ToString()); // Role 검사

                    if (jobj?["UserPerms"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    JObject? parse = new JObject(JObject.Parse(jobj["UserPerms"]!.ToString()));
                    if (parse == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    if (parse?["UserPerm_Basic"] == null ||
                        parse?["UserPerm_Machine"] == null ||
                        parse?["UserPerm_Elec"] == null ||
                        parse?["UserPerm_Lift"] == null ||
                        parse?["UserPerm_Fire"] == null ||
                        parse?["UserPerm_Construct"] == null ||
                        parse?["UserPerm_Network"] == null ||
                        parse?["UserPerm_Beauty"] == null ||
                        parse?["UserPerm_Security"] == null ||
                        parse?["UserPerm_Material"] == null ||
                        parse?["UserPerm_Energy"] == null ||
                        parse?["UserPerm_User"] == null ||
                        parse?["UserPerm_Voc"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    /* 사용자 권한 */
                    context.Items.Add("UserPerm_Basic", parse["UserPerm_Basic"]!.ToString());
                    context.Items.Add("UserPerm_Machine", parse["UserPerm_Machine"]!.ToString());
                    context.Items.Add("UserPerm_Elec", parse["UserPerm_Elec"]!.ToString());
                    context.Items.Add("UserPerm_Lift", parse["UserPerm_Lift"]!.ToString());
                    context.Items.Add("UserPerm_Fire", parse["UserPerm_Fire"]!.ToString());
                    context.Items.Add("UserPerm_Construct", parse["UserPerm_Construct"]!.ToString());
                    context.Items.Add("UserPerm_Network", parse["UserPerm_Network"]!.ToString());
                    context.Items.Add("UserPerm_Beauty", parse["UserPerm_Beauty"]!.ToString());
                    context.Items.Add("UserPerm_Security", parse["UserPerm_Security"]!.ToString());
                    context.Items.Add("UserPerm_Material", parse["UserPerm_Material"]!.ToString());
                    context.Items.Add("UserPerm_Energy", parse["UserPerm_Energy"]!.ToString());
                    context.Items.Add("UserPerm_User", parse["UserPerm_User"]!.ToString());
                    context.Items.Add("UserPerm_Voc", parse["UserPerm_Voc"]!.ToString());


                    if (jobj?["VocPerms"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    /* VOC 권한 */
                    parse = new JObject(JObject.Parse(jobj["VocPerms"]!.ToString()));
                    if (parse == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }


                    if (parse?["VocMachine"] == null ||
                        parse?["VocElec"] == null ||
                        parse?["VocLift"] == null ||
                        parse?["VocFire"] == null ||
                        parse?["VocConstruct"] == null ||
                        parse?["VocNetwork"] == null ||
                        parse?["VocBeauty"] == null ||
                        parse?["VocSecurity"] == null ||
                        parse?["VocDefault"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    context.Items.Add("VocMachine", parse["VocMachine"]!.ToString());
                    context.Items.Add("VocElec", parse["VocElec"]!.ToString());
                    context.Items.Add("VocLift", parse["VocLift"]!.ToString());
                    context.Items.Add("VocFire", parse["VocFire"]!.ToString());
                    context.Items.Add("VocConstruct", parse["VocConstruct"]!.ToString());
                    context.Items.Add("VocNetwork", parse["VocNetwork"]!.ToString());
                    context.Items.Add("VocBeauty", parse["VocBeauty"]!.ToString());
                    context.Items.Add("VocSecurity", parse["VocSecurity"]!.ToString());
                    context.Items.Add("VocDefault", parse["VocDefault"]!.ToString());

                    if (jobj?["UserPerms"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    string? PlaceIdx = Convert.ToString(jobj["PlacePerms"]);
                    if (!string.IsNullOrWhiteSpace(PlaceIdx))
                    {
                        // 사업장 까지 선택한 상태
                        context.Items.Add("PlaceIdx", jobj["PlaceIdx"]!.ToString()); // 사업장ID
                        context.Items.Add("PlaceName", jobj["PlaceName"]!.ToString()); // 사업장이름
                        context.Items.Add("PlaceCreateDT", jobj["PlaceCreateDT"]!.ToString()); // 사업장 생성일

                        parse = new JObject(JObject.Parse(jobj["PlacePerms"]!.ToString()));
                        if (parse == null)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        if (parse?["PlacePerm_Machine"] == null ||
                            parse?["PlacePerm_Elec"] == null ||
                            parse?["PlacePerm_Lift"] == null ||
                            parse?["PlacePerm_Fire"] == null ||
                            parse?["PlacePerm_Construct"] == null ||
                            parse?["PlacePerm_Network"] == null ||
                            parse?["PlacePerm_Beauty"] == null ||
                            parse?["PlacePerm_Security"] == null ||
                            parse?["PlacePerm_Material"] == null ||
                            parse?["PlacePerm_Energy"] == null ||
                            parse?["PlacePerm_Voc"] == null)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        context.Items.Add("PlacePerm_Machine", parse["PlacePerm_Machine"]!.ToString());
                        context.Items.Add("PlacePerm_Elec", parse["PlacePerm_Elec"]!.ToString());
                        context.Items.Add("PlacePerm_Lift", parse["PlacePerm_Lift"]!.ToString());
                        context.Items.Add("PlacePerm_Fire", parse["PlacePerm_Fire"]!.ToString());
                        context.Items.Add("PlacePerm_Construct", parse["PlacePerm_Construct"]!.ToString());
                        context.Items.Add("PlacePerm_Network", parse["PlacePerm_Network"]!.ToString());
                        context.Items.Add("PlacePerm_Beauty", parse["PlacePerm_Beauty"]!.ToString());
                        context.Items.Add("PlacePerm_Security", parse["PlacePerm_Security"]!.ToString());
                        context.Items.Add("PlacePerm_Material", parse["PlacePerm_Material"]!.ToString());
                        context.Items.Add("PlacePerm_Energy", parse["PlacePerm_Energy"]!.ToString());
                        context.Items.Add("PlacePerm_Voc", parse["PlacePerm_Voc"]!.ToString());
                    }
                }
                else // 아님 - 일반유저
                {
                    if (jobj?["UserIdx"] == null ||
                     jobj?["Name"] == null ||
                     jobj?["AlarmYN"] == null ||
                     jobj?["AdminYN"] == null ||
                     jobj?["UserType"] == null ||
                     jobj?["jti"] == null ||
                     jobj?["Role"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    context.Items.Add("UserIdx", jobj["UserIdx"]!.ToString());
                    context.Items.Add("Name", jobj["Name"]!.ToString());
                    context.Items.Add("AlarmYN", jobj["AlarmYN"]!.ToString());
                    context.Items.Add("AdminYN", jobj["AdminYN"]!.ToString());
                    context.Items.Add("UserType", jobj["UserType"]!.ToString());
                    context.Items.Add("jti", jobj["jti"]!.ToString());
                    context.Items.Add("Role", jobj["Role"]!.ToString());


                    if (jobj?["UserPerms"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    JObject? parse = new JObject(JObject.Parse(jobj["UserPerms"]!.ToString()));

                    if (parse["UserPerm_Basic"] == null ||
                        parse["UserPerm_Machine"] == null ||
                        parse["UserPerm_Elec"] == null ||
                        parse["UserPerm_Lift"] == null ||
                        parse["UserPerm_Fire"] == null ||
                        parse["UserPerm_Construct"] == null ||
                        parse["UserPerm_Network"] == null ||
                        parse["UserPerm_Beauty"] == null ||
                        parse["UserPerm_Security"] == null ||
                        parse["UserPerm_Material"] == null ||
                        parse["UserPerm_Energy"] == null ||
                        parse["UserPerm_User"] == null ||
                        parse["UserPerm_Voc"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    /* 사용자 권한 */
                    context.Items.Add("UserPerm_Basic", parse["UserPerm_Basic"]!.ToString());
                    context.Items.Add("UserPerm_Machine", parse["UserPerm_Machine"]!.ToString());
                    context.Items.Add("UserPerm_Elec", parse["UserPerm_Elec"]!.ToString());
                    context.Items.Add("UserPerm_Lift", parse["UserPerm_Lift"]!.ToString());
                    context.Items.Add("UserPerm_Fire", parse["UserPerm_Fire"]!.ToString());
                    context.Items.Add("UserPerm_Construct", parse["UserPerm_Construct"]!.ToString());
                    context.Items.Add("UserPerm_Network", parse["UserPerm_Network"]!.ToString());
                    context.Items.Add("UserPerm_Beauty", parse["UserPerm_Beauty"]!.ToString());
                    context.Items.Add("UserPerm_Security", parse["UserPerm_Security"]!.ToString());
                    context.Items.Add("UserPerm_Material", parse["UserPerm_Material"]!.ToString());
                    context.Items.Add("UserPerm_Energy", parse["UserPerm_Energy"]!.ToString());
                    context.Items.Add("UserPerm_User", parse["UserPerm_User"]!.ToString());
                    context.Items.Add("UserPerm_Voc", parse["UserPerm_Voc"]!.ToString());

                    if (jobj?["VocPerms"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    /* VOC 권한 */
                    parse = new JObject(JObject.Parse(jobj["VocPerms"]!.ToString()));

                    if (parse["VocMachine"] == null ||
                        parse["VocElec"] == null ||
                        parse["VocLift"] == null ||
                        parse["VocFire"] == null ||
                        parse["VocConstruct"] == null ||
                        parse["VocNetwork"] == null ||
                        parse["VocBeauty"] == null ||
                        parse["VocSecurity"] == null ||
                        parse["VocDefault"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    
                    context.Items.Add("VocMachine", parse["VocMachine"]!.ToString());
                    context.Items.Add("VocElec", parse["VocElec"]!.ToString());
                    context.Items.Add("VocLift", parse["VocLift"]!.ToString());
                    context.Items.Add("VocFire", parse["VocFire"]!.ToString());
                    context.Items.Add("VocConstruct", parse["VocConstruct"]!.ToString());
                    context.Items.Add("VocNetwork", parse["VocNetwork"]!.ToString());
                    context.Items.Add("VocBeauty", parse["VocBeauty"]!.ToString());
                    context.Items.Add("VocSecurity", parse["VocSecurity"]!.ToString());
                    context.Items.Add("VocDefault", parse["VocDefault"]!.ToString());


                    if (jobj["PlaceIdx"] == null ||
                        jobj["PlaceName"] == null ||
                        jobj["PlaceCreateDT"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    /* 사업장 권한 */
                    context.Items.Add("PlaceIdx", jobj["PlaceIdx"]!.ToString()); // 사업장ID
                    context.Items.Add("PlaceName", jobj["PlaceName"]!.ToString()); // 사업장이름
                    context.Items.Add("PlaceCreateDT", jobj["PlaceCreateDT"]!.ToString()); // 사업장 생성일

                    parse = new JObject(JObject.Parse(jobj["PlacePerms"]!.ToString()));
                    if (parse == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    if (parse["PlacePerm_Machine"] == null ||
                        parse["PlacePerm_Elec"] == null ||
                        parse["PlacePerm_Lift"] == null ||
                        parse["PlacePerm_Fire"] == null ||
                        parse["PlacePerm_Construct"] == null ||
                        parse["PlacePerm_Network"] == null ||
                        parse["PlacePerm_Beauty"] == null ||
                        parse["PlacePerm_Security"] == null ||
                        parse["PlacePerm_Energy"] == null ||
                        parse["PlacePerm_Voc"] == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    context.Items.Add("PlacePerm_Machine", parse["PlacePerm_Machine"]!.ToString());
                    context.Items.Add("PlacePerm_Elec", parse["PlacePerm_Elec"]!.ToString());
                    context.Items.Add("PlacePerm_Lift", parse["PlacePerm_Lift"]!.ToString());
                    context.Items.Add("PlacePerm_Fire", parse["PlacePerm_Fire"]!.ToString());
                    context.Items.Add("PlacePerm_Construct", parse["PlacePerm_Construct"]!.ToString());
                    context.Items.Add("PlacePerm_Network", parse["PlacePerm_Network"]!.ToString());
                    context.Items.Add("PlacePerm_Beauty", parse["PlacePerm_Beauty"]!.ToString());
                    context.Items.Add("PlacePerm_Security", parse["PlacePerm_Security"]!.ToString());
                    context.Items.Add("PlacePerm_Material", parse["PlacePerm_Material"]!.ToString());
                    context.Items.Add("PlacePerm_Energy", parse["PlacePerm_Energy"]!.ToString());
                    context.Items.Add("PlacePerm_Voc", parse["PlacePerm_Voc"]!.ToString());
                }

                await Next(context);
                return;
            }
            catch(SecurityTokenExpiredException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

    }
}
