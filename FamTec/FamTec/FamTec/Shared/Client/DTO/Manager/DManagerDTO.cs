using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Manager
{
    public class DManagerDTO
    {
        [Display(Name = "이름")]
        public string Name { get; set; }
        [Display(Name="아이디")]
        public string UserId { get; set; }
        [Display(Name = "비밀번호")]
        public string Password { get; set; }
        [Display(Name = "이메일")]
        public string Email { get; set; }
        [Display(Name = "전화번호")]
        public string Phone { get; set; }
        [Display(Name = "계정 유형")]
        public string Type { get; set; }
        [Display(Name = "부서")]
        public string Department { get; set; }
        public string Image { get; set; }

    }
}
