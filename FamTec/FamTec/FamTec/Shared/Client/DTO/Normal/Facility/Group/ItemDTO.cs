using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Group
{
    public class ItemDTO
    {
        public ItemDTO()
        {
            valueList = new List<ItemValueDTO>();
        }



        public int Id { get; set; }
        public string ItemKey { get; set; }
        public string Unit { get; set; }
        public List<ItemValueDTO> valueList { get; set; }
        //public int GroupIdx { get; set; }


        public ItemDTO(ItemDTO source)
        {
            Id = source.Id;
            ItemKey = source.ItemKey;
            Unit = source.Unit;
            valueList = source.valueList?.Select(v => new ItemValueDTO(v)).ToList() ?? new List<ItemValueDTO>();
        }
    }
}
