using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material
{
    public class AddMaterialDTO
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Standard { get; set; }
        public string? Unit { get; set; }
        public string? ManufacturingComp { get; set; }
        public int SafeNum { get; set; }
        public int? RoomId { get; set; }
        public List<byte[]>? Image { get; set; }
        public List<string>? ImageName { get; set; }

    }
}
