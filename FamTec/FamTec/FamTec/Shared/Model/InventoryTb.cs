using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("inventory_tb")]
[Index("MaterailTbId", Name = "fk_inventory_tb_material_tb1_idx")]
[Index("PlaceTbId", Name = "fk_inventory_tb_place_tb1_idx")]
[Index("RoomTbId", Name = "fk_inventory_tb_room_tb_idx")]
public partial class InventoryTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("NUM", TypeName = "int(11)")]
    public int? Num { get; set; }

    [Column("UNIT_PRICE")]
    public float? UnitPrice { get; set; }

    [Column("MATERIAL_COL_KEY", TypeName = "int(11)")]
    public int? MaterialColKey { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime? CreateDt { get; set; }

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string? CreateUser { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("ROOM_TB_ID", TypeName = "int(11)")]
    public int RoomTbId { get; set; }

    [Column("MATERAIL_TB_ID", TypeName = "int(11)")]
    public int MaterailTbId { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [ForeignKey("MaterailTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual MaterialTb MaterailTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [ForeignKey("RoomTbId")]
    [InverseProperty("InventoryTbs")]
    public virtual RoomTb RoomTb { get; set; } = null!;

    [InverseProperty("InventoryTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();
}
