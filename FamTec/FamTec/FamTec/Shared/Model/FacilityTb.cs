using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("facility_tb")]
[Index("RoomTbid", Name = "FK_ROOM_202406211458")]
public partial class FacilityTb
{
    /// <summary>
    /// 설비 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 카테고리(기계,전기..)
    /// </summary>
    [Column("CATEGORY")]
    [StringLength(255)]
    public string? Category { get; set; }

    /// <summary>
    /// 설비명칭
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 형식
    /// </summary>
    [Column("TYPE")]
    [StringLength(255)]
    public string? Type { get; set; }

    /// <summary>
    /// 규격용량
    /// </summary>
    [Column("STANDARD_CAPACITY")]
    public float? StandardCapacity { get; set; }

    /// <summary>
    /// 설치년월
    /// </summary>
    [Column("FAC_CREATE_DT", TypeName = "datetime")]
    public DateTime? FacCreateDt { get; set; }

    /// <summary>
    /// 내용연수
    /// </summary>
    [Column("LIFESPAN")]
    [StringLength(255)]
    public string? Lifespan { get; set; }

    /// <summary>
    /// 규격용량단위
    /// </summary>
    [Column("STANDARD_CAPACITY_UNIT")]
    [StringLength(255)]
    public string? StandardCapacityUnit { get; set; }

    /// <summary>
    /// 교체년월
    /// </summary>
    [Column("FAC_UPDATE_DT", TypeName = "datetime")]
    public DateTime? FacUpdateDt { get; set; }

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

    [ForeignKey("RoomTbid")]
    [InverseProperty("FacilityTbs")]
    public virtual RoomTb? RoomTb { get; set; }
}
