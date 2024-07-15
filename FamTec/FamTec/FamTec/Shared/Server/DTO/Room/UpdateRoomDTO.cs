namespace FamTec.Shared.Server.DTO.Room
{
    public class UpdateRoomDTO
    {
        /// <summary>
        /// 공간 ID
        /// </summary>
        public int? RoomId { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? Name { get; set; }
    }
}
