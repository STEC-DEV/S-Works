using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

[Table("building_tb")]
[Index("PlaceTbId", Name = "fk_BUILDING_TB_PLACE_TB1_idx")]
public partial class BuildingTb
{
    /// <summary>
    /// 건물 테이블 인덱스
    /// </summary>
    [Key]
    [Column("ID", TypeName = "int(11)")]
    public int Id { get; set; }

    /// <summary>
    /// 건물코드
    /// </summary>
    [Column("BUILDING_CD")]
    [StringLength(255)]
    public string? BuildingCd { get; set; }

    /// <summary>
    /// 건물명
    /// </summary>
    [Column("NAME")]
    [StringLength(255)]
    public string? Name { get; set; }

    /// <summary>
    /// 주소
    /// </summary>
    [Column("ADDRESS")]
    [StringLength(255)]
    public string? Address { get; set; }

    /// <summary>
    /// 전화번호
    /// </summary>
    [Column("TEL")]
    [StringLength(255)]
    public string? Tel { get; set; }

    /// <summary>
    /// 건물용도
    /// </summary>
    [Column("USAGE")]
    [StringLength(255)]
    public string? Usage { get; set; }

    /// <summary>
    /// 시공업체
    /// </summary>
    [Column("CONST_COMP")]
    [StringLength(255)]
    public string? ConstComp { get; set; }

    /// <summary>
    /// 준공년월
    /// </summary>
    [Column("COMPLETION_DT", TypeName = "datetime")]
    public DateTime? CompletionDt { get; set; }

    /// <summary>
    /// 건물구조
    /// </summary>
    [Column("BUILDING_STRUCT")]
    [StringLength(255)]
    public string? BuildingStruct { get; set; }

    /// <summary>
    /// 지붕구조
    /// </summary>
    [Column("ROOF_STRUCT")]
    [StringLength(255)]
    public string? RoofStruct { get; set; }

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
    /// 첨부파일
    /// </summary>
    [StringLength(255)]
    public string? Image { get; set; }

    /// <summary>
    /// (외래키) 사업장 인덱스
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    [InverseProperty("BuildingTb")]
    public virtual ICollection<FloorTb> FloorTbs { get; set; } = new List<FloorTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<MaterialTb> MaterialTbs { get; set; } = new List<MaterialTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<MeterReaderTb> MeterReaderTbs { get; set; } = new List<MeterReaderTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("BuildingTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }

    [InverseProperty("BuildingTb")]
    public virtual ICollection<VocTb> VocTbs { get; set; } = new List<VocTb>();
}
