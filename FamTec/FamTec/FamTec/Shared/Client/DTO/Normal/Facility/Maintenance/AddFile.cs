using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility.Maintenance
{
    public class AddFile
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
    }
}
