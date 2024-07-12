using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 설비 &gt; 그룹 &gt; 키 &gt; 값
/// </summary>
[Table("facility_item_value_tb")]
[Index("FacilityItemKeyTbId", Name = "fk_facility_item_value_facility_item_key1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class FacilityItemValueTb
{
    /// <summary>
    /// 값 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 값
    /// </summary>
    [Column("ITEM_VALUE")]
    [StringLength(255)]
    public string? ItemValue { get; set; }

    /// <summary>
    /// 단위
    /// </summary>
    [Column("UNIT")]
    [StringLength(255)]
    public string? Unit { get; set; }

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

    /// <summary>
    /// 수정일자
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

    [Column("FACILITY_ITEM_KEY_TB_ID", TypeName = "int(11)")]
    public int? FacilityItemKeyTbId { get; set; }

    [ForeignKey("FacilityItemKeyTbId")]
    [InverseProperty("FacilityItemValueTbs")]
    public virtual FacilityItemKeyTb? FacilityItemKeyTb { get; set; }
}
