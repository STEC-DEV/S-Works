﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 설비 &gt; 그룹 &gt; 키
/// </summary>
[Table("facility_item_key_tb")]
[Index("FacilityItemGroupTbId", Name = "fk_facility_item_key_facility_item_group1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class FacilityItemKeyTb
{
    /// <summary>
    /// 요소명 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 요소명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

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

    [Column("FACILITY_ITEM_GROUP_TB_ID", TypeName = "int(11)")]
    public int FacilityItemGroupTbId { get; set; }

    [ForeignKey("FacilityItemGroupTbId")]
    [InverseProperty("FacilityItemKeyTbs")]
    public virtual FacilityItemGroupTb FacilityItemGroupTb { get; set; } = null!;

    [InverseProperty("FacilityItemKeyTb")]
    public virtual ICollection<FacilityItemValueTb> FacilityItemValueTbs { get; set; } = new List<FacilityItemValueTb>();
}
