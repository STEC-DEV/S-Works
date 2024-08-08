using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 층
/// </summary>
[Table("floor_tb")]
[Index("BuildingTbId", Name = "fk_floor_tb_building_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class FloorTb
{
    /// <summary>
    /// 층 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 층 이름
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

    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int BuildingTbId { get; set; }

    [ForeignKey("BuildingTbId")]
    [InverseProperty("FloorTbs")]
    public virtual BuildingTb BuildingTb { get; set; } = null!;

    [InverseProperty("FloorTb")]
    public virtual ICollection<RoomTb> RoomTbs { get; set; } = new List<RoomTb>();
}
