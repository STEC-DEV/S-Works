using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 설비
/// </summary>
[Table("facility_tb")]
[Index("RoomTbId", Name = "fk_facility_tb_room_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class FacilityTb
{
    /// <summary>
    /// 설비 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 카테고리 (설비유형)
    /// </summary>
    [Column("CATEGORY")]
    [StringLength(255)]
    public string Category { get; set; } = null!;

    /// <summary>
    /// 설비명칭
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 형식
    /// </summary>
    [Column("TYPE")]
    [StringLength(255)]
    public string? Type { get; set; }

    /// <summary>
    /// 수량
    /// </summary>
    [Column("NUM", TypeName = "int(11)")]
    public int? Num { get; set; }

    /// <summary>
    /// 단위
    /// </summary>
    [Column("UNIT")]
    [StringLength(255)]
    public string? Unit { get; set; }

    /// <summary>
    /// 설치년월
    /// </summary>
    [Column("EQUIP_DT", TypeName = "datetime")]
    public DateTime? EquipDt { get; set; }

    /// <summary>
    /// 내용연수
    /// </summary>
    [Column("LIFESPAN")]
    [StringLength(255)]
    public string? Lifespan { get; set; }

    /// <summary>
    /// 규격용량
    /// </summary>
    [Column("STANDARD_CAPACITY")]
    [StringLength(255)]
    public string? StandardCapacity { get; set; }

    /// <summary>
    /// 교체년월
    /// </summary>
    [Column("CHANGE_DT", TypeName = "datetime")]
    public DateTime? ChangeDt { get; set; }

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

    [StringLength(255)]
    public string? Image { get; set; }

    [Column("ROOM_TB_ID", TypeName = "int(11)")]
    public int RoomTbId { get; set; }

    [InverseProperty("FacilityTb")]
    public virtual ICollection<FacilityItemGroupTb> FacilityItemGroupTbs { get; set; } = new List<FacilityItemGroupTb>();

    [InverseProperty("FacilityTb")]
    public virtual ICollection<MaintenenceHistoryTb> MaintenenceHistoryTbs { get; set; } = new List<MaintenenceHistoryTb>();

    [ForeignKey("RoomTbId")]
    [InverseProperty("FacilityTbs")]
    public virtual RoomTb RoomTb { get; set; } = null!;
}
