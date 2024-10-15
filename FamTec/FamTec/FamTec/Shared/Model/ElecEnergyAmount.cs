using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 전기 월청구 요금
/// </summary>
[Table("elec_energy_amount")]
[Index("PlaceTbId", Name = "fk_place_tb_id")]
[Index("Year", "Month", "PlaceTbId", Name = "uk_date", IsUnique = true)]
public partial class ElecEnergyAmount
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 년
    /// </summary>
    [Column("YEAR", TypeName = "int(11)")]
    public int Year { get; set; }

    /// <summary>
    /// 월
    /// </summary>
    [Column("MONTH", TypeName = "int(11)")]
    public int Month { get; set; }

    /// <summary>
    /// 월 청구요금
    /// </summary>
    [Column("CHARGE_PRICE")]
    public float? ChargePrice { get; set; }

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

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [ForeignKey("PlaceTbId")]
    [InverseProperty("ElecEnergyAmounts")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;
}
