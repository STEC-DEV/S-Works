using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 설비 &gt; 그룹
/// </summary>
[Table("facility_item_group_tb")]
[Index("FacilityTbId", Name = "fk_facility_item_group_facility_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class FacilityItemGroupTb
{
    /// <summary>
    /// 그룹 아이디
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 그룹명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

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

    [Column("FACILITY_TB_ID", TypeName = "int(11)")]
    public int FacilityTbId { get; set; }

    [InverseProperty("FacilityItemGroupTb")]
    public virtual ICollection<FacilityItemKeyTb> FacilityItemKeyTbs { get; set; } = new List<FacilityItemKeyTb>();

    [ForeignKey("FacilityTbId")]
    [InverseProperty("FacilityItemGroupTbs")]
    public virtual FacilityTb FacilityTb { get; set; } = null!;
}
