using Microsoft.AspNetCore.SignalR;

namespace FamTec.Server.Hubs
{
    public class BroadcastHub : Hub
    {
        /// <summary>
        /// SIGNALR JOIN GROUP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task JoinRoomAsync(string roomName)
        {
            /*
                DB 검증로직
                    - DB의 USERID의 실제권한.
                    - (이 사람이 Role이 VOC 알람을 받는 사람이 맞는지.
             */
            Console.WriteLine(roomName);
            Console.WriteLine(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} {roomName} Join Success");
        }

        /// <summary>
        /// SIGNALR REMOVE GROUP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task RemoveRoomAsync(string roomName)
        {
            Console.WriteLine(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} {roomName} Remove Success");
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            string userId = Context.UserIdentifier ?? "UnknownUser";

            // 필요한 경우 연결 끊김에 대한 추가 로직을 처리
            // 예: 다른 클라이언트에게 알림 보내기, 데이터베이스 상태 업데이트 등

            await base.OnDisconnectedAsync(exception);
        }



    }
}
