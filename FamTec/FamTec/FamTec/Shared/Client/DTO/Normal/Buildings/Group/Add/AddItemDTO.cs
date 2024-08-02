using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings.Group.Add
{
    public class AddItemDTO
    {
        public int GroupID { get; set; } // 상위 그룹ID
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        public string? Name { get; set; } // Key 내용

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        public List<AddItemValueDTO>? ItemValues { get; set; } = new List<AddItemValueDTO>();
    }
}
