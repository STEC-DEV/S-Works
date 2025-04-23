namespace FamTec.Server.Services
{
    public class ConsoleLogService<T>
    {
        private readonly ILogger<T> ConsoleLogger;
        private static readonly object _lock = new object();
        public ConsoleLogService(ILogger<T> _logger)
        {
            this.ConsoleLogger = _logger;
        }

        public void ConsoleText(string message)
        {
            lock (_lock)
            {
                // 현재 색상 백업
                var prevBg = Console.BackgroundColor;
                var prevFg = Console.ForegroundColor;
                try
                {
                    
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
                }
                finally
                {
                    // 예외 여부와 상관없이 색상 복원
                    Console.BackgroundColor = prevBg;
                    Console.ForegroundColor = prevFg;
                }
            }
        }

        public void ConsoleWarning(string message)
        {
            lock (_lock)
            {
                // 현재 색상 백업
                var prevBg = Console.BackgroundColor;
                var prevFg = Console.ForegroundColor;

                try
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;

                    // 로그 출력
                    Console.WriteLine($"[WARNING] {DateTime.Now}: {message}");
                }
                finally
                {
                    // 예외 여부와 상관없이 색상 복원
                    Console.BackgroundColor = prevBg;
                    Console.ForegroundColor = prevFg;
                }
            }
        }

        public void ConsoleLog(Exception ex)
        {
            lock (_lock)
            {
                // 현재 색상 백업
                var prevBg = Console.BackgroundColor;
                var prevFg = Console.ForegroundColor;
                
                try
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;

                    // 로그 출력
                    Console.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}");
                }
                finally
                {
                    // 예외 여부와 상관없이 색상 복원
                    Console.BackgroundColor = prevBg;
                    Console.ForegroundColor = prevFg;
                }
            }
        }
    }

    public class LogService : ILogService
    {
        // 클래스 내에 정적 lock 객체를 선언합니다.
        private static readonly object LogLock = new object();
        public void LogMessage(string? message)
        {
            try
            {
                DateTime thisday = DateTime.Now;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemLog", thisday.Year.ToString());

                DirectoryInfo di = new DirectoryInfo(path);

                // 년도 디렉터리 생성
                if (!di.Exists)
                {
                    di.Create();
                }

                // 월
                path = Path.Combine(path, thisday.Month.ToString());
                di = new DirectoryInfo(path);

                if (!di.Exists)
                {
                    di.Create();
                }

                // 일
                string filepath = Path.Combine(path, $"{thisday.Year}_{thisday.Month}_{thisday.Day}.txt");

                // 일.txt + 로그내용
                lock (LogLock)
                {
                    using (var fs = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        System.Diagnostics.StackTrace objStackTrace = new System.Diagnostics.StackTrace(new System.Diagnostics.StackFrame(1));
                        var s = objStackTrace.ToString(); // 호출한 함수 위치
                        writer.WriteLine($"[{thisday.ToString()}]\t{message}");

#if DEBUG
                    Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                    Console.ForegroundColor = ConsoleColor.Green; // 텍스트 색상 설정
                    Console.WriteLine($"[{thisday.ToString()}]\t{message}");
                    Console.ResetColor();
#endif
                    }
                }
            }
            catch(Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
    }
}
