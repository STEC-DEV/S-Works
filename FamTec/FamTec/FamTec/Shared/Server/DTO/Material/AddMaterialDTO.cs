﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Server.DTO.Material
{
    public class AddMaterialDTO
    {
        /// <summary>
        /// 자재(품목) INDEX
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 입출고 구분 I:IN / O:OUT
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 자재코드
        /// </summary>
        public string? MaterialCode { get; set; }

        /// <summary>
        /// 자재명
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
        public string? Mfr { get; set; }

        /// <summary>
        /// 안전재고
        /// </summary>
        public int? SafeNum { get; set; }

        /// <summary>
        /// 재고위치
        /// </summary>
        public int? RoomIdx { get; set; }


    }
}
