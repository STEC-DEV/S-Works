using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Admin
{
    public class DepartmentDTO
    {
        /// <summary>
        /// 부서 인덱스
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        public bool IsSelect { get; set; } = false;

        /// <summary>
        /// 부서명
        /// </summary>
        public string? Name { get; set; }

        public string? Description { get; set; } = null;

    }
}
