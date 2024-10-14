using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 에너지 월별 사용량
/// </summary>
[Table("energy_month_usage_tb")]
[Index("MeterItemId", "Year", "Month", "PlaceTbId", Name = "UK_METER_ITEM_ID_YEAR_MONTH", IsUnique = true)]
[Index("MeterItemId", Name = "fk_energy_month_usage_tb_meter_item_tb1_idx")]
[Index("PlaceTbId", Name = "fk_place_tb_id")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class EnergyMonthUsageTb
{
    [Key]
    [Column("MONTH_USAGE_ID", TypeName = "int(11)")]
    public int MonthUsageId { get; set; }

    /// <summary>
    /// 년도
    /// </summary>
    [Column("YEAR", TypeName = "int(11)")]
    public int Year { get; set; }

    /// <summary>
    /// 월
    /// </summary>
    [Column("MONTH", TypeName = "int(11)")]
    public int Month { get; set; }

    /// <summary>
    /// 월 총사용량
    /// </summary>
    [Column("TOTAL_USAGE")]
    public float? TotalUsage { get; set; }

    /// <summary>
    /// 청구금액
    /// </summary>
    [Column("TOTAL_PRICE")]
    public float? TotalPrice { get; set; }

    /// <summary>
    /// 단가금액
    /// </summary>
    [Column("UNIT_PRICE")]
    public float? UnitPrice { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

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
    /// 검침기 인덱스
    /// </summary>
    [Column("METER_ITEM_ID", TypeName = "int(11)")]
    public int MeterItemId { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [ForeignKey("MeterItemId")]
    [InverseProperty("EnergyMonthUsageTbs")]
    public virtual MeterItemTb MeterItem { get; set; } = null!;
}
