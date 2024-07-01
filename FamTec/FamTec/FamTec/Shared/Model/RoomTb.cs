using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("room_tb")]
[Index("FloorTbId", Name = "fk_ROOM_TB_FLOOR_TB1_idx")]
public partial class RoomTb
{
    /// <summary>
    /// 공간 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 공간명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

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
    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

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
    /// (외래키) 층 인덱스
    /// </summary>
    [Column("FLOOR_TB_ID", TypeName = "int(11)")]
    public int? FloorTbId { get; set; }

    [InverseProperty("RoomTb")]
    public virtual ICollection<FacilityTb> FacilityTbs { get; set; } = new List<FacilityTb>();

    [ForeignKey("FloorTbId")]
    [InverseProperty("RoomTbs")]
    public virtual FloorTb? FloorTb { get; set; }

    [InverseProperty("RoomTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();
}
