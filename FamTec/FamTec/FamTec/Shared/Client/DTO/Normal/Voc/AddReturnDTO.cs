﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class AddVocReturnDTO
    {
        /// <summary>
        /// 접수코드
        /// </summary>
        public string ReceiptCode { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 접수일자
        /// </summary>
        public string CreateDT { get; set; }
    }
}
