using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Group
{
    public class ItemValueDTO
    {
        public ItemValueDTO() { }

        public int? Id {  get; set; }
        public string itemValue { get; set; }

        //public int ItemIdx { get; set; }


        public ItemValueDTO(ItemValueDTO source)
        {
            Id = source.Id;
            itemValue = source.itemValue;
        }
    }
}
