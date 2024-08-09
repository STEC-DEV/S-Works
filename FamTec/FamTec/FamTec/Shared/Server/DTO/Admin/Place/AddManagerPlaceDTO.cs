namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class AddManagerPlaceDTO
    {
        /// <summary>
        /// 관리자 생성 후 만들고 나온 ID
        /// </summary>
        public int? AdminID { get; set; }
        
        /// <summary>
        /// 선택한 사업장목록
        /// </summary>
        public List<int>? PlaceList { get; set; } = new List<int>();
    }
}
