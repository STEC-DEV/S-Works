namespace FamTec.Shared.Server.DTO.Place
{
    public class UpdatePlaceManagerDTO
    {
        /// <summary>
        /// 대상 사업장ID
        /// </summary>
        public int PlaceId { get; set; }

        /// <summary>
        /// 대상 Target AdminID
        /// </summary>
        public List<int> AdminId { get; set; } = new List<int>();
    }
}
