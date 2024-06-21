using FamTec.Shared.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.DTO
{
    public class ResponseModel<T>
    { 
        public string? Message { get; set; }

        public List<T>? Data { get; set; }

        public int? StatusCode { get; set; }
    }


    public class ResponseOBJ<T>
    {
        public ResponseModel<T> RESPMessage(string message, T dto, int code)
        {
            ResponseModel<T> obj = new ResponseModel<T>()
            {
                Message = message,
                Data = new List<T>
                {
                    dto
                },
                StatusCode = code
            };
            return obj;
        }

        public ResponseModel<T> RESPMessageList(string message, List<T> dto, int code)
        {
            ResponseModel<T> obj = new ResponseModel<T>()
            {
                Message = message,
                Data = dto,
                StatusCode = code
            };
            return obj;
        }


    }
    
}
