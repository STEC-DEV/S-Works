using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("admin_place_tb")]
[Index("AdminTbId", Name = "fk_ADMIN_TB_has_PLACE_ADMIN_TB1_idx")]
[Index("PlaceId", Name = "fk_ADMIN_TB_has_PLACE_PLACE1_idx")]
public partial class AdminPlaceTb
{
    /// <summary>
    /// 관리자 사업장 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

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
    /// (외래키) 관리자 테이블 인덱스
    /// </summary>
    [Column("ADMIN_TB_ID", TypeName = "int(11)")]
    public int? AdminTbId { get; set; }

    /// <summary>
    /// (외래키) 사업장 테이블 인덱스
    /// </summary>
    [Column("PLACE_ID", TypeName = "int(11)")]
    public int? PlaceId { get; set; }

    [ForeignKey("AdminTbId")]
    [InverseProperty("AdminPlaceTbs")]
    public virtual AdminTb? AdminTb { get; set; }

    [ForeignKey("PlaceId")]
    [InverseProperty("AdminPlaceTbs")]
    public virtual PlaceTb? Place { get; set; }
}
