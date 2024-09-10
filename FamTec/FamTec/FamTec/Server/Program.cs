using FamTec.Server.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Admin.AdminUser;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.Room;
using FamTec.Server.Repository.Unit;
using FamTec.Server.Repository.User;
using FamTec.Server.Services.Admin.Account;
using FamTec.Server.Services.Admin.Department;
using FamTec.Server.Services.Admin.Place;
using FamTec.Server.Services.Building;
using FamTec.Server.Services.Floor;
using FamTec.Server.Services.Room;
using FamTec.Server.Services.Unit;
using FamTec.Server.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FamTec.Server.Services;
using FamTec.Server.Tokens;
using FamTec.Server.Services.Voc;
using FamTec.Server.Repository.Voc;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Middleware;
using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Material;
using FamTec.Server.Services.Material;
using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Server;
using FamTec.Server.Databases;
using FamTec.Server.Services.Building.Group;
using FamTec.Server.Services.Building.Key;
using FamTec.Server.Services.Building.Value;
using FamTec.Server.Repository.Facility.Group;
using FamTec.Server.Repository.Facility.ItemKey;
using FamTec.Server.Repository.Facility.ItemValue;
using FamTec.Server.Services.Facility.Group;
using FamTec.Server.Services.Facility.Key;
using FamTec.Server.Services.Facility.Value;
using FamTec.Server.Services.Facility.Type.Machine;
using FamTec.Server.Services.Facility.Type.Electronic;
using FamTec.Server.Services.Facility.Type.Lift;
using FamTec.Server.Services.Facility.Type.Fire;
using FamTec.Server.Services.Facility.Type.Contstruct;
using FamTec.Server.Services.Facility.Type.Network;
using FamTec.Server.Services.Facility.Type.Beauty;
using FamTec.Server.Services.Facility.Type.Security;
using FamTec.Server.Repository.Store;
using FamTec.Server.Services.Store;
using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Maintenence;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Services.Voc.Hub;
using FamTec.Server.Services.Alarm;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using FamTec.Server.Services.Maintenance;
using FamTec.Server.Services.BlackList;
using FamTec.Server.Services.KakaoLog;
using Microsoft.AspNetCore.HttpOverrides;
using FamTec.Server.Repository.Meter;
using FamTec.Server.Repository.Meter.Contract;
using FamTec.Server.Services.Meter;
using FamTec.Server.Services.Meter.Contract;
using FamTec.Server.Repository.Meter.Energy;
using FamTec.Server.Services.Meter.Energy;


var builder = WebApplication.CreateBuilder(args);

string? Http = builder.Configuration["Kestrel:Endpoints:Http:Url"]; // HTTP
string? Https = builder.Configuration["Kestrel:Endpoints:Https:Url"]; // HTTPS

#region Kestrel 서버
builder.WebHost.UseKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
    // Keep-Alive TimeOut 3분설정 Keep-Alive 타임아웃: 일반적으로 2~5분. 너무 짧으면 연결이 자주 끊어질 수 있고, 너무 길면 리소스가 낭비될 수 있음.
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
    // 최대 동시 업그레이드 연결 수:  일반적으로 1000 ~ 5000 사이로 설정하는 것이 좋음
    options.Limits.MaxConcurrentUpgradedConnections = 3000; 
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        // 프로토콜 설정: HTTP/1.1과 HTTP/2를 모두 지원하는 것을 권장.
        // HTTP/2는 성능 향상과 효율적인 데이터 전송을 제공한다.
        endpointOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    /*
    options.Listen(IPAddress.Loopback, 5245, listenOptions =>
    {
        // SSL 설정 (경로, 비밀번호)
        listenOptions.UseHttps("path/to/cert.pfx"", "testPassword");
    });
    */
});

#endregion
/*
 분산 메모리 캐시를 설정.
 IDistributedCache 인터페이스의 기본 구현을 메모리에 저장하도록 추가함.
 이를 통해 애플리케이션은 캐시 데이터를 서버 메모리에 저장하고, 이를 여러 요청 간에 공유할 수 있습니다
 실제 분산 캐시는 아니지만 개발단계에서 유용하게 사용할 수 있음. - 실제사용시 IDistributedCache _cache; 를 구현해서 사용함.
*/
//builder.Services.AddDistributedMemoryCache();

/* HttpClientFactory 유용하게 사용하기 위함. */
//builder.Services.AddHttpClient();

// Add services to the container. - Repository
builder.Services.AddTransient<IPlaceInfoRepository, PlaceInfoRepository>();
builder.Services.AddTransient<IBuildingInfoRepository, BuildingInfoRepository>();
builder.Services.AddTransient<IBuildingGroupItemInfoRepository, BuildingGroupItemInfoRepository>();
builder.Services.AddTransient<IBuildingItemKeyInfoRepository, BuildingItemKeyInfoRepository>();
builder.Services.AddTransient<IBuildingItemValueInfoRepository, BuildingItemValueInfoRepository>();
builder.Services.AddTransient<IUserInfoRepository, UserInfoRepository>();
builder.Services.AddTransient<IAdminUserInfoRepository, AdminUserInfoRepository>();
builder.Services.AddTransient<IAdminPlacesInfoRepository, AdminPlaceInfoRepository>();
builder.Services.AddTransient<IFloorInfoRepository, FloorInfoRepository>();
builder.Services.AddTransient<IDepartmentInfoRepository, DepartmentInfoRepository>();
builder.Services.AddTransient<IRoomInfoRepository, RoomInfoRepository>();
builder.Services.AddTransient<IUnitInfoRepository, UnitInfoRepository>();
builder.Services.AddTransient<IVocInfoRepository, VocInfoRepository>();
builder.Services.AddTransient<IAlarmInfoRepository, AlarmInfoRepository>();
builder.Services.AddTransient<IMaterialInfoRepository, MaterialInfoRepository>();
builder.Services.AddTransient<IVocCommentRepository, VocCommentRepository>();
builder.Services.AddTransient<IFacilityInfoRepository, FacilityInfoRepository>();
builder.Services.AddTransient<IFacilityGroupItemInfoRepository, FacilityGroupItemInfoRepository>();
builder.Services.AddTransient<IFacilityItemKeyInfoRepository, FacilityItemKeyInfoRepository>();
builder.Services.AddTransient<IFacilityItemValueInfoRepository, FacilityItemValueInfoRepository>();
builder.Services.AddTransient<IInventoryInfoRepository, InventoryInfoRepository>();
builder.Services.AddTransient<IStoreInfoRepository, StoreInfoRepository>();
builder.Services.AddTransient<IMaintanceRepository, MaintanceRepository>();
builder.Services.AddTransient<IBlackListInfoRepository, BlackListInfoRepository>();
builder.Services.AddTransient<IKakaoLogInfoRepository, KakaoLogInfoRepository>();
builder.Services.AddTransient<IBlackListInfoRepository, BlackListInfoRepository>();
builder.Services.AddTransient<IMeterInfoRepository, MeterInfoRepository>();
builder.Services.AddTransient<IContractInfoRepository, ContractInfoRepository>();
builder.Services.AddTransient<IEnergyInfoRepository, EnergyInfoRepository>();

// Add services to the container. - Logic
builder.Services.AddTransient<IAdminAccountService, AdminAccountService>();
builder.Services.AddTransient<IAdminPlaceService, AdminPlaceService>();
builder.Services.AddTransient<IBuildingService, BuildingService>();
builder.Services.AddTransient<IBuildingGroupService, BuildingGroupService>();
builder.Services.AddTransient<IBuildingKeyService, BuildingKeyService>();
builder.Services.AddTransient<IBuildingValueService, BuildingValueService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IFloorService, FloorService>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IUnitService, UnitService>();
builder.Services.AddTransient<IHubService, HubService>();
builder.Services.AddTransient<IVocService, VocService>();
builder.Services.AddTransient<ILogService, LogService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IMaterialService, MaterialService>();
builder.Services.AddTransient<IVocCommentService, VocCommentService>();
builder.Services.AddTransient<IMachineFacilityService, MachineFacilityService>(); // 기계설비 비즈니스 로직
builder.Services.AddTransient<IElectronicFacilityService, ElectronicFacilityService>(); // 전기설비 비즈니스 로직
builder.Services.AddTransient<ILiftFacilityService, LiftFacilityService>(); // 승강설비 비즈니스 로직
builder.Services.AddTransient<IFireFacilityService, FireFacilityService>(); // 소방설비 비즈니스 로직
builder.Services.AddTransient<IConstructFacilityService, ConstructFacilityService>(); // 건축설비 비즈니스 로직
builder.Services.AddTransient<INetworkFacilityService, NetworkFacilityService>(); // 통신설비 비즈니스 로직
builder.Services.AddTransient<IBeautyFacilityService, BeautyFacilityService>(); // 미화설비 비즈니스 로직
builder.Services.AddTransient<ISecurityFacilityService, SecurityFacilityService>(); // 보안설비 비즈니스 로직
builder.Services.AddTransient<IFacilityGroupService, FacilityGroupService>();
builder.Services.AddTransient<IFacilityKeyService, FacilityKeyService>();
builder.Services.AddTransient<IFacilityValueService, FacilityValueService>();
builder.Services.AddTransient<IInVentoryService, InVentoryService>();
builder.Services.AddTransient<IMaintanceService, MaintanceService>();
builder.Services.AddTransient<ITokenComm, TokenComm>();
builder.Services.AddTransient<IKakaoService, KakaoService>();
builder.Services.AddTransient<IAlarmService, AlarmService>();
builder.Services.AddTransient<IBlackListService, BlackListService>();
builder.Services.AddTransient<IKakaoLogService, KakaoLogService>();
builder.Services.AddTransient<IMeterService, MeterService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IEnergyService, EnergyService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region 속도제한 LIMIT 
/*
// Fixed windoww limit 알고리즘 방식으로 속도 제한 처리
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "LimiterPolicy", options =>
{
    // 요청 허용 갯수 :1
    options.PermitLimit = 1;
    // 창 이동시간 5초 [5초 동안 최대 1개의 요청만 처리 가능]
    options.Window = TimeSpan.FromSeconds(5);
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    // 제한 시 10개의 요청만 대기열에 추가
    options.QueueLimit = 10;
}));
*/
#endregion
#region JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true; // 토큰을 저장하도록 설정
    options.RequireHttpsMetadata = true; // HTTPS 요구
    // 토큰검증 매개변수
    options.TokenValidationParameters = new TokenValidationParameters() 
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "https://sws.s-tec.co.kr/",
        ValidIssuer = "https://sws.s-tec.co.kr/",
        //ValidAudience = "https://localhost:5245/",
        //ValidIssuer = "https://localhost:5245/",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:authSigningKey"]!))
    };
});
#endregion

#region DB연결 정보
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!String.IsNullOrWhiteSpace(connectionString))
{
    //builder.Services.AddDbContext<WorksContext>(options =>
    //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    builder.Services.AddDbContext<WorksContext>(options =>
      options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
      mySqlOptions =>
      {
          mySqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // 자동 재시도 설정 (최대 3회, 5초 대기)
          // CommandTimeout을 60초로 설정
          mySqlOptions.CommandTimeout(60);
          // 다른 성능 및 안정성 옵션 추가
          mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // 복잡한 쿼리의 성능 향상을 위한 쿼리 분할 사용
      }));
}
else
    // 예외를 던져 프로그램이 시작되지 않도록 함.
    throw new InvalidOperationException("Connection string 'DefaultConnection' is null or empty."); 
#endregion


#region SIGNAL R 등록
builder.Services.AddSignalR().AddHubOptions<BroadcastHub>(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
});
#endregion

#region SIGNAL R CORS 등록


#if DEBUG
// 개발용
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        //policy.AllowAnyOrigin()
        policy.WithOrigins(
            "http://localhost:5245",
            "https://localhost:5246",
            "http://127.0.0.1:5245",
            "https://127.0.0.1:5246",
            "http://123.2.156.28:5247",
            "https://123.2.156.28:5248",
            "http://123.2.156.148:5245", 
            "https://123.2.156.148:5246", 
            "http://123.2.156.229:5245", 
            "https://123.2.156.229:5246")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // With Origins만 사용가능
    });
});
#else
// 배포용
var MyAllowSpectificOrigins = "MyPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpectificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://123.2.156.229:5245")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });

});
#endif


builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true; // HTTPS 요청에서도 응답압축 활성화
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
        {
            "application/octet-stream",
            "application/json",
            "application/xml",
            "text/plain",
            "text/css",
            "text/javascript"
        }).Except(new[] { "text/html" });
    opts.Providers.Add<BrotliCompressionProvider>();
    opts.Providers.Add<GzipCompressionProvider>();
});



// Brotli와 Gzip 압축 제공자 설정
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
#endregion

var app = builder.Build();
/*
#region 속도제한 사용
app.UseRateLimiter();
#endregion
*/

//app.UseHttpsRedirection();

#region 역방향 프록시 서버 사용
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

#endregion



#region SIGNALR HUB 사용
app.UseResponseCompression();
app.MapHub<BroadcastHub>("/VocHub");
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection(); // 위치변경
}

app.UseBlazorFrameworkFiles(); // Blazor 정적 파일 제공 설정
app.UseStaticFiles(); // 정적 파일 제공 설정

app.UseRouting();

#region CORS 사용
#if DEBUG
app.UseCors();
#else
app.UseCors(MyAllowSpectificOrigins);
#endif
#endregion

#region MiddleWare

string[]? adminPaths = new string[]
{
    "/api/AdminUser/sign", // AdminUser [설정] 컨트롤러 미들웨어 추가
    "/api/Department/sign", // Department [설정] 컨트롤러 미들웨어 추가
    "/api/AdminPlace/sign" // AdminPlace [설정] 컨트롤러 미들웨어 추가
};

string[]? userPaths = new string[]
{
    "/api/Login/sign", // Login 컨트롤러 미들웨어 추가
    "/api/Voc/sign", // Voc 컨트롤러 미들웨어 추가
    "/api/Building/sign", // Building 컨트롤러 미들웨어 추가
    "/api/Unit/sign", // Unit 컨트롤러 미들웨어 추가
    "/api/Room/sign", // Room 컨트롤러 미들웨어 추가
    "/api/User/sign", // User 컨트롤러 미들웨어 추가
    "/api/Floor/sign", // Floor 컨트롤러 미들웨어 추가
    "/api/Material/sign", // Material 컨트롤러 미들웨어 추가
    "/api/VocComment/sign", // VocComment 컨트롤러 미들웨어 추가
    "/api/BuildingGroup/sign", // BuildingGroup 컨트롤러 미들웨어 추가
    "/api/BuildingGroupKey/sign", // BuildingGroupKey 컨트롤러 미들웨어 추가
    "/api/BuildingGroupValue/sign", // BuildingGroupValue 컨트롤러 미들웨어 추가
    "/api/MachineFacility/sign", // 기계설비 컨트롤러 미들웨어 추가
    "/api/ElectronicFacility/sign", // 전기설비 컨트롤러 미들웨어 추가
    "/api/LiftFacility/sign", // 승강설비 컨트롤러 미들웨어 추가
    "/api/FireFacility/sign", // 소방설비 컨트롤러 미들웨어 추가
    "/api/ConstructFacility/sign", // 건축설비 컨트롤러 미들웨어 추가
    "/api/NetworkFacility/sign", // 통신설비 컨트롤러 미들웨어 추가
    "/api/BeautyFacility/sign", // 미화설비 컨트롤러 미들웨어 추가
    "/api/SecurityFacility/sign", // 보안설비 컨트롤러 미들웨어 추가
    "/api/FacilityGroup/sign", // 설비그룹 컨트롤러 미들웨어 추가
    "/api/FacilityGroupKey/sign", // 설비그룹 Key 컨트롤러 미들에어 추가
    "/api/FacilityGroupValue/sign", // 설비그룹 Value 컨트롤러 미들웨어 추가
    "/api/Store/sign",
    "/api/Maintenance/sign",
    "/api/Alarm/sign",
    "/api/BlackList/sign",
    "/api/KakaoLog/sign",
    "/api/Meter/sign",
    "/api/Contract/sign",
    "/api/Energy/sign"
};



foreach (var path in adminPaths)
{
    app.UseWhen(context => context.Request.Path.StartsWithSegments(path), appBuilder =>
    {
        appBuilder.UseMiddleware<AdminMiddleware>();
    });
}

foreach (var path in userPaths)
{
    app.UseWhen(context => context.Request.Path.StartsWithSegments(path), appBuilder =>
    {
        appBuilder.UseMiddleware<UserMiddleware>();
    });
}

#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
/*
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
*/
app.MapControllers();
app.MapFallbackToFile("index.html");

WorksSetting settings = new();
await settings.DefaultSetting();
app.Run();