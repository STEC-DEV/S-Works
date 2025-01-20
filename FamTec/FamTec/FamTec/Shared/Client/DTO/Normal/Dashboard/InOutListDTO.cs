using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Dashboard
{
    public class InOutListDTO
    {
        /// <summary>
        /// 입고 List
        /// </summary>
        public List<InOutDataDTO> InPutList { get; set; } = new List<InOutDataDTO>();

        /// <summary>
        /// 출고 List
        /// </summary>
        public List<InOutDataDTO> OutPutList { get; set; } = new List<InOutDataDTO>();

    }

    public class InOutDataDTO
    {
        /// <summary>
        /// StoreID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단가
        /// </summary>
        public float UnitPrice { get; set; }

        /// <summary>
        /// 입출고가격
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 공간ID
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 자재ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 자재명칭
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 날짜
        /// </summary>
        public DateTime Date { get; set; }

    }
}
