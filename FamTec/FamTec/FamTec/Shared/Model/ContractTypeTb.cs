using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("contract_type_tb")]
[Index("PlaceTbId", Name = "FK_PLACE_TB_ID_202409021051")]
[Index("PlaceTbId", "Name", Name = "UK_NAME", IsUnique = true)]
public partial class ContractTypeTb
{
    /// <summary>
    /// 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 계약종류
    /// </summary>
    [Column("NAME")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 생성일
    /// </summary>
    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// 사업장 외래키
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [InverseProperty("ContractTb")]
    public virtual ICollection<MeterItemTb> MeterItemTbs { get; set; } = new List<MeterItemTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("ContractTypeTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;
}
