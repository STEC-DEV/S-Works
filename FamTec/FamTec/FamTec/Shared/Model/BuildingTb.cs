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
    [Column("COMPLETION_DT")]
    [StringLength(255)]
    public string? CompletionDt { get; set; }

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
    /// 연면적
    /// </summary>
    [Column("GROSSFLOORAREA")]
    [StringLength(255)]
    public string? Grossfloorarea { get; set; }

    /// <summary>
    /// 대지면적
    /// </summary>
    [Column("LANDAREA")]
    [StringLength(255)]
    public string? Landarea { get; set; }

    /// <summary>
    /// 건축면적
    /// </summary>
    [Column("BUILDINGAREA")]
    [StringLength(255)]
    public string? Buildingarea { get; set; }

    /// <summary>
    /// 건물층수
    /// </summary>
    [Column("FLOORNUM")]
    [StringLength(255)]
    public string? Floornum { get; set; }

    /// <summary>
    /// 지상층수
    /// </summary>
    [Column("GROUNDFLOORNUM")]
    [StringLength(255)]
    public string? Groundfloornum { get; set; }

    /// <summary>
    /// 지하층수
    /// </summary>
    [Column("BASEMENTFLOORNUM")]
    [StringLength(255)]
    public string? Basementfloornum { get; set; }

    /// <summary>
    /// 건물높이
    /// </summary>
    [Column("BUILDINGHEIGHT")]
    [StringLength(255)]
    public string? Buildingheight { get; set; }

    /// <summary>
    /// 지상높이
    /// </summary>
    [Column("GROUNDHEIGHT")]
    [StringLength(255)]
    public string? Groundheight { get; set; }

    /// <summary>
    /// 지하깊이
    /// </summary>
    [Column("BASEMENTHEIGHT")]
    [StringLength(255)]
    public string? Basementheight { get; set; }

    /// <summary>
    /// 주차대수
    /// </summary>
    [Column("PARKINGNUM")]
    [StringLength(255)]
    public string? Parkingnum { get; set; }

    /// <summary>
    /// 옥내대수
    /// </summary>
    [Column("INNERPARKINGNUM")]
    [StringLength(255)]
    public string? Innerparkingnum { get; set; }

    /// <summary>
    /// 옥외대수
    /// </summary>
    [Column("OUTERPARKINGNUM")]
    [StringLength(255)]
    public string? Outerparkingnum { get; set; }

    /// <summary>
    /// 전기용량
    /// </summary>
    [Column("ELECCAPACITY")]
    [StringLength(255)]
    public string? Eleccapacity { get; set; }

    /// <summary>
    /// 수전용량
    /// </summary>
    [Column("FAUCETCAPACITY")]
    [StringLength(255)]
    public string? Faucetcapacity { get; set; }

    /// <summary>
    /// 발전용량
    /// </summary>
    [Column("GENERATIONCAPACITY")]
    [StringLength(255)]
    public string? Generationcapacity { get; set; }

    /// <summary>
    /// 급수용량
    /// </summary>
    [Column("WATERCAPACITY")]
    [StringLength(255)]
    public string? Watercapacity { get; set; }

    /// <summary>
    /// 고가수조
    /// </summary>
    [Column("ELEVWATERCAPACITY")]
    [StringLength(255)]
    public string? Elevwatercapacity { get; set; }

    /// <summary>
    /// 저수조
    /// </summary>
    [Column("WATERTANK")]
    [StringLength(255)]
    public string? Watertank { get; set; }

    /// <summary>
    /// 가스용량
    /// </summary>
    [Column("GASCAPACITY")]
    [StringLength(255)]
    public string? Gascapacity { get; set; }

    /// <summary>
    /// 보일러
    /// </summary>
    [Column("BOILER")]
    [StringLength(255)]
    public string? Boiler { get; set; }

    /// <summary>
    /// 냉온수기
    /// </summary>
    [Column("WATERDISPENSER")]
    [StringLength(255)]
    public string? Waterdispenser { get; set; }

    /// <summary>
    /// 승강기 대수
    /// </summary>
    [Column("LIFTNUM")]
    [StringLength(255)]
    public string? Liftnum { get; set; }

    /// <summary>
    /// 인승용 대수
    /// </summary>
    [Column("PEOPLELIFTNUM")]
    [StringLength(255)]
    public string? Peopleliftnum { get; set; }

    /// <summary>
    /// 화물용 대수
    /// </summary>
    [Column("CARGOLIFTNUM")]
    [StringLength(255)]
    public string? Cargoliftnum { get; set; }

    /// <summary>
    /// 냉난방용량
    /// </summary>
    [Column("COOLHEATCAPACITY")]
    [StringLength(255)]
    public string? Coolheatcapacity { get; set; }

    /// <summary>
    /// 난방용량
    /// </summary>
    [Column("HEATCAPACITY")]
    [StringLength(255)]
    public string? Heatcapacity { get; set; }

    /// <summary>
    /// 냉방용량
    /// </summary>
    [Column("COOLCAPACITY")]
    [StringLength(255)]
    public string? Coolcapacity { get; set; }

    /// <summary>
    /// 조경면적
    /// </summary>
    [Column("LANDSCAPEAREA")]
    [StringLength(255)]
    public string? Landscapearea { get; set; }

    /// <summary>
    /// 지상면적
    /// </summary>
    [Column("GROUNDAREA")]
    [StringLength(255)]
    public string? Groundarea { get; set; }

    /// <summary>
    /// 옥상면적
    /// </summary>
    [Column("ROOFTOPAREA")]
    [StringLength(255)]
    public string? Rooftoparea { get; set; }

    /// <summary>
    /// 화장실개수
    /// </summary>
    [Column("TOILETNUM")]
    [StringLength(255)]
    public string? Toiletnum { get; set; }

    /// <summary>
    /// 남자화장실개수
    /// </summary>
    [Column("MENTOILETNUM")]
    [StringLength(255)]
    public string? Mentoiletnum { get; set; }

    /// <summary>
    /// 여자화장실개수
    /// </summary>
    [Column("WOMENTOILETNUM")]
    [StringLength(255)]
    public string? Womentoiletnum { get; set; }

    /// <summary>
    /// 소방등급
    /// </summary>
    [Column("FIRERATING")]
    [StringLength(255)]
    public string? Firerating { get; set; }

    /// <summary>
    /// 정화조용량
    /// </summary>
    [Column("SEPTICTANKCAPACITY")]
    [StringLength(255)]
    public string? Septictankcapacity { get; set; }

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
