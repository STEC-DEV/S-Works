using FamTec.Server.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FamTec.Server.Hubs
{
    public class BroadcastHub : Hub
    {
        // 각 connectionId가 가입한 그룹 목록을 ConcurrentDictionary<string, byte>로 관리합니다.
        // byte는 단순히 값이 필요 없으므로 자리 표시자로 사용합니다.
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _connectionGroups = 
            new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();

        private readonly ILogService LogService;
        private readonly ConsoleLogService<BroadcastHub> CreateBuilderLogger;

        public BroadcastHub(ILogService _logservice, ConsoleLogService<BroadcastHub> _createbuilderlogger)
        {
            LogService = _logservice;
            CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 그룹에 가입합니다.
        /// </summary>
        /// <param name="roomName">가입할 그룹명</param>
        /// <returns></returns>
        public async Task JoinRoomAsync(string roomName)
        {
            try
            {
                // SignalR 내장 메서드를 통해 그룹에 추가
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

                // _connectionGroups에 해당 connectionId가 없으면 새 ConcurrentDictionary를 생성하고,
                // 이미 존재하면 기존 딕셔너리에 그룹명을 추가합니다.
                var groups = _connectionGroups.GetOrAdd(
                    Context.ConnectionId,
                    _ => new ConcurrentDictionary<string, byte>());

                groups.TryAdd(roomName, 0);

#if DEBUG
                Console.WriteLine($"SIGNALR {nameof(JoinRoomAsync)} 호출");
                CreateBuilderLogger.ConsoleText("그룹 추가: " + _connectionGroups.Count + " / " + Context.ConnectionId + " / " + roomName + " Join");
#endif
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
            }
        }

        /// <summary>
        /// 그룹에서 탈퇴합니다.
        /// </summary>
        /// <param name="roomName">탈퇴할 그룹명</param>
        /// <returns></returns>
        public async Task RemoveRoomAsync(string roomName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

#if DEBUG
                Console.WriteLine($"SIGNALR {nameof(RemoveRoomAsync)} 호출");
                CreateBuilderLogger.ConsoleText($"그룹 삭제: {_connectionGroups.Count} / {Context.ConnectionId} / {roomName} Remove");
#endif

                if (_connectionGroups.TryGetValue(Context.ConnectionId, out var groups))
                {
                    groups.TryRemove(roomName, out _);

                    // 더 이상 가입한 그룹이 없으면 전체 connectionId 항목도 제거합니다.
                    if (groups.IsEmpty)
                    {
                        _connectionGroups.TryRemove(Context.ConnectionId, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
            }
        }

        /// <summary>
        /// 연결 종료 시 모든 그룹에서 해당 connectionId를 제거합니다.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                await RemoveFromAllGroups(Context.ConnectionId);
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
            }
        }

        /// <summary>
        /// 주어진 connectionId가 가입한 모든 그룹에서 제거합니다.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task RemoveFromAllGroups(string connectionId)
        {
            try
            {
#if DEBUG
                Console.WriteLine($"SIGNALR {nameof(RemoveFromAllGroups)} 호출");
#endif
                // _connectionGroups에서 해당 connectionId에 대한 그룹 목록을 꺼내고, 동시에 제거합니다.
                if (_connectionGroups.TryRemove(connectionId, out var groups))
                {
                    // ConcurrentDictionary의 Keys 열거는 스레드 안전하므로 별도 락 없이 순회가 가능합니다.
                    foreach (var roomName in groups.Keys)
                    {
                        await Groups.RemoveFromGroupAsync(connectionId, roomName);
                    }
#if DEBUG
                    CreateBuilderLogger.ConsoleText("그룹 삭제: " + _connectionGroups.Count + " / " + connectionId);
#endif       
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
            }
        }

#region Regacy
        /*
        // 각 connectionId가 가입된 그룹 목록을 관리하는 ConcurrentDictionary
        private static ConcurrentDictionary<string, HashSet<string>> _connectionGroups = new ConcurrentDictionary<string, HashSet<string>>();

        private readonly ILogService LogService;
        private readonly ConsoleLogService<BroadcastHub> CreateBuilderLogger;

        public BroadcastHub(ILogService _logservice, ConsoleLogService<BroadcastHub> _createbuilderlogger)
        {
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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

#if DEBUG
                CreateBuilderLogger.ConsoleText("그룹추가 :" + _connectionGroups.Count + "/" + Context.ConnectionId + "/" + roomName + "Join");
#endif

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
#if DEBUG
                CreateBuilderLogger.ConsoleText($"그룹삭제 : {_connectionGroups.Count} / {Context.ConnectionId} / {roomName} Remove");
#endif
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
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
                    }

                    // 그룹 목록에서 제거
                    _connectionGroups.TryRemove(connectionId, out _);
                }
#if DEBUG
                CreateBuilderLogger.ConsoleText("그룹삭제 :" + _connectionGroups.Count + "/" + connectionId);
#endif       
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
            }
        }
        */
#endregion
    }
}
