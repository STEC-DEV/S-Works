using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class ApiResponse
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public Header Header { get; set; }
        public Body Body { get; set; }
    }

    public class Header
    {
        public string ResultCode { get; set; }
        public string ResultMsg { get; set; }
    }

    public class Body
    {
        public List<Item> Items { get; set; }
        public int NumOfRows { get; set; }
        public int PageNo { get; set; }
        public int TotalCount { get; set; }
    }

    public class Item
    {
        public string PrdctClsfcNo { get; set; }
        public string PrdctClsfcNoNm { get; set; }
        public string Uslfsvc { get; set; }
        public string RgstDate { get; set; }
        public string EndDate { get; set; }
        public string ChgRsn { get; set; }
    }
}
