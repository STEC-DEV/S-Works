using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("material_tb")]
[Index("BuildingTbId", Name = "BuildingTbId_202406211648")]
[Index("PlaceTbId", Name = "PlaceTbId_202406211648")]
public partial class MaterialTb
{
    /// <summary>
    /// 자재 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 자재명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 단위
    /// </summary>
    [Column("UNIT")]
    [StringLength(255)]
    public string? Unit { get; set; }

    /// <summary>
    /// 자재위치
    /// </summary>
    [Column("DEFAULT_LOCATION")]
    [StringLength(255)]
    public string? DefaultLocation { get; set; }

    /// <summary>
    /// 규격
    /// </summary>
    [Column("STANDARD")]
    [StringLength(255)]
    public string? Standard { get; set; }

    /// <summary>
    /// 제조사
    /// </summary>
    [Column("MFR")]
    [StringLength(255)]
    public string? Mfr { get; set; }

    /// <summary>
    /// 안전재고수량
    /// </summary>
    [Column("SAFE_NUM", TypeName = "int(11)")]
    public int? SafeNum { get; set; }

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
    /// 삭제여부
    /// </summary>
    [Column("DEL_USER")]
    [StringLength(255)]
    public string? DelUser { get; set; }

    /// <summary>
    /// (외래키) 사업장 아이디
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    /// <summary>
    /// (외래키) 건물 아이디
    /// </summary>
    [Column("BUILDING_TB_ID", TypeName = "int(11)")]
    public int? BuildingTbId { get; set; }

    [ForeignKey("BuildingTbId")]
    [InverseProperty("MaterialTbs")]
    public virtual BuildingTb? BuildingTb { get; set; }

    [ForeignKey("PlaceTbId")]
    [InverseProperty("MaterialTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }

    [InverseProperty("MaterialTb")]
    public virtual ICollection<StoreTb> StoreTbs { get; set; } = new List<StoreTb>();
}
