using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Manager
{
    public class AdminImg
    {
        public int adminId { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }

    }
}
