namespace FamTec.Shared.Server.DTO.Maintenence
{
    public class AddMaintanceMaterialDTO
    {
        /// <summary>
        /// 유지보수 이력ID
        /// </summary>
        public int MaintanceID { get; set; }

        /// <summary>
        /// 사용자재 List
        /// </summary>
        public List<MaterialDTO> MaterialList { get; set; } = new List<MaterialDTO>();
    }

    public class MaterialDTO
    {
        /// <summary>
        /// 사용자재 ID
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
