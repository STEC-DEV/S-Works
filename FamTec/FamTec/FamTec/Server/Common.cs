namespace FamTec.Server
{
    /// <summary>
    /// 공통 참조파일
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 파일서버 경로
        /// </summary>
        public static readonly string FileServer = String.Format(@"{0}\\FileServer", AppDomain.CurrentDomain.BaseDirectory);

        // 사진 확장자
        public static readonly string[] ImageAllowedExtensions = { ".jpg", ".png", ".bmp", ".jpeg" };

        /// <summary>
        /// 1MB
        /// </summary>
        public static readonly int MEGABYTE_1 = 1048576;

        /// <summary>
        /// 2MB
        /// </summary>
        public static readonly int MEGABYTE_2 = 2097152;

        /// <summary>
        /// 3MB
        /// </summary>
        public static readonly int MEGABYTE_3 = 3145728;

        /// <summary>
        /// 4MB
        /// </summary>
        public static readonly int MEGABYTE_4 = 4194304;

        /// <summary>
        /// 5MB
        /// </summary>
        public static readonly int MEGABYTE_5 = 5242880;
    }
}
