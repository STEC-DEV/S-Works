using FamTec.Shared.Server.DTO.Admin.Place;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class ManagerDTO
    {
        public int Id { get; set; }
        
        [Display(Name = "아이디")]
        public string UserId { get; set;}
        [Display(Name = "이름")]
        public string Name { get; set; }
        [Display(Name = "부서")]
        public string Department { get; set; }
        [Display(Name = "선택")]
        public bool IsSelect { get; set; } = false;

        // 기본 생성자
        public ManagerDTO()
        {
        }

        // 복사 생성자
        public ManagerDTO(ManagerDTO source)
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
        public ManagerDTO DeepCopy()
        {
            return new ManagerDTO(this);
        }
    }
}
