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
        [Display(Name = "선택")]
        public bool IsSelect { get; set; } = false;
        [Display(Name = "아이디")]
        public string UserId { get; set;}
        [Display(Name = "이름")]
        public string Name { get; set; }
        [Display(Name = "부서")]
        public string Department { get; set; }
    }
}
