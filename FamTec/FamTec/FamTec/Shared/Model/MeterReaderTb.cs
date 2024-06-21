using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("meter_reader_tb")]
[Index("BuildingTbId", Name = "fk_METER_READER_TB_BUILDING_TB1_idx")]
public partial class MeterReaderTb
{
    /// <summary>
    /// 자동증가 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 카테고리
    /// </summary>
    [Column("CATEGORY")]
    [StringLength(45)]
    public string? Category { get; set; }

    /// <summary>
    /// 계약종별
    /// </summary>
    [Column("TYPE")]
    [StringLength(45)]
    public string? Type { get; set; }

    /// <summary>
    /// 검침항목
    /// </summary>
    [Column("METER_ITEM")]
    [StringLength(45)]
    public string? MeterItem { get; set; }

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
    /// (외래키)건물인덱스
    /// </summary>
    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int? BuildingTbId { get; set; }

    [ForeignKey("BuildingTbId")]
    [InverseProperty("MeterReaderTbs")]
    public virtual BuildingTb? BuildingTb { get; set; }

    [InverseProperty("MeterReaderTb")]
    public virtual ICollection<EnergyMonthUsageTb> EnergyMonthUsageTbs { get; set; } = new List<EnergyMonthUsageTb>();

    [InverseProperty("MeterReaderTb")]
    public virtual ICollection<MeterItemTb> MeterItemTbs { get; set; } = new List<MeterItemTb>();
}
