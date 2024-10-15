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

        public UserMiddleware(RequestDelegate _next, ITokenComm _tokencomm)
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
                    return;

                bool AdminYN = Boolean.Parse(jobj["AdminYN"]!.ToString());

                if (AdminYN == true) // 관리자
                {
                    context.Items.Add("UserIdx", jobj["UserIdx"]!.ToString()); // 사용자테이블 인덱스
                    context.Items.Add("Name", jobj["Name"]!.ToString()); // 사용자이름
                    context.Items.Add("AlarmYN", jobj["AlarmYN"]!.ToString()); // 알람유무
                    context.Items.Add("AdminYN", jobj["AdminYN"]!.ToString()); // 관리자 유무
                    context.Items.Add("UserType", jobj["UserType"]!.ToString()); // 사용자 타입(유저 or 관리자)
                    context.Items.Add("AdminIdx", jobj["AdminIdx"]!.ToString()); // 관리자인덱스
                    context.Items.Add("jti", jobj["jti"]!.ToString());
                    context.Items.Add("Role", jobj["Role"]!.ToString()); // Role 검사

                    JObject parse = new JObject(JObject.Parse(jobj["UserPerms"]!.ToString()));

                    /* 사용자 권한 */
                    context.Items.Add("UserPerm_Basic", parse["UserPerm_Basic"].ToString());
                    context.Items.Add("UserPerm_Machine", parse["UserPerm_Machine"].ToString());
                    context.Items.Add("UserPerm_Elec", parse["UserPerm_Elec"].ToString());
                    context.Items.Add("UserPerm_Lift", parse["UserPerm_Lift"].ToString());
                    context.Items.Add("UserPerm_Fire", parse["UserPerm_Fire"].ToString());
                    context.Items.Add("UserPerm_Construct", parse["UserPerm_Construct"].ToString());
                    context.Items.Add("UserPerm_Network", parse["UserPerm_Network"].ToString());
                    context.Items.Add("UserPerm_Beauty", parse["UserPerm_Beauty"].ToString());
                    context.Items.Add("UserPerm_Security", parse["UserPerm_Security"].ToString());
                    context.Items.Add("UserPerm_Material", parse["UserPerm_Material"].ToString());
                    context.Items.Add("UserPerm_Energy", parse["UserPerm_Energy"].ToString());
                    context.Items.Add("UserPerm_User", parse["UserPerm_User"].ToString());
                    context.Items.Add("UserPerm_Voc", parse["UserPerm_Voc"].ToString());

                    /* VOC 권한 */
                    parse = new JObject(JObject.Parse(jobj["VocPerms"].ToString()));
                    context.Items.Add("VocMachine", parse["VocMachine"].ToString());
                    context.Items.Add("VocElec", parse["VocElec"].ToString());
                    context.Items.Add("VocLift", parse["VocLift"].ToString());
                    context.Items.Add("VocFire", parse["VocFire"].ToString());
                    context.Items.Add("VocConstruct", parse["VocConstruct"].ToString());
                    context.Items.Add("VocNetwork", parse["VocNetwork"].ToString());
                    context.Items.Add("VocBeauty", parse["VocBeauty"].ToString());
                    context.Items.Add("VocSecurity", parse["VocSecurity"].ToString());
                    context.Items.Add("VocDefault", parse["VocDefault"].ToString());

                    string? PlaceIdx = Convert.ToString(jobj["PlacePerms"]);
                    if (!string.IsNullOrWhiteSpace(PlaceIdx))
                    {
                        // 사업장 까지 선택한 상태
                        context.Items.Add("PlaceIdx", jobj["PlaceIdx"]!.ToString()); // 사업장ID
                        context.Items.Add("PlaceName", jobj["PlaceName"]!.ToString()); // 사업장이름
                        context.Items.Add("PlaceCreateDT", jobj["PlaceCreateDT"]!.ToString()); // 사업장 생성일

                        parse = new JObject(JObject.Parse(jobj["PlacePerms"].ToString()));
                        context.Items.Add("PlacePerm_Machine", parse["PlacePerm_Machine"].ToString());
                        context.Items.Add("PlacePerm_Elec", parse["PlacePerm_Elec"].ToString());
                        context.Items.Add("PlacePerm_Lift", parse["PlacePerm_Lift"].ToString());
                        context.Items.Add("PlacePerm_Fire", parse["PlacePerm_Fire"].ToString());
                        context.Items.Add("PlacePerm_Construct", parse["PlacePerm_Construct"].ToString());
                        context.Items.Add("PlacePerm_Network", parse["PlacePerm_Network"].ToString());
                        context.Items.Add("PlacePerm_Beauty", parse["PlacePerm_Beauty"].ToString());
                        context.Items.Add("PlacePerm_Security", parse["PlacePerm_Security"].ToString());
                        context.Items.Add("PlacePerm_Material", parse["PlacePerm_Material"].ToString());
                        context.Items.Add("PlacePerm_Energy", parse["PlacePerm_Energy"].ToString());
                        context.Items.Add("PlacePerm_Voc", parse["PlacePerm_Voc"].ToString());
                    }
                }
                else // 아님 - 일반유저
                {
                    context.Items.Add("UserIdx", jobj["UserIdx"].ToString());
                    context.Items.Add("Name", jobj["Name"].ToString());
                    context.Items.Add("AlarmYN", jobj["AlarmYN"].ToString());
                    context.Items.Add("AdminYN", jobj["AdminYN"].ToString());
                    context.Items.Add("UserType", jobj["UserType"].ToString());
                    context.Items.Add("jti", jobj["jti"].ToString());
                    context.Items.Add("Role", jobj["Role"].ToString());

                    JObject parse = new JObject(JObject.Parse(jobj["UserPerms"].ToString()));

                    /* 사용자 권한 */
                    context.Items.Add("UserPerm_Basic", parse["UserPerm_Basic"].ToString());
                    context.Items.Add("UserPerm_Machine", parse["UserPerm_Machine"].ToString());
                    context.Items.Add("UserPerm_Elec", parse["UserPerm_Elec"].ToString());
                    context.Items.Add("UserPerm_Lift", parse["UserPerm_Lift"].ToString());
                    context.Items.Add("UserPerm_Fire", parse["UserPerm_Fire"].ToString());
                    context.Items.Add("UserPerm_Construct", parse["UserPerm_Construct"].ToString());
                    context.Items.Add("UserPerm_Network", parse["UserPerm_Network"].ToString());
                    context.Items.Add("UserPerm_Beauty", parse["UserPerm_Beauty"].ToString());
                    context.Items.Add("UserPerm_Security", parse["UserPerm_Security"].ToString());
                    context.Items.Add("UserPerm_Material", parse["UserPerm_Material"].ToString());
                    context.Items.Add("UserPerm_Energy", parse["UserPerm_Energy"].ToString());
                    context.Items.Add("UserPerm_User", parse["UserPerm_User"].ToString());
                    context.Items.Add("UserPerm_Voc", parse["UserPerm_Voc"].ToString());

                    parse = new JObject(JObject.Parse(jobj["VocPerms"].ToString()));
                    /* VOC 권한 */
                    context.Items.Add("VocMachine", parse["VocMachine"].ToString());
                    context.Items.Add("VocElec", parse["VocElec"].ToString());
                    context.Items.Add("VocLift", parse["VocLift"].ToString());
                    context.Items.Add("VocFire", parse["VocFire"].ToString());
                    context.Items.Add("VocConstruct", parse["VocConstruct"].ToString());
                    context.Items.Add("VocNetwork", parse["VocNetwork"].ToString());
                    context.Items.Add("VocBeauty", parse["VocBeauty"].ToString());
                    context.Items.Add("VocSecurity", parse["VocSecurity"].ToString());
                    context.Items.Add("VocDefault", parse["VocDefault"].ToString());

                    /* 사업장 권한 */
                    context.Items.Add("PlaceIdx", jobj["PlaceIdx"]!.ToString()); // 사업장ID
                    context.Items.Add("PlaceName", jobj["PlaceName"]!.ToString()); // 사업장이름
                    context.Items.Add("PlaceCreateDT", jobj["PlaceCreateDT"]!.ToString()); // 사업장 생성일

                    parse = new JObject(JObject.Parse(jobj["PlacePerms"].ToString()));
                    context.Items.Add("PlacePerm_Machine", parse["PlacePerm_Machine"].ToString());
                    context.Items.Add("PlacePerm_Elec", parse["PlacePerm_Elec"].ToString());
                    context.Items.Add("PlacePerm_Lift", parse["PlacePerm_Lift"].ToString());
                    context.Items.Add("PlacePerm_Fire", parse["PlacePerm_Fire"].ToString());
                    context.Items.Add("PlacePerm_Construct", parse["PlacePerm_Construct"].ToString());
                    context.Items.Add("PlacePerm_Network", parse["PlacePerm_Network"].ToString());
                    context.Items.Add("PlacePerm_Beauty", parse["PlacePerm_Beauty"].ToString());
                    context.Items.Add("PlacePerm_Security", parse["PlacePerm_Security"].ToString());
                    context.Items.Add("PlacePerm_Material", parse["PlacePerm_Material"].ToString());
                    context.Items.Add("PlacePerm_Energy", parse["PlacePerm_Energy"].ToString());
                    context.Items.Add("PlacePerm_Voc", parse["PlacePerm_Voc"].ToString());
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
