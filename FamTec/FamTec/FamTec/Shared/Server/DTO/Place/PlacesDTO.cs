using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Place
{
    /// <summary>
    /// 사업장 DTO
    /// </summary>
    public class PlacesDTO
    {
        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? PlaceIndex { get; set; }

        /// <summary>
        /// 사업장 코드
        /// </summary>
        public string? PlaceCd { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        public string? CONTRACT_NUM { get; set; }

        /// <summary>
        /// 사업장 명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 계약일자
        /// </summary>
        public DateTime? ContractDT { get; set; }

        /// <summary>
        /// 해약일자
        /// </summary>
        public DateTime? CancelDT { get; set; }

        /// <summary>
        /// 설비메뉴 권한
        /// </summary>
        public sbyte? PermMachine { get; set; }

        /// <summary>
        /// 승강메뉴 권한
        /// </summary>
        public sbyte? PermLift { get; set; }

        /// <summary>
        /// 소방메뉴 권한
        /// </summary>
        public sbyte? PermFire { get; set; }

        /// <summary>
        /// 건축메뉴 권한
        /// </summary>
        public sbyte? PermConstruct { get; set; }
        
        /// <summary>
        /// 통신메뉴 권한
        /// </summary>
        public sbyte? PermNetwork { get; set; }

        /// <summary>
        /// 미화메뉴 권한
        /// </summary>
        public sbyte? PermBeauty { get; set; }

        /// <summary>
        /// 보안메뉴 권한
        /// </summary>
        public sbyte? PermSecurity { get; set; }
    
        /// <summary>
        /// 자재메뉴 권한
        /// </summary>
        public sbyte? PermMaterial { get; set; }
            
        /// <summary>
        /// 에너지메뉴 권한
        /// </summary>
        public sbyte? PermEnergy { get; set; }

        /// <summary>
        /// 상태
        /// </summary>
        public sbyte? Status { get; set; }
    }
}
