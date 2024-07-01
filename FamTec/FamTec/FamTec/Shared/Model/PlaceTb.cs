using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("place_tb")]
public partial class PlaceTb
{
    /// <summary>
    /// 사업장 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사업장코드
    /// </summary>
    [Column("PLACE_CD")]
    [StringLength(255)]
    public string? PlaceCd { get; set; }

    /// <summary>
    /// 계약번호
    /// </summary>
    [Column("CONTRACT_NUM")]
    [StringLength(255)]
    public string? ContractNum { get; set; }

    /// <summary>
    /// 사업장명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("TEL")]
    [StringLength(255)]
    public string? Tel { get; set; }

    /// <summary>
    /// 비고
    /// </summary>
    [Column("NOTE")]
    [StringLength(255)]
    public string? Note { get; set; }

    /// <summary>
    /// 주소
    /// </summary>
    [Column("ADDRESS")]
    [StringLength(255)]
    public string? Address { get; set; }

    /// <summary>
    /// 계약일자
    /// </summary>
    [Column("CONTRACT_DT", TypeName = "datetime")]
    public DateTime? ContractDt { get; set; }

    /// <summary>
    /// 해약일자
    /// </summary>
    [Column("CANCEL_DT", TypeName = "datetime")]
    public DateTime? CancelDt { get; set; }

    /// <summary>
    /// 기계메뉴 권한
    /// </summary>
    [Column("PERM_MACHINE", TypeName = "tinyint(4)")]
    public sbyte? PermMachine { get; set; }

    /// <summary>
    /// 승강메뉴 권한
    /// </summary>
    [Column("PERM_LIFT", TypeName = "tinyint(4)")]
    public sbyte? PermLift { get; set; }

    /// <summary>
    /// 소방메뉴 권한
    /// </summary>
    [Column("PERM_FIRE", TypeName = "tinyint(4)")]
    public sbyte? PermFire { get; set; }

    /// <summary>
    /// 건축메뉴 권한
    /// </summary>
    [Column("PERM_CONSTRUCT", TypeName = "tinyint(4)")]
    public sbyte? PermConstruct { get; set; }

    /// <summary>
    /// 통신메뉴 권한
    /// </summary>
    [Column("PERM_NETWORK", TypeName = "tinyint(4)")]
    public sbyte? PermNetwork { get; set; }

    /// <summary>
    /// 미화메뉴 권한
    /// </summary>
    [Column("PERM_BEAUTY", TypeName = "tinyint(4)")]
    public sbyte? PermBeauty { get; set; }

    /// <summary>
    /// 보안메뉴 권한
    /// </summary>
    [Column("PERM_SECURITY", TypeName = "tinyint(4)")]
    public sbyte? PermSecurity { get; set; }

    /// <summary>
    /// 자재메뉴 권한
    /// </summary>
    [Column("PERM_MATERIAL", TypeName = "tinyint(4)")]
    public sbyte? PermMaterial { get; set; }

    /// <summary>
    /// 에너지메뉴 권한
    /// </summary>
    [Column("PERM_ENERGY", TypeName = "tinyint(4)")]
    public sbyte? PermEnergy { get; set; }

    /// <summary>
    /// 민원 권한
    /// </summary>
    [Column("PERM_VOC", TypeName = "tinyint(4)")]
    public sbyte? PermVoc { get; set; }

    /// <summary>
    /// 계약상태
    /// </summary>
    [Column("STATUS", TypeName = "tinyint(4)")]
    public sbyte? Status { get; set; }

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

    [InverseProperty("Place")]
    public virtual ICollection<AdminPlaceTb> AdminPlaceTbs { get; set; } = new List<AdminPlaceTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<BuildingTb> BuildingTbs { get; set; } = new List<BuildingTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<MaterialTb> MaterialTbs { get; set; } = new List<MaterialTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<UnitTb> UnitTbs { get; set; } = new List<UnitTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<UserTb> UserTbs { get; set; } = new List<UserTb>();
}
