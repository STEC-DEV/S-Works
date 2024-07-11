using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Buildings
{
    public class BuildingDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tel {  get; set; }
        public string Address { get; set; }
        public string Usage { get; set; }
        public string ConstCompany { get; set; }
        public string BuildingStructure { get; set; }
        public string RoofStructure {  get; set; }
        public string CompleteDT {  get; set; }

        public string GrossFloorArea { get; set; }
        public string LandArea {  get; set; }
        public string BuildingArea {  get; set; }
        public string FloorNum { get; set; }
        public string GroundFloorNum { get; set; }
        public string BasementFloorNum { get; set; }
        public string BuildingHeight { get; set; }
        public string GroundHeight { get; set; }
        public string BasementHeight { get; set; }
        public string ParkingNum { get; set; }
        public string InnerParkingNum { get; set; }
        public string OutterParkingNum { get; set; }
        public string ElecCapacity { get; set; }
        public string FaucetCapacity { get; set; }
        public string GenerationCapacity { get; set; }
        public string WaterCapacity { get; set; }
        public string ElevWaterCapacity { get; set; }
        public string WaterTank {  get; set; }
        public string GasCapacity { get; set; }
        public string Boiler {  get; set; }
        public string Water_Dispenser { get; set; }
        public string LiftNum { get; set; }
        public string PeopleLiftNum { get; set; }
        public string CargoLiftNum { get; set; }
        public string CoolHeatCapacity { get; set; }
        public string HeatCapacity { get; set; }
        public string CoolCapacity { get; set; }
        public string LandScapeArea { get; set; }
        public string GroundArea { get; set; }
        public string RoofTopArea { get; set; }
        public string ToiletNum { get; set; }
        public string MenToiletNum { get; set; }
        public string WomenToiletNum { get; set; }
        public string FireRating { get; set; }
        public string SepticTankCapacity { get; set; }
    }
}
