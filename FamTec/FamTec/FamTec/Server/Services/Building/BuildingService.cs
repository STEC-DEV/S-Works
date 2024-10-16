using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Building;

namespace FamTec.Server.Services.Building
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;

        private IFileService FileService;
        private ILogService LogService;

        // 파일디렉토리
        private DirectoryInfo? di;
        private string? PlaceFileFolderPath;

        public BuildingService(
            IBuildingInfoRepository _buildinginforepository,
            IFloorInfoRepository _floorinforepository,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;

            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> TotalBuildingCount(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int? Count = await BuildingInfoRepository.TotalBuildingCount(Convert.ToInt32(placeid)).ConfigureAwait(false);
                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = Count, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 사업장에 건물추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddBuildingDTO>> AddBuildingService(HttpContext? context, AddBuildingDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddBuildingDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(Creater) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<AddBuildingDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                string NewFileName = files is not null ? FileService.SetNewFileName(UserIdx, files) : String.Empty;

                // 건물 관련한 폴더 없으면 만들기
                //PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeidx.ToString()); // 사업장
                PlaceFileFolderPath = Path.Combine(Common.FileServer, placeidx.ToString(), "Building");

                di = new DirectoryInfo(PlaceFileFolderPath);
                if (!di.Exists) di.Create();

                BuildingTb? model = new BuildingTb()
                {
                    BuildingCd = dto.Code, /* 건물코드 */
                    Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!, /* 건물명 */
                    Address = dto.Address, // 주소
                    Tel = dto.Tel, // 전화번호
                    Usage = dto.Usage, // 건물용도
                    ConstComp = dto.ConstCompany, // 시공업체
                    CompletionDt = dto.CompletionDT, // 준공년월
                    BuildingStruct = dto.BuildingStruct, // 건물구조
                    RoofStruct = dto.RoofStruct, // 지붕구조
                    GrossFloorArea = dto.GrossFloorArea, // 연면적
                    LandArea = dto.LandArea, // 대지면적
                    BuildingArea = dto.BuildingArea, // 건축면적
                    FloorNum = dto.FloorNum, // 건물층수
                    GroundFloorNum = dto.GroundFloorNum, // 지상층수
                    BasementFloorNum = dto.BasementFloorNum, // 지하층수
                    BuildingHeight = dto.BuildingHeight, // 건물높이
                    GroundHeight = dto.GroundHeight, // 지상높이
                    BasementHeight = dto.BasementHeight, // 지하깊이
                    ParkingNum = dto.ParkingNum, // 지상높이
                    InnerParkingNum = dto.InnerParkingNum, // 옥내대수
                    OuterParkingNum = dto.OuterParkingNum, // 옥외대수
                    ElecCapacity = dto.ElecCapacity, // 전기용량
                    FaucetCapacity = dto.FaucetCapacity, // 수전용량
                    GenerationCapacity = dto.GenerationCapacity, // 발전용량
                    WaterCapacity = dto.WaterCapacity, // 급수용량
                    ElevWaterCapacity = dto.ElevWaterCapacity, // 고가수조
                    WaterTank = dto.WaterTank, // 저수조
                    GasCapacity = dto.GasCapacity, // 가스용량
                    Boiler = dto.Boiler, // 보일러
                    WaterDispenser = dto.WaterDispenser, // 냉온수기
                    LiftNum = dto.LiftNum, // 승강기대수
                    PeopleLiftNum = dto.PeopleLiftNum, // 인승용
                    CargoLiftNum = dto.CargoLiftNum, // 화물용
                    CoolHeatCapacity = dto.CoolHeatCapacity, // 냉난방용량
                    HeatCapacity = dto.HeatCapacity, // 난방용량
                    CoolCapacity = dto.CoolCapacity, // 냉방용량
                    LandscapeArea = dto.LandScapeArea, // 조경면적
                    GroundArea = dto.GroundArea, // 지상면적
                    RooftopArea = dto.RooftopArea, // 옥상면적
                    ToiletNum = dto.ToiletNum, // 화장실개수
                    MenToiletNum = dto.MenToiletNum, // 남자화장실 개소
                    WomenToiletNum = dto.WomenToiletNum, // 여자화장실 개소
                    FireRating = dto.FireRating, // 소방등급
                    SepticTankCapacity = dto.SeptictankCapacity,
                    CreateDt = ThisTime,
                    CreateUser = Creater,
                    UpdateDt = ThisTime,
                    UpdateUser = Creater,
                    PlaceTbId = Int32.Parse(placeidx),
                    Image = files is not null ? NewFileName : null
                };

                BuildingTb? buildingtb = await BuildingInfoRepository.AddAsync(model).ConfigureAwait(false);
                
                if(buildingtb is not null)
                {
                    if(files is not null)
                    {
                        // 파일 넣기
                        bool? AddFile = await FileService.AddResizeImageFile(NewFileName, PlaceFileFolderPath, files);
                    }

                    return new ResponseUnit<AddBuildingDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = new AddBuildingDTO()
                        {
                            Id = buildingtb.Id, // 아이디
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
                            ParkingNum = buildingtb.ParkingNum, // 주차장 대수
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
                    return new ResponseUnit<AddBuildingDTO>() { message = "요청이 처리되지 않았습니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddBuildingDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 등록되어있는 건물리스트 출력
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BuildingTb>? model = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeidx)).ConfigureAwait(false);

                if (model is not null && model.Any())
                {
                    var BuildingData = model.Select(e => new BuildinglistDTO
                    {
                        ID = e.Id,
                        BuildingCD = e.BuildingCd,
                        Name = e.Name,
                        Tel = e.Tel,
                        TotalFloor = e.FloorNum,
                        Address = e.Address,
                        CompletionDT = e.CompletionDt,
                        CreateDT = e.CreateDt
                    }).ToList();

                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = BuildingData, code = 200 };
                }
                else
                {
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = null, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<BuildinglistDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물리스트 조회 - 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<ResponseList<BuildinglistDTO>> GetBuildingListPageService(HttpContext context, int skip, int take)
        {
            try
            {
                if (context is null)
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BuildingTb>? model = await BuildingInfoRepository.GetAllBuildingPageList(Int32.Parse(placeidx), skip, take).ConfigureAwait(false);

                if (model is not null && model.Any())
                {
                    var BuildingData = model.Select(e => new BuildinglistDTO
                    {
                        ID = e.Id,
                        BuildingCD = e.BuildingCd,
                        Name = e.Name,
                        Address = e.Address,
                        CompletionDT = e.CompletionDt,
                        CreateDT = e.CreateDt
                    }).ToList();

                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = BuildingData, code = 200 };
                }
                else
                {
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<BuildinglistDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 로그인한 아이디의 사업장의 건물명 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<PlaceBuildingNameDTO>> GetPlaceBuildingNameService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BuildingTb>? model = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeidx)).ConfigureAwait(false);

                if (model is not null && model.Any())
                {
                    var PlaceBuildingNameData = model.Select(e => new PlaceBuildingNameDTO
                    {
                        ID = e.Id,
                        Name = e.Name
                    }).ToList();

                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = PlaceBuildingNameData, code = 200 };
                }
                else
                {
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = null, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<PlaceBuildingNameDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 건물 상세정보 보기
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<DetailBuildingDTO>> GetDetailBuildingService(HttpContext context, int buildingId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(buildingId).ConfigureAwait(false);
                if (model is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 404 };

                DetailBuildingDTO dto = new DetailBuildingDTO()
                {
                    ID = model.Id, // 건물인덱스
                    Code = model.BuildingCd, // 건물코드
                    Name = model.Name, // 건물명
                    Address = model.Address, // 주소
                    Tel = model.Tel, // 전화번호
                    Usage = model.Usage, // 건물용도
                    ConstCompany = model.ConstComp, // 시공사
                    CompletionDT = model.CompletionDt, // 준공년월
                    BuildingStruct = model.BuildingStruct, // 건물구조
                    RoofStruct = model.RoofStruct, // 지붕구조
                    GrossFloorArea = model.GrossFloorArea, // 연면적
                    LandArea = model.LandArea, // 대지면적
                    BuildingArea = model.BuildingArea, // 건축면적
                    FloorNum = model.FloorNum, // 건물층수
                    GroundFloorNum = model.GroundFloorNum, // 지상층수
                    BasementFloorNum = model.BasementFloorNum, // 지하층수
                    BuildingHeight = model.BuildingHeight, // 건물높이
                    GroundHeight = model.GroundHeight, // 지상높이
                    BasementHeight = model.BasementHeight, // 지하깊이
                    ParkingNum = model.ParkingNum, // 주차장 대수
                    InnerParkingNum = model.InnerParkingNum, // 옥내 대수
                    OuterParkingNum = model.OuterParkingNum, // 옥외 대수
                    ElecCapacity = model.ElecCapacity, // 전기용량
                    FaucetCapacity = model.FaucetCapacity, // 수전용량
                    GenerationCapacity = model.GenerationCapacity, // 발전용량
                    WaterCapacity = model.WaterCapacity, // 급수용량
                    ElevWaterCapacity = model.ElevWaterCapacity, // 고가수조
                    WaterTank = model.WaterTank, // 저수조
                    GasCapacity = model.GasCapacity, // 가스용량
                    Boiler = model.Boiler, // 보일러
                    WaterDispenser = model.WaterDispenser, // 냉온수기
                    LiftNum = model.LiftNum, // 승강기 대수
                    PeopleLiftNum = model.PeopleLiftNum, // 인승용
                    CargoLiftNum = model.CargoLiftNum, // 화물용
                    CoolHeatCapacity = model.CoolHeatCapacity, // 냉난방용량
                    HeatCapacity = model.HeatCapacity, // 난방용량
                    CoolCapacity = model.CoolCapacity, // 냉방용량
                    LandScapeArea = model.LandscapeArea, // 조경면적
                    GroundArea = model.GroundArea, // 지상면적
                    RooftopArea = model.RooftopArea, // 옥상면적
                    ToiletNum = model.ToiletNum, // 화장실개소
                    MenToiletNum = model.MenToiletNum, // 남자화장실 개소
                    WomenToiletNum = model.WomenToiletNum, // 여자화장실 개소
                    FireRating = model.FireRating, // 소방등급
                    SeptictankCapacity = model.SepticTankCapacity, // 정화조 용량
                };

                //string PlaceFileName = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString());
                string PlaceFileName = Path.Combine(Common.FileServer, placeid.ToString(), "Building");

                di = new DirectoryInfo(PlaceFileName);
                if (!di.Exists) di.Create();

                if(!String.IsNullOrWhiteSpace(model.Image))
                {
                    byte[]? Images = await FileService.GetImageFile(PlaceFileName, model.Image);

                    dto.ImageName = model.Image;
                    dto.Image = Images;
                }

                return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailBuildingDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateBuildingService(HttpContext context, DetailBuildingDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;


                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                //string PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString());
                string PlaceFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Building");

                di = new DirectoryInfo(PlaceFileFolderPath);
                if (!di.Exists) di.Create();

                DateTime ThisTime = DateTime.Now;

                BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(dto.ID!.Value).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                model.BuildingCd = dto.Code;
                model.Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!; /* 건물명 */
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
                model.UpdateDt = ThisTime; // 수정일자
                model.UpdateUser = creater; // 수정자
                model.PlaceTbId = Convert.ToInt32(placeid);
                    
                
                if (files is not null) // 파일이 공백이 아닌 경우
                {
                    if(files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다르면.
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image)) // 다른데 DB에 저장된 이미지가 NULL이 아니면
                        {
                            deleteFileName = model.Image;
                        }

                        // 새로운 파일명 설정
                        string newFileName = FileService.SetNewFileName(UserIdx, files);
                        NewFileName = newFileName; // 파일명 리스트에 추가
                        model.Image = newFileName; // DB Image명칭 업데이트

                        RemoveTemp = newFileName; // 실패시 삭제명단에 넣어야함.
                    }
                }
                else // 파일이 공백인 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB의 이미지가 공백이 아니면
                    {
                        deleteFileName = model.Image; // 기존 파일 삭제 목록에 추가
                        model.Image = null; // 모델의 파일명 비우기
                    }
                }

                // 먼저 파일 삭제 처리
                // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                byte[]? ImageBytes = null;
                if (!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    ImageBytes = await FileService.GetImageFile(PlaceFileFolderPath, deleteFileName);
                }
                
                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if(ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if (!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(PlaceFileFolderPath, deleteFileName);
                }

                // 새 파일 저장
                if(files is not null)
                {
                    if(String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, PlaceFileFolderPath, files);
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? updateBuilding = await BuildingInfoRepository.UpdateBuildingInfo(model).ConfigureAwait(false);
                if(updateBuilding == true)
                {
                    // 성공했으면 그걸로 끝.
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을 원래대로 돌려놔야함.
                    if(AddTemp is not null)
                    {
                        try
                        {
                            if(FileService.IsFileExists(PlaceFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, PlaceFileFolderPath, files).ConfigureAwait(false);
                            }
                        }
                        catch(Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
                        }
                    }

                    if(!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(PlaceFileFolderPath, RemoveTemp);
                        }
                        catch(Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
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
        public async Task<ResponseUnit<bool?>> DeleteBuildingService(HttpContext context, List<int> buildingid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                
                // 토큰 로그인 사용자 검사 && 토큰 사업장 검사
                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // - 층이 있으면 삭제안됨
                // - 건물 삭제시 하위 추가내용들 전체삭제
                foreach(int delIdx in buildingid)
                {
                    bool? DelCheck = await BuildingInfoRepository.DelBuildingCheck(delIdx).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "참조하고있는 하위 정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                bool? DeleteResult = await BuildingInfoRepository.DeleteBuildingList(buildingid, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속한 건물-층 리스트 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<PlaceBuildingListDTO>> GetPlaceBuildingService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PlaceBuildingListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<PlaceBuildingListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeidx)).ConfigureAwait(false);
                if(buildinglist is not { Count: > 0})
                    return new ResponseList<PlaceBuildingListDTO>() { message = "조회결과가 없습니다.", data = new List<PlaceBuildingListDTO>(), code = 200 };

                List<PlaceBuildingListDTO> dto = new List<PlaceBuildingListDTO>();

                foreach(BuildingTb Building in buildinglist)
                {
                    PlaceBuildingListDTO buidlingDTO = new PlaceBuildingListDTO();
                    buidlingDTO.Id = Building.Id; // 건물ID
                    buidlingDTO.Name = Building.Name; // 건물명

                    List<FloorTb>? FloorList = await FloorInfoRepository.GetFloorList(Building.Id).ConfigureAwait(false);
                    if(FloorList is not null && FloorList.Any())
                    {
                        foreach(FloorTb Floor in FloorList)
                        {
                            buidlingDTO.FloorList.Add(new BuildingFloor
                            {
                                Id = Floor.Id,
                                Name = Floor.Name
                            });
                        }
                    }
                    dto.Add(buidlingDTO);
                }
                return new ResponseList<PlaceBuildingListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<PlaceBuildingListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 건물ID로 건물이름 반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<string?>> GetBuildingName(int buildingid)
        {
            try
            {
                BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(buildingid).ConfigureAwait(false);
                if (BuildingTB is not null)
                    return new ResponseUnit<string?>() { message = "요청이 정상 처리되었습니다.", data = BuildingTB.Name, code = 200 };
                else
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 자재가 포함되어있는 건물 리스트 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<ResponseList<PlaceBuildingNameDTO>> GetPlaceAvailableBuildingList(HttpContext context, int materialid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetPlaceAvailableBuildingList(Convert.ToInt32(placeidx), materialid);
                if (BuildingList is [_, ..])
                {
                    List<PlaceBuildingNameDTO> model = BuildingList.Select(e => new PlaceBuildingNameDTO
                    {
                        ID = e.Id,
                        Name = e.Name
                    }).ToList();

                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<PlaceBuildingNameDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<PlaceBuildingNameDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<PlaceBuildingNameDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
