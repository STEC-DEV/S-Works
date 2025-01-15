namespace FamTec.Shared.Server.DTO.DashBoard
{
    public class VocWeekStatusCountDTO
    {
        /// <summary>
        /// 날짜 구분
        /// </summary>
        public string Date { get; set; }

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

        /// <summary>
        /// 발생 수
        /// </summary>
        public int Total { get; set; }
    }
}
