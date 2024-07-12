using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 검침기 + 항목
/// </summary>
[Table("meter_item_tb")]
[Index("BuildingTbId", Name = "fk_meter_item_tb_building_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class MeterItemTb
{
    [Key]
    [Column("METER_ITEM_ID", TypeName = "int(11)")]
    public int MeterItemId { get; set; }

    /// <summary>
    /// 검침항목
    /// </summary>
    [Column("METER_ITEM")]
    [StringLength(255)]
    public string? MeterItem { get; set; }

    /// <summary>
    /// 누적사용량
    /// </summary>
    [Column("ACCUM_USAGE")]
    public float? AccumUsage { get; set; }

    /// <summary>
    /// 전기, 기계 ..
    /// </summary>
    [Column("CATEGORY")]
    [StringLength(255)]
    public string? Category { get; set; }

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

    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int? BuildingTbId { get; set; }

    [ForeignKey("BuildingTbId")]
    [InverseProperty("MeterItemTbs")]
    public virtual BuildingTb? BuildingTb { get; set; }

    [InverseProperty("MeterItem")]
    public virtual ICollection<EnergyMonthUsageTb> EnergyMonthUsageTbs { get; set; } = new List<EnergyMonthUsageTb>();

    [InverseProperty("MeterItem")]
    public virtual ICollection<EnergyUsageTb> EnergyUsageTbs { get; set; } = new List<EnergyUsageTb>();
}
