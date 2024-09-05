using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("inventory_tb")]
[Index("MaterialTbId", Name = "fk_invenory_tb_material_tb1_idx")]
[Index("PlaceTbId", Name = "fk_invenory_tb_place_tb1_idx")]
[Index("RoomTbId", Name = "fk_invenory_tb_room_tb1_idx")]
public partial class InventoryTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 동시성 제어
    /// </summary>
    [ConcurrencyCheck]
    [Column("NUM", TypeName = "int(11)")]
    public int Num { get; set; }

    [Column("UNIT_PRICE")]
    public float UnitPrice { get; set; }

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

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [Column("ROOM_TB_ID", TypeName = "int(11)")]
    public int RoomTbId { get; set; }

    [Column("MATERIAL_TB_ID", TypeName = "int(11)")]
    public int MaterialTbId { get; set; }
    
    /// <summary>
    /// 동시성 제어
    /// </summary>
    [ConcurrencyCheck]
    [Column("ROW_VERSION", TypeName = "bigint(20)")]
    public long RowVersion { get; set; }

    [ForeignKey("MaterialTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual MaterialTb MaterialTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [ForeignKey("RoomTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual RoomTb RoomTb { get; set; } = null!;
}
