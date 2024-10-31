namespace FamTec.Shared.Server.DTO.Voc
{
    public class VocEmployeeDetailDTO
    {
        /// <summary>
        /// 민원 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 접수코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 민원 신청일
        /// </summary>
        public string? CreateDT { get; set; }

        /// <summary>
        /// 민원 상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// 민원 유형
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 웹 / 모바일 여부
        /// </summary>
        public int? Division { get; set; }

        /// <summary>
        /// 민원 제목
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 민원 내용
        /// </summary>
        public string? Contents { get; set; }

        /// <summary>
        /// 민원 신청자 이름
        /// </summary>
        public string? CreateUser { get; set; }

        /// <summary>
        /// 민원인 전화번호
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 이미지 파일명
        /// </summary>
        public List<string?> ImageName { get; set; } = new List<string?>();

        /// <summary>
        /// 이미지
        /// </summary>
        public List<byte[]?> Images { get; set; } = new List<byte[]?>();
    }
}
