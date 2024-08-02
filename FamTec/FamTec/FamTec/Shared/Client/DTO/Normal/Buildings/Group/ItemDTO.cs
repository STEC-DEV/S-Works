using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings.Group
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string ItemKey { get; set; }
        public string Unit { get; set; }
        public List<ItemValueDTO> valueList { get; set; }
        //public int GroupIdx { get; set; }
    }
}
