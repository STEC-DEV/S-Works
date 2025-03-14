﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material
{
    public class MaterialListDTO
    {
        /// <summary>
        /// 자재(품목) INDEX
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 품목코드
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 자재(품목)명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 규격
        /// </summary>
        public string? Standard { get; set; }

        /// <summary>
        /// 제조사
        /// </summary>
        public string? ManufacturingComp { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 안전재고
        /// </summary>
        public int? SafeNum { get; set; }
        /// <summary>
        /// 이미지
        /// </summary>
        public byte[]? Image { get; set; }

    }
}
