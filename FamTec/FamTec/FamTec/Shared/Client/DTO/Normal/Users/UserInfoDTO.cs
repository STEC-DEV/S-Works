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
        public string Status { get; set; }
        public sbyte? AlarmYN { get; set; }
    }
}
