using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("alarm_tb")]
[Index("UserTbId", Name = "FK_USER_202406141623")]
[Index("VocTbId", Name = "FK_VOC_202406141624")]
public partial class AlarmTb
{
    /// <summary>
    /// 알람 테이블 인덱스
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
    [StringLength(15)]
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
    [StringLength(15)]
    public string? UpdateUser { get; set; }

    /// <summary>
    /// 삭제일
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
    /// (외래키) 사용자 테이블 인덱스
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int? UserTbId { get; set; }

    /// <summary>
    /// (외래키) 민원 테이블 인덱스
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int? VocTbId { get; set; }

    [ForeignKey("UserTbId")]
    [InverseProperty("AlarmTbs")]
    public virtual UserTb? UserTb { get; set; }

    [ForeignKey("VocTbId")]
    [InverseProperty("AlarmTbs")]
    public virtual VocTb? VocTb { get; set; }
}
