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
        public int AdminId { get; set; }
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
        public int DepartmentId { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }


        public DManagerDTO() 
        { 
        }

        public DManagerDTO(DManagerDTO source)
        {
            if(source != null)
            {
                AdminId = source.AdminId; 
                Name = source.Name; 
                UserId = source.UserId;
                Password = source.Password;
                Email = source.Email;
                Phone = source.Phone;
                Type = source.Type;
                Department = source.Department;
                DepartmentId = source.DepartmentId;
                Image = source.Image;
                ImageName = source.ImageName;
            }
        }
        public DManagerDTO DeepCopy()
        {
            return new DManagerDTO(this);
        }
    }
}
