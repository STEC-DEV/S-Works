using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("comment_tb")]
[Index("VocTbid", Name = "VOC_202406211546")]
public partial class CommentTb
{
    /// <summary>
    /// 댓글 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 댓글 내용
    /// </summary>
    [Column("CONTENT")]
    [StringLength(255)]
    public string? Content { get; set; }

    /// <summary>
    /// 처리 상태
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int? Status { get; set; }

    /// <summary>
    /// 생성일
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(255)]
    public string? CreateUser { get; set; }

    /// <summary>
    /// 수정일
    /// </summary>
    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제여부
    /// </summary>
    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    /// <summary>
    /// 삭제일
    /// </summary>
    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    /// <summary>
    /// 삭제자
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// (외래키) 민원 인덱스
    /// </summary>
    [Column("VOC_TBID", TypeName = "int(11)")]
    public int? VocTbid { get; set; }

    [ForeignKey("VocTbid")]
    [InverseProperty("CommentTbs")]
    public virtual VocTb? VocTb { get; set; }
}
