using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class Voc
    {
        public int? Id { get; set; }
        public string? Location { get; set; }
        public int Type { get; set; }
        public string? Writer { get; set; }
        public string? Tel { get; set; }
        public string? Title { get; set; }
        public string? Context { get; set; }
        public int Status { get; set; }
        public DateTime? Occur_DT { get; set; }
    }
}
