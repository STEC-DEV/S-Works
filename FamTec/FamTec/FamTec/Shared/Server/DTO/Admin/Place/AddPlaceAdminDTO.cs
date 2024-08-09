namespace FamTec.Shared.Server.DTO.Admin.Place
{
    /// <summary>
    /// 사업장에 이미 생성되어있는 관리자들 할당해줄때 필요한 DTO
    /// </summary>
    public class AddPlaceAdminDTO
    {
        /// <summary>
        /// ADMIN_PLACE_TB 인덱스
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 관리자테이블 인덱스 
        /// </summary>
        public int? AdminTBID { get; set; }

        /// <summary>
        /// 사업장테이블 인덱스
        /// </summary>
        public int? PlaceID { get; set; }
    }
}
