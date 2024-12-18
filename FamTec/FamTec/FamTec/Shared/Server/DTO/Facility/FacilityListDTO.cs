﻿namespace FamTec.Shared.Server.DTO.Facility
{
    /// <summary>
    /// 설비 LIST DTO
    /// </summary>
    public class FacilityListDTO
    {
        /// <summary>
        /// 설비 인덱스
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 설비명칭
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 형식
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 개수
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 설치년월
        /// </summary>
        public DateTime? EquipDT { get; set; }

        /// <summary>
        /// 내용연수
        /// </summary>
        public string? LifeSpan { get; set; }

        /// <summary>
        /// 규격용량
        /// </summary>
        public string? StandardCapacity { get; set; }

        /// <summary>
        /// 교체년월
        /// </summary>
        public DateTime? ChangeDT { get; set; }

        /// <summary>
        /// 공간 명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 건물명칭
        /// </summary>
        public string? BuildingName { get; set; }

    }
}
