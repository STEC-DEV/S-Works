using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Shared.Model;

/// <summary>
/// 건물
/// </summary>
[Table("building_tb")]
[Index("PlaceTbId", Name = "fk_building_tb_place_tb1_idx")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class BuildingTb
{
    /// <summary>
    /// 건물 인덱스
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
    /// 이미지
    /// </summary>
    [Column("IMAGE")]
    [StringLength(255)]
    public string? Image { get; set; }

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
    [Column("GROSS_FLOOR_AREA")]
    [StringLength(255)]
    public string? GrossFloorArea { get; set; }

    /// <summary>
    /// 대지면적
    /// </summary>
    [Column("LAND_AREA")]
    [StringLength(255)]
    public string? LandArea { get; set; }

    /// <summary>
    /// 건물면적
    /// </summary>
    [Column("BUILDING_AREA")]
    [StringLength(255)]
    public string? BuildingArea { get; set; }

    /// <summary>
    /// 건물층수
    /// </summary>
    [Column("FLOOR_NUM")]
    [StringLength(255)]
    public string? FloorNum { get; set; }

    /// <summary>
    /// 지상
    /// </summary>
    [Column("GROUND_FLOOR_NUM")]
    [StringLength(255)]
    public string? GroundFloorNum { get; set; }

    /// <summary>
    /// 지하
    /// </summary>
    [Column("BASEMENT_FLOOR_NUM")]
    [StringLength(255)]
    public string? BasementFloorNum { get; set; }

    /// <summary>
    /// 건물높이
    /// </summary>
    [Column("BUILDING_HEIGHT")]
    [StringLength(255)]
    public string? BuildingHeight { get; set; }

    /// <summary>
    /// 지상
    /// </summary>
    [Column("GROUND_HEIGHT")]
    [StringLength(255)]
    public string? GroundHeight { get; set; }

    /// <summary>
    /// 지하
    /// </summary>
    [Column("BASEMENT_HEIGHT")]
    [StringLength(255)]
    public string? BasementHeight { get; set; }

    /// <summary>
    /// 주차대수
    /// </summary>
    [Column("PARKING_NUM")]
    [StringLength(255)]
    public string? ParkingNum { get; set; }

    /// <summary>
    /// 옥내
    /// </summary>
    [Column("INNER_PARKING_NUM")]
    [StringLength(255)]
    public string? InnerParkingNum { get; set; }

    /// <summary>
    /// 옥외
    /// </summary>
    [Column("OUTER_PARKING_NUM")]
    [StringLength(255)]
    public string? OuterParkingNum { get; set; }

    /// <summary>
    /// 전기용량
    /// </summary>
    [Column("ELEC_CAPACITY")]
    [StringLength(255)]
    public string? ElecCapacity { get; set; }

    /// <summary>
    /// 수전용량
    /// </summary>
    [Column("FAUCET_CAPACITY")]
    [StringLength(255)]
    public string? FaucetCapacity { get; set; }

    /// <summary>
    /// 발전용량
    /// </summary>
    [Column("GENERATION_CAPACITY")]
    [StringLength(255)]
    public string? GenerationCapacity { get; set; }

    /// <summary>
    /// 급수용량
    /// </summary>
    [Column("WATER_CAPACITY")]
    [StringLength(255)]
    public string? WaterCapacity { get; set; }

    /// <summary>
    /// 고가수조
    /// </summary>
    [Column("ELEV_WATER_CAPACITY")]
    [StringLength(255)]
    public string? ElevWaterCapacity { get; set; }

    /// <summary>
    /// 저수조
    /// </summary>
    [Column("WATER_TANK")]
    [StringLength(255)]
    public string? WaterTank { get; set; }

    /// <summary>
    /// 가스용량
    /// </summary>
    [Column("GAS_CAPACITY")]
    [StringLength(255)]
    public string? GasCapacity { get; set; }

    /// <summary>
    /// 보일러
    /// </summary>
    [Column("BOILER")]
    [StringLength(255)]
    public string? Boiler { get; set; }

    /// <summary>
    /// 냉온수기
    /// </summary>
    [Column("WATER_DISPENSER")]
    [StringLength(255)]
    public string? WaterDispenser { get; set; }

    /// <summary>
    /// 승강기대수
    /// </summary>
    [Column("LIFT_NUM")]
    [StringLength(255)]
    public string? LiftNum { get; set; }

    /// <summary>
    /// 인승용
    /// </summary>
    [Column("PEOPLE_LIFT_NUM")]
    [StringLength(255)]
    public string? PeopleLiftNum { get; set; }

    /// <summary>
    /// 화물용
    /// </summary>
    [Column("CARGO_LIFT_NUM")]
    [StringLength(255)]
    public string? CargoLiftNum { get; set; }

    /// <summary>
    /// 냉난방용량
    /// </summary>
    [Column("COOL_HEAT_CAPACITY")]
    [StringLength(255)]
    public string? CoolHeatCapacity { get; set; }

    /// <summary>
    /// 난방용량
    /// </summary>
    [Column("HEAT_CAPACITY")]
    [StringLength(255)]
    public string? HeatCapacity { get; set; }

    /// <summary>
    /// 냉방용량
    /// </summary>
    [Column("COOL_CAPACITY")]
    [StringLength(255)]
    public string? CoolCapacity { get; set; }

    /// <summary>
    /// 조경면적
    /// </summary>
    [Column("LANDSCAPE_AREA")]
    [StringLength(255)]
    public string? LandscapeArea { get; set; }

    /// <summary>
    /// 지상면적
    /// </summary>
    [Column("GROUND_AREA")]
    [StringLength(255)]
    public string? GroundArea { get; set; }

    /// <summary>
    /// 옥상
    /// </summary>
    [Column("ROOFTOP_AREA")]
    [StringLength(255)]
    public string? RooftopArea { get; set; }

    /// <summary>
    /// 화잘실수
    /// </summary>
    [Column("TOILET_NUM")]
    [StringLength(255)]
    public string? ToiletNum { get; set; }

    /// <summary>
    /// 남자
    /// </summary>
    [Column("MEN_TOILET_NUM")]
    [StringLength(255)]
    public string? MenToiletNum { get; set; }

    /// <summary>
    /// 여자
    /// </summary>
    [Column("WOMEN_TOILET_NUM")]
    [StringLength(255)]
    public string? WomenToiletNum { get; set; }

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
    [StringLength(255)]
    public string? SepticTankCapacity { get; set; }

    /// <summary>
    /// 생성일자
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
    /// 수정일자
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
    /// 삭제일자
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
    /// 사업장 인덱스
    /// </summary>
    [Column("PLACE_TB_ID", TypeName = "int(11)")]
    public int? PlaceTbId { get; set; }

    [InverseProperty("BuildingTb")]
    public virtual ICollection<BuildingItemGroupTb> BuildingItemGroupTbs { get; set; } = new List<BuildingItemGroupTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<FloorTb> FloorTbs { get; set; } = new List<FloorTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<MaterialTb> MaterialTbs { get; set; } = new List<MaterialTb>();

    [InverseProperty("BuildingTb")]
    public virtual ICollection<MeterItemTb> MeterItemTbs { get; set; } = new List<MeterItemTb>();

    [ForeignKey("PlaceTbId")]
    [InverseProperty("BuildingTbs")]
    public virtual PlaceTb? PlaceTb { get; set; }
}
