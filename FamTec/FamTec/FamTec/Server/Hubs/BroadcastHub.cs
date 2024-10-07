using FamTec.Server.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FamTec.Server.Hubs
{
    public class BroadcastHub : Hub
    {
        // 각 connectionId가 가입된 그룹 목록을 관리하는 ConcurrentDictionary
        private static ConcurrentDictionary<string, HashSet<string>> _connectionGroups = new ConcurrentDictionary<string, HashSet<string>>();

        private ILogService LogService;

        public BroadcastHub(ILogService _logservice)
        {
            this.LogService = _logservice;
        }

        /// <summary>
        /// SIGNALR JOIN GROUP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task JoinRoomAsync(string roomName)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

                // 해당 connectionId의 그룹 목록 업데이트
                if (_connectionGroups.TryGetValue(Context.ConnectionId, out var existingGroupList))
                {
                    lock (existingGroupList)
                    {
                        existingGroupList.Add(roomName);
                    }
                }
                else
                {
                    var newGroupList = new HashSet<string> { roomName };
                    if (!_connectionGroups.TryAdd(Context.ConnectionId, newGroupList))
                    {
                        // 이미 다른 스레드에 의해 추가된 경우 다시 시도
                        if (_connectionGroups.TryGetValue(Context.ConnectionId, out existingGroupList))
                        {
                            lock (existingGroupList)
                            {
                                existingGroupList.Add(roomName);
                            }
                        }
                    }
                }

                Console.WriteLine("그룹추가 :" + _connectionGroups.Count + "/" + Context.ConnectionId + "/" + roomName + "Join");
                //await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} {roomName} Join Success");
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
            }
        }

        /// <summary>
        /// SIGNALR REMOVE GROUP
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task RemoveRoomAsync(string roomName)
        {
            try
            {

                // 그룹에서 사용자 제거
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

                Console.WriteLine($"그룹삭제 : {_connectionGroups.Count} / {Context.ConnectionId} / {roomName} Remove");

                // 해당 connectionId의 그룹 목록 업데이트
                if (_connectionGroups.TryGetValue(Context.ConnectionId, out var groupList))
                {
                    lock (groupList)
                    {
                        groupList.Remove(roomName);
                        if (groupList.Count == 0)
                        {
                            _connectionGroups.TryRemove(Context.ConnectionId, out _);
                        }
                    }
                }

                //await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} {roomName} Remove Success");
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
            }
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                // 연결이 끊어진 사용자를 모든 그룹에서 제거
                await RemoveFromAllGroups(Context.ConnectionId);

                // 필요한 경우 연결 끊김에 대한 추가 로직을 처리
                // 예: 다른 클라이언트에게 알림 보내기, 데이터베이스 상태 업데이트 등
                await base.OnDisconnectedAsync(exception);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
            }
        }

        public async Task RemoveFromAllGroups(string connectionId)
        {
            try
            {
                if (_connectionGroups.TryGetValue(connectionId, out var groups))
                {
                    foreach (var group in groups)
                    {
                        await Groups.RemoveFromGroupAsync(connectionId, group);
                        await Clients.Group(group).SendAsync("ReceiveMessage", $"{connectionId} has been removed from the group {group}");
                    }

                    // 그룹 목록에서 제거
                    _connectionGroups.TryRemove(connectionId, out _);
                }

                Console.WriteLine("그룹삭제 :" + _connectionGroups.Count + "/" + connectionId);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
            }
        }


    }
}
