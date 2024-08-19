
using FamTec.Client;
using FamTec.Client.Middleware;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Tewr.Blazor.FileReader;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using FamTec.Client.Shared.Provider;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456;
});

builder.Services.AddFileReaderService(options =>
{
    options.UseWasmSharedBuffer = true;
});

builder.Services.AddScoped<SessionService>();


builder.Services.AddScoped<ApiManager>();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddAuthorizationCore();


// 연결 -- 아래 코드 (게시용)
//string HubUrl = $"{builder.HostEnvironment.BaseAddress}VocHub";
string HubUrl = "http://123.2.156.148:5245/VocHub/";
HubObject.hubConnection = new HubConnectionBuilder()
    .WithUrl(HubUrl, transports: Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling) // 뒤에 붙는 url은 상관없이 같기만 하면 되는지 check
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

await HubObject.hubConnection.StartAsync();

await builder.Build().RunAsync();

