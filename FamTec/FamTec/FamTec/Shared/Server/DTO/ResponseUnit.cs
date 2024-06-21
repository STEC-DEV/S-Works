using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO
{
    public class ResponseUnit<T>
    {
        public string? message { get; set; }
        public T? data { get; set; }
        public int? code { get; set; }
    }
}
