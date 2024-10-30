using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("kakao_log_tb")]
[Index("BuildingTbId", Name = "FK_BUILDING_TB_ID_202408080816")]
[Index("PlaceTbId", Name = "FK_PLACE_TB_ID_202408080816")]
[Index("VocTbId", Name = "FK_VOC_TB_ID_202408080816")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class KakaoLogTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// MSGID
    /// </summary>
    [Column("MSGID")]
    [StringLength(50)]
    public string? Msgid { get; set; }

    /// <summary>
    /// 전송결과 코드
    /// </summary>
    [Column("CODE")]
    [StringLength(255)]
    public string? Code { get; set; }

    /// <summary>
    /// 전송결과 사유
    /// </summary>
    [Column("MESSAGE")]
    [StringLength(255)]
    public string? Message { get; set; }

    /// <summary>
    /// 수신자번호
    /// </summary>
    [Column("PHONE")]
    [StringLength(50)]
    public string? Phone { get; set; }

    /// <summary>
    /// 전송결과업데이트
    /// </summary>
    [Column("MSG_UPDATE")]
    public bool? MsgUpdate { get; set; }

    /// <summary>
    /// rslt (상태)
    /// </summary>
    [Column("RSLT")]
    [StringLength(50)]
    public string? Rslt { get; set; }

    /// <summary>
    /// message (사유)
    /// </summary>
    [Column("RSLT_MESSAGE")]
    [StringLength(255)]
    public string? RsltMessage { get; set; }

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

    [Column("VOC_TB_ID", TypeName = "int(11)")]
    public int VocTbId { get; set; }

    /// <summary>
    /// 사업장ID
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    /// <summary>
    /// 건물ID
    /// </summary>
    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int BuildingTbId { get; set; }

    [ForeignKey("BuildingTbId")]
    [InverseProperty("KakaoLogTbs")]
    public virtual BuildingTb BuildingTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("KakaoLogTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [ForeignKey("VocTbId")]
    [InverseProperty("KakaoLogTbs")]
    public virtual VocTb VocTb { get; set; } = null!;
}
