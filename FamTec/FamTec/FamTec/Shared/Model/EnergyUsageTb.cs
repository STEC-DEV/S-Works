using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 에너지 검침 기록 - 일별
/// </summary>
[Table("energy_usage_tb")]
[Index("MeterItemId", Name = "fk_energy_usage_tb_meter_item_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class EnergyUsageTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사용량
    /// </summary>
    [Column("USE_AMOUNT")]
    public float? UseAmount { get; set; }

    /// <summary>
    /// 검침일자
    /// </summary>
    [Column("METER_DT", TypeName = "datetime")]
    public DateTime? MeterDt { get; set; }

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

    [Column("METER_ITEM_ID", TypeName = "int(11)")]
    public int? MeterItemId { get; set; }

    [ForeignKey("MeterItemId")]
    [InverseProperty("EnergyUsageTbs")]
    public virtual MeterItemTb? MeterItem { get; set; }
}
