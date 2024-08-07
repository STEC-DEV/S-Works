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
using FamTec.Server.Services.Admin.Maintenance;
using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Services.Voc.Hub;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

//builder.Services.AddHttpClient();



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

// Add services to the container.
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

builder.Services.AddTransient<IMachineFacilityService, MachineFacilityService>(); // ��輳��
builder.Services.AddTransient<IElectronicFacilityService, ElectronicFacilityService>(); // ���⼳��
builder.Services.AddTransient<ILiftFacilityService, LiftFacilityService>(); // �°�����
builder.Services.AddTransient<IFireFacilityService, FireFacilityService>(); // �ҹ漳��
builder.Services.AddTransient<IConstructFacilityService, ConstructFacilityService>(); // ���༳��
builder.Services.AddTransient<INetworkFacilityService, NetworkFacilityService>(); // ��ż���
builder.Services.AddTransient<IBeautyFacilityService, BeautyFacilityService>(); // ��ȭ����
builder.Services.AddTransient<ISecurityFacilityService, SecurityFacilityService>(); // ���ȼ���


builder.Services.AddTransient<IFacilityGroupService, FacilityGroupService>();
builder.Services.AddTransient<IFacilityKeyService, FacilityKeyService>();
builder.Services.AddTransient<IFacilityValueService, FacilityValueService>();

builder.Services.AddTransient<IInVentoryService, InVentoryService>();

builder.Services.AddTransient<IMaintanceService, MaintanceService>();

builder.Services.AddTransient<ITokenComm, TokenComm>();

builder.Services.AddTransient<IKakaoService, KakaoService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


#region JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "https://localhost:7114/",
        ValidIssuer = "https://localhost:7114/",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:authSigningKey"]!))
    };
});
#endregion

string? HostUrl = builder.Configuration["Kestrel:Endpoints:MyHttpEndpoint:Url"];

#region DB���� ����
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WorksContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
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
// ���ο�
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7114", "http://123.2.156.148:5245", "http://123.2.156.229:5245", "http://123.2.156.28:5245", HostUrl)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed((host) => true);
    });
});
#else
// ������
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(HostUrl)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed((host) => true);
    });
});
#endif

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

#endregion



var app = builder.Build();


#region CORS ���
app.UseCors();
#endregion

#region SIGNALR HUB ���
app.UseResponseCompression();

app.MapHub<BroadcastHub>("/VocHub"); // �������� �����ؾ��ϳ�
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
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

#region MiddleWare

// [����] AdminUser ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/AdminUser/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<AdminMiddleware>();
});

// [����] Department ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Department/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<AdminMiddleware>();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/AdminPlace/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<AdminMiddleware>();
});

// Login ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Login/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Voc ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Voc/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Building ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Building/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Unit ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Unit/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Room ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Room/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// User ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/User/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Floor ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Floor/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});



// Material ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Material/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// VocComment ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/VocComment/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Group ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/BuildingGroup/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// GroupKey ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/BuildingGroupKey/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// GroupValue ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/BuildingGroupValue/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility ��� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/MachineFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});
// Facility ���� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/ElectronicFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility �°� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/LiftFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility �ҹ� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/FireFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility ���� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/ConstructFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility ��� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/NetworkFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility ��ȭ ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/BeautyFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility ���� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/SecurityFacility/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Facility �׷� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/FacilityGroup/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});
// Facility Ű ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/FacilityGroupKey/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});
// Facility �� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/FacilityGroupValue/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// InStore �� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Store/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

// Maintenence �� ��Ʈ�ѷ� �̵���� �߰�
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Maintenance/sign"), appBuilder =>
{
    appBuilder.UseMiddleware<UserMiddleware>();
});

#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

//Console.WriteLine($"{AppDomain.CurrentDomain.BaseDirectory} : �⺻���");
/*
app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Get && context.Request.Query["mdw"] == "test")
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Middleware running.\n");
    }

    await next();
});
*/
//app.MapGet("/", () => "Hello World!");

//app.MapGet("/hi", () => "Hello!");
/*
app.Use(async (context, next) =>
{
    Console.WriteLine("Use Middleware1 Incoming Request \n");

    await next();
    Console.WriteLine("Use Middleware1 Outgoing Response \n");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Use Middleware2 Incoming Request \n");
    await next();
    Console.WriteLine("Use Middleware2 Outgoing Response \n");
});
app.Run();
*/


WorksSetting settings = new();
await settings.DefaultSetting();

app.Run();