using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 부서
/// </summary>
[Table("departments_tb")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class DepartmentsTb
{
    /// <summary>
    /// 부서 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 부서명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

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
    /// 관리부서YN
    /// </summary>
    [Column("MANAGEMENT_YN")]
    public bool ManagementYn { get; set; }

    [InverseProperty("DepartmentTb")]
    public virtual ICollection<AdminTb> AdminTbs { get; set; } = new List<AdminTb>();

    [InverseProperty("DepartmentTb")]
    public virtual ICollection<PlaceTb> PlaceTbs { get; set; } = new List<PlaceTb>();
}
