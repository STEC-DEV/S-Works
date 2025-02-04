﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material
{
    public class DetailMaterialDTO
    {
        /// <summary>
        /// 품목ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 품목코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 품목명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacturingComp { get; set; }

        /// <summary>
        /// 안전재고 수량
        /// </summary>
        public int? SafeNum { get; set; }

        /// <summary>
        /// 건물id
        /// </summary>
        public int? BuildingId { get; set; }

        /// <summary>
        /// 건물 이름
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 공간ID
        /// </summary>
        public int? RoomID { get; set; }

        /// <summary>
        /// 공간명칭
        /// </summary>
        public string? RoomName { get; set; }

        /// <summary>
        /// 이미지 파일명
        /// </summary>
        public string? ImageName { get; set; }

        /// <summary>
        /// 이미지
        /// </summary>
        public byte[]? Image { get; set; }
    }
}
