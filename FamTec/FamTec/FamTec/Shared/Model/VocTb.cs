using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("voc_tb")]
[Index("BuildingTbId", Name = "FK_BULDING_202406141619")]
public partial class VocTb
{
    /// <summary>
    /// 민원 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 민원인 이름
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 민원 제목
    /// </summary>
    [Column("TITLE")]
    [StringLength(255)]
    public string? Title { get; set; }

    /// <summary>
    /// 민원 내용
    /// </summary>
    [Column("CONTENT")]
    [StringLength(255)]
    public string? Content { get; set; }

    /// <summary>
    /// 민원인 전화번호
    /// </summary>
    [Column("PHONE")]
    [StringLength(255)]
    public string? Phone { get; set; }

    /// <summary>
    /// 민원처리 상태
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int? Status { get; set; }

    /// <summary>
    /// 민원유형
    /// </summary>
    [Column("TYPE", TypeName = "int(11)")]
    public int? Type { get; set; }

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
    /// 첨부파일_1
    /// </summary>
    [StringLength(255)]
    public string? Image1 { get; set; }

    /// <summary>
    /// 첨부파일_2
    /// </summary>
    [StringLength(255)]
    public string? Image2 { get; set; }

    /// <summary>
    /// 첨부파일_3
    /// </summary>
    [StringLength(255)]
    public string? Image3 { get; set; }

    /// <summary>
    /// 완료시간
    /// </summary>
    [Column("COMPLETE_TIME", TypeName = "datetime")]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 소요시간
    /// </summary>
    [Column("TOTAL_TIME", TypeName = "datetime")]
    public DateTime? TotalTime { get; set; }

    /// <summary>
    /// (외래키) 건물 인덱스
    /// </summary>
    [Column(TypeName = "int(11)")]
    public int? BuildingTbId { get; set; }

    [InverseProperty("VocTb")]
    public virtual ICollection<AlarmTb> AlarmTbs { get; set; } = new List<AlarmTb>();

    [ForeignKey("BuildingTbId")]
    [InverseProperty("VocTbs")]
    public virtual BuildingTb? BuildingTb { get; set; }

    [InverseProperty("VocTb")]
    public virtual ICollection<CommentTb> CommentTbs { get; set; } = new List<CommentTb>();
}
