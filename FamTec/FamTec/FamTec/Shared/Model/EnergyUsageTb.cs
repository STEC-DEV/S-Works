using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("energy_usage_tb")]
[Index("MeterItemTbId", Name = "fk_ENERGY_USAGE_TB_METER_ITEM_TB1_idx")]
public partial class EnergyUsageTb
{
    /// <summary>
    /// 에너지 사용량 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사용량
    /// </summary>
    [Column("USAGE")]
    public float? Usage { get; set; }

    /// <summary>
    /// 검침일자
    /// </summary>
    [Column("METER_DT", TypeName = "datetime")]
    public DateTime? MeterDt { get; set; }

    /// <summary>
    /// 생성일
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(45)]
    public string? CreateUser { get; set; }

    /// <summary>
    /// 수정일
    /// </summary>
    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
    [Column("UPDATE_USER")]
    [StringLength(45)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제여부
    /// </summary>
    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    /// <summary>
    /// 삭제일
    /// </summary>
    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    /// <summary>
    /// 삭제자
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(45)]
    public string? DelUser { get; set; }

    /// <summary>
    /// (외래키)검침항목 인덱스
    /// </summary>
    [Column("METER_ITEM_TB_ID", TypeName = "int(11)")]
    public int? MeterItemTbId { get; set; }

    [ForeignKey("MeterItemTbId")]
    [InverseProperty("EnergyUsageTbs")]
    public virtual MeterItemTb? MeterItemTb { get; set; }
}
