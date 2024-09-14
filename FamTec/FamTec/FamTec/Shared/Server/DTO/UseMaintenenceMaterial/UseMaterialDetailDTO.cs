namespace FamTec.Shared.Server.DTO.UseMaintenenceMaterial
{
    public class UseMaterialDetailDTO
    {
        /// <summary>
        /// 유지보수 사용자재 ID
        /// </summary>
        public int UseMaterialId { get; set; }
        
        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 총 금액
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 공간 ID
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 공간 명칭
        /// </summary>
        public string? RoomName { get; set; }
        
        /// <summary>
        /// 사용자재 ID
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// 사용자재명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int MaintenanceId { get; set; }
        
        /// <summary>
        /// 공간 가용재고
        /// </summary>
        public int TotalAvailableInventory { get; set; }

        /// <summary>
        /// 사용자재 LIST
        /// </summary>
        public List<UseDetailStoreDTO> UseList { get; set; } = new List<UseDetailStoreDTO>();
    }

    public class UseDetailStoreDTO
    {
         /// <summary>
         /// 세부항목 ID - StoreTB 인덱스
         /// </summary>
         public int Id { get; set; }

        /// <summary>
        /// 입출고 구분
        /// </summary>
        public int InOut { get; set; }

         /// <summary>
         /// 수량
         /// </summary>
         public int Num { get; set; }

         /// <summary>
         /// 단가
         /// </summary>
         public float UnitPrice { get; set; }

         /// <summary>
         /// 총 금액
         /// </summary>
         public float TotalPrice { get; set; } // 총 금액
    }
}
