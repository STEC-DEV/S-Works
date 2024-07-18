namespace FamTec.Shared.Server.DTO.Place
{
    public class UpdatePlaceDTO
    {
        /// <summary>
        /// 사업장 정보
        /// </summary>
        public PlaceInfo PlaceInfo { get; set; } = new PlaceInfo();

        /// <summary>
        /// 사업장 권한
        /// </summary>
        public PlacePerm PlacePerm { get; set; } = new PlacePerm();
    }
}
