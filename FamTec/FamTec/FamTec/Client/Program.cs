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
builder.Services.AddBlazoredSessionStorage(); // ���� ���丮�� ����
builder.Services.AddAuthorizationCore(); // ���Ѻο� ����

//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>(); // ���� ���� ������ ����
builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // ����� ���� ���� ���� ������

builder.Services.AddScoped<ApiManager>();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddAuthorizationCore();


// ���� -- �Ʒ� �ڵ� (�Խÿ�)
//string HubUrl = $"{builder.HostEnvironment.BaseAddress}VocHub";

string HubUrl = "http://123.2.156.148:5245/VocHub";


HubObject.hubConnection = new HubConnectionBuilder()
      .WithUrl(HubUrl, options =>
      {
          options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents |
                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
      })
    .WithAutomaticReconnect() // �������� ������ �������� �ڵ����� �翬��
    .ConfigureLogging(logging =>
    {
        //logging.AddConsole();
        // This will set ALL logging to Debug level
        logging.SetMinimumLevel(LogLevel.Debug);
    })
   .Build();

HubObject.hubConnection.KeepAliveInterval = System.TimeSpan.FromSeconds(15); //�ּ� ���������� ��5��.
HubObject.hubConnection.ServerTimeout = System.TimeSpan.FromSeconds(30); // �����κ��� 30�� �ȿ� �޽����� ���� ���ϸ� Ŭ���̾�Ʈ�� ����

try
{
    await HubObject.hubConnection.StartAsync();
}
catch(Exception ex)
{
    Console.WriteLine($"SignalR ���� �� ���� �߻�: {ex.Message}");
}
await builder.Build().RunAsync();

//string HubUrl = $"{builder.HostEnvironment.BaseAddress}VocHub";
//HubObject.hubConnection = new HubConnectionBuilder()
//      .WithUrl(HubUrl, options =>
//      {
//          options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
//                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents |
//                               Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
//      })
//    .WithAutomaticReconnect() // �������� ������ �������� �ڵ����� �翬��
//    .ConfigureLogging(logging =>
//    {
//        //logging.AddConsole();
//        // This will set ALL logging to Debug level
//        logging.SetMinimumLevel(LogLevel.Debug);
//    })
//   .Build();