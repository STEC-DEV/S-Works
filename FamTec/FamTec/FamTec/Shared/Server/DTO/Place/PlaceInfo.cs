﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FamTec.Shared.Server.DTO.Place
{
    public class PlaceInfo
    {
        /// <summary>
        /// 사업장 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 사업장주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 사업장명
        /// </summary>
        [Display(Name = "사업장명")]
        public string? Name { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        [Display(Name = "전화번호")]
        public string? Tel { get; set; }

        /// <summary>
        /// 계약번호
        /// </summary>
        [Display(Name = "계약번호")]
        public string? ContractNum { get; set; }

        /// <summary>
        /// 계약일자
        /// </summary>
        [Display(Name = "계약일자")]
        public DateTime? ContractDt { get; set; }

        /// <summary>
        /// 해약일자
        /// </summary>
        [Display(Name = "해약일자")]
        public DateTime? CancelDt { get; set; }

        /// <summary>
        /// 계약상태
        /// </summary>
        [Display(Name = "계약상태")]
        public bool? Status { get; set; }

        /// <summary>
        /// 관리부서 인덱스
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// 관리부서 명칭
        /// </summary>
        public string? DepartmentName { get; set; }



        public PlaceInfo()
        {
        }

        // 복사 생성자 추가
        public PlaceInfo(PlaceInfo source)
        {
            if (source != null)
            {
                Id = source.Id;
                Name = source.Name;
                Tel = source.Tel;
                ContractNum = source.ContractNum;
                ContractDt = source.ContractDt;
                CancelDt = source.CancelDt;
                Status = source.Status;
            }
        }

        // 깊은 복사 메서드 추가
        public PlaceInfo DeepCopy()
        {
            return new PlaceInfo(this);
        }

    }

}
