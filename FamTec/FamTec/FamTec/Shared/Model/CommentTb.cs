﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 민원 답변
/// </summary>
[Table("comment_tb")]
[Index("UserTbId", Name = "FKUsertbId_202407311313")]
[Index("VocTbId", Name = "fk_comment_tb_voc_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class CommentTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 댓글내용
    /// </summary>
    [Column("CONTENT")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    /// <summary>
    /// 처리상태
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int Status { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string UpdateUser { get; set; } = null!;

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    [StringLength(255)]
    public string? Image1 { get; set; }

    [StringLength(255)]
    public string? Image2 { get; set; }

    [StringLength(255)]
    public string? Image3 { get; set; }

    /// <summary>
    /// 카카오API 전송 유무
    /// </summary>
    [Column("KAKAOSEND_YN")]
    public bool KakaosendYn { get; set; }

    [Column("VOC_TB_ID", TypeName = "int(11)")]
    public int VocTbId { get; set; }

    [Column("USER_TB_ID", TypeName = "int(11)")]
    public int UserTbId { get; set; }

    [ForeignKey("UserTbId")]
    [InverseProperty("CommentTbs")]
    public virtual UsersTb UserTb { get; set; } = null!;

    [ForeignKey("VocTbId")]
    [InverseProperty("CommentTbs")]
    public virtual VocTb VocTb { get; set; } = null!;
}
