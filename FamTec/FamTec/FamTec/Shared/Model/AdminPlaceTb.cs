using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 관리자 사업장테이블
/// </summary>
[Table("admin_place_tb")]
[Index("AdminTbId", Name = "fk_admin_place_tb_admin_tb1_idx")]
[Index("PlaceTbId", Name = "fk_admin_place_tb_place_tb1_idx")]
[Index("AdminTbId", "PlaceTbId", Name = "uk_admin", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class AdminPlaceTb
{
    /// <summary>
    /// 관리자 사업장 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 생성일자
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// 수정일자
    /// </summary>
    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string UpdateUser { get; set; } = null!;

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

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    [Column("ADMIN_TB_ID", TypeName = "int(11)")]
    public int AdminTbId { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [ForeignKey("AdminTbId")]
    [InverseProperty("AdminPlaceTbs")]
    public virtual AdminTb AdminTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("AdminPlaceTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;
}
