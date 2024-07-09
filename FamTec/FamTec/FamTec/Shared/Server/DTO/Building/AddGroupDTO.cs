using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    public class AddGroupDTO
    {
        /// <summary>
        /// 명칭 ex) 주차장
        /// </summary>
        public string? Name { get; set; }

        public List<AddGroupItemKeyDTO> AddGroupKey { get; set; } = new List<AddGroupItemKeyDTO>();

    }
}
