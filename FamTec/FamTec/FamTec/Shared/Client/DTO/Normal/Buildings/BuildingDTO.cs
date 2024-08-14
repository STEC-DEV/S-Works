using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings
{
    public class BuildingDTO
    {
        public BuildingDTO() { }
        public int? ID { get; set; }

        /// <summary>
        /// 건물코드
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 건물명
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string? Tel { get; set; }

        /// <summary>
        /// 건물용도
        /// </summary>
        public string? Usage { get; set; }

        /// <summary>
        /// 시공업체
        /// </summary>
        public string? ConstCompany { get; set; }

        /// <summary>
        /// 준공년월
        /// </summary>
        public string? CompletionDT { get; set; }

        /// <summary>
        /// 건물구조
        /// </summary>
        public string? BuildingStruct { get; set; }

        /// <summary>
        /// 지붕구조
        /// </summary>
        public string? RoofStruct { get; set; }

        /// <summary>
        /// 연면적
        /// </summary>
        public string? GrossFloorArea { get; set; }

        /// <summary>
        /// 대지면적
        /// </summary>
        public string? LandArea { get; set; }

        /// <summary>
        /// 건축면적
        /// </summary>
        public string? BuildingArea { get; set; }

        /// <summary>
        /// 건물층수
        /// </summary>
        public string? FloorNum { get; set; }

        /// <summary>
        /// 지상층수
        /// </summary>
        public string? GroundFloorNum { get; set; }

        /// <summary>
        /// 지하층수
        /// </summary>
        public string? BasementFloorNum { get; set; }

        /// <summary>
        /// 건물높이
        /// </summary>
        public string? BuildingHeight { get; set; }

        /// <summary>
        /// 지상높이
        /// </summary>
        public string? GroundHeight { get; set; }

        /// <summary>
        /// 지하깊이
        /// </summary>
        public string? BasementHeight { get; set; }

        /// <summary>
        /// 주차대수
        /// </summary>
        public string? ParkingNum { get; set; }

        /// <summary>
        /// 옥내대수
        /// </summary>
        public string? InnerParkingNum { get; set; }

        /// <summary>
        /// 옥외대수
        /// </summary>
        public string? OuterParkingNum { get; set; }

        /// <summary>
        /// 전기용량
        /// </summary>
        public string? ElecCapacity { get; set; }

        /// <summary>
        /// 수전용량
        /// </summary>
        public string? FaucetCapacity { get; set; }

        /// <summary>
        /// 발전용량
        /// </summary>
        public string? GenerationCapacity { get; set; }

        /// <summary>
        /// 급수용량
        /// </summary>
        public string? WaterCapacity { get; set; }

        /// <summary>
        /// 고가수조
        /// </summary>
        public string? ElevWaterCapacity { get; set; }

        /// <summary>
        /// 저수조
        /// </summary>
        public string? WaterTank { get; set; }

        /// <summary>
        /// 가스용량
        /// </summary>
        public string? GasCapacity { get; set; }

        /// <summary>
        /// 보일러
        /// </summary>
        public string? Boiler { get; set; }

        /// <summary>
        /// 냉온수기
        /// </summary>
        public string? WaterDispenser { get; set; }

        /// <summary>
        /// 승강기대수
        /// </summary>
        public string? LiftNum { get; set; }

        /// <summary>
        /// 인승용
        /// </summary>
        public string? PeopleLiftNum { get; set; }

        /// <summary>
        /// 화물용
        /// </summary>
        public string? CargoLiftNum { get; set; }

        /// <summary>
        /// 냉난방용량
        /// </summary>
        public string? CoolHeatCapacity { get; set; }

        /// <summary>
        /// 난방용량
        /// </summary>
        public string? HeatCapacity { get; set; }

        /// <summary>
        /// 냉방용량
        /// </summary>
        public string? CoolCapacity { get; set; }

        /// <summary>
        /// 조경면적
        /// </summary>
        public string? LandScapeArea { get; set; }

        /// <summary>
        /// 지상면적
        /// </summary>
        public string? GroundArea { get; set; }

        /// <summary>
        /// 옥상면적
        /// </summary>
        public string? RooftopArea { get; set; }

        /// <summary>
        /// 화장실개수
        /// </summary>
        public string? ToiletNum { get; set; }

        /// <summary>
        /// 남자화장실 개소
        /// </summary>
        public string? MenToiletNum { get; set; }

        /// <summary>
        /// 여자화장실 개소
        /// </summary>
        public string? WomenToiletNum { get; set; }

        /// <summary>
        /// 소방등급
        /// </summary>
        public string? FireRating { get; set; }

        /// <summary>
        /// 정화조용량
        /// </summary>
        public string? SeptictankCapacity { get; set; }

        /// <summary>
        /// 이미지
        /// </summary>
        public byte[]? Image { get; set; }
        public string ImageName { get; set; }


        //복사 생성자 추가
        public BuildingDTO(BuildingDTO source)
        {
            if (source != null)
            {
                ID = source.ID;
                Code = source.Code;
                Name = source.Name;
                Tel = source.Tel;
                Address = source.Address;
                Usage = source.Usage;
                ConstCompany = source.ConstCompany;
                BuildingStruct = source.BuildingStruct;
                RoofStruct = source.RoofStruct;
                CompletionDT = source.CompletionDT;
                GrossFloorArea = source.GrossFloorArea;
                LandArea = source.LandArea;
                BuildingArea = source.BuildingArea;
                FloorNum = source.FloorNum;
                GroundFloorNum = source.GroundFloorNum;
                BasementFloorNum = source.BasementFloorNum;
                BuildingHeight = source.BuildingHeight;
                GroundHeight = source.GroundHeight;
                BasementHeight = source.BasementHeight;
                ParkingNum = source.ParkingNum;
                InnerParkingNum = source.InnerParkingNum;
                OuterParkingNum = source.OuterParkingNum;
                ElecCapacity = source.ElecCapacity;
                FaucetCapacity = source.FaucetCapacity;
                GenerationCapacity = source.GenerationCapacity;
                WaterCapacity = source.WaterCapacity;
                ElevWaterCapacity = source.ElevWaterCapacity;
                WaterTank = source.WaterTank;
                GasCapacity = source.GasCapacity;
                Boiler = source.Boiler;
                WaterDispenser = source.WaterDispenser;
                LiftNum = source.LiftNum;
                PeopleLiftNum = source.PeopleLiftNum;
                CargoLiftNum = source.CargoLiftNum;
                CoolHeatCapacity = source.CoolHeatCapacity;
                HeatCapacity = source.HeatCapacity;
                CoolCapacity = source.CoolCapacity;
                LandScapeArea = source.LandScapeArea;
                GroundArea = source.GroundArea;
                RooftopArea = source.RooftopArea;
                ToiletNum = source.ToiletNum;
                MenToiletNum = source.MenToiletNum;
                WomenToiletNum = source.WomenToiletNum;
                FireRating = source.FireRating;
                SeptictankCapacity = source.SeptictankCapacity;

                // 배열 타입은 복사된 새로운 배열을 만들어 복사합니다.
                Image = source.Image != null ? (byte[])source.Image.Clone() : null;
                ImageName = source.ImageName;
            }

            
        }
        public BuildingDTO DeepCopy()
        {
            return new BuildingDTO(this);
        }
    }
}
