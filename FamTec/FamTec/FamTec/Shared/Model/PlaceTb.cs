using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("place_tb")]
[Index("DepartmentTbId", Name = "FK_departmenttb_20240806")]
[Index("PlaceCd", Name = "uk_check", IsUnique = true)]
public partial class PlaceTb
{
    /// <summary>
    /// 사업장 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 사업장 코드
    /// </summary>
    [Column("PLACE_CD")]
    public string PlaceCd { get; set; } = null!;

    /// <summary>
    /// 사업장명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 계약번호
    /// </summary>
    [Column("CONTRACT_NUM")]
    [StringLength(255)]
    public string? ContractNum { get; set; }

    /// <summary>
    /// 비고
    /// </summary>
    [Column("NOTE")]
    [StringLength(255)]
    public string? Note { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("TEL")]
    [StringLength(255)]
    public string Tel { get; set; } = null!;

    /// <summary>
    /// 주소
    /// </summary>
    [Column("ADDRESS")]
    [StringLength(255)]
    public string? Address { get; set; }

    /// <summary>
    /// 해약일자
    /// </summary>
    [Column("CANCEL_DT", TypeName = "datetime")]
    public DateTime? CancelDt { get; set; }

    /// <summary>
    /// 계약일자
    /// </summary>
    [Column("CONTRACT_DT", TypeName = "datetime")]
    public DateTime? ContractDt { get; set; }

    /// <summary>
    /// 계약상태
    /// </summary>
    [Column("STATUS")]
    public bool Status { get; set; }

    /// <summary>
    /// 기게정보권한
    /// </summary>
    [Column("PERM_MACHINE")]
    public bool PermMachine { get; set; }

    /// <summary>
    /// 전기관리 권한
    /// </summary>
    [Column("PERM_ELEC")]
    public bool PermElec { get; set; }

    /// <summary>
    /// 승강관리 권한
    /// </summary>
    [Column("PERM_LIFT")]
    public bool PermLift { get; set; }

    /// <summary>
    /// 소방관리 권한
    /// </summary>
    [Column("PERM_FIRE")]
    public bool PermFire { get; set; }

    /// <summary>
    /// 건축관리 권한
    /// </summary>
    [Column("PERM_CONSTRUCT")]
    public bool PermConstruct { get; set; }

    /// <summary>
    /// 통신관리 권한
    /// </summary>
    [Column("PERM_NETWORK")]
    public bool PermNetwork { get; set; }

    /// <summary>
    /// 미화 권한
    /// </summary>
    [Column("PERM_BEAUTY")]
    public bool PermBeauty { get; set; }

    /// <summary>
    /// 보안 권한
    /// </summary>
    [Column("PERM_SECURITY")]
    public bool PermSecurity { get; set; }

    /// <summary>
    /// 자재관리 권한
    /// </summary>
    [Column("PERM_MATERIAL")]
    public bool PermMaterial { get; set; }

    /// <summary>
    /// 에너지관리 권한
    /// </summary>
    [Column("PERM_ENERGY")]
    public bool PermEnergy { get; set; }

    /// <summary>
    /// 민원관리 권한
    /// </summary>
    [Column("PERM_VOC")]
    public bool PermVoc { get; set; }

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

    /// <summary>
    /// 관리부서 인덱스
    /// </summary>
    [Column("DEPARTMENT_TB_ID", TypeName = "int(11)")]
    public int? DepartmentTbId { get; set; }

    [InverseProperty("PlaceTb")]
    public virtual ICollection<AdminPlaceTb> AdminPlaceTbs { get; set; } = new List<AdminPlaceTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<BuildingTb> BuildingTbs { get; set; } = new List<BuildingTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<ContractTypeTb> ContractTypeTbs { get; set; } = new List<ContractTypeTb>();

    [ForeignKey("DepartmentTbId")]
    [InverseProperty("PlaceTbs")]
    public virtual DepartmentsTb? DepartmentTb { get; set; }

    [InverseProperty("PlaceTb")]
    public virtual ICollection<InventoryTb> InventoryTbs { get; set; } = new List<InventoryTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<KakaoLogTb> KakaoLogTbs { get; set; } = new List<KakaoLogTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<MaterialTb> MaterialTbs { get; set; } = new List<MaterialTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<UnitTb> UnitTbs { get; set; } = new List<UnitTb>();

    [InverseProperty("PlaceTb")]
    public virtual ICollection<UsersTb> UsersTbs { get; set; } = new List<UsersTb>();
}
