using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class AddCommentDTO
    {
        //public int Id { get; set; }
        public int Status {get; set;}
        public string Content { get; set; }

        public List<byte[]> Image { get; set; }

        public int VocTbId { get; set; }
    }
}

