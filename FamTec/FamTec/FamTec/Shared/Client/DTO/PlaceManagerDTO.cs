using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class PlaceManagerDTO
    {
        public bool IsSelect { get; set; } = false;
        public string ImgPath { get; set; }
        public string ID { get; set;}
        public string UserId { get; set; }
        public string NAME { get; set; }
        public string DEPARTMENT { get; set; }
    }
}
