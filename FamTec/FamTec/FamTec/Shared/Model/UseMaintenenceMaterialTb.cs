using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("use_maintenence_material_tb")]
[Index("MaintenanceTbId", Name = "FK_MaintenanceTB_20240906_1151")]
[Index("MaterialTbId", Name = "FK_MaterialTB_20240906_1150")]
[Index("PlaceTbId", Name = "FK_PlaceTB_20240906_1237")]
[Index("RoomTbId", Name = "FK_RoomTB_20240906_1150")]
[Index("StoreTbId", Name = "FK_StoreTB_20240906_1151")]
public partial class UseMaintenenceMaterialTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("INOUT", TypeName = "int(11)")]
    public int Inout { get; set; }

    [Column("UNITPRICE")]
    public float Unitprice { get; set; }

    [Column("NUM", TypeName = "int(11)")]
    public int Num { get; set; }

    [Column("TOTALPRICE")]
    public float Totalprice { get; set; }

    [Column("MATERIAL_TB_ID", TypeName = "int(11)")]
    public int MaterialTbId { get; set; }

    [Column("ROOM_TB_ID", TypeName = "int(11)")]
    public int RoomTbId { get; set; }

    [Column("STORE_TB_ID", TypeName = "int(11)")]
    public int StoreTbId { get; set; }

    [Column("MAINTENANCE_TB_ID", TypeName = "int(11)")]
    public int MaintenanceTbId { get; set; }

    [Column("NOTE")]
    [StringLength(255)]
    public string? Note { get; set; }

    [Column("NOTE2")]
    [StringLength(255)]
    public string? Note2 { get; set; }

    [Column("CREATE_DT", TypeName = "datetime")]
    public DateTime CreateDt { get; set; }

    [Column("CREATE_USER")]
    [StringLength(255)]
    public string CreateUser { get; set; } = null!;

    [Column("UPDATE_DT", TypeName = "datetime")]
    public DateTime? UpdateDt { get; set; }

    [Column("UPDATE_USER")]
    [StringLength(255)]
    public string? UpdateUser { get; set; }

    [Column("DEL_YN")]
    public bool? DelYn { get; set; }

    [Column("DEL_DT", TypeName = "datetime")]
    public DateTime? DelDt { get; set; }

    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    [ForeignKey("MaintenanceTbId")]
    [InverseProperty("UseMaintenenceMaterialTbs")]
    public virtual MaintenenceHistoryTb MaintenanceTb { get; set; } = null!;

    [ForeignKey("MaterialTbId")]
    [InverseProperty("UseMaintenenceMaterialTbs")]
    public virtual MaterialTb MaterialTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("UseMaintenenceMaterialTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [ForeignKey("RoomTbId")]
    [InverseProperty("UseMaintenenceMaterialTbs")]
    public virtual RoomTb RoomTb { get; set; } = null!;

    [ForeignKey("StoreTbId")]
    [InverseProperty("UseMaintenenceMaterialTbs")]
    public virtual StoreTb StoreTb { get; set; } = null!;
}
