﻿using Microsoft.Identity.Client;

namespace FamTec.Shared.Server.DTO.Maintenence
{
    public class DetailMaintanceDTO
    {
        /// <summary>
        /// 유지보수 ID
        /// </summary>
        public int MaintanceID { get; set; }
        
        /// <summary>
        /// 작업일자
        /// </summary>
        public string? WorkDT { get; set; } 

        /// <summary>
        /// 작업명칭
        /// </summary>
        public string? WorkName { get; set; }

        /// <summary>
        /// 작업구분
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 작업자
        /// </summary>
        public string? Worker { get; set; }

        /// <summary>
        /// 소요비용
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 이미지 파일명
        /// </summary>
        public string? ImageName { get; set; }

        /// <summary>
        /// 이미지
        /// </summary>
        public byte[]? Image { get; set; }

        public List<UseMaterialDTO> UseMaterialList { get; set; } = new List<UseMaterialDTO>();
    }

    /// <summary>
    /// 사용자재 DTO
    /// </summary>
    public class UseMaterialDTO
    {
        /// <summary>
        /// 사용자재 테이블 ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 품목 ID
        /// </summary>
        public int MaterialID { get; set; }

        /// <summary>
        /// 출고창고 ID
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// 품목 Code
        /// </summary>
        public string? MaterialCode { get; set;}

        /// <summary>
        /// 품목명
        /// </summary>
        public string? MaterialName { get; set; }
        
        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacuringComp { get; set; }

        /// <summary>
        /// 출고창고 명
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 수량
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 출고금액
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Note { get; set; }
    }
}
