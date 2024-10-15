using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Voc
{
    public class DetailVocCommentDTO
    {
        /// <summary>
        /// VOC Comment 인덱스
        /// </summary>
        public int? VocCommentId { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 처리상태
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 작성일
        /// </summary>
        public DateTime? CreateDT { get; set; }

        /// <summary>
        /// 작성자
        /// </summary>
        public string? CreateUser { get; set; }


        /// <summary>
        /// 이미지 파일명
        /// </summary>
        public List<string?> ImageName { get; set; } = new List<string?>();

        /// <summary>
        /// 이미지
        /// </summary>
        public List<byte[]?> Images { get; set; } = new List<byte[]?>(); // 이미지

        public DetailVocCommentDTO() { }

        public DetailVocCommentDTO(DetailVocCommentDTO source)
        {
            VocCommentId = source.VocCommentId;
            Content = source.Content;
            Status = source.Status;
            CreateDT = source.CreateDT;
            CreateUser = source.CreateUser;
            ImageName = new List<string?>(source.ImageName);
            Images = new List<byte[]?>(source.Images);
        }
    }
}
