using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("voc_tb")]
[Index("BuildingTbId", Name = "building_tb_202407250842")]
[Index("Code", Name = "uk_voccode", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class VocTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// VOC코드_민원조회용
    /// </summary>
    [Column("CODE")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 민원제목
    /// </summary>
    [Column("TITLE")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 민원내용
    /// </summary>
    [Column("CONTENT")]
    [StringLength(255)]
    public string Content { get; set; } = null!;

    /// <summary>
    /// 모바일:0 / 웹:1
    /// </summary>
    [Column("DIVISION", TypeName = "int(11)")]
    public int? Division { get; set; }

    /// <summary>
    /// 0 : 미분류
    /// 1 : 기계
    /// 2 : 전기
    /// 3 : 승강
    /// 4 : 소방
    /// 5 : 건축
    /// 6 : 통신
    /// 7 : 미화
    /// 8 : 보안
    /// </summary>
    [Column("TYPE", TypeName = "int(11)")]
    public int Type { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("PHONE")]
    [StringLength(255)]
    public string? Phone { get; set; }

    /// <summary>
    /// 민원처리상태
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int Status { get; set; }

    /// <summary>
    /// 답변회신여부
    /// </summary>
    [Column("REPLY_YN")]
    public bool ReplyYn { get; set; }

    /// <summary>
    /// 완료시간
    /// </summary>
    [Column("COMPLETE_DT", TypeName = "datetime")]
    public DateTime? CompleteDt { get; set; }

    /// <summary>
    /// 소요시간
    /// </summary>
    [Column("DURATION_DT")]
    [StringLength(255)]
    public string? DurationDt { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string UpdateUser { get; set; } = null!;

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image1 { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image2 { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [StringLength(255)]
    public string? Image3 { get; set; }

    /// <summary>
    /// 카카오API 전송 유무
    /// </summary>
    [Column("KAKAOSEND_YN")]
    public bool KakaosendYn { get; set; }

    /// <summary>
    /// 건물 인덱스
    /// </summary>
    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int BuildingTbId { get; set; }

    [InverseProperty("VocTb")]
    public virtual ICollection<AlarmTb> AlarmTbs { get; set; } = new List<AlarmTb>();

    [ForeignKey("BuildingTbId")]
    [InverseProperty("VocTbs")]
    public virtual BuildingTb BuildingTb { get; set; } = null!;

    [InverseProperty("VocTb")]
    public virtual ICollection<CommentTb> CommentTbs { get; set; } = new List<CommentTb>();

    [InverseProperty("VocTb")]
    public virtual ICollection<KakaoLogTb> KakaoLogTbs { get; set; } = new List<KakaoLogTb>();
}
