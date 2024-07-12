using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("voc_tb")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class VocTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 민원제목
    /// </summary>
    [Column("TITLE")]
    [StringLength(255)]
    public string? Title { get; set; }

    /// <summary>
    /// 민원내용
    /// </summary>
    [Column("CONTENT")]
    [StringLength(255)]
    public string? Content { get; set; }

    [Column("TYPE", TypeName = "int(11)")]
    public int? Type { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("PHONE")]
    [StringLength(255)]
    public string? Phone { get; set; }

    /// <summary>
    /// 민원처리상태
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int? Status { get; set; }

    /// <summary>
    /// 답변회신여부
    /// </summary>
    [Column("REPLY_YN")]
    public bool? ReplyYn { get; set; }

    [Column("COMPLETE_DT", TypeName = "datetime")]
    public DateTime? CompleteDt { get; set; }

    [Column("DURATION_DT", TypeName = "datetime")]
    public DateTime? DurationDt { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string? CreateUser { get; set; }

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// 건물 인덱스
    /// </summary>
    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int? BuildingTbId { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image1 { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image2 { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image3 { get; set; }

    [InverseProperty("VocTb")]
    public virtual ICollection<AlarmTb> AlarmTbs { get; set; } = new List<AlarmTb>();

    [InverseProperty("VocTb")]
    public virtual ICollection<CommentTb> CommentTbs { get; set; } = new List<CommentTb>();
}
