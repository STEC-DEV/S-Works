using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Group
{
    public class UpdateGroupDTO
    {
        /// <summary>
        /// 그룹 인덱스
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 변경할 그룹명
        /// </summary>
        public string? GroupName { get; set; }

    }
}
