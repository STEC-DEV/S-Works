using FamTec.Shared.Server.DTO.Place;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class ManagerLoginResultDTO
    {
        /// <summary>
        /// 사용자 INDEX
        /// </summary>
        public int? USER_INDEX { get; set; }

        /// <summary>
        /// 로그인 ID
        /// </summary>
        public string? USERID { get; set; }

        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string? NAME { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string? PASSWORD { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string? EMAIL { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? PHONE { get; set; }

        /// <summary>
        /// 관리자유무
        /// </summary>
        public bool? ADMIN_YN { get; set; }

        /// <summary>
        /// 알람 유무
        /// </summary>
        public bool? ALRAM_YN { get; set; }

        /// <summary>
        /// 재직 유무
        /// </summary>
        public int? STATUS { get; set; }

        /* 여기까지가 USERTB 관련됨 */

        /// <summary>
        /// 관리자테이블 인덱스
        /// </summary>
        public int? ADMIN_INDEX { get; set; }

        /// <summary>
        /// 관리자 종류
        /// </summary>
        public string? TYPE { get; set; }
        /* 여기까지가 ADMINTB 관련됨 */

        /// <summary>
        /// 부서테이블 인덱스
        /// </summary>
        public int? DEPARTMENT_INDEX { get; set; }

        /// <summary>
        /// 부서명
        /// </summary>
        public string? DEPARTMENT_NAME { get; set; }

        /// <summary>
        /// 사업장정보
        /// </summary>
        public List<PlacesDTO>? placeDTO { get; set; } = new List<PlacesDTO>();
    }
}
