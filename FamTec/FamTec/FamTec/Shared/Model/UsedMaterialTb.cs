using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("used_material_tb")]
[Index("MaintenenceHistoryTbId", Name = "fk_used_material_maintenence_history_tb1_idx")]
[Index("MaterialTbId", Name = "fk_used_material_material_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class UsedMaterialTb
{
    /// <summary>
    /// 사용자재 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 생성일자
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(255)]
    public string? CreateUser { get; set; }

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제여부
    /// </summary>
    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    /// <summary>
    /// 삭제일자
    /// </summary>
    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    /// <summary>
    /// 삭제자
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    [Column("MAINTENENCE_HISTORY_TB_ID", TypeName = "int(11)")]
    public int? MaintenenceHistoryTbId { get; set; }

    [Column("MATERIAL_TB_ID", TypeName = "int(11)")]
    public int? MaterialTbId { get; set; }

    [ForeignKey("MaintenenceHistoryTbId")]
    [InverseProperty("UsedMaterialTbs")]
    public virtual MaintenenceHistoryTb? MaintenenceHistoryTb { get; set; }

    [ForeignKey("MaterialTbId")]
    [InverseProperty("UsedMaterialTbs")]
    public virtual MaterialTb? MaterialTb { get; set; }
}
