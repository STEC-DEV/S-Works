using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 자재
/// </summary>
[Table("material_tb")]
[Index("PlaceTbId", "Code", Name = "UK_PlaceCode", IsUnique = true)]
[Index("PlaceTbId", Name = "fk_material_tb_place_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class MaterialTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 품목코드
    /// </summary>
    [Column("CODE")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 자재명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 단위
    /// </summary>
    [Column("UNIT")]
    [StringLength(255)]
    public string? Unit { get; set; }

    /// <summary>
    /// 규격
    /// </summary>
    [Column("STANDARD")]
    [StringLength(255)]
    public string? Standard { get; set; }

    /// <summary>
    /// 제조사
    /// </summary>
    [Column("MANUFACTURING_COMP")]
    [StringLength(255)]
    public string? ManufacturingComp { get; set; }

    /// <summary>
    /// 안전재고수량
    /// </summary>
    [Column("SAFE_NUM", TypeName = "int(11)")]
    public int? SafeNum { get; set; }

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
    /// 기본위치
    /// </summary>
    [Column("DEFAULT_LOCATION", TypeName = "int(11)")]
    public int DefaultLocation { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [InverseProperty("MaterialTb")]
    public virtual ICollection<InventoryTb> InventoryTbs { get; set; } = new List<InventoryTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("MaterialTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [InverseProperty("MaterialTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();
}
