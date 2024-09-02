using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model
{
    [Keyless]
    public partial class MaterialInventory
    {
        /// <summary>
        /// 창고ID
        /// </summary>
        public int? R_ID { get; set; }

        /// <summary>
        /// 창고명
        /// </summary>
        public string? R_NM { get; set; }

        /// <summary>
        /// 자재ID
        /// </summary>
        public int? M_ID { get; set; }

        /// <summary>
        /// 자재코드
        /// </summary>
        public string? M_CODE { get; set; }

        /// <summary>
        /// 자재명
        /// </summary>
        public string? M_NM { get; set; }
        
        /// <summary>
        /// 자재수량
        /// </summary>
        public int? TOTAL { get; set; }
    }
}
