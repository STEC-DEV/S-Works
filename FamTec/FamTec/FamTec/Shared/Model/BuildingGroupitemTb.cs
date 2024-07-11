using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("building_groupitem_tb")]
public partial class BuildingGroupitemTb
{
    /// <summary>
    /// 그룹테이블 아이디
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 명칭 _주차장
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 생성시간
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(15)]
    public string? CreateUser { get; set; }

    /// <summary>
    /// 수정시간
    /// </summary>
    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
    [Column("UPDATE_USER")]
    [StringLength(15)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제시간
    /// </summary>
    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    /// <summary>
    /// 삭제자
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(15)]
    public string? DelUser { get; set; }

    /// <summary>
    /// 삭제여부
    /// </summary>
    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    /// <summary>
    /// 건물테이블아이디(외래키)
    /// </summary>
    [Column("BUILDING_ID", TypeName = "int(11)")]
    public int? BuildingId { get; set; }

    [InverseProperty("GroupItem")]
    public virtual ICollection<BuildingItemkeyTb> BuildingItemkeyTbs { get; set; } = new List<BuildingItemkeyTb>();
}
