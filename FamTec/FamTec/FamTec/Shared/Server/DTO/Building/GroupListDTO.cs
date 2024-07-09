using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    public class GroupListDTO
    {
        public int? ID { get; set; } // 그룹 리스트 아이디
        public string? Name { get; set; } // 그룹명

        public List<GroupKeyListDTO>? KeyListDTO { get; set; } = new List<GroupKeyListDTO>();
    }
}
