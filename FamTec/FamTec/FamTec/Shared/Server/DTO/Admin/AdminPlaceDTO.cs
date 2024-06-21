using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Admin
{
    public class AdminPlaceDTO
    {
        /// <summary>
        /// PlaceID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// PlaceCode
        /// </summary>
        public string? PlaceCd { get; set; }
        
        /// <summary>
        /// 사업장명
        /// </summary>
        public string? Name { get; set; } = null;
        
        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; } = null;
        
        /// <summary>
        /// 계약번호
        /// </summary>
        public string? ContractNum { get; set; } = null;
        
        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDt { get; set; }
        
        /// <summary>
        /// 계약상태
        /// </summary>
        public sbyte? Status { get; set; }

    }
}
