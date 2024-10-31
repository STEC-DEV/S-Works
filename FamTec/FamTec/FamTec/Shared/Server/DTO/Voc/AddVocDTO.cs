namespace FamTec.Shared.Server.DTO.Voc
{
    /// <summary>
    /// 민원등록 - 민원인용
    /// </summary>
    public class AddVocDTO
    {
        /// <summary>
        /// 모바일 여부 0 : 모바일 / 1 : 웹
        /// </summary>
        public int Division { get; set; }

        /// <summary>
        /// 미분류 - 기계 , 전기 등...
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 민원의 제목
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string? Contents { get; set; }

        /// <summary>
        /// 작성자 이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 휴대폰번호
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 빌딩 인덱스
        /// </summary>
        public int? Buildingid { get; set; }

        /// <summary>
        /// 사업장인덱스
        /// </summary>
        public int? Placeid { get; set; }
    }
}
