using FamTec.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class AddMaintetanceMaterialDTO
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
        public int? RoomID { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }

        /// <summary>
        /// 총 긍맥
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }
    }

}
