namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 유지보수 이력리스트 DTO
    /// </summary>
    public class MaintanceListDTO
    {
        /// <summary>
        /// 유지보수 이력 인덱스
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 작업일자
        /// </summary>
        public DateTime? WorkDT { get; set; }

        /// <summary>
        /// 유지보수 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 작업구분
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 사용자재 리스트
        /// </summary>
        public List<UsedMaterialDTO> UsedMaterialList { get; set; } = new List<UsedMaterialDTO>();
    
        /// <summary>
        /// 총 비용
        /// </summary>
        public float? TotalPrice { get; set; }
    }

    /// <summary>
    /// 사용자재 DTO
    /// </summary>
    public class UsedMaterialDTO
    {
        /// <summary>
        /// 입출고이력 테이블 ID
        /// </summary>
        public int? StoreID { get; set; }

        /// <summary>
        /// 공간 ID - 재입고시 필요
        /// </summary>
        public int? RoomTBID { get; set; }

        /// <summary>
        /// 사용자재 인덱스 - 재입고시 필요
        /// </summary>
        public int? MaterialID { get; set; }

        /// <summary>
        /// 사용자재 명
        /// </summary>
        public string? MaterialName { get; set; }
    }
  
}
