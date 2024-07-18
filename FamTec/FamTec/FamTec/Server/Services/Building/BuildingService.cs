using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Server.Repository.Floor;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;

namespace FamTec.Server.Services.Building
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IBuildingGroupItemInfoRepository BuildingGroupItemInfoRepository;
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepsitory;

        private ILogService LogService;

        // 파일디렉토리
        private DirectoryInfo? di;
        private string? PlaceFileFolderPath;

      

        public BuildingService(
            IBuildingInfoRepository _buildinginforepository,
            IBuildingGroupItemInfoRepository _buildinggroupiteminforepository,
            IBuildingItemKeyInfoRepository _buildingitemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            IFloorInfoRepository _floorinforepository,
            ILogService _logservice)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.BuildingGroupItemInfoRepository = _buildinggroupiteminforepository;

            this.BuildingItemKeyInfoRepository = _buildingitemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;

            this.FloorInfoRepsitory = _floorinforepository;

            this.LogService = _logservice;
        }

        /// <summary>
        /// 해당 사업장에 건물추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddBuildingDTO?>> AddBuildingService(HttpContext? context, AddBuildingDTO? dto, IFormFile? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<AddBuildingDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<AddBuildingDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<AddBuildingDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddBuildingDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


               // 건물 관련한 폴더 없으면 만들기
                PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeidx.ToString()); // 사업장

               di = new DirectoryInfo(PlaceFileFolderPath);
               if (!di.Exists) di.Create();


                BuildingTb? model = new BuildingTb();
                model.BuildingCd = dto.Code; // 건물코드
                model.Name = dto.Name; // 건물명
                model.Address = dto.Address; // 주소
                model.Tel = dto.Tel; // 전화번호
                model.Usage = dto.Usage; // 건물용도
                model.ConstComp = dto.ConstCompany; // 시공업체
                model.CompletionDt = dto.CompletionDT; // 준공년월
                model.BuildingStruct = dto.BuildingStruct; // 건물구조
                model.RoofStruct = dto.RoofStruct; // 지붕구조
                model.GrossFloorArea = dto.GrossFloorArea; // 연면적
                model.LandArea = dto.LandArea; // 대지면적
                model.BuildingArea = dto.BuildingArea; // 건축면적
                model.FloorNum = dto.FloorNum; // 건물층수
                model.GroundFloorNum = dto.GroundFloorNum; // 지상층수
                model.BasementFloorNum = dto.BasementFloorNum; // 지하층수
                model.BuildingHeight = dto.BuildingHeight; // 건물높이
                model.ParkingNum = dto.ParkingNum; // 지상높이
                model.InnerParkingNum = dto.InnerParkingNum; // 옥내대수
                model.OuterParkingNum = dto.OuterParkingNum; // 옥외대수
                model.ElecCapacity = dto.ElecCapacity; // 전기용량
                model.FaucetCapacity = dto.FaucetCapacity; // 수전용량
                model.WaterCapacity = dto.WaterCapacity; // 급수용량
                model.ElevWaterCapacity = dto.ElevWaterCapacity; // 고가수조
                model.WaterTank = dto.WaterTank; // 저수조
                model.GasCapacity = dto.GasCapacity; // 가스용량
                model.Boiler = dto.Boiler; // 보일러
                model.WaterDispenser = dto.WaterDispenser; // 냉온수기
                model.LiftNum = dto.LiftNum; // 승강기대수
                model.PeopleLiftNum = dto.PeopleLiftNum; // 인승용
                model.CargoLiftNum = dto.CargoLiftNum; // 화물용
                model.CoolHeatCapacity = dto.CoolHeatCapacity; // 냉난방용량
                model.HeatCapacity = dto.HeatCapacity; // 난방용량
                model.CoolCapacity = dto.CoolCapacity; // 냉방용량
                model.LandscapeArea = dto.LandScapeArea; // 조경면적
                model.GroundArea = dto.GroundArea; // 지상면적
                model.RooftopArea = dto.RooftopArea; // 옥상면적
                model.ToiletNum = dto.ToiletNum; // 화장실개수
                model.MenToiletNum = dto.MenToiletNum; // 남자화장실 개소
                model.WomenToiletNum = dto.WomenToiletNum; // 여자화장실 개소
                model.FireRating = dto.FireRating; // 소방등급
                model.SepticTankCapacity = dto.SeptictankCapacity;
                model.CreateDt = DateTime.Now;
                model.CreateUser = Creater;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = Creater;
                model.PlaceTbId = Int32.Parse(placeidx);

                if (files is not null)
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<AddBuildingDTO?>() { message = "올바르지 않은 파일형식입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        // 이미지경로
                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}"; // 암호화된 파일명

                        string? newFilePath = Path.Combine(PlaceFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                }
                else
                {
                    model.Image = null;
                }

                BuildingTb? buildingtb = await BuildingInfoRepository.AddAsync(model);
                
                if(buildingtb is not null)
                {
                    return new ResponseUnit<AddBuildingDTO?>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = new AddBuildingDTO()
                        {
                            ID = buildingtb.Id, // 추가된 후 건물ID
                            Code = buildingtb.BuildingCd, // 건물코드
                            Name = buildingtb.Name, // 건물명
                            Address = buildingtb.Address,  // 건물주소
                            Tel = buildingtb.Tel, // 전화번호
                            Usage = buildingtb.Usage,
                            ConstCompany = buildingtb.ConstComp, // 시공업체
                            CompletionDT = buildingtb.CompletionDt, // 준공년월
                            BuildingStruct = buildingtb.BuildingStruct, // 건물구조
                            RoofStruct = buildingtb.RoofStruct, // 지붕구조
                            GrossFloorArea = buildingtb.GrossFloorArea, // 연면적
                            LandArea = buildingtb.LandArea, // 대지면적
                            BuildingArea = buildingtb.BuildingArea, // 건축면적
                            FloorNum = buildingtb.FloorNum, // 건물층수
                            GroundFloorNum = buildingtb.GroundFloorNum, // 지상층수
                            BasementFloorNum = buildingtb.BasementFloorNum, // 지하층수
                            BuildingHeight = buildingtb.BuildingHeight, // 건물높이
                            GroundHeight = buildingtb.GroundHeight, // 지상높이
                            BasementHeight = buildingtb.BasementHeight, // 지하깊이
                            ParkingNum =  buildingtb.ParkingNum, // 주차장 대수
                            InnerParkingNum = buildingtb.InnerParkingNum, // 옥내 대수
                            OuterParkingNum = buildingtb.OuterParkingNum, // 옥외 대수
                            ElecCapacity = buildingtb.ElecCapacity, // 전기용량
                            FaucetCapacity = buildingtb.FaucetCapacity, // 수전용량
                            GenerationCapacity = buildingtb.GenerationCapacity, // 발전용량
                            WaterCapacity = buildingtb.WaterCapacity, // 급수용량
                            ElevWaterCapacity = buildingtb.ElevWaterCapacity, // 고가수조
                            WaterTank = buildingtb.WaterTank, // 저수조
                            GasCapacity = buildingtb.GasCapacity, // 가스용량
                            Boiler = buildingtb.Boiler, // 보일러
                            WaterDispenser = buildingtb.WaterDispenser, // 냉온수기
                            LiftNum = buildingtb.LiftNum, // 승강기 대수
                            PeopleLiftNum = buildingtb.PeopleLiftNum, // 인승용
                            CargoLiftNum = buildingtb.CargoLiftNum, // 화물용
                            CoolHeatCapacity = buildingtb.CoolHeatCapacity, // 냉난방용량
                            HeatCapacity = buildingtb.HeatCapacity, // 난방용량
                            CoolCapacity = buildingtb.CoolCapacity, // 냉방용량
                            LandScapeArea = buildingtb.LandscapeArea, // 조경면적
                            GroundArea = buildingtb.GroundArea, // 지상면적
                            RooftopArea = buildingtb.RooftopArea, // 옥상면적
                            ToiletNum = buildingtb.ToiletNum, // 화장실개소
                            MenToiletNum = buildingtb.MenToiletNum, // 남자화장실 개소
                            WomenToiletNum = buildingtb.WomenToiletNum, // 여자화장실 개소
                            FireRating = buildingtb.FireRating, // 소방등급
                            SeptictankCapacity = buildingtb.SepticTankCapacity // 정화조 용량
                        },
                        code = 200
                    };
                   
                }
                else
                {
                    return new ResponseUnit<AddBuildingDTO?>() { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddBuildingDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 등록되어있는 건물리스트 출력
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext? context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = new List<BuildinglistDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = new List<BuildinglistDTO>(), code = 404 };

                List<BuildingTb>? model = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<BuildinglistDTO>()
                    {
                        message = "요청이 정상적으로 처리되었습니다.",
                        data = model.Select(e => new BuildinglistDTO
                        {
                            ID = e.Id,
                            BuildingCD = e.BuildingCd,
                            Name = e.Name,
                            Address = e.Address,
                            CompletionDT = e.CompletionDt,
                            CreateDT = e.CreateDt
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = new List<BuildinglistDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<BuildinglistDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<BuildinglistDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 건물 상세정보 보기
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DetailBuildingDTO>?> GetDetailBuildingService(HttpContext? context, int? buildingId)
        {
            try
            {
               if (context is null)
                   return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };
               
                if (buildingId is null)
                   return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

               string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
               if(String.IsNullOrWhiteSpace(placeid))
                   return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

               BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(buildingId);

                if (model is not null)
                {
                    DetailBuildingDTO dto = new DetailBuildingDTO();
                    dto.ID = model.Id; // 건물인덱스
                    dto.Code = model.BuildingCd; // 건물코드
                    dto.Name = model.Name; // 건물명
                    dto.Address = model.Address; // 주소
                    dto.Tel = model.Tel; // 전화번호
                    dto.Usage = model.Usage; // 건물용도
                    dto.ConstCompany = model.ConstComp; // 시공사
                    dto.CompletionDT = model.CompletionDt; // 준공년월
                    dto.BuildingStruct = model.BuildingStruct; // 건물구조
                    dto.RoofStruct = model.RoofStruct; // 지붕구조
                    dto.GrossFloorArea = model.GrossFloorArea; // 연면적
                    dto.LandArea = model.LandArea; // 대지면적
                    dto.BuildingArea = model.BuildingArea; // 건축면적
                    dto.FloorNum = model.FloorNum; // 건물층수
                    dto.GroundFloorNum = model.GroundFloorNum; // 지상층수
                    dto.BasementFloorNum = model.BasementFloorNum; // 지하층수
                    dto.BuildingHeight = model.BuildingHeight; // 건물높이
                    dto.GroundHeight = model.GroundHeight; // 지상높이
                    dto.BasementHeight = model.BasementHeight; // 지하깊이
                    dto.ParkingNum = model.ParkingNum; // 주차장 대수
                    dto.InnerParkingNum = model.InnerParkingNum; // 옥내 대수
                    dto.OuterParkingNum = model.OuterParkingNum; // 옥외 대수
                    dto.ElecCapacity = model.ElecCapacity; // 전기용량
                    dto.FaucetCapacity = model.FaucetCapacity; // 수전용량
                    dto.GenerationCapacity = model.GenerationCapacity; // 발전용량
                    dto.WaterCapacity = model.WaterCapacity; // 급수용량
                    dto.ElevWaterCapacity = model.ElevWaterCapacity; // 고가수조
                    dto.WaterTank = model.WaterTank; // 저수조
                    dto.GasCapacity = model.GasCapacity; // 가스용량
                    dto.Boiler = model.Boiler; // 보일러
                    dto.WaterDispenser = model.WaterDispenser; // 냉온수기
                    dto.LiftNum = model.LiftNum; // 승강기 대수
                    dto.PeopleLiftNum = model.PeopleLiftNum; // 인승용
                    dto.CargoLiftNum = model.CargoLiftNum; // 화물용
                    dto.CoolHeatCapacity = model.CoolHeatCapacity; // 냉난방용량
                    dto.HeatCapacity = model.HeatCapacity; // 난방용량
                    dto.CoolCapacity = model.CoolCapacity; // 냉방용량
                    dto.LandScapeArea = model.LandscapeArea; // 조경면적
                    dto.GroundArea = model.GroundArea; // 지상면적
                    dto.RooftopArea = model.RooftopArea; // 옥상면적
                    dto.ToiletNum = model.ToiletNum; // 화장실개소
                    dto.MenToiletNum = model.MenToiletNum; // 남자화장실 개소
                    dto.WomenToiletNum = model.WomenToiletNum; // 여자화장실 개소
                    dto.FireRating = model.FireRating; // 소방등급
                    dto.SeptictankCapacity = model.SepticTankCapacity; // 정화조 용량

                    string? Image = model.Image;
                    if (!String.IsNullOrWhiteSpace(Image))
                    {
                        string PlaceFileName = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString());
                        string[] FileList = Directory.GetFiles(PlaceFileName);
                        if (FileList is [_, ..])
                        {
                            foreach (var file in FileList)
                            {
                                if (file.Contains(Image))
                                {
                                    byte[] ImageBytes = File.ReadAllBytes(file);
                                    dto.Image = Convert.ToBase64String(ImageBytes);
                                }
                            }
                        }
                        else
                        {
                            dto.Image = model.Image;
                        }
                    }
                    else
                    {
                        dto.Image = model.Image;
                    }

                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<DetailBuildingDTO>() { message = "데이터가 존재하지 않습니다.", data = new DetailBuildingDTO(), code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailBuildingDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailBuildingDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateBuildingService(HttpContext? context, DetailBuildingDTO? dto, IFormFile? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(dto.ID);
                if (model is not null)
                {
                    model.BuildingCd = dto.Code; // 건물코드
                    model.Name = dto.Name; // 건물명
                    model.Address = dto.Address; // 건물주소
                    model.Tel = dto.Tel; // 전화번호
                    model.Usage = dto.Usage; // 건물용도
                    model.ConstComp = dto.ConstCompany; // 시공사
                    model.CompletionDt = dto.CompletionDT; // 준공년월
                    model.BuildingStruct = dto.BuildingStruct; // 건물구조
                    model.RoofStruct = dto.RoofStruct; // 지붕구조
                    model.GrossFloorArea = dto.GrossFloorArea; // 연면적
                    model.LandArea = dto.LandArea; // 대지면적
                    model.BuildingArea = dto.BuildingArea; // 건축면적
                    model.FloorNum = dto.FloorNum; // 건물층수
                    model.GroundFloorNum = dto.GroundFloorNum; // 지상층수
                    model.BasementFloorNum = dto.BasementFloorNum; // 지하층수
                    model.BuildingHeight = dto.BuildingHeight; // 건물높이
                    model.GroundHeight = dto.GroundHeight; // 지상높이
                    model.BasementHeight = dto.BasementHeight; // 지하깊이
                    model.ParkingNum = dto.ParkingNum; // 주차대수
                    model.InnerParkingNum = dto.InnerParkingNum; // 옥내대수
                    model.OuterParkingNum = dto.OuterParkingNum; // 옥외대수
                    model.ElecCapacity = dto.ElecCapacity; // 전기용량
                    model.FaucetCapacity = dto.FaucetCapacity; // 수전용량
                    model.GenerationCapacity = dto.GenerationCapacity; // 발전용량
                    model.WaterCapacity = dto.WaterCapacity; // 급수용량
                    model.ElevWaterCapacity = dto.ElevWaterCapacity; // 고가수조
                    model.WaterTank = dto.WaterTank; // 저수조
                    model.GasCapacity = dto.GasCapacity; // 가스용량
                    model.Boiler = dto.Boiler; // 보일러
                    model.WaterDispenser = dto.WaterDispenser; // 냉온수기
                    model.LiftNum = dto.LiftNum; // 승강기대수
                    model.PeopleLiftNum = dto.PeopleLiftNum; // 인승용 대수
                    model.CargoLiftNum = dto.CargoLiftNum; // 화물용 대수
                    model.CoolHeatCapacity = dto.CoolHeatCapacity; // 냉난방용량
                    model.HeatCapacity = dto.HeatCapacity; // 난방용량
                    model.CoolCapacity = dto.CoolCapacity; // 냉방용량
                    model.LandscapeArea = dto.LandScapeArea; // 조경면적
                    model.GroundArea = dto.GroundArea; // 지상면적
                    model.RooftopArea = dto.RooftopArea; // 옥상면적
                    model.ToiletNum = dto.ToiletNum; // 화장실개수
                    model.MenToiletNum = dto.MenToiletNum; // 남자화장실 개소
                    model.WomenToiletNum = dto.WomenToiletNum; // 여자화장실 개소
                    model.FireRating = dto.FireRating; // 소방등급
                    model.SepticTankCapacity = dto.SeptictankCapacity; // 정화조용량
                    model.UpdateDt = DateTime.Now; // 수정일자
                    model.UpdateUser = creater; // 수정자

                    // 이미지 변경 or 삭제
                    if (files is not null) // 파일이 공백이 아닌경우 - 삭제 업데이트 or insert
                    {
                        FileName = files.FileName;
                        FileExtenstion = Path.GetExtension(FileName);
                        if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                        {
                            return new ResponseUnit<bool?>() { message = "이미지의 형식이 올바르지 않습니다.", data = null, code = 404 };
                        }

                        // DB 파일 삭제
                        string? filePath = model.Image;
                        PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString());
                        di = new DirectoryInfo(PlaceFileFolderPath);

                        if (di.Exists)
                        {
                            if (!String.IsNullOrWhiteSpace(filePath))
                            {
                                FileName = String.Format(@"{0}\\{1}", PlaceFileFolderPath, filePath);
                                if (File.Exists(FileName))
                                {
                                    File.Delete(FileName);
                                }
                            }
                        }
                        else
                        {
                            di.Create();
                        }

                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}";

                        string? newFilePath = Path.Combine(PlaceFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                    else // 파일이 공백인 경우 db에 해당 값이 있으면 삭제
                    {
                        string? filePath = model.Image;
                        if (!String.IsNullOrWhiteSpace(filePath))
                        {
                            PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString()); // 사업장
                            FileName = String.Format(@"{0}\\{1}", PlaceFileFolderPath, filePath);
                            if (File.Exists(FileName))
                            {
                                File.Delete(FileName);
                            }
                            model.Image = null;
                        }
                    }

                    bool? updateBuilding = await BuildingInfoRepository.UpdateBuildingInfo(model);
                    if(updateBuilding == true)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }

        }


        /// <summary>
        /// 건물정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> DeleteBuildingService(HttpContext? context, List<int>? buildingid)
        {
            try
            {
                int delCount = 0;

                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (buildingid is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]); // 토큰 로그인 사용자 검사
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                // - 층이 있으면 삭제안됨
                // - 건물 삭제시 하위 추가내용들 전체삭제
                for (int i = 0; i < buildingid.Count(); i++)
                {
                    List<FloorTb>? floortb = await FloorInfoRepsitory.GetFloorList(buildingid[i]);

                    if (floortb is [_, ..])
                        return new ResponseUnit<int?> { message = "해당 건물에 속한 층정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                for (int i = 0; i < buildingid.Count(); i++)
                {
                    BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(buildingid[i]);
                    
                    if (BuildingTB is not null)
                    {
                        BuildingTB.DelDt = DateTime.Now;
                        BuildingTB.DelUser = creater;
                        BuildingTB.DelYn = true;

                        bool? delBuilding = await BuildingInfoRepository.DeleteBuildingInfo(BuildingTB);
                        if(delBuilding != true)
                        {
                            return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }

                        List<BuildingItemGroupTb>? GroupTBList = await BuildingGroupItemInfoRepository.GetAllGroupList(buildingid[i]);
                        if (GroupTBList is [_, ..])
                        {
                            foreach(BuildingItemGroupTb GroupTB in GroupTBList)
                            {
                                GroupTB.DelDt = DateTime.Now;
                                GroupTB.DelUser = creater;
                                GroupTB.DelYn = true;

                                bool? delGroup = await BuildingGroupItemInfoRepository.DeleteGroupInfo(GroupTB);
                                if(delGroup != true)
                                {
                                    return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                }

                                List<BuildingItemKeyTb>? GroupKeyList = await BuildingItemKeyInfoRepository.GetAllKeyList(GroupTB.Id);

                                if(GroupKeyList is [_, ..])
                                {
                                    foreach(BuildingItemKeyTb KeyTB in GroupKeyList)
                                    {
                                        KeyTB.DelDt = DateTime.Now;
                                        KeyTB.DelUser = creater;
                                        KeyTB.DelYn = true;

                                        bool? delKey = await BuildingItemKeyInfoRepository.DeleteKeyInfo(KeyTB);
                                        if(delKey != true)
                                        {
                                            return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                        }

                                        List<BuildingItemValueTb>? GroupValueList = await BuildingItemValueInfoRepository.GetAllValueList(KeyTB.Id);

                                        if (GroupValueList is [_, ..])
                                        {
                                            foreach (BuildingItemValueTb ValueTB in GroupValueList)
                                            {
                                                ValueTB.DelDt = DateTime.Now;
                                                ValueTB.DelUser = creater;
                                                ValueTB.DelYn = true;

                                                bool? delValue = await BuildingItemValueInfoRepository.DeleteValueInfo(ValueTB);

                                                if(delValue != true)
                                                {
                                                    return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                    delCount++;
                }
                return new ResponseUnit<int?>() { message = $"{delCount}건 삭제 성공.", data = delCount, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      

    }
}
