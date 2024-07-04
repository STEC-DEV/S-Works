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
        public string? ContractDT { get; set; }

        /// <summary>
        /// 설비메뉴 권한
        /// </summary>
        public sbyte? PermMachine { get; set; } = 0;

        /// <summary>
        /// 승강메뉴 권한
        /// </summary>
        public sbyte? PermLift { get; set; } = 0;

        /// <summary>
        /// 소방메뉴 권한
        /// </summary>
        public sbyte? PermFire { get; set; } = 0;

        /// <summary>
        /// 건축메뉴 권한
        /// </summary>
        public sbyte? PermConstruct { get; set; } = 0;

        /// <summary>
        /// 통신메뉴 권한
        /// </summary>
        public sbyte? PermNetwork { get; set; } = 0;

        /// <summary>
        /// 미화 권한
        /// </summary>
        public sbyte? PermBeauty { get; set; } = 0;

        /// <summary>
        /// 보안메뉴 권한
        /// </summary>
        public sbyte? PermSecurity { get; set; } = 0;

        /// <summary>
        /// 자재메뉴 권한
        /// </summary>
        public sbyte? PermMaterial { get; set; } = 0;

        /// <summary>
        /// 에너지메뉴 권한
        /// </summary>
        public sbyte? PermEnergy { get; set; } = 0;

        /// <summary>
        /// VOC 권한
        /// </summary>
        public sbyte? PermVoc { get; set; } = 0;

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