using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("kakao_log_tb")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class KakaoLogTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("RESULT")]
    [StringLength(255)]
    public string? Result { get; set; }

    [Column("RECEIVER")]
    [StringLength(255)]
    public string? Receiver { get; set; }

    [Column("sender")]
    [StringLength(255)]
    public string? Sender { get; set; }

    [Column("content")]
    [StringLength(255)]
    public string? Content { get; set; }

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
}
