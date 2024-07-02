using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Building.SubItem;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;

namespace FamTec.Server.Services.Building
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IBuildingSubItemInfoRepository BuildingSubItemInfoRepository;

        private ILogService LogService;

        public BuildingService(
            IBuildingInfoRepository _buildinginforepository,
            IBuildingSubItemInfoRepository _buildingsubiteminforepository,
            ILogService _logservice)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.BuildingSubItemInfoRepository = _buildingsubiteminforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 해당 사업장에 건물추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> AddBuildingService(HttpContext? context, BuildingsDTO? dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                BuildingTb? model = new BuildingTb
                {
                    BuildingCd = dto.BuildingCode,
                    Name = dto.Name,
                    Address = dto.Address,
                    Tel = dto.Tel,
                    Usage = dto.Usage,
                    ConstComp = dto.ConstComp,
                    CompletionDt = dto.CompletionDt,
                    BuildingStruct = dto.BuildingStruct,
                    RoofStruct = dto.RoofStruct,
                    GrossFloorArea = dto.GrossFloorArea,
                    LandArea = dto.LandArea,
                    BuildingArea = dto.BuildingArea,
                    FloorNum = dto.FloorNum,
                    GroundFloorNum = dto.GroundFloorNum,
                    BasementFloorNum = dto.BasementFloorNum,
                    BuildingHeight = dto.BuildingHeight,
                    GroundHeight = dto.GroundHeight,
                    BasementHeight = dto.BasementHeight,
                    ParkingNum = dto.PackingNum,
                    InnerParkingNum = dto.InnerPackingNum,
                    OuterParkingNum = dto.OuterPackingNum,
                    ElecCapacity = dto.ElecCapacity,
                    FaucetCapacity = dto.FaucetCapacity,
                    GenerationCapacity = dto.GenerationCapacity,
                    WaterCapacity = dto.WaterCapacity,
                    ElevWaterCapacity = dto.ElevWaterCapacity,
                    WaterTank = dto.WaterTank,
                    GasCapacity = dto.GasCapacity,
                    Boiler = dto.Boiler,
                    WaterDispenser = dto.WaterDispenser,
                    LiftNum = dto.LiftNum,
                    PeopleLiftNum = dto.PeopleLiftNum,
                    CargoLiftNum = dto.CargoLiftNum,
                    HeatCapacity = dto.HeatCapacity,
                    CoolCapacity = dto.CoolCapacity,
                    LandscapeArea = dto.LandScapeArea,
                    GroundArea = dto.GroundArea,
                    RooftopArea = dto.RooftopArea,
                    ToiletNum = dto.ToiletNum,
                    MenToiletNum = dto.MenToiletNum,
                    WomenToiletNum = dto.WomenToiletNum,
                    FireRating = dto.FireRating,
                    SepticTankCapacity = dto.SepticTankCapacity,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    PlaceTbId = Int32.Parse(placeidx)
                };

                BuildingTb? buildingtb = await BuildingInfoRepository.AddAsync(model);
                
                if(buildingtb is null)
                    return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };

                if (dto.subitem!.Count() > 0)
                {
                    List<BuildingSubitemTb> subitem = new List<BuildingSubitemTb>();

                    for (int i = 0; i < dto.subitem!.Count(); i++)
                    {
                        subitem.Add(new BuildingSubitemTb
                        {
                            Itemname = dto.subitem[i].ItemName,
                            Unit = dto.subitem[i].Unit,
                            Value = dto.subitem[i].Value,
                            CreateDt = DateTime.Now,
                            CreateUser = Creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = Creater,
                            BuildingTbId = buildingtb.Id
                        });
                    }

                    // SubItem에 ADD
                    for (int i = 0; i < subitem.Count; i++) 
                    {
                        BuildingSubitemTb? submodel = await BuildingSubItemInfoRepository.AddAsync(subitem[i]);

                        if (submodel is null)
                            return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                    }
                    // 성공 처리
                    return new ResponseUnit<bool>() { message = "요청이 정상적으로 처리되었습니다.", data = true, code = 200 };
                }
                
                // 성공 처리
                if (buildingtb is not null)
                    return new ResponseUnit<bool>() { message = "요청이 정상적으로 처리되었습니다.", data = true, code = 200 };
                else
                    return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
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
                            PlaceID = e.PlaceTbId,
                            Name = e.Name,
                            Address = e.Address,
                            FloorNum = e.FloorNum,
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

                List<BuildingTb>? tokencheck = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeid));
                if(tokencheck is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                int? idx = tokencheck.FindIndex(item => item.Id.Equals(buildingId));
                if(idx is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };


                BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(buildingId);

                if (model is not null)
                {

                    DetailBuildingDTO dto = new DetailBuildingDTO();
                    dto.Id = model.Id;
                    dto.BuildingCD = model.BuildingCd;
                    dto.Name = model.Name;
                    dto.Address = model.Address;
                    dto.Tel = model.Tel;
                    dto.Usage = model.Usage;
                    dto.ConstComp = model.ConstComp;
                    dto.CompletionDT = model.CompletionDt;
                    dto.BuildingStruct = model.BuildingStruct;
                    dto.RoofStruct = model.RoofStruct;
                    dto.GrossFloorArea = model.GrossFloorArea;
                    dto.LandArea = model.LandArea;
                    dto.BuildingArea = model.BuildingArea;
                    dto.FloorNum = model.FloorNum;
                    dto.GroundFloorNum = model.GroundFloorNum;
                    dto.BasementFloorNum = model.BasementFloorNum;
                    dto.BuildingHeight = model.BuildingHeight;
                    dto.GroundHeight = model.GroundHeight;
                    dto.BasementHeight = model.BasementHeight;
                    dto.ParkingNum = model.ParkingNum;
                    dto.InnerPackingNum = model.InnerParkingNum;
                    dto.OuterPackingNum = model.OuterParkingNum;
                    dto.ElecCapacity = model.ElecCapacity;
                    dto.FaucetCapacity = model.FaucetCapacity;
                    dto.GenerationCapacity = model.GenerationCapacity;
                    dto.WaterCapacity = model.WaterCapacity;
                    dto.ElevWaterCapacity = model.ElevWaterCapacity;
                    dto.WaterTank = model.WaterTank;
                    dto.GasCapacity = model.GasCapacity;
                    dto.Boiler = model.Boiler;
                    dto.WaterDispenser = model.WaterDispenser;
                    dto.LiftNum = model.LiftNum;
                    dto.PeopleLiftNum = model.PeopleLiftNum;
                    dto.CargoLiftNum = model.CargoLiftNum;
                    dto.CoolHeatCapacity = model.CoolHeatCapacity;
                    dto.HeatCapacity = model.HeatCapacity;
                    dto.CoolCapacity = model.CoolCapacity;
                    dto.LandSacpeArea = model.LandscapeArea;
                    dto.GroundArea = model.GroundArea;
                    dto.RooftopArea = model.RooftopArea;
                    dto.ToiletNum = model.ToiletNum;
                    dto.MenToiletNum = model.MenToiletNum;
                    dto.WomenToiletNum = model.WomenToiletNum;
                    dto.FireRating = model.FireRating;
                    dto.SepticTankCapacity = model.SepticTankCapacity;

                    List<BuildingSubitemTb>? submodel = await BuildingSubItemInfoRepository.GetAllBuildingSubItemList(buildingId);
                    if(submodel is [_, ..])
                    {
                        for (int i = 0; i < submodel.Count(); i++)
                        {
                            dto.subitem.Add(new DetailBuildingSubItemDTO
                            {
                                Id = submodel[i].Id,
                                ItemName = submodel[i].Itemname,
                                Unit = submodel[i].Unit,
                                Value = submodel[i].Value
                            });
                        }
                    }

                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

                }
                else
                {
                    return new ResponseUnit<DetailBuildingDTO>() { message = "데이터가 존재하지 않습니다.", data = new DetailBuildingDTO(), code = 200 };
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
        public async ValueTask<ResponseUnit<DetailBuildingDTO>?> UpdateBuildingService(HttpContext? context, DetailBuildingDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                List<BuildingTb>? tokencheck = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeid));
                if (tokencheck is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                int? idx = tokencheck.FindIndex(item => item.Id.Equals(dto.Id));
                if (idx is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                // 업데이트할 모델 조회해서 가져온 후 넘겨야함. - 업데이트 적용됨
                BuildingTb? updatemodel = await BuildingInfoRepository.GetBuildingInfo(dto.Id);
                if (updatemodel is not null)
                {
                    updatemodel.BuildingCd = dto.BuildingCD; // 건물코드
                    updatemodel.Name = dto.Name; // 건물명
                    updatemodel.Address = dto.Address; // 주소
                    updatemodel.Tel = dto.Tel; // 전화번호
                    updatemodel.Usage = dto.Usage; // 건물용도
                    updatemodel.ConstComp = dto.ConstComp; // 시공업체
                    updatemodel.CompletionDt = dto.CompletionDT; // 준공년월
                    updatemodel.BuildingStruct = dto.BuildingStruct; // 건물구조
                    updatemodel.RoofStruct = dto.RoofStruct; // 지붕구조
                    updatemodel.GrossFloorArea = dto.GrossFloorArea; // 연면적
                    updatemodel.LandArea = dto.LandArea; // 대지면적
                    updatemodel.BuildingArea = dto.BuildingArea; // 건물면적
                    updatemodel.FloorNum = dto.FloorNum; // 건물층수
                    updatemodel.GroundFloorNum = dto.GroundFloorNum; // 지상층수
                    updatemodel.BasementFloorNum = dto.BasementFloorNum; // 지하층수
                    updatemodel.BuildingHeight = dto.BuildingHeight; // 건물높이
                    updatemodel.GroundHeight = dto.GroundHeight; // 지상 높이
                    updatemodel.BasementHeight = dto.BasementHeight; // 지하깊이
                    updatemodel.ParkingNum = dto.ParkingNum; // 주차대수
                    updatemodel.InnerParkingNum = dto.InnerPackingNum; // 옥내대수
                    updatemodel.OuterParkingNum = dto.OuterPackingNum; // 옥외대수
                    updatemodel.ElecCapacity = dto.ElecCapacity; // 전기용량
                    updatemodel.FaucetCapacity = dto.FaucetCapacity; // 수전용량
                    updatemodel.GenerationCapacity = dto.GenerationCapacity; // 발전용량
                    updatemodel.WaterCapacity = dto.WaterCapacity; // 급수용량
                    updatemodel.ElevWaterCapacity = dto.ElevWaterCapacity; // 고가수조
                    updatemodel.WaterTank = dto.WaterTank; // 저수조
                    updatemodel.GasCapacity = dto.GasCapacity; // 가스용량
                    updatemodel.Boiler = dto.Boiler; // 보일러
                    updatemodel.WaterDispenser = dto.WaterDispenser; // 냉온수기
                    updatemodel.LiftNum = dto.LiftNum; // 승강기대수
                    updatemodel.PeopleLiftNum = dto.PeopleLiftNum; // 인승용
                    updatemodel.CargoLiftNum = dto.CargoLiftNum; // 화물용
                    updatemodel.CoolHeatCapacity = dto.CoolHeatCapacity; // 냉난방용량
                    updatemodel.HeatCapacity = dto.HeatCapacity; // 난방용량
                    updatemodel.CoolCapacity = dto.CoolCapacity; // 냉방용량
                    updatemodel.LandscapeArea = dto.LandSacpeArea; // 조경면적
                    updatemodel.GroundArea = dto.GroundArea; // 지상면적
                    updatemodel.RooftopArea = dto.RooftopArea; // 옥상면적
                    updatemodel.ToiletNum = dto.ToiletNum; // 화장실개수
                    updatemodel.MenToiletNum = dto.MenToiletNum; // 남자화장실 개수
                    updatemodel.WomenToiletNum = dto.WomenToiletNum; // 여자화장실 개수
                    updatemodel.FireRating = dto.FireRating; // 소방등급
                    updatemodel.SepticTankCapacity = dto.SepticTankCapacity; // 정화조용량

                    updatemodel.UpdateDt = DateTime.Now; // 수정일
                    updatemodel.UpdateUser = creater; // 수정자

                    bool? result = await BuildingInfoRepository.UpdateBuildingInfo(updatemodel);
                    if (result == true)
                    {
                        // 추가일떄만 업데이트 하면될듯
                        if (dto.subitem.Count() > 0)
                        {

                            for (int i = 0; i < dto.subitem.Count(); i++)
                            {
                                BuildingSubitemTb? submodel = await BuildingSubItemInfoRepository.GetBuildingSubItemInfo(dto.subitem[i].Id);
                                if (submodel is not null)
                                {
                                    submodel.Itemname = dto.subitem[i].ItemName;
                                    submodel.Unit = dto.subitem[i].Unit;
                                    submodel.Value = dto.subitem[i].Value;
                                    submodel.UpdateDt = DateTime.Now;
                                    submodel.UpdateUser = creater;

                                    // Update 하면됨.
                                    bool? subresult = await BuildingSubItemInfoRepository.UpdateBuildingSubItemInfo(submodel);
                                    
                                    if(subresult != true)
                                        return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 처리되지 않았습니다.", data = dto, code = 200 };
                                }
                                else
                                {
                                    return new ResponseUnit<DetailBuildingDTO>() { message = "잘못된 요청입니다.", data = dto, code = 404 };
                                }
                            }
                        }

                        return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else if (result == false)
                    {
                        return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 처리되지 않았습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<DetailBuildingDTO>() { message = "잘못된 요청입니다.", data = dto, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailBuildingDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailBuildingDTO(), code = 500 };
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


                // context 에서 placeidx로 위의 building이 있는지 조회
                List<BuildingTb>? tokenchk = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeidx));
                if (tokenchk is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                //      - 해당 사업장의 전체 building에서 선택된 buildingID의 모델을 조회
                var contextchk = tokenchk.Where(_ => buildingid.Contains(_.Id));
                if (contextchk.Count() != buildingid.Count()) // 조회한 모델 Count랑 선택된 buildingID의 Count랑 다르면 null - 토큰에러
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                // 삭제대상 조회
                List<BuildingTb>? target = await BuildingInfoRepository.GetDeleteList(buildingid);

                if (target is [_, ..])
                {
                    for (int i = 0; i < target.Count(); i++)
                    {
                        target[i].DelDt = DateTime.Now;
                        target[i].DelYn = true;
                        target[i].DelUser = creater;
                        bool? result = await BuildingInfoRepository.DeleteBuildingInfo(target[i]);

                        if (result == true)
                        {
                            bool? subresult = await BuildingSubItemInfoRepository.DeleteBuildingSubItemInfo(target[i].Id, creater);

                            if(subresult != true)
                            {
                                return new ResponseUnit<int?>() { message = $"데이터가 {i + 1}건 삭제되었습니다.", data = i, code = 200 };
                            }
                        }
                        else
                        {
                            // Main 삭제하다 실패.
                            return new ResponseUnit<int?>() { message = $"데이터가 {i+1}건 삭제되었습니다.", data = i, code = 200 };
                        }
                    }

                    return new ResponseUnit<int?>() { message = $"데이터가 {target.Count()}건 삭제되었습니다.", data = target.Count(), code = 200 };
                }
                else
                {
                    return new ResponseUnit<int?>() { message = $"데이터가 {target.Count()}건 삭제되었습니다.", data = target.Count(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
