using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings
{
    public class ItemGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemKey> itemKeys { get; set; }
    }
}
