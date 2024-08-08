namespace FamTec.Shared.Server.DTO.Building.Group.Key.Value
{
    public class UpdateValueDTO
    {
        /// <summary>
        /// 아이템 ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 값
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string Unit { get; set; } // 단위
    }
}
