using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared
{
    public class SessionInfo
    {
        /// <summary>
        /// 로그인 후 인덱스
        /// </summary>
        public int? UserIdx { get; set; } = 4;

        /// <summary>
        /// 로그인아이디
        /// </summary>
        public string? UserId { get; set; } = "TESTUSER";

        /// <summary>
        /// 사용자이름
        /// </summary>
        public string? Name { get; set; } = "테스트관리자";

        /// <summary>
        /// 관리자 여부
        /// </summary>
        public sbyte? AdminYN { get; set; } = 1;

        /// <summary>
        /// 알람여부
        /// </summary>
        public sbyte? AlarmYN { get; set; } = 1;

        /// <summary>
        /// 재직여부
        /// </summary>
        public sbyte? Status { get; set; } = 1;

        /// <summary>
        /// 계정유형
        /// </summary>
        public string? Type { get; set; } = "마스터";

        /// <summary>
        /// 부서명
        /// </summary>
        public string? DepartMentName { get; set; } = "에스텍시스템";

        /// <summary>
        /// 직책
        /// </summary>
        public string? Job { get; set; } = "부서장";

        /// <summary>
        /// 관리자의경우 사업장 여러개
        /// </summary>
        public List<int> PlaceIdx = new List<int>() { 4, 5, 6 };

        /// <summary>
        /// 그중 선택한거
        /// </summary>
        public int? selectPlace { get; set; } = 5;

    }
}
