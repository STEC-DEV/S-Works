namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class VocWeekStatusCountDTO
    {
        /// <summary>
        /// 날짜 구분
        /// </summary>
        public DateTime Date { get; set; }

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
