using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Users
{
    public class UserDTO
    {
        public int Id { get; set; }
        public UserInfoDTO UserInfo { get; set; }
        public UserPermDTO UserPerm { get; set; }
        public UserVocPermDTO UserVocPerm { get; set; }
    }
}
