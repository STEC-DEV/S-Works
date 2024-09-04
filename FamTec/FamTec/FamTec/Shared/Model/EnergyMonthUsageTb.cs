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
[Index("MeterItemId", Name = "fk_energy_month_usage_tb_meter_item_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class EnergyMonthUsageTb
{
    [Key]
    [Column("MONTH_USAGE_ID", TypeName = "int(11)")]
    public int MonthUsageId { get; set; }

    /// <summary>
    /// 년도
    /// </summary>
    [Column("YEARS", TypeName = "int(11)")]
    public int Years { get; set; }

    /// <summary>
    /// 1월
    /// </summary>
    [Column("JAN")]
    public float? Jan { get; set; }

    /// <summary>
    /// 2월
    /// </summary>
    [Column("FEB")]
    public float? Feb { get; set; }

    /// <summary>
    /// 3월
    /// </summary>
    [Column("MAR")]
    public float? Mar { get; set; }

    /// <summary>
    /// 4월
    /// </summary>
    [Column("APR")]
    public float? Apr { get; set; }

    /// <summary>
    /// 5월
    /// </summary>
    [Column("MAY")]
    public float? May { get; set; }

    /// <summary>
    /// 6월
    /// </summary>
    [Column("JUN")]
    public float? Jun { get; set; }

    /// <summary>
    /// 7월
    /// </summary>
    [Column("JUL")]
    public float? Jul { get; set; }

    /// <summary>
    /// 8월
    /// </summary>
    [Column("AUG")]
    public float? Aug { get; set; }

    /// <summary>
    /// 9월
    /// </summary>
    [Column("SEP")]
    public float? Sep { get; set; }

    /// <summary>
    /// 10월
    /// </summary>
    [Column("OCT")]
    public float? Oct { get; set; }

    /// <summary>
    /// 11월
    /// </summary>
    [Column("NOV")]
    public float? Nov { get; set; }

    /// <summary>
    /// 12월
    /// </summary>
    [Column("DEC")]
    public float? Dec { get; set; }

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

    [ForeignKey("MeterItemId")]
    [InverseProperty("EnergyMonthUsageTbs")]
    public virtual MeterItemTb MeterItem { get; set; } = null!;
}
