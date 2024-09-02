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

        /// <summary>
        /// 사진 확장자
        /// </summary>
        public static readonly string[] ImageAllowedExtensions = { ".jpg", ".png", ".bmp", ".jpeg" };

        /// <summary>
        /// 엑셀 확장자
        /// </summary>
        public static readonly string[] XlsxAllowedExtensions = { ".xlsx", ".xls" };

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

        /// <summary>
        /// 서버전용 HTTP 전송객체
        /// </summary>
        public static HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// 카카오 알림톡 API KEY
        /// </summary>
        public static readonly string KakaoAPIKey = "i60oeb079x8ivosskcwfrfku7liks4ke";

        /// <summary>
        /// 카카오 알림톡 UserID
        /// </summary>
        public static readonly string KakaoUserId = "sworks";

        /// <summary>
        /// 카카오톡 알림톡 발신번호
        ///  * 알리고에 등록된 발신번호만 등록가능
        /// </summary>
        public static readonly string KakaoSenders = "15770722";

        /// <summary>
        /// 카카오톡 알림톡 발신자 Key
        /// </summary>
        public static readonly string KakaoSenderKey = "27fa43355ea9c4fedf48a8d37e7b08bc864888b8";

        /// <summary>
        /// 카카오톡 알림톡 템플릿 Code [민원접수]
        /// </summary>
        public static readonly string KakaoTemplateCode_1 = "TU_6120";
        
        /// <summary>
        /// 카카오톡 알림톡 템플릿 Code [민원 진행상태 변경]
        /// </summary>
        public static readonly string KakaoTemplateCode_2 = "TU_6125";
    }
}