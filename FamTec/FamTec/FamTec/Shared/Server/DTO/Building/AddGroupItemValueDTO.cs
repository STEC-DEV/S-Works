using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Building
{
    public class AddGroupItemValueDTO
    {
        /// <summary>
        /// 값
        /// </summary>
        public string? Values { get; set; }
        
        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }
    }
}
