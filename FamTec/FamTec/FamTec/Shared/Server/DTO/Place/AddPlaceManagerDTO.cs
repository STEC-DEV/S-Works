using FamTec.Shared.Server.DTO.Admin.Place;

namespace FamTec.Shared.Server.DTO.Place
{
    /// <summary>
    /// 사업장에 관리자추가 DTO
    /// </summary>
    public class AddPlaceManagerDTO<T>
    {
        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? PlaceId { get; set; }

        /// <summary>
        /// 매니저 리스트
        /// </summary>
        public List<ManagerListDTO>? PlaceManager { get; set; } = new List<ManagerListDTO>();
    }
}
