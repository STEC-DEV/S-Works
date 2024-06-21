namespace FamTec.Server
{
    public class CommPath
    {
        /// <summary>
        /// 파일서버 경로
        /// </summary>
        public static readonly string FileServer = String.Format(@"{0}\\FileServer", AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// VOC 이미지들 경로
        /// </summary>
        public static readonly string VocFileImages = String.Format(@"{0}\\VocImages", CommPath.FileServer);
    }
}
