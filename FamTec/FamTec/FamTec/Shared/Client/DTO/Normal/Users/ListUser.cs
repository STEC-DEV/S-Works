using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class ListUser
    {
        public int Id { get; set; }
        [Display(Name = "아이디")]
        public string? UserId { get; set; }
        [Display(Name = "이름")]
        public string? Name { get; set; }
        [Display(Name = "이메일")]
        public string? Email { get; set; }
        [Display(Name = "전화번호")]
        public string? Phone { get; set; }
        [Display(Name = "직책")]
        public string? Type { get; set; }
        [Display(Name = "생성일자")]
        //public DateTime? Created { get; set; }
        public string Created { get; set; }
        [Display(Name = "재직 상태")]
        public sbyte? Status { get; set; }
    }
}
