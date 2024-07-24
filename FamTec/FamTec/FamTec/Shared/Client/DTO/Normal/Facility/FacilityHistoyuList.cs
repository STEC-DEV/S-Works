using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Facility
{
    public class FacilityHistoyuList
    {
        public int Id { get; set; }
        //작업명
        public string Name { get; set; }
        //작업구분
        public string Type {  get; set; }
        //작업자
        public string Worker { get; set; }
        //사용자재 수
        public int UseMaterial { get; set; }
        //소요비용
        public float TotalPrice { get; set; }
        //작업일자
        public DateTime WorkDT { get; set; }


    }
}
