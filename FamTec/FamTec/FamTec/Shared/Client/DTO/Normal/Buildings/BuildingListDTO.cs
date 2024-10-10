using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings
{
    public class BuildingListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BuildingCd { get; set; }
        public string Address { get; set; }
        public string Tel {  get; set; }
        public string TotalFloor { get; set; }

        public DateTime? CompletionDt {  get; set; }
        public DateTime? CreateDt { get; set; }
    }
}
