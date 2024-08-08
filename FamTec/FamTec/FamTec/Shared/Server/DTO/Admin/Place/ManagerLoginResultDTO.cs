using FamTec.Shared.Server.DTO.Place;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    public class ManagerLoginResultDTO
    {
        /// <summary>
        /// 사용자 INDEX
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사용자 인덱스는 공백일 수 없습니다.")]
        public int USER_INDEX { get; set; }

        /// <summary>
        /// 로그인 ID
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "사용자 아이디는 공백일 수 없습니다.")]
        public string USERID { get; set; } = null!;

        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string? NAME { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "비밀번호는 공백일 수 없습니다.")]
        public string PASSWORD { get; set; } = null!;

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
        [NotNull]
        [Required(ErrorMessage = "알람 유무는 공백일 수 없습니다.")]
        public bool ALRAM_YN { get; set; }

        /// <summary>
        /// 재직 유무
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "재직 유무는 공백일 수 없습니다.")]
        public int STATUS { get; set; }

        /* 여기까지가 USERTB 관련됨 */

        /// <summary>
        /// 관리자테이블 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "관리자 테이블 인덱스는 공백일 수 없습니다.")]
        public int ADMIN_INDEX { get; set; }

        /// <summary>
        /// 관리자 종류
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "관리자 타입은 공백일 수 없습니다.")]
        public string TYPE { get; set; } = null!;
        /* 여기까지가 ADMINTB 관련됨 */

        /// <summary>
        /// 부서테이블 인덱스
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서 인덱스는 공백일 수 없습니다.")]
        public int DEPARTMENT_INDEX { get; set; }

        /// <summary>
        /// 부서명
        /// </summary>
        [NotNull]
        [Required(ErrorMessage = "부서명은 공백일 수 없습니다.")]
        public string DEPARTMENT_NAME { get; set; } = null!;

        /// <summary>
        /// 사업장정보
        /// </summary>
        public List<PlacesDTO>? placeDTO { get; set; } = new List<PlacesDTO>();
    }
}
