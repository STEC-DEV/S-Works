
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

builder.Services.AddScoped<CustomAuthenticationStateProvider>();


// ����
//HubObject.hubConnection = new HubConnectionBuilder()
//    .WithUrl("http://123.2.156.148:5245/VocHub", transports: Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling) // �ڿ� �ٴ� url�� ������� ���⸸ �ϸ� �Ǵ��� check
//    .WithAutomaticReconnect() // �������� ������ �������� �ڵ����� �翬��
//    .ConfigureLogging(logging =>
//    {
//        //logging.AddConsole();
//        // This will set ALL logging to Debug level
//        logging.SetMinimumLevel(LogLevel.Debug);
//    })
//    .Build();

//HubObject.hubConnection.KeepAliveInterval = System.TimeSpan.FromSeconds(15); //�ּ� ���������� ��5��.
//HubObject.hubConnection.ServerTimeout = System.TimeSpan.FromSeconds(30); // �����κ��� 30�� �ȿ� �޽����� ���� ���ϸ� Ŭ���̾�Ʈ�� ����

//await HubObject.hubConnection.StartAsync();

await builder.Build().RunAsync();
