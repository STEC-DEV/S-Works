using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("itemvalue_tb")]
[Index("ItemKeyId", Name = "FK_ItemKey_202407041727")]
public partial class ItemvalueTb
{
    /// <summary>
    /// 값 테이블 아이디
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 아이템의 값 몇개
    /// </summary>
    [Column("ITEMVALUE")]
    [StringLength(255)]
    public string Itemvalue { get; set; } = null!;

    /// <summary>
    /// 단위
    /// </summary>
    [Column("UNIT")]
    [StringLength(255)]
    public string? Unit { get; set; }

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
    /// 키 테이블 인덱스
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int? ItemKeyId { get; set; }

    [ForeignKey("ItemKeyId")]
    [InverseProperty("ItemvalueTbs")]
    public virtual ItemkeyTb? ItemKey { get; set; }
}
