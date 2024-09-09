using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("store_tb")]
[Index("PlaceTbId", Name = "FK_PLACE_202407231358")]
[Index("RoomTbId", Name = "FK_ROOM_202407231358")]
[Index("MaintenenceMaterialTbId", Name = "fk_maintenence_material_tb")]
[Index("MaintenenceHistoryTbId", Name = "fk_store_tb_maintenence_history_tb1_idx")]
[Index("MaterialTbId", Name = "fk_store_tb_material_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class StoreTb
{
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 입출고 구분
    /// </summary>
    [Column("INOUT", TypeName = "int(11)")]
    public int Inout { get; set; }

    /// <summary>
    /// 수량
    /// </summary>
    [Column("NUM", TypeName = "int(11)")]
    public int Num { get; set; }

    /// <summary>
    /// 단가
    /// </summary>
    [Column("UNIT_PRICE")]
    public float UnitPrice { get; set; }

    /// <summary>
    /// 입출고 가격
    /// </summary>
    [Column("TOTAL_PRICE")]
    public float TotalPrice { get; set; }

    /// <summary>
    /// 입출고 날짜
    /// </summary>
    [Column("INOUT_DATE", TypeName = "datetime")]
    public DateTime InoutDate { get; set; }

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
    /// 현재재고수량
    /// </summary>
    [Column("CURRENT_NUM", TypeName = "int(11)")]
    public int CurrentNum { get; set; }

    /// <summary>
    /// 비고
    /// </summary>
    [Column("NOTE")]
    [StringLength(255)]
    public string? Note { get; set; }

    /// <summary>
    /// 유지보수취소_시스템설명
    /// </summary>
    [Column("NOTE2")]
    [StringLength(255)]
    public string? Note2 { get; set; }

    /// <summary>
    /// 공간ID
    /// </summary>
    [Column("ROOM_TB_ID", TypeName = "int(11)")]
    public int RoomTbId { get; set; }

    /// <summary>
    /// 사업장ID
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int PlaceTbId { get; set; }

    /// <summary>
    /// 품목ID
    /// </summary>
    [Column("MATERIAL_TB_ID", TypeName = "int(11)")]
    public int MaterialTbId { get; set; }

    /// <summary>
    /// 유지보수이력ID
    /// </summary>
    [Column("MAINTENENCE_HISTORY_TB_ID", TypeName = "int(11)")]
    public int? MaintenenceHistoryTbId { get; set; }

    [Column("MAINTENENCE_MATERIAL_TB_ID", TypeName = "int(11)")]
    public int? MaintenenceMaterialTbId { get; set; }

    [ForeignKey("MaintenenceHistoryTbId")]
    [InverseProperty("StoreTbs")]
    public virtual MaintenenceHistoryTb? MaintenenceHistoryTb { get; set; }

    [ForeignKey("MaintenenceMaterialTbId")]
    [InverseProperty("StoreTbs")]
    public virtual UseMaintenenceMaterialTb? MaintenenceMaterialTb { get; set; }

    [ForeignKey("MaterialTbId")]
    [InverseProperty("StoreTbs")]
    public virtual MaterialTb MaterialTb { get; set; } = null!;

    [ForeignKey("PlaceTbId")]
    [InverseProperty("StoreTbs")]
    public virtual PlaceTb PlaceTb { get; set; } = null!;

    [ForeignKey("RoomTbId")]
    [InverseProperty("StoreTbs")]
    public virtual RoomTb RoomTb { get; set; } = null!;
}
