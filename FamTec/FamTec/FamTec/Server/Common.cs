namespace FamTec.Server
{
    public class Common
    {
        /// <summary>
        /// 파일서버 경로
        /// </summary>
        public static readonly string FileServer = String.Format(@"{0}\\FileServer", AppDomain.CurrentDomain.BaseDirectory);

        // 사진 확장자
        public static readonly string[] ImageAllowedExtensions = { ".jpg", ".png", ".bmp", ".jpeg" };
    }
}
