using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Admin.Place
{
    /// <summary>
    /// 사업장 등록 화면 DTO
    /// </summary>
    public class AddPlaceDTO
    {
   
        /// <summary>
        /// 사업장코드
        /// </summary>
        public string? PlaceCd { get; set; }

        /// <summary>
        /// 사업장 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 사업장 주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        public string? ContractNum { get; set; }

        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDT { get; set; }

        /// <summary>
        /// 설비메뉴 권한
        /// </summary>
        public bool? PermMachine { get; set; } = false;

        /// <summary>
        /// 승강메뉴 권한
        /// </summary>
        public bool? PermLift { get; set; } = false;

        /// <summary>
        /// 소방메뉴 권한
        /// </summary>
        public bool? PermFire { get; set; } = false;

        /// <summary>
        /// 건축메뉴 권한
        /// </summary>
        public bool? PermConstruct { get; set; } = false;

        /// <summary>
        /// 통신메뉴 권한
        /// </summary>
        public bool? PermNetwork { get; set; } = false;

        /// <summary>
        /// 미화 권한
        /// </summary>
        public bool? PermBeauty { get; set; } = false;

        /// <summary>
        /// 보안메뉴 권한
        /// </summary>
        public bool? PermSecurity { get; set; } = false;

        /// <summary>
        /// 자재메뉴 권한
        /// </summary>
        public bool? PermMaterial { get; set; } = false;

        /// <summary>
        /// 에너지메뉴 권한
        /// </summary>
        public bool? PermEnergy { get; set; } = false;

        /// <summary>
        /// VOC 권한
        /// </summary>
        public bool? PermVoc { get; set; } = false;

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        public bool? Status { get; set; } = true;

    }
}