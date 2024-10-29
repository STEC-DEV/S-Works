namespace FamTec.Shared.Server.DTO.KakaoLog
{
    public class AddKaKaoSendResult
    {
        /// <summary>
        /// VOCID
        /// </summary>
        public int VocID { get; set; }

        /// <summary>
        /// 사업장ID
        /// </summary>
        public int PlaceID { get; set; }

        /// <summary>
        /// 건물ID
        /// </summary>
        public int BuildingID { get; set; }

        /// <summary>
        /// 메시지ID
        /// </summary>
        public string MID { get; set; }
    }
}
