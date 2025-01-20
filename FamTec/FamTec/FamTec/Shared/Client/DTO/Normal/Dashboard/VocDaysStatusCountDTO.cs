using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Dashboard
{
    public class VocDaysStatusCountDTO
    {
        /// <summary>
        /// 전체
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 미처리
        /// </summary>
        public int UnProcessed { get; set; }

        /// <summary>
        /// 처리중
        /// </summary>

        public int Processing { get; set; }

        /// <summary>
        /// 처리완료
        /// </summary>
        public int Completed { get; set; }
    }
}
