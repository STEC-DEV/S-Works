using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 관리자 테이블
/// </summary>
[Table("admin_tb")]
[Index("DepartmentTbId", Name = "fk_admin_tb_departments_tb1_idx")]
[Index("UserTbId", Name = "fk_admin_tb_users_tb_idx")]
[Index("UserTbId", "DepartmentTbId", Name = "uk_adminid", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class AdminTb
{
    /// <summary>
    /// 관리자 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 계정유형
    /// </summary>
    [Column("TYPE")]
    [StringLength(255)]
    public string Type { get; set; } = null!;

    /// <summary>
    /// 생성일자
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    /// <summary>
    /// 수정일자
    /// </summary>
    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime UpdateDt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
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

    /// <summary>
    /// 삭제자
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// 사용자 인덱스
    /// </summary>
    [Column("USER_TB_ID", TypeName = "int(11)")]
    public int UserTbId { get; set; }

    /// <summary>
    /// 부서 인덱스\\n
    /// </summary>
    [Column("DEPARTMENT_TB_ID", TypeName = "int(11)")]
    public int DepartmentTbId { get; set; }

    [InverseProperty("AdminTb")]
    public virtual ICollection<AdminPlaceTb> AdminPlaceTbs { get; set; } = new List<AdminPlaceTb>();

    [ForeignKey("DepartmentTbId")]
    [InverseProperty("AdminTbs")]
    public virtual DepartmentsTb DepartmentTb { get; set; } = null!;

    [ForeignKey("UserTbId")]
    [InverseProperty("AdminTbs")]
    public virtual UsersTb UserTb { get; set; } = null!;
}
