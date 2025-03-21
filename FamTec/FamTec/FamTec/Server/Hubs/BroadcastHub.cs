using FamTec.Server.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FamTec.Server.Hubs
{
    public class BroadcastHub : Hub
    {
        /*
            ConCurrentDictionary
            - 스레드 안전(Thread-Safe)한 사전(Dictionary) 컬렉션이다.
            일반 Dictionary와 달리, 여러 스레드가 동시에 읽기 및 쓰기 작업을 수행할 때 별도의 동기화(lock)을 직접 구현하지 않아도 안전하게 데이터를 추가, 삭제, 수정할 수 있도록 설계됨

            - 특징
                1. 스레드 안전
                    : 여러 스레드가 동시에 접근해도 데이터 무결성을 보장한다.
                2. 내부 잠금 관리
                    : 내부적으로 잠금(lock) 메커니즘을 사용하여 동시 업데이트 시 충돌을 방지한다. - 개발자가 별도로 락을 구현안해도 됨
                3. 성능 최적화
    \               : 높은 동시성을 요구하는 환경에서 성능 저하 없이 안전하게 사용할 수 있다.
         */
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
    }
}
