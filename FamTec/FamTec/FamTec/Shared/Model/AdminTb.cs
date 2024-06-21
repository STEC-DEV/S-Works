using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("admin_tb")]
[Index("DepartmentTbId", Name = "fk_ADMIN_TB_DEPARTMENT_TB1_idx")]
[Index("UserTbId", Name = "fk_ADMIN_TB_USER_TB_idx")]
public partial class AdminTb
{
    /// <summary>
    /// 관리자 테이블 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 계정유형
    /// </summary>
    [Column("TYPE")]
    [StringLength(255)]
    public string? Type { get; set; }

    /// <summary>
    /// 생성일
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(255)]
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
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제여부
    /// </summary>
    [Column("DEL_YN", TypeName = "tinyint(4)")]
    public sbyte? DelYn { get; set; }

    /// <summary>
    /// 삭제일
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
    /// (외래키) 사용자 테이블 인덱스
    /// </summary>
    [Column("USER_TB_ID", TypeName = "int(11)")]
    public int? UserTbId { get; set; }

    /// <summary>
    /// (외래키) 부서 테이블 인덱스
    /// </summary>
    [Column("DEPARTMENT_TB_ID", TypeName = "int(11)")]
    public int? DepartmentTbId { get; set; }

    [InverseProperty("AdminTb")]
    public virtual ICollection<AdminPlaceTb> AdminPlaceTbs { get; set; } = new List<AdminPlaceTb>();

    [ForeignKey("DepartmentTbId")]
    [InverseProperty("AdminTbs")]
    public virtual DepartmentTb? DepartmentTb { get; set; }

    [ForeignKey("UserTbId")]
    [InverseProperty("AdminTbs")]
    public virtual UserTb? UserTb { get; set; }
}
