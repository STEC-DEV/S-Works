using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.InOut
{
    public class MaterialSearchListDTO
    {
        public int Id { get; set; }
        public string Code {  get; set; }
        public string Name { get; set; }
        public string Mfr { get; set; }
        public string Standard { get; set; }
    }
}
