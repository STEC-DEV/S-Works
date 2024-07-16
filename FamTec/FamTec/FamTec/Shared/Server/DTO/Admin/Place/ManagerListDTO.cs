using Microsoft.Identity.Client.TelemetryCore.TelemetryClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class ManagerListDTO
    {
        /// <summary>
        /// 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 선택여부
        /// </summary>
        [Display(Name = "선택")]
        public bool? IsSelect { get; set; } = false;

        /// <summary>
        /// 아이디
        /// </summary>
        [Display(Name = "아이디")]
        public string? UserId { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        [Display(Name = "이름")]
        public string? Name { get; set; }

        /// <summary>
        /// 부서
        /// </summary>
        [Display(Name = "부서")]
        public string? Department { get; set; }



        // 기본 생성자
        public ManagerListDTO()
        {
        }

        // 복사 생성자
        public ManagerListDTO(ManagerListDTO source)
        {
            if (source != null)
            {
                Id = source.Id;
                IsSelect = source.IsSelect;
                UserId = source.UserId;
                Name = source.Name;
                Department = source.Department;
            }
        }

        // 깊은 복사 메서드
        public ManagerListDTO DeepCopy()
        {
            return new ManagerListDTO(this);
        }
    }
}
