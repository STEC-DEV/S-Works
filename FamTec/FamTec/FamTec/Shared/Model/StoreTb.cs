using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("store_tb")]
[Index("MaterialTbid", Name = "FK_MATERIAL_202406211523")]
[Index("RoomTbid", Name = "FK_ROOM_202406211523")]
public partial class StoreTb
{
    /// <summary>
    /// 입출고 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 입출고 구분
    /// </summary>
    [Column("IN_OUT", TypeName = "int(11)")]
    public int? InOut { get; set; }

    /// <summary>
    /// 수량
    /// </summary>
    [Column("NUM", TypeName = "int(11)")]
    public int? Num { get; set; }

    /// <summary>
    /// 단가
    /// </summary>
    [Column("UNIT_PRICE")]
    public float? UnitPrice { get; set; }

    /// <summary>
    /// 금액
    /// </summary>
    [Column("PRICE")]
    public float? Price { get; set; }

    /// <summary>
    /// 입출고날짜
    /// </summary>
    [Column("INOUT_DATE", TypeName = "datetime")]
    public DateTime? InoutDate { get; set; }

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
    /// (외래키)공간 인덱스
    /// </summary>
    [Column("ROOM_TBID", TypeName = "int(11)")]
    public int? RoomTbid { get; set; }

    /// <summary>
    /// (외래키)자재 인덱스
    /// </summary>
    [Column("MATERIAL_TBID", TypeName = "int(11)")]
    public int? MaterialTbid { get; set; }

    [ForeignKey("MaterialTbid")]
    [InverseProperty("StoreTbs")]
    public virtual MaterialTb? MaterialTb { get; set; }

    [ForeignKey("RoomTbid")]
    [InverseProperty("StoreTbs")]
    public virtual RoomTb? RoomTb { get; set; }
}
