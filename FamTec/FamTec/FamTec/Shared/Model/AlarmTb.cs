using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 실시간_알람
/// </summary>
[Table("alarm_tb")]
[Index("UsersTbId", Name = "fk_alarm_tb_users_tb1_idx")]
[Index("VocTbId", Name = "fk_alarm_tb_voc_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class AlarmTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 0: 접수 / 1: 변경 / ....
    /// </summary>
    [Column("TYPE", TypeName = "int(11)")]
    public int Type { get; set; }

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

    [Column("USERS_TB_ID", TypeName = "int(11)")]
    public int UsersTbId { get; set; }

    [Column("VOC_TB_ID", TypeName = "int(11)")]
    public int VocTbId { get; set; }

    [ForeignKey("UsersTbId")]
    [InverseProperty("AlarmTbs")]
    public virtual UsersTb UsersTb { get; set; } = null!;

    [ForeignKey("VocTbId")]
    [InverseProperty("AlarmTbs")]
    public virtual VocTb VocTb { get; set; } = null!;
}
