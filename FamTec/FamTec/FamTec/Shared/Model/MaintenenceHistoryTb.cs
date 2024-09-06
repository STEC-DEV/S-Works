using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 유지보수 이력
/// </summary>
[Table("maintenence_history_tb")]
[Index("FacilityTbId", Name = "fk_maintenence_history_tb_facility_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class MaintenenceHistoryTb
{
    /// <summary>
    /// 이력 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 이력명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 작업구분
    /// </summary>
    [Column("TYPE", TypeName = "int(11)")]
    public int Type { get; set; }

    /// <summary>
    /// 작업자
    /// </summary>
    [Column("WORKER")]
    [StringLength(255)]
    public string Worker { get; set; } = null!;

    /// <summary>
    /// 작업일자
    /// </summary>
    [Column("WORKDT", TypeName = "datetime")]
    public DateTime Workdt { get; set; }

    /// <summary>
    /// 소요비용
    /// </summary>
    [Column("TOTAL_PRICE")]
    public float TotalPrice { get; set; }

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

    /// <summary>
    /// 유지보수 취소사유 설명
    /// </summary>
    [Column("NOTE")]
    [StringLength(50)]
    public string? Note { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }

    [Column("FACILITY_TB_ID", TypeName = "int(11)")]
    public int FacilityTbId { get; set; }

    [ForeignKey("FacilityTbId")]
    [InverseProperty("MaintenenceHistoryTbs")]
    public virtual FacilityTb FacilityTb { get; set; } = null!;

    [InverseProperty("MaintenenceHistoryTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();

    [InverseProperty("MaintenanceTb")]
    public virtual ICollection<UseMaintenenceMaterialTb> UseMaintenenceMaterialTbs { get; set; } = new List<UseMaintenenceMaterialTb>();
}
