using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("meter_item_tb")]
[Index("MeterReaderTbId", Name = "fk_METER_ITEM_TB_METER_READER_TB1_idx")]
public partial class MeterItemTb
{
    /// <summary>
    /// 자동증가 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 검침항목
    /// </summary>
    [Column("METER_ITEM")]
    [StringLength(45)]
    public string? MeterItem { get; set; }

    /// <summary>
    /// 누적사용량
    /// </summary>
    [Column("ACCUM_USAGE")]
    public float? AccumUsage { get; set; }

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
    /// (외래키)검침기 인덱스
    /// </summary>
    [Column("METER_READER_TB_ID", TypeName = "int(11)")]
    public int? MeterReaderTbId { get; set; }

    [InverseProperty("MeterItemTb")]
    public virtual ICollection<EnergyUsageTb> EnergyUsageTbs { get; set; } = new List<EnergyUsageTb>();

    [ForeignKey("MeterReaderTbId")]
    [InverseProperty("MeterItemTbs")]
    public virtual MeterReaderTb? MeterReaderTb { get; set; }
}
