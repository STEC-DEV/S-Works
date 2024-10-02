using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class UserInfoDTO
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Job {  get; set; }
        public int Status { get; set; }
        public bool AlarmYN { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }

        public UserInfoDTO Clone()
        {
            return new UserInfoDTO
            {
                Name = this.Name,
                UserId = this.UserId,
                Password = this.Password,
                Email = this.Email,
                Phone = this.Phone,
                Job = this.Job,
                Status = this.Status,
                AlarmYN = this.AlarmYN,
                Image = this.Image?.ToArray(), // 새로운 배열을 생성하여 복사
                ImageName = this.ImageName
            };
        }
    }
}
