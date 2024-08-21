using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class MonthVocListDTO
    {
        public int? Years { get; set; }
        public int? Month { get; set; }

        public string Dates {  get; set; }
        public List<ListVocDTO> VocList { get; set; } = new List<ListVocDTO>();
    }
}
