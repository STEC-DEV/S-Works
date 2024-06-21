using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class AddManagerDTO
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public int DepartmentId {  get; set; }
        public List<int> PlaceList { get; set; } = new List<int>();
    }
}
