using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.InOut
{
    public class OutContentDTO
    {
        public DateTime? Date { get; set; }
        public int RoomId { get; set;  }
        public int Num { get; set; }
        public string Note { get; set; }
    }
}
