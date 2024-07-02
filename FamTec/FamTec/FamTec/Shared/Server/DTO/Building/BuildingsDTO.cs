using System.ComponentModel.DataAnnotations;

namespace FamTec.Shared.Server.DTO.Building
{
    /// <summary>
    /// 건물 DTO
    /// </summary>
    public class BuildingsDTO
    {
        public int BuildingID { get; set; }

        /// <summary>
        /// 건물코드
        /// </summary>
        public string? BuildingCode { get; set; }

        /// <summary>
        /// 건물이름
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        [MaxLength(20)]
        public string? Tel { get; set; }

        /// <summary>
        /// 건물용도
        /// </summary>
        [MaxLength(20)]
        public string? Usage { get; set; }

        /// <summary>
        /// 시공업체
        /// </summary>
        [MaxLength(20)]
        public string? ConstComp { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public DateTime? CompletionDt { get; set; }

        /// <summary>
        /// 건물구조
        /// </summary>
        [MaxLength(30)]
        public string? BuildingStruct { get; set; }

        /// <summary>
        /// 지붕구조
        /// </summary>
        [MaxLength(30)]
        public string? RoofStruct { get; set; }

        /// <summary>
        /// 연면적
        /// </summary>
        public float? GrossFloorArea { get; set; }

        /// <summary>
        /// 대지면적
        /// </summary>
        public float? LandArea { get; set; }

        /// <summary>
        /// 건축면적
        /// </summary>
        public float? BuildingArea { get; set; }

        /// <summary>
        /// 건물층수
        /// </summary>
        public int? FloorNum { get; set; }
        /// <summary>
        /// 지상 층수
        /// </summary>
        public int? GroundFloorNum { get; set; }

        /// <summary>
        /// 지하 층수
        /// </summary>
        public int? BasementFloorNum { get; set; }

        /// <summary>
        /// 건물 높이
        /// </summary>
        public float? BuildingHeight { get; set; }

        /// <summary>
        /// 건물 지상높이
        /// </summary>
        public float? GroundHeight { get; set; }

        /// <summary>
        /// 건물 지하깊이
        /// </summary>
        public float? BasementHeight { get; set; }

        /// <summary>
        /// 주차대수
        /// </summary>
        public int? PackingNum { get; set; }

        /// <summary>
        /// 옥내 대수
        /// </summary>
        public int? InnerPackingNum { get; set; }

        /// <summary>
        /// 옥외 대수
        /// </summary>
        public int? OuterPackingNum { get; set; }

        /// <summary>
        /// 전기용량
        /// </summary>
        public float? ElecCapacity { get; set; }

        /// <summary>
        /// 수전용량
        /// </summary>
        public float? FaucetCapacity { get; set; }

        /// <summary>
        /// 발전용량
        /// </summary>
        public float? GenerationCapacity { get; set; }

        /// <summary>
        /// 급수용량
        /// </summary>
        public float? WaterCapacity { get; set; }

        /// <summary>
        /// 고가 수조
        /// </summary>
        public float? ElevWaterCapacity { get; set; }

        /// <summary>
        /// 저수조
        /// </summary>
        public float? WaterTank { get; set; }

        /// <summary>
        /// 가스용량
        /// </summary>
        public float? GasCapacity { get; set; }

        /// <summary>
        /// 보일러
        /// </summary>
        public float? Boiler { get; set; }

        /// <summary>
        /// 냉온수기
        /// </summary>
        public float? WaterDispenser { get; set; }

        /// <summary>
        /// 승강대수
        /// </summary>
        public int? LiftNum { get; set; }

        /// <summary>
        /// 인승용
        /// </summary>
        public int? PeopleLiftNum { get; set; }

        /// <summary>
        /// 화물용
        /// </summary>
        public int? CargoLiftNum { get; set; }

        /// <summary>
        /// 냉 난방 용량
        /// </summary>
        public float? CoolHeatCapacity { get; set; }

        /// <summary>
        /// 난방용량
        /// </summary>
        public float? HeatCapacity { get; set; }

        /// <summary>
        /// 냉방용량
        /// </summary>
        public float? CoolCapacity { get; set; }

        /// <summary>
        /// 조경면적
        /// </summary>
        public float? LandScapeArea { get; set; }

        /// <summary>
        /// 지상면적
        /// </summary>
        public float? GroundArea { get; set; }

        /// <summary>
        /// 옥상면적
        /// </summary>
        public float? RooftopArea { get; set; }

        /// <summary>
        /// 화장실 개수
        /// </summary>
        public int? ToiletNum { get; set; }

        /// <summary>
        /// 남자화장실 개수
        /// </summary>
        public int? MenToiletNum { get; set; }

        /// <summary>
        /// 여자화장실 개수
        /// </summary>
        public int? WomenToiletNum { get; set; }

        /// <summary>
        /// 소방등급
        /// </summary>
        public string? FireRating { get; set; }

        /// <summary>
        /// 정화조 용량
        /// </summary>
        public float? SepticTankCapacity { get; set; }

        /// <summary>
        /// 생성일자
        /// </summary>
        public DateTime? CreateDT { get; set; }

       
        /// <summary>
        /// 추가항목
        /// </summary>
        public List<BuildingSubItemDTO>? subitem { get; set; } = new List<BuildingSubItemDTO>();

    }
}
