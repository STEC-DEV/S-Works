using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.Class
{
    public class FailResult
    {
        /// <summary>
        /// 반환 값
        /// </summary>
        public int? ReturnResult { get; set; }

        /// <summary>
        /// 실패 List
        /// </summary>
        public List<FailInventory> FailList { get; set; } = new List<FailInventory>();
    }

    public class FailInventory
    {
        /// <summary>
        /// 품목 인덱스
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 공간 인덱스
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 공간 명
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 이용가능한 수량
        /// </summary>
        public int AvailableNum { get; set; }
    }
}
