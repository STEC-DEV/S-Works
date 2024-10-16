using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Unit
{
    public class UnitDTO
    {
        public int Id { get; set; }
        public bool SystemCreate { get; set; }

        public string Unit { get; set; }


        public UnitDTO() { }
        public UnitDTO(UnitDTO source) 
        {
            this.Id = source.Id;
            this.SystemCreate = source.SystemCreate;
            this.Unit = source.Unit;
        }
    }
}
