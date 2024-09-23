using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material
{
    public class UpdateMaterialDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SafeNum { get; set; }
        public string Unit { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }

    }
}
