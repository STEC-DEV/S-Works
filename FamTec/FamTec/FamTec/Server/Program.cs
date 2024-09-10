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

#region Kestrel ����
builder.WebHost.UseKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
    // Keep-Alive TimeOut 3�м��� Keep-Alive Ÿ�Ӿƿ�: �Ϲ������� 2~5��. �ʹ� ª���� ������ ���� ������ �� �ְ�, �ʹ� ��� ���ҽ��� ����� �� ����.
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
    // �ִ� ���� ���׷��̵� ���� ��:  �Ϲ������� 1000 ~ 5000 ���̷� �����ϴ� ���� ����
    options.Limits.MaxConcurrentUpgradedConnections = 3000; 
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        // �������� ����: HTTP/1.1�� HTTP/2�� ��� �����ϴ� ���� ����.
        // HTTP/2�� ���� ���� ȿ������ ������ ������ �����Ѵ�.
        endpointOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    /*
    options.Listen(IPAddress.Loopback, 5245, listenOptions =>
    {
        // SSL ���� (���, ��й�ȣ)
        listenOptions.UseHttps("path/to/cert.pfx"", "testPassword");
    });
    */
});

#endregion
/*
 �л� �޸� ĳ�ø� ����.
 IDistributedCache �������̽��� �⺻ ������ �޸𸮿� �����ϵ��� �߰���.
 �̸� ���� ���ø����̼��� ĳ�� �����͸� ���� �޸𸮿� �����ϰ�, �̸� ���� ��û ���� ������ �� �ֽ��ϴ�
 ���� �л� ĳ�ô� �ƴ����� ���ߴܰ迡�� �����ϰ� ����� �� ����. - �������� IDistributedCache _cache; �� �����ؼ� �����.
*/
//builder.Services.AddDistributedMemoryCache();

/* HttpClientFactory �����ϰ� ����ϱ� ����. */
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
builder.Services.AddTransient<IMachineFacilityService, MachineFacilityService>(); // ��輳�� ����Ͻ� ����
builder.Services.AddTransient<IElectronicFacilityService, ElectronicFacilityService>(); // ���⼳�� ����Ͻ� ����
builder.Services.AddTransient<ILiftFacilityService, LiftFacilityService>(); // �°����� ����Ͻ� ����
builder.Services.AddTransient<IFireFacilityService, FireFacilityService>(); // �ҹ漳�� ����Ͻ� ����
builder.Services.AddTransient<IConstructFacilityService, ConstructFacilityService>(); // ���༳�� ����Ͻ� ����
builder.Services.AddTransient<INetworkFacilityService, NetworkFacilityService>(); // ��ż��� ����Ͻ� ����
builder.Services.AddTransient<IBeautyFacilityService, BeautyFacilityService>(); // ��ȭ���� ����Ͻ� ����
builder.Services.AddTransient<ISecurityFacilityService, SecurityFacilityService>(); // ���ȼ��� ����Ͻ� ����
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

#region �ӵ����� LIMIT 
/*
// Fixed windoww limit �˰��� ������� �ӵ� ���� ó��
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "LimiterPolicy", options =>
{
    // ��û ��� ���� :1
    options.PermitLimit = 1;
    // â �̵��ð� 5�� [5�� ���� �ִ� 1���� ��û�� ó�� ����]
    options.Window = TimeSpan.FromSeconds(5);
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    // ���� �� 10���� ��û�� ��⿭�� �߰�
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
    options.SaveToken = true; // ��ū�� �����ϵ��� ����
    options.RequireHttpsMetadata = true; // HTTPS �䱸
    // ��ū���� �Ű�����
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

#region DB���� ����
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!String.IsNullOrWhiteSpace(connectionString))
{
    //builder.Services.AddDbContext<WorksContext>(options =>
    //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    builder.Services.AddDbContext<WorksContext>(options =>
      options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
      mySqlOptions =>
      {
          mySqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // �ڵ� ��õ� ���� (�ִ� 3ȸ, 5�� ���)
          // CommandTimeout�� 60�ʷ� ����
          mySqlOptions.CommandTimeout(60);
          // �ٸ� ���� �� ������ �ɼ� �߰�
          mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // ������ ������ ���� ����� ���� ���� ���� ���
      }));
}
else
    // ���ܸ� ���� ���α׷��� ���۵��� �ʵ��� ��.
    throw new InvalidOperationException("Connection string 'DefaultConnection' is null or empty."); 
#endregion


#region SIGNAL R ���
builder.Services.AddSignalR().AddHubOptions<BroadcastHub>(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
});
#endregion

#region SIGNAL R CORS ���


#if DEBUG
// ���߿�
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
        .AllowCredentials(); // With Origins�� ��밡��
    });
});
#else
// ������
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
    opts.EnableForHttps = true; // HTTPS ��û������ ������� Ȱ��ȭ
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



// Brotli�� Gzip ���� ������ ����
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
#region �ӵ����� ���
app.UseRateLimiter();
#endregion
*/

//app.UseHttpsRedirection();

#region ������ ���Ͻ� ���� ���
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

#endregion



#region SIGNALR HUB ���
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
    app.UseHttpsRedirection(); // ��ġ����
}

app.UseBlazorFrameworkFiles(); // Blazor ���� ���� ���� ����
app.UseStaticFiles(); // ���� ���� ���� ����

app.UseRouting();

#region CORS ���
#if DEBUG
app.UseCors();
#else
app.UseCors(MyAllowSpectificOrigins);
#endif
#endregion

#region MiddleWare

string[]? adminPaths = new string[]
{
    "/api/AdminUser/sign", // AdminUser [����] ��Ʈ�ѷ� �̵���� �߰�
    "/api/Department/sign", // Department [����] ��Ʈ�ѷ� �̵���� �߰�
    "/api/AdminPlace/sign" // AdminPlace [����] ��Ʈ�ѷ� �̵���� �߰�
};

string[]? userPaths = new string[]
{
    "/api/Login/sign", // Login ��Ʈ�ѷ� �̵���� �߰�
    "/api/Voc/sign", // Voc ��Ʈ�ѷ� �̵���� �߰�
    "/api/Building/sign", // Building ��Ʈ�ѷ� �̵���� �߰�
    "/api/Unit/sign", // Unit ��Ʈ�ѷ� �̵���� �߰�
    "/api/Room/sign", // Room ��Ʈ�ѷ� �̵���� �߰�
    "/api/User/sign", // User ��Ʈ�ѷ� �̵���� �߰�
    "/api/Floor/sign", // Floor ��Ʈ�ѷ� �̵���� �߰�
    "/api/Material/sign", // Material ��Ʈ�ѷ� �̵���� �߰�
    "/api/VocComment/sign", // VocComment ��Ʈ�ѷ� �̵���� �߰�
    "/api/BuildingGroup/sign", // BuildingGroup ��Ʈ�ѷ� �̵���� �߰�
    "/api/BuildingGroupKey/sign", // BuildingGroupKey ��Ʈ�ѷ� �̵���� �߰�
    "/api/BuildingGroupValue/sign", // BuildingGroupValue ��Ʈ�ѷ� �̵���� �߰�
    "/api/MachineFacility/sign", // ��輳�� ��Ʈ�ѷ� �̵���� �߰�
    "/api/ElectronicFacility/sign", // ���⼳�� ��Ʈ�ѷ� �̵���� �߰�
    "/api/LiftFacility/sign", // �°����� ��Ʈ�ѷ� �̵���� �߰�
    "/api/FireFacility/sign", // �ҹ漳�� ��Ʈ�ѷ� �̵���� �߰�
    "/api/ConstructFacility/sign", // ���༳�� ��Ʈ�ѷ� �̵���� �߰�
    "/api/NetworkFacility/sign", // ��ż��� ��Ʈ�ѷ� �̵���� �߰�
    "/api/BeautyFacility/sign", // ��ȭ���� ��Ʈ�ѷ� �̵���� �߰�
    "/api/SecurityFacility/sign", // ���ȼ��� ��Ʈ�ѷ� �̵���� �߰�
    "/api/FacilityGroup/sign", // ����׷� ��Ʈ�ѷ� �̵���� �߰�
    "/api/FacilityGroupKey/sign", // ����׷� Key ��Ʈ�ѷ� �̵鿡�� �߰�
    "/api/FacilityGroupValue/sign", // ����׷� Value ��Ʈ�ѷ� �̵���� �߰�
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