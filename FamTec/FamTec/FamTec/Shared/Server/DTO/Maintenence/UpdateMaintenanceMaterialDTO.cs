namespace FamTec.Shared.Server.DTO.Maintenence
{
    /// <summary>
    /// 유지보수 - 품목 입출고 내용 수정
    /// </summary>
    public class UpdateMaintenanceMaterialDTO
    {
        /// <summary>
        /// 사용내역 ID
        /// </summary>
        public int UseMaintanceID { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

    
    }
}
