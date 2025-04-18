namespace FamTec.Shared.Server.DTO.Voc
{
    public class VocListDTOV2
    {
        /// <summary>
        /// VOC ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? buildingName { get; set; }

        /// <summary>
        /// 유형
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 모바일 / 웹 구분
        /// </summary>
        public int division { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// 처리상태
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 민원처리 완료 시간
        /// </summary>
        public string? completeDT { get; set; }

        /// <summary>
        /// 민원처리 소요 시간
        /// </summary>
        public string? durationDT { get; set; }

        /// <summary>
        /// 요청 일시
        /// </summary>
        public string? createDT { get; set; }

        /// <summary>
        /// 작성자
        /// </summary>
        public string? createUser { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? phone { get; set; }

        /// <summary>
        /// 여기서 이제 체크박스 해야할듯
        /// </summary>
        public bool replyYn { get; set; }


    }
}
