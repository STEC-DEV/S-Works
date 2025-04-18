using FamTec.Client;
using FamTec.Client.Middleware;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Tewr.Blazor.FileReader;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FamTec.Client.Shared.Provider;
using DevExpress.Blazor;
using MudBlazor.Services;
using Radzen;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456; // 256MB
});

builder.Services.AddFileReaderService(options =>
{
    options.UseWasmSharedBuffer = true;
});

builder.Services.AddScoped<SessionService>();

builder.Services.AddScoped<ApiManager>();
builder.Services.AddBlazoredSessionStorage(); // 세션 스토리지 서비스
builder.Services.AddAuthorizationCore(); // 권한부여 서비스

//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>(); // 인증 상태 공급자 주입
builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // 사용자 정의 인증 상태 공급자

builder.Services.AddScoped<ApiManager>();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddScoped<PermissionService>();
//devexpress
builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = BootstrapVersion.v5);
//mudblazor
builder.Services.AddMudServices();
//RadzenBlazor
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();




// 연결 -- 아래 코드 (게시용)

#if DEBUG
//string HubUrl = "http://123.2.156.148:5245/VocHub";
string HubUrl = $"{builder.HostEnvironment.BaseAddress}VocHub";
#else
string HubUrl = "https://sws.s-tec.co.kr/VocHub";
#endif

HubObject.hubConnection = new HubConnectionBuilder()
      .WithUrl(HubUrl, options =>
      {
          options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents |
                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
      })
    .WithAutomaticReconnect() // 서버와의 연결이 끊어지면 자동으로 재연결
    .ConfigureLogging(logging =>
    {
        //logging.AddConsole();
        // This will set ALL logging to Debug level
        logging.SetMinimumLevel(LogLevel.Debug);
    })
   .Build();

HubObject.hubConnection.KeepAliveInterval = System.TimeSpan.FromSeconds(15); //최소 설정가능한 값5초.
HubObject.hubConnection.ServerTimeout = System.TimeSpan.FromSeconds(30); // 서버로부터 30초 안에 메시지를 수신 못하면 클라이언트가 끊음

try
{
    await HubObject.hubConnection.StartAsync();
}
catch(Exception ex)
{
    Console.WriteLine($"SignalR 연결 중 오류 발생: {ex.Message}");
}
await builder.Build().RunAsync();

