
namespace FamTec.Server.Services
{
    public class LogService : ILogService
    {
        public void LogMessage(string? message)
        {
            try
            {
                DateTime thisday = DateTime.Now;

                string path = string.Format(@"{0}\\SystemLog/{1}", AppDomain.CurrentDomain.BaseDirectory, thisday.Year);

                DirectoryInfo di = new DirectoryInfo(path);

                // 년도 디렉터리 생성
                if (!di.Exists)
                {
                    di.Create();
                }

                // 월
                path = string.Format(@"{0}/{1}", path, thisday.Month);
                di = new DirectoryInfo(path);

                if (!di.Exists)
                {
                    di.Create();
                }

                // 일
                string filepath = Path.Combine(path, String.Format("{0}_{1}_{2}.txt", thisday.Year, thisday.Month, thisday.Day));

                // 일.txt + 로그내용
                using (StreamWriter writer = new StreamWriter(filepath, true))
                {
                    System.Diagnostics.StackTrace objStackTrace = new System.Diagnostics.StackTrace(new System.Diagnostics.StackFrame(1));
                    var s = objStackTrace.ToString(); // 호출한 함수 위치
                    writer.WriteLine($"[{thisday.ToString()}]\t{message}");

#if DEBUG
                    Console.WriteLine($"[{thisday.ToString()}]\t{message}");
#endif
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
