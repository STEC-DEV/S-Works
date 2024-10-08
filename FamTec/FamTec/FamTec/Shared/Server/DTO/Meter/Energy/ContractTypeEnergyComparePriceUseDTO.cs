using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Meter.Energy
{
    public class ContractTypeEnergyComparePriceUseDTO
    {
        /// <summary>
        /// 계약종별 ID
        /// </summary>
        public int ContractTypeId { get; set; }

        /// <summary>
        /// 계약종별 이름
        /// </summary>
        public string? ContractTypeName { get; set; }

        /// <summary>
        /// 이번달 총 사용량
        /// </summary>
        public float ThisMonthTotalUse { get; set; }

        /// <summary>
        /// 지난달 총 사용량
        /// </summary>
        public float BeforeMonthTotalUse { get; set; }

        /// <summary>
        /// 작년 동월 총 사용량
        /// </summary>
        public float LastYearSameMonthTotalUse { get; set; }
    }
}
