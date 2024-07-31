using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings.Group
{
    public class GroupDTO
    {
        public int id { get; set; }
        public string GroupName { get; set; }
        public List<ItemDTO> Items { get; set; }

    }
}
