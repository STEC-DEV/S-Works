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
    /// 연면적
    /// </summary>
    [Column("GROSS_FLOOR_AREA")]
    public float? GrossFloorArea { get; set; }

    /// <summary>
    /// 대지면적
    /// </summary>
    [Column("LAND_AREA")]
    public float? LandArea { get; set; }

    /// <summary>
    /// 건물면적
    /// </summary>
    [Column("BUILDING_AREA")]
    public float? BuildingArea { get; set; }

    /// <summary>
    /// 건물층수
    /// </summary>
    [Column("FLOOR_NUM", TypeName = "int(11)")]
    public int? FloorNum { get; set; }

    /// <summary>
    /// 지상 층수
    /// </summary>
    [Column("GROUND_FLOOR_NUM", TypeName = "int(11)")]
    public int? GroundFloorNum { get; set; }

    /// <summary>
    /// 지하 층수
    /// </summary>
    [Column("BASEMENT_FLOOR_NUM", TypeName = "int(11)")]
    public int? BasementFloorNum { get; set; }

    /// <summary>
    /// 건물 높이
    /// </summary>
    [Column("BUILDING_HEIGHT")]
    public float? BuildingHeight { get; set; }

    /// <summary>
    /// 지상 높이
    /// </summary>
    [Column("GROUND_HEIGHT")]
    public float? GroundHeight { get; set; }

    /// <summary>
    /// 지하 깊이
    /// </summary>
    [Column("BASEMENT_HEIGHT")]
    public float? BasementHeight { get; set; }

    /// <summary>
    /// 주차대수
    /// </summary>
    [Column("PARKING_NUM", TypeName = "int(11)")]
    public int? ParkingNum { get; set; }

    /// <summary>
    /// 옥내대수
    /// </summary>
    [Column("INNER_PARKING_NUM", TypeName = "int(11)")]
    public int? InnerParkingNum { get; set; }

    /// <summary>
    /// 옥외대수
    /// </summary>
    [Column("OUTER_PARKING_NUM", TypeName = "int(11)")]
    public int? OuterParkingNum { get; set; }

    /// <summary>
    /// 전기용량
    /// </summary>
    [Column("ELEC_CAPACITY")]
    public float? ElecCapacity { get; set; }

    /// <summary>
    /// 수전용량
    /// </summary>
    [Column("FAUCET_CAPACITY")]
    public float? FaucetCapacity { get; set; }

    /// <summary>
    /// 발전용량
    /// </summary>
    [Column("GENERATION_CAPACITY")]
    public float? GenerationCapacity { get; set; }

    /// <summary>
    /// 급수용량
    /// </summary>
    [Column("WATER_CAPACITY")]
    public float? WaterCapacity { get; set; }

    /// <summary>
    /// 고가수조
    /// </summary>
    [Column("ELEV_WATER_CAPACITY")]
    public float? ElevWaterCapacity { get; set; }

    /// <summary>
    /// 저수조
    /// </summary>
    [Column("WATER_TANK")]
    public float? WaterTank { get; set; }

    /// <summary>
    /// 가스용량
    /// </summary>
    [Column("GAS_CAPACITY")]
    public float? GasCapacity { get; set; }

    /// <summary>
    /// 보일러
    /// </summary>
    [Column("BOILER")]
    public float? Boiler { get; set; }

    /// <summary>
    /// 냉온수기
    /// </summary>
    [Column("WATER_DISPENSER")]
    public float? WaterDispenser { get; set; }

    /// <summary>
    /// 승강기대수
    /// </summary>
    [Column("LIFT_NUM", TypeName = "int(11)")]
    public int? LiftNum { get; set; }

    /// <summary>
    /// 인승용
    /// </summary>
    [Column("PEOPLE_LIFT_NUM", TypeName = "int(11)")]
    public int? PeopleLiftNum { get; set; }

    /// <summary>
    /// 화물용
    /// </summary>
    [Column("CARGO_LIFT_NUM", TypeName = "int(11)")]
    public int? CargoLiftNum { get; set; }

    /// <summary>
    /// 냉난방용량
    /// </summary>
    [Column("COOL_HEAT_CAPACITY")]
    public float? CoolHeatCapacity { get; set; }

    /// <summary>
    /// 난방용량
    /// </summary>
    [Column("HEAT_CAPACITY")]
    public float? HeatCapacity { get; set; }

    /// <summary>
    /// 냉방용량
    /// </summary>
    [Column("COOL_CAPACITY")]
    public float? CoolCapacity { get; set; }

    /// <summary>
    /// 조경면적
    /// </summary>
    [Column("LANDSCAPE_AREA")]
    public float? LandscapeArea { get; set; }

    /// <summary>
    /// 지상면적
    /// </summary>
    [Column("GROUND_AREA")]
    public float? GroundArea { get; set; }

    /// <summary>
    /// 옥상면적
    /// </summary>
    [Column("ROOFTOP_AREA")]
    public float? RooftopArea { get; set; }

    /// <summary>
    /// 화장실개수
    /// </summary>
    [Column("TOILET_NUM", TypeName = "int(11)")]
    public int? ToiletNum { get; set; }

    /// <summary>
    /// 남자화장실개수
    /// </summary>
    [Column("MEN_TOILET_NUM", TypeName = "int(11)")]
    public int? MenToiletNum { get; set; }

    /// <summary>
    /// 여자화장실개수
    /// </summary>
    [Column("WOMEN_TOILET_NUM", TypeName = "int(11)")]
    public int? WomenToiletNum { get; set; }

    /// <summary>
    /// 소방등급
    /// </summary>
    [Column("FIRE_RATING")]
    [StringLength(255)]
    public string? FireRating { get; set; }

    /// <summary>
    /// 정화조용량
    /// </summary>
    [Column("SEPTIC_TANK_CAPACITY")]
    public float? SepticTankCapacity { get; set; }

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
    [Column("DEL_YN", TypeName = "tinyint(4)")]
    public sbyte? DelYn { get; set; }

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
    /// (외래키) 사업장 인덱스
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    [InverseProperty("BuildingTb")]
    public virtual ICollection<FloorTb> FloorTbs { get; set; } = new List<FloorTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<MeterReaderTb> MeterReaderTbs { get; set; } = new List<MeterReaderTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("BuildingTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }

    [InverseProperty("BuildingTb")]
    public virtual ICollection<VocTb> VocTbs { get; set; } = new List<VocTb>();
}
