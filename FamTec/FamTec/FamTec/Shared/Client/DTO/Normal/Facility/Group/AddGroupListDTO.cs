using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Group
{
    public class AddGroupListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AddGroupKey> AddGroupKey { get; set; } = new List<AddGroupKey>();
    }


    public class AddGroupKey
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public List<AddGroupItemValueDTO> ItemValues { get; set; } = new List<AddGroupItemValueDTO>();

        public AddGroupKey () { }
        public AddGroupKey(AddGroupKey source)
        {
            this.Name = source.Name;
            this.Unit = source.Unit;
            foreach (var item in source.ItemValues)
            {
                // 각 AddGroupItemValueDTO 객체를 복사
                this.ItemValues.Add(new AddGroupItemValueDTO
                {
                    Values = item.Values
                });
            }
        }

    }


    public class AddGroupItemValueDTO
    {
        public string Values { get; set; }


    }
}
