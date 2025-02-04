using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Dashboard
{
    public class ShowMaterialIdxDTO
    {
        /// <summary>
        /// 자재 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 자재 이름
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 보여줄지 여부
        /// </summary>
        public bool? DashBoardYN { get; set; }

        /// <summary>
        /// 품목코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacturingComp { get; set; }

    }
}
