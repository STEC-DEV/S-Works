using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.User
{
    /// <summary>
    /// USER DTO
    /// </summary>
    public class UsersDTO
    {
        /// <summary>
        /// 사용자 인덱스
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// 사용자이름
        /// </summary>
        public string? NAME { get; set; }

        /// <summary>
        /// 직책
        /// </summary>
        public string? JOB { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? PHONE { get; set; }


        /// <summary>
        /// 이메일
        /// </summary>
        public string? EMAIL { get; set; }

        /// <summary>
        /// 사용자 아이디
        /// </summary>

        public string USERID { get; set; }

        /// <summary>
        /// 비밀번호
        /// </summary>
        public string PASSWORD { get; set; }

        /// <summary>
        /// 사용자 이미지 - 클라이언트 담는용
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// 사용자 이미지 - 클라이언트 반환용
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 기본정보등록 권한
        /// </summary>
        public int PERM_BASIC { get; set; } = 0;

        /// <summary>
        /// 설비 권한
        /// </summary>
        public int PERM_MACHINE { get; set; } = 0;

        /// <summary>
        /// 전기 권한
        /// </summary>
        public int PERM_ELEC { get; set; } = 0;

        /// <summary>
        /// 승강 권한
        /// </summary>
        public int PERM_LIFT { get; set; } = 0;

        /// <summary>
        /// 소방 권한
        /// </summary>
        public int PERM_FIRE { get; set; } = 0;

        /// <summary>
        /// 건축 권한
        /// </summary>
        public int PERM_CONSTRUCT { get; set; } = 0;

        /// <summary>
        /// 통신 권한
        /// </summary>
        public int PERM_NETWORK { get; set; } = 0;

        /// <summary>
        /// 미화 권한
        /// </summary>
        public int PERM_BEAUTY { get; set; } = 0;

        /// <summary>
        /// 보안 권한
        /// </summary>
        public int PERM_SECURITY { get; set; } = 0;

        /// <summary>
        /// 자재 권한
        /// </summary>
        public int PERM_MATERIAL { get; set; } = 0;

        /// <summary>
        /// 에너지 권한
        /// </summary>
        public int PERM_ENERGY { get; set; } = 0;

        /// <summary>
        /// 사용자 설정 권한
        /// </summary>
        public int PERM_USER { get; set; } = 0;

        /// <summary>
        /// VOC 권한
        /// </summary>
        public int PERM_VOC { get; set; } = 0;

        /// <summary>
        /// 관리자유무
        /// </summary>
        public bool ADMIN_YN { get; set; } = false;

        /// <summary>
        /// 알람유무
        /// </summary>
        public bool ALRAM_YN { get; set; } = false;

        /// <summary>
        /// 재직여부
        /// </summary>
        public int STATUS { get; set; } = 2;

        /// <summary>
        /// 기계민원 처리권한
        /// </summary>
        public bool VOC_MACHINE { get; set; } = false;

        /// <summary>
        /// 전기민원 처리권한
        /// </summary>
        public bool VOC_ELEC { get; set; } = false;

        /// <summary>
        /// 승강민원 처리권한
        /// </summary>
        public bool VOC_LIFT { get; set; } = false;

        /// <summary>
        /// 소방민원 처리권한
        /// </summary>
        public bool VOC_FIRE { get; set; } = false;

        /// <summary>
        /// 건축민원 처리권한
        /// </summary>
        public bool VOC_CONSTRUCT { get; set; } = false;

        /// <summary>
        /// 통신민원 처리권한
        /// </summary>
        public bool VOC_NETWORK { get; set; } = false;

        /// <summary>
        /// 미화민원 처리권한
        /// </summary>
        public bool VOC_BEAUTY { get; set; } = false;

        /// <summary>
        /// 보안민원 처리권한
        /// </summary>
        public bool VOC_SECURITY { get; set; } = false;

        /// <summary>
        /// 기타 처리권한
        /// </summary>
        public bool VOC_ETC { get; set; } = false;
    }
}
