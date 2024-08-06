using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("users_tb")]
[Index("PlaceTbId", Name = "fk_users_tb_place_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class UsersTb
{
    /// <summary>
    /// 사용자 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사용자 아아디
    /// </summary>
    [Column("USER_ID")]
    [StringLength(255)]
    public string? UserId { get; set; }

    /// <summary>
    /// 비밀번호
    /// </summary>
    [Column("PASSWORD")]
    [StringLength(255)]
    public string? Password { get; set; }

    /// <summary>
    /// 이름
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 이메일
    /// </summary>
    [Column("EMAIL")]
    [StringLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("PHONE")]
    [StringLength(255)]
    public string? Phone { get; set; }

    /// <summary>
    /// 기본정보 권한
    /// </summary>
    [Column("PERM_BASIC", TypeName = "int(11)")]
    public int? PermBasic { get; set; }

    /// <summary>
    /// 기계관리 권한
    /// </summary>
    [Column("PERM_MACHINE", TypeName = "int(11)")]
    public int? PermMachine { get; set; }

    /// <summary>
    /// 전기관리 권한
    /// </summary>
    [Column("PERM_ELEC", TypeName = "int(11)")]
    public int? PermElec { get; set; }

    /// <summary>
    /// 승강관리 권한
    /// </summary>
    [Column("PERM_LIFT", TypeName = "int(11)")]
    public int? PermLift { get; set; }

    /// <summary>
    /// 소방관리 권한
    /// </summary>
    [Column("PERM_FIRE", TypeName = "int(11)")]
    public int? PermFire { get; set; }

    /// <summary>
    /// 건축관리 권한
    /// </summary>
    [Column("PERM_CONSTRUCT", TypeName = "int(11)")]
    public int? PermConstruct { get; set; }

    /// <summary>
    /// 통신연동 권한
    /// </summary>
    [Column("PERM_NETWORK", TypeName = "int(11)")]
    public int? PermNetwork { get; set; }

    /// <summary>
    /// 미화 권한
    /// </summary>
    [Column("PERM_BEAUTY", TypeName = "int(11)")]
    public int? PermBeauty { get; set; }

    /// <summary>
    /// 보안 권한
    /// </summary>
    [Column("PERM_SECURITY", TypeName = "int(11)")]
    public int? PermSecurity { get; set; }

    /// <summary>
    /// 자재관리 권한
    /// </summary>
    [Column("PERM_MATERIAL", TypeName = "int(11)")]
    public int? PermMaterial { get; set; }

    /// <summary>
    /// 에너지관리 권한
    /// </summary>
    [Column("PERM_ENERGY", TypeName = "int(11)")]
    public int? PermEnergy { get; set; }

    /// <summary>
    /// 사용자 관리 권한
    /// </summary>
    [Column("PERM_USER", TypeName = "int(11)")]
    public int? PermUser { get; set; }

    /// <summary>
    /// 민원관리 권한
    /// </summary>
    [Column("PERM_VOC", TypeName = "int(11)")]
    public int? PermVoc { get; set; }

    [Column("VOC_MACHINE")]
    public bool? VocMachine { get; set; }

    [Column("VOC_ELEC")]
    public bool? VocElec { get; set; }

    [Column("VOC_LIFT")]
    public bool? VocLift { get; set; }

    [Column("VOC_FIRE")]
    public bool? VocFire { get; set; }

    [Column("VOC_CONSTRUCT")]
    public bool? VocConstruct { get; set; }

    [Column("VOC_NETWORK")]
    public bool? VocNetwork { get; set; }

    [Column("VOC_BEAUTY")]
    public bool? VocBeauty { get; set; }

    [Column("VOC_SECURITY")]
    public bool? VocSecurity { get; set; }

    [Column("VOC_ETC")]
    public bool? VocEtc { get; set; }

    /// <summary>
    /// 관리자 여부
    /// </summary>
    [Column("ADMIN_YN")]
    public bool? AdminYn { get; set; }

    /// <summary>
    /// 알람여부
    /// </summary>
    [Column("ALARM_YN")]
    public bool? AlarmYn { get; set; }

    /// <summary>
    /// 재직여부
    /// </summary>
    [Column("STATUS", TypeName = "int(11)")]
    public int? Status { get; set; }

    /// <summary>
    /// 생성일자
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
    /// 수정일자
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

    /// <summary>
    /// 직책
    /// </summary>
    [Column("JOB")]
    [StringLength(255)]
    public string? Job { get; set; }

    /// <summary>
    /// 이미지
    /// </summary>
    [Column("IMAGE")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    [InverseProperty("UserTb")]
    public virtual ICollection<AdminTb> AdminTbs { get; set; } = new List<AdminTb>();

    [InverseProperty("UsersTb")]
    public virtual ICollection<AlarmTb> AlarmTbs { get; set; } = new List<AlarmTb>();

    [InverseProperty("UserTb")]
    public virtual ICollection<CommentTb> CommentTbs { get; set; } = new List<CommentTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("UsersTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }
}
