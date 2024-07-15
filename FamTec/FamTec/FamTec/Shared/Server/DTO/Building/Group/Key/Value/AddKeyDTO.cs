using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building.Group.Key.Value
{
    public class AddKeyDTO
    {
        public int GroupID { get; set; } // 상위 그룹ID
        /// <summary>
        /// 아이템의 이름 ex) 전기차
        /// </summary>
        public string? Name { get; set; } // Key 내용

        public List<AddGroupItemValueDTO>? ItemValues { get; set; } = new List<AddGroupItemValueDTO>();
    }
}
