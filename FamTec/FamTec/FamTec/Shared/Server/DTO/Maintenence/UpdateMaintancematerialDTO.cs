namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 새로 테스트중 - 유지보수 담아서 업데이트 수정치는 DTO
    /// </summary>
    public class UpdateMaintancematerialDTO
    {
        /// <summary>
        /// 유지보수이력 ID
        /// </summary>
        public int MaintanceID { get; set; }

        public List<UpdateUseMaterialDTO> UpdateUsematerialDTO { get; set; } = new List<UpdateUseMaterialDTO>();
        
    }

    public class UpdateUseMaterialDTO
    {
        /// <summary>
        /// 사용자재 ID
        /// </summary>
        public int? UseID { get; set; }
        
        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 공간ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }
    }
}
