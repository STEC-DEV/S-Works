namespace FamTec.Server.Services
{
    public class ConsoleLogService<T>
    {
        private readonly ILogger<T> ConsoleLogger;

        public ConsoleLogService(ILogger<T> _logger)
        {
            this.ConsoleLogger = _logger;
        }

        public void ConsoleText(string message)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;

                // 로그 출력
                Console.WriteLine($"[INFO] {DateTime.Now}: {message}");

                Console.ResetColor();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ConsoleWarning(string message)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;

                // 로그 출력
                Console.WriteLine($"[WARNING] {DateTime.Now}: {message}");

                Console.ResetColor();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void ConsoleLog(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;

                // 로그 출력
                Console.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}");
                Console.ResetColor();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class LogService : ILogService
    {
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
                using (StreamWriter writer = new StreamWriter(filepath, true))
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
