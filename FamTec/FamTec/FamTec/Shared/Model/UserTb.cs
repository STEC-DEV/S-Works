using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("user_tb")]
[Index("PlaceTbId", Name = "fk_USER_TB_PLACE_TB1_idx")]
public partial class UserTb
{
    /// <summary>
    /// 사용자 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사용자 아이디
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
    /// 기본정보관리 권한
    /// </summary>
    [Column("PERM_BASIC", TypeName = "int(11)")]
    public int? PermBasic { get; set; }

    /// <summary>
    /// 기계 권한
    /// </summary>
    [Column("PERM_MACHINE", TypeName = "int(11)")]
    public int? PermMachine { get; set; }

    /// <summary>
    /// 전기 권한
    /// </summary>
    [Column("PERM_ELEC", TypeName = "int(11)")]
    public int? PermElec { get; set; }

    /// <summary>
    /// 승강 권한
    /// </summary>
    [Column("PERM_LIFT", TypeName = "int(11)")]
    public int? PermLift { get; set; }

    /// <summary>
    /// 소방 권한
    /// </summary>
    [Column("PERM_FIRE", TypeName = "int(11)")]
    public int? PermFire { get; set; }

    /// <summary>
    /// 건축 권한
    /// </summary>
    [Column("PERM_CONSTRUCT", TypeName = "int(11)")]
    public int? PermConstruct { get; set; }

    /// <summary>
    /// 통신 권한
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
    /// 사용자관리 권한
    /// </summary>
    [Column("PERM_USER", TypeName = "int(11)")]
    public int? PermUser { get; set; }

    /// <summary>
    /// 민원관리 권한
    /// </summary>
    [Column("PERM_VOC", TypeName = "int(11)")]
    public int? PermVoc { get; set; }

    /// <summary>
    /// 기계민원 처리권한
    /// </summary>
    [Column("VOC_MACHINE", TypeName = "int(11)")]
    public int? VocMachine { get; set; }

    /// <summary>
    /// 전기민원 처리권한
    /// </summary>
    [Column("VOC_ELEC", TypeName = "int(11)")]
    public int? VocElec { get; set; }

    /// <summary>
    /// 승강민원 처리권한
    /// </summary>
    [Column("VOC_LIFT", TypeName = "int(11)")]
    public int? VocLift { get; set; }

    /// <summary>
    /// 소방민원 처리권한
    /// </summary>
    [Column("VOC_FIRE", TypeName = "int(11)")]
    public int? VocFire { get; set; }

    /// <summary>
    /// 건축민원 처리권한
    /// </summary>
    [Column("VOC_CONSTRUCT", TypeName = "int(11)")]
    public int? VocConstruct { get; set; }

    /// <summary>
    /// 통신민원 처리권한
    /// </summary>
    [Column("VOC_NETWORK", TypeName = "int(11)")]
    public int? VocNetwork { get; set; }

    /// <summary>
    /// 미화민원 처리권한
    /// </summary>
    [Column("VOC_BEAUTY", TypeName = "int(11)")]
    public int? VocBeauty { get; set; }

    /// <summary>
    /// 보안민원 처리권한
    /// </summary>
    [Column("VOC_SECURITY", TypeName = "int(11)")]
    public int? VocSecurity { get; set; }

    /// <summary>
    /// 기타 처리권한
    /// </summary>
    [Column("VOC_DEFAULT", TypeName = "int(11)")]
    public int? VocDefault { get; set; }

    /// <summary>
    /// 관리자 유무
    /// </summary>
    [Column("ADMIN_YN")]
    public bool? AdminYn { get; set; }

    /// <summary>
    /// 알람 유무
    /// </summary>
    [Column("ALRAM_YN")]
    public bool? AlramYn { get; set; }

    /// <summary>
    /// 재직여부
    /// </summary>
    [Column("STATUS")]
    public bool? Status { get; set; }

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
    /// 직급
    /// </summary>
    [Column("JOB")]
    [StringLength(255)]
    public string? Job { get; set; }

    /// <summary>
    /// 첨부파일
    /// </summary>
    [StringLength(255)]
    public string? Image { get; set; }

    /// <summary>
    /// (외래키) 사업장 인덱스
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    [InverseProperty("UserTb")]
    public virtual ICollection<AdminTb> AdminTbs { get; set; } = new List<AdminTb>();

    [InverseProperty("UserTb")]
    public virtual ICollection<AlarmTb> AlarmTbs { get; set; } = new List<AlarmTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("UserTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }
}
