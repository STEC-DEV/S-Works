using ClosedXML.Excel;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Material;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialInfoRepository MaterialInfoRepository;
        private readonly IInventoryInfoRepository InventoryInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;

        private IFileService FileService;
        private ILogService LogService;

        private DirectoryInfo? di;
        private string? MaterialFileFolderPath;

        public MaterialService(IMaterialInfoRepository _materialinforepository,
            IInventoryInfoRepository _inventoryinforepository,
            IRoomInfoRepository _roominforepository,
            IBuildingInfoRepository _buildinginforepository,
            IFloorInfoRepository _floorinforepository,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MaterialInfoRepository = _materialinforepository;
            this.InventoryInfoRepository = _inventoryinforepository;
            this.RoomInfoRepository = _roominforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;

            this.FileService = _fileservice;
            this.LogService = _logservice;
        }
        
        /// <summary>
        /// 자재추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddMaterialDTO>> AddMaterialService(HttpContext context, AddMaterialDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(Creater) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeidx);

                di = new DirectoryInfo(MaterialFileFolderPath);
                if (!di.Exists) di.Create();

                MaterialTb matertialtb = new MaterialTb()
                {
                    Code = dto.Code!, // 품목코드
                    Name = dto.Name!, // 자재명
                    Unit = dto.Unit, // 단위
                    Standard = dto.Standard, // 규격
                    ManufacturingComp = dto.ManufacturingComp, // 제조사
                    SafeNum = dto.SafeNum, // 안전재고수량
                    DefaultLocation = dto.DefaultLocation!.Value, // 공간위치 인덱스
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    PlaceTbId = Int32.Parse(placeidx), // 사업장ID
                    Image = NewFileName
                };

                MaterialTb? model = await MaterialInfoRepository.AddAsync(matertialtb);
                if(model is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 처리되지 않았습니다.", data = new AddMaterialDTO(), code = 404 };

               
                if(files is not null)
                {
                    // 파일 넣기
                    await FileService.AddResizeImageFile(NewFileName, MaterialFileFolderPath, files);
                }

                return new ResponseUnit<AddMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddMaterialDTO()
                {
                    Code = model.Code, // 품목코드
                    Name = model.Name, // 자재명
                    Unit = model.Unit, // 단위
                    Standard = model.Standard, // 규격
                    ManufacturingComp = model.ManufacturingComp, // 제조사
                    SafeNum = model.SafeNum, // 안전재고수량
                    DefaultLocation = model.DefaultLocation, // 기본위치
                }, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaterialListDTO>> GetPlaceMaterialListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                
                List<MaterialTb>? model = await MaterialInfoRepository.GetPlaceAllMaterialList(Int32.Parse(placeid));
                if(model is null || !model.Any())
                    return new ResponseList<MaterialListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MaterialListDTO>(), code = 200 };
            
                List<MaterialListDTO> ListDTO = new List<MaterialListDTO>();
                foreach (MaterialTb MaterialTB in model)
                {
                    MaterialListDTO DTO = new MaterialListDTO()
                    {
                        ID = MaterialTB.Id, // 품목 인덱스
                        Code = MaterialTB.Code, // 품목 코드
                        Name = MaterialTB.Name, // 품목명
                        Unit = MaterialTB.Unit, // 단위
                        Standard = MaterialTB.Standard, // 규격
                        ManufacturingComp = MaterialTB.ManufacturingComp, // 제조사
                        SafeNum = MaterialTB.SafeNum, // 안전재고수량
                        Image = !String.IsNullOrWhiteSpace(MaterialTB.Image) ? await FileService.GetImageFile(MaterialFileFolderPath, MaterialTB.Image) : null
                    };

                    ListDTO.Add(DTO);
                }
                return new ResponseList<MaterialListDTO>() { message = "요청이 정상 처리되었습니다.", data = ListDTO, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaterialListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<MaterialListDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 자재 상세정보 보기
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseUnit<DetailMaterialDTO>> GetDetailMaterialService(HttpContext context, int materialid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                MaterialTb? model = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), materialid);
                if(model is not null)
                {
                    DetailMaterialDTO dto = new DetailMaterialDTO();
                    dto.Id = model.Id; // 품목 ID
                    dto.Code = model.Code; // 품목코드
                    dto.Name = model.Name; // 품목명
                    dto.Unit = model.Unit; // 품목단위
                    dto.Standard = model.Standard; // 규격
                    dto.ManufacturingComp = model.ManufacturingComp; // 제조사
                    dto.SafeNum = model.SafeNum; // 안전재고 수량
                    dto.RoomID = model.DefaultLocation; // 기본위치

                    MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);

                    if(!String.IsNullOrWhiteSpace(model.Image))
                    {
                        dto.Image = await FileService.GetImageFile(MaterialFileFolderPath, model.Image);
                    }

                    return new ResponseUnit<DetailMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<DetailMaterialDTO>() { message = "데이터가 존재하지 않습니다.", data = new DetailMaterialDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseUnit<bool?>> UpdateMaterialService(HttpContext context, UpdateMaterialDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                MaterialTb? model = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), dto.Id!.Value);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Code = dto.Code!; // 품목코드
                model.Name = dto.Name!; // 품목명
                model.Unit = dto.Unit; // 단위
                model.Standard = dto.Standard; // 규격
                model.ManufacturingComp = dto.ManufacturingComp; // 제조사
                model.SafeNum = dto.SafeNum; // 안전재고수량
                model.DefaultLocation = dto.RoomID!.Value; // 공간위치
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;

                MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                    
                if(files is not null)
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                    {
                        deleteFileName = model.Image; // 삭제할 이름을 넣는다.
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                    else // DB엔 없는 경우
                    {
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                }
                else // 파일이 공백인 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                    {
                        deleteFileName = model.Image; // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image = null; // 모델의 파일명을 비운다.
                    }
                }

                bool? updateMaterial = await MaterialInfoRepository.UpdateMaterialInfo(model);
                if(updateMaterial == true)
                {
                    if(files is not null) // 파일이 공백이 아닌경우
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            // 파일넣기
                            bool? AddFile = await FileService.AddResizeImageFile(NewFileName, MaterialFileFolderPath, files);
                        }
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 파일 삭제
                            bool DeleteFile = FileService.DeleteImageFile(MaterialFileFolderPath, deleteFileName);
                        }
                    } // 파일이 공백인경우
                    else
                    {
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 삭제할거
                            bool DeleteFile = FileService.DeleteImageFile(MaterialFileFolderPath, deleteFileName);
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 품목 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteMaterialService(HttpContext context, List<int> delIdx)
        {
            try
            {
                if (context is null || delIdx is null || !delIdx.Any())
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                
                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                foreach(int index in delIdx)
                {
                    List<InventoryTb>? Inventory = await InventoryInfoRepository.GetPlaceMaterialInventoryList(Convert.ToInt32(placeid), index);
                    
                    if (Inventory is [_, ..])
                    {
                        List<InventoryTb>? CheckModel = Inventory.Where(m => m.Num > 0).ToList();
                        if (CheckModel is [_, ..])
                        {
                            // 조건걸림
                            return new ResponseUnit<bool?>() { message = "남아있는 재고가 있는 품목이 있어 삭제가 불가능합니다.", data = null, code = 200 };
                        }
                    }
                }

                //  품목삭제 하면됨
                bool? DeleteResult = await MaterialInfoRepository.DeleteMaterialInfo(delIdx, creater);
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
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 자재정보 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> ImportMaterialService(HttpContext context, IFormFile files)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<ExcelMaterialInfo> materiallist = new List<ExcelMaterialInfo>();

                using (var stream = new MemoryStream())
                {
                    await files!.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        int total = worksheet.LastRowUsed().RowNumber(); // Row 개수 반환

                        for (int i = 2; i <= total; i++)
                        {
                            var Data = new ExcelMaterialInfo();

                            Data.Code = Convert.ToString(worksheet.Cell("A" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Code))
                                return new ResponseUnit<string?>() { message = "자재코드는 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.Name = Convert.ToString(worksheet.Cell("B" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Name))
                                return new ResponseUnit<string?>() { message = "자재명은 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.Unit = Convert.ToString(worksheet.Cell("C" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Unit))
                                return new ResponseUnit<string?>() { message = "단위는 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.Standard = Convert.ToString(worksheet.Cell("D" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Standard))
                                return new ResponseUnit<string?>() { message = "규격은 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.MFC = Convert.ToString(worksheet.Cell("E" + i).GetValue<string>().Trim());
                            if (string.IsNullOrWhiteSpace(Data.MFC))
                                return new ResponseUnit<string?>() { message = "제조사는 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.SafeNum = Convert.ToString(worksheet.Cell("F" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.SafeNum))
                                return new ResponseUnit<string?>() { message = "안전재고 수량은 공백을 입력할 수 없습니다.", data = null, code = 200 };

                            Data.Location = Convert.ToString(worksheet.Cell("G" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Location))
                                return new ResponseUnit<string?>() { message = "위치는 공백을 입력할 수 없습니다.", data = null, code = 200 };


                            materiallist.Add(Data);
                        }

                        if (materiallist is not [_, ..])
                            return new ResponseUnit<string?>() { message = "등록할 자재 정보가 없습니다.", data = null, code = 200 };

                        // 사업장 -빌딩
                        // 빌딩- 층
                        // 층 - 룸 List
                        List<BuildingTb>? BuildingList = await BuildingInfoRepository.GetAllBuildingList(Convert.ToInt32(placeidx));
                        if (BuildingList is not [_, ..])
                            return new ResponseUnit<string?>() { message = "건물정보가 존재하지 않습니다.", data = null, code = 200 };


                        List<FloorTb>? FloorList = await FloorInfoRepository.GetFloorList(BuildingList);
                        if (FloorList is not [_, ..])
                            return new ResponseUnit<string?>() { message = "층정보가 존재하지 않습니다.", data = null, code = 200 };

                        List<RoomTb>? RoomList = await RoomInfoRepository.GetFloorRoomList(FloorList);
                        if (RoomList is not [_, ..])
                            return new ResponseUnit<string?>() { message = "공간 정보가 존재하지 않습니다.", data = null, code = 200 };

                        // 공간 이름만 뽑아온다.
                        List<string> DBRoomName = RoomList.Select(m => m.Name).ToList();
                        List<string> ExcelRoomName = materiallist.Select(m => m.Location!).ToList();
                        ExcelRoomName = ExcelRoomName.Distinct().ToList(); // 이름 중복제거

                        List<string> NotContainName = ExcelRoomName.Except(DBRoomName).ToList();
                        if (NotContainName is [_, ..])
                            return new ResponseUnit<string?>() { message = "올바르지 않는 공간 명칭이 입력되었습니다.", data = null, code = 200 };


                        List<MaterialTb> model = (from ExcelList in materiallist
                                                  join Room in RoomList
                                                  on ExcelList.Location equals Room.Name
                                                  select new MaterialTb
                                                  {
                                                      Code = ExcelList.Code!,
                                                      Name = ExcelList.Name!,
                                                      Unit = ExcelList.Unit,
                                                      Standard = ExcelList.Standard,
                                                      ManufacturingComp = ExcelList.MFC,
                                                      SafeNum = Convert.ToInt32(ExcelList.SafeNum),
                                                      CreateDt = DateTime.Now,
                                                      CreateUser = creater,
                                                      UpdateDt = DateTime.Now,
                                                      UpdateUser = creater,
                                                      DefaultLocation = Room.Id,
                                                      PlaceTbId = Convert.ToInt32(placeidx)
                                                  }).ToList();

                        bool? AddResult = await MaterialInfoRepository.AddMaterialList(model);
                        return AddResult switch
                        {
                            true => new ResponseUnit<string?>() { message = "요청이 정상 처리되었습니다.", data = model.Count.ToString(), code = 200 },
                            false => new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                            _ => new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                        };

                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
