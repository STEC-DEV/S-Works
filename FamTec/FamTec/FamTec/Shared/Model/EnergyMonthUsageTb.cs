using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("energy_month_usage_tb")]
[Index("MeterReaderTbId", Name = "fk_ENERGY_MONTH_USAGE_TB_METER_READER_TB1_idx")]
public partial class EnergyMonthUsageTb
{
    /// <summary>
    /// 에너지 월별 사용량 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

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
    [Column("DEV")]
    public float? Dev { get; set; }

    /// <summary>
    /// (외래키)검침기 인덱스
    /// </summary>
    [Column("METER_READER_TB_ID", TypeName = "int(11)")]
    public int? MeterReaderTbId { get; set; }

    [ForeignKey("MeterReaderTbId")]
    [InverseProperty("EnergyMonthUsageTbs")]
    public virtual MeterReaderTb? MeterReaderTb { get; set; }
}
