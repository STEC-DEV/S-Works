using Microsoft.AspNetCore.SignalR.Client;

namespace FamTec.Client
{
    public class HubObject
    {
        public static HubConnection? hubConnection { get; set; }
    }
}
