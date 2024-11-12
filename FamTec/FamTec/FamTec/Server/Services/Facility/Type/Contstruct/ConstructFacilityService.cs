using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Model;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Floor;
using ClosedXML.Excel;
using FamTec.Shared.Server.DTO.Excel;

namespace FamTec.Server.Services.Facility.Type.Contstruct
{
    public class ConstructFacilityService : IConstructFacilityService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<ConstructFacilityService> CreateBuilderLogger;

        private DirectoryInfo? di;
        private string? ConstructFileFolderPath;


        public ConstructFacilityService(
           IBuildingInfoRepository _buildinginforepository,
           IFloorInfoRepository _floorinforepository,
           IFacilityInfoRepository _facilityinforepository,
           IRoomInfoRepository _roominforepository,
           IFileService _fileservice,
           ILogService _logService,
           ConsoleLogService<ConstructFacilityService> _createbuilderlogger)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;
            this.RoomInfoRepository = _roominforepository;
            this.FacilityInfoRepository = _facilityinforepository;
            
            this.FileService = _fileservice;
            this.LogService = _logService;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        private IXLWorksheet CreateCell(IXLWorksheet sheet, List<string> title, List<RoomTb> RoomList)
        {
            try
            {
                IXLRange mergerange = sheet.Range(sheet.Cell("A1"), sheet.Cell("B1"));
                mergerange.Merge(); // Merge() 에서 범위의 셀의 결합

                mergerange.Value = "공간정보";
                mergerange.Style.Font.FontName = "맑은 고딕";
                mergerange.Style.Font.Bold = true;
                mergerange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mergerange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                mergerange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                mergerange.Style.Border.OutsideBorderColor = XLColor.Black;
                mergerange.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);

                // 타이틀 작업
                for (int i = 0; i < title.Count; i++)
                {
                    sheet.Cell(2, i + 1).Value = title[i].ToString();
                    sheet.Column(i + 1).Width = 25; // 너비
                    sheet.Row(2).Height = 15; // 높이

                    IXLCell cell = sheet.Cell(2, i + 1); // A1, A2, A3 .. 셀선택
                    cell.Style.Font.FontName = "맑은 고딕"; // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                }

                // 내용작업
                for (int i = 0; i < RoomList.Count; i++)
                {
                    sheet.Cell(i + 3, 1).Value = RoomList[i].Id;
                    IXLCell cell = sheet.Cell(i + 3, 1);
                    cell.Style.Font.FontName = "맑은 고딕"; // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬

                    sheet.Cell(i + 3, 2).Value = RoomList[i].Name ?? null;
                    cell = sheet.Cell(i + 3, 2);
                    cell.Style.Font.FontName = "맑은 고딕"; // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Horizontal 중앙정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬
                }
                return sheet;

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        private IXLWorksheet CreateCell_2(IXLWorksheet sheet, List<string> title)
        {
            try
            {
                IXLRange mergerange = sheet.Range(sheet.Cell("A1"), sheet.Cell("H1"));
                mergerange.Merge(); // Merge

                mergerange.Value = "설비정보";
                mergerange.Style.Font.FontName = "맑은 고딕";
                mergerange.Style.Font.Bold = true;
                mergerange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mergerange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                mergerange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                mergerange.Style.Border.OutsideBorderColor = XLColor.Black;
                mergerange.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);

                // 타이틀 작업
                for (int i = 0; i < title.Count; i++)
                {
                    sheet.Cell(2, i + 1).Value = title[i].ToString();
                    sheet.Column(i + 1).Width = 25; // 너비
                    sheet.Row(2).Height = 15; // 높이

                    IXLCell cell = sheet.Cell(2, i + 1); // A1, A2, A3 .. 셀선택
                    cell.Style.Font.FontName = "맑은 고딕";  // 셀의 문자의 글꼴을 설정
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // 중앙정렬
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // 중앙정렬
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                }

                return sheet;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 건축설비 엑셀양식 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadConstructFacilityForm(HttpContext context)
        {
            try
            {
                string? PlaceId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceId))
                    return null;

                List<RoomTb>? RoomList = await RoomInfoRepository.GetPlaceAllRoomList(Convert.ToInt32(PlaceId));
                if (RoomList is null || !RoomList.Any())
                    return null;

                var workbook = new XLWorkbook();

                // 첫 번째 시트 생성
                var worksheet1 = workbook.Worksheets.Add("공간정보");
                List<string> title1 = new List<string>
                {
                    "아이디",
                    "공간명칭"
                };
                worksheet1 = CreateCell(worksheet1, title1, RoomList);
                // 여기서 첫번째 시트는 만들어졌음.

                // 두번째 시트 생성
                var worksheet2 = workbook.Worksheets.Add("건축설비정보");
                List<string> title2 = new List<string>
                {
                    "*공간아이디",
                    "*설비이름",
                    "형식",
                    "규격용량",
                    "수량",
                    "내용년수",
                    "설치년월",
                    "교체년월"
                };
                worksheet2 = CreateCell_2(worksheet2, title2);

                using MemoryStream xlsStream = new();
                workbook.SaveAs(xlsStream, false);
                return xlsStream.ToArray();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 건축설비 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<bool>> ImportConstructFacilityService(HttpContext context, IFormFile? file)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                List<RoomTb>? RoomList = await RoomInfoRepository.GetPlaceAllRoomList(Convert.ToInt32(placeidx));
                if (RoomList is null || !RoomList.Any())
                    return new ResponseUnit<bool>() { message = "공간정보가 존재하지 않습니다.", data = false, code = 204 };

                List<ExcelFacilityInfo> Facilitylist = new List<ExcelFacilityInfo>();

                using(var stream = new MemoryStream())
                {
                    await file!.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        // 두번째 시트 읽음
                        var worksheet = workbook.Worksheet(2);

                        if (worksheet.Cell("A2").GetValue<string>().Trim() != "*공간아이디")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("B2").GetValue<string>().Trim() != "*설비이름")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("C2").GetValue<string>().Trim() != "형식")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("D2").GetValue<string>().Trim() != "규격용량")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("E2").GetValue<string>().Trim() != "수량")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("F2").GetValue<string>().Trim() != "내용년수")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("G2").GetValue<string>().Trim() != "설치년월")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("H2").GetValue<string>().Trim() != "교체년월")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };

                        int total = worksheet.LastRowUsed().RowNumber(); // Row 개수 반환

                        for (int i = 3; i <= total; i++)
                        {
                            var Data = new ExcelFacilityInfo();

                            // 공간인덱스
                            string? DataTypeCheck = worksheet.Cell("A" + i).GetValue<string>().Trim();
                            if (String.IsNullOrWhiteSpace(DataTypeCheck))
                                return new ResponseUnit<bool>() { message = "설비의 공간인덱스가 유효하지 않습니다.", data = false, code = 204 };

                            Data.RoomId = int.TryParse(DataTypeCheck, out int parsedValue) ? parsedValue : (int?)null;
                            if(Data.RoomId is null)
                                return new ResponseUnit<bool>() { message = "데이터의 형식이 올바르지 않습니다.", data = false, code = 204 };

                            // 설비이름
                            Data.Name = Convert.ToString(worksheet.Cell("B" + i).GetValue<string>().Trim());
                            if (String.IsNullOrWhiteSpace(Data.Name))
                            {
                                return new ResponseUnit<bool>() { message = "설비의 이름은 공백이 될 수 없습니다.", data = false, code = 204 };
                            }

                            // 형식
                            Data.Type = Convert.ToString(worksheet.Cell("C" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Type))
                            {
                                Data.Type = null;
                            }

                            // 규격용량
                            Data.StandardCapacity = Convert.ToString(worksheet.Cell("D" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.StandardCapacity))
                            {
                                Data.StandardCapacity = null;
                            }

                            // 수량
                            DataTypeCheck = worksheet.Cell("E" + i).GetValue<string>().Trim();
                            Data.Num = int.TryParse(DataTypeCheck, out int ParsedValue) ? ParsedValue : (int?)null;

                            // 내용연수
                            Data.LifeSpan = Convert.ToString(worksheet.Cell("F" + i).GetValue<string>().Trim());
                            if(Data.LifeSpan is null)
                            {
                                Data.LifeSpan = null;
                            }

                            // 설치년월
                            string? DateCheck = Convert.ToString(worksheet.Cell("G" + i).GetValue<string>().Trim());
                            if (!String.IsNullOrWhiteSpace(DateCheck))
                            {
                                DateTime? ConvertDate = DateTime.TryParse(DateCheck, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
                                if (ConvertDate is null)
                                {
                                    return new ResponseUnit<bool>() { message = "yyyy-MM-dd 타입의 날짜만 입력가능합니다.", data = false, code = 204 };
                                }
                                else
                                {
                                    Data.EquipDT = ConvertDate;
                                }
                            }
                            else
                            {
                                Data.EquipDT = null;
                            }

                            // 교체년월
                            DateCheck = Convert.ToString(worksheet.Cell("H" + i).GetValue<string>().Trim());

                            if (!String.IsNullOrWhiteSpace(DateCheck))
                            {
                                DateTime? ConvertDate = DateTime.TryParse(DateCheck, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
                                if (ConvertDate is null)
                                {
                                    return new ResponseUnit<bool>() { message = "yyyy-MM-dd 타입의 날짜만 입력가능합니다.", data = false, code = 204 };
                                }
                                else
                                {
                                    Data.ChangeDT = ConvertDate;
                                }
                            }
                            else
                            {
                                Data.ChangeDT = null;
                            }

                            Facilitylist.Add(Data);
                        }

                        List<FacilityTb> model = Facilitylist.Select(m => new FacilityTb
                        {
                            Category = "건축",
                            Name = m.Name!, // 설비명칭
                            Type = m.Type, // 형식
                            Num = m.Num,
                            EquipDt = m.EquipDT,
                            Lifespan = m.LifeSpan,
                            StandardCapacity = m.StandardCapacity,
                            ChangeDt = m.ChangeDT,
                            CreateDt = ThisDate,
                            CreateUser = creater,
                            UpdateDt = ThisDate,
                            UpdateUser = creater,
                            RoomTbId = m.RoomId!.Value
                        }).ToList();

                        bool AddResult = await FacilityInfoRepository.AddFacilityList(model).ConfigureAwait(false);
                        return AddResult switch
                        {
                            true => new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                            false => new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 }
                        };

                    }
                }


            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 건축설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<FacilityDTO>> AddConstructFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (string.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                DateTime ThisTime = DateTime.Now;

                RoomTb? RoomInfo = await RoomInfoRepository.GetRoomInfo(dto.RoomId!.Value).ConfigureAwait(false);
                if (RoomInfo is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creator) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                // 전기설비 관련한 폴더 없으면 만들기 
                //ConstructFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Construct", Common.FileServer, placeidx);
                ConstructFileFolderPath = Path.Combine(Common.FileServer, placeidx.ToString(), "Facility", "Construct");

                di = new DirectoryInfo(ConstructFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = new FacilityTb()
                {
                    Category = "건축", //  카테고리 - 건축
                    Name = dto.Name!, // 설비명칭
                    Type = dto.Type, // 형식
                    Num = dto.Num, // 개수
                    Unit = dto.Unit, // 단위
                    EquipDt = dto.EquipDT, // 설치년월
                    Lifespan = dto.LifeSpan, // 내용연수
                    StandardCapacity = dto.Standard_capacity, // 규격용량
                    ChangeDt = dto.ChangeDT, // 교체년월
                    CreateDt = ThisTime,
                    CreateUser = creator,
                    UpdateDt = ThisTime,
                    UpdateUser = creator,
                    RoomTbId = dto.RoomId.Value, // 공간 ID
                    Image = NewFileName
                };

                FacilityTb? result = await FacilityInfoRepository.AddAsync(model).ConfigureAwait(false);
                if (result is not null)
                {
                    if(files is not null)
                    {
                        // 파일넣기
                        bool? AddFile = await FileService.AddResizeImageFile(NewFileName, ConstructFileFolderPath, files).ConfigureAwait(false);
                    }
                    
                    return new ResponseUnit<FacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = new FacilityDTO()
                    {
                        ID = result.Id,
                        Category = result.Category,
                        Num = result.Num,
                        Name = result.Name,
                        LifeSpan = result.Lifespan,
                        ChangeDT = result.ChangeDt,
                        EquipDT = result.EquipDt,
                        Type = result.Type,
                        Unit = result.Unit,
                        Standard_capacity = result.StandardCapacity,
                        RoomId = result.RoomTbId
                    }, code = 200 };
                }
                else
                    return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 등록되어있는 건축설비 List 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<FacilityListDTO>> GetConstructFacilityListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<FacilityListDTO>? model = await FacilityInfoRepository.GetPlaceConstructFacilityList(Convert.ToInt32(placeidx)).ConfigureAwait(false);

                if (model is [_, ..])
                {
                    return new ResponseList<FacilityListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<FacilityListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FacilityListDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<FacilityListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<FacilityDetailDTO>> GetConstructDetailFacilityService(HttpContext context, int facilityId, bool isMobile)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(facilityId).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                RoomTb? room = await RoomInfoRepository.GetRoomInfo(model.RoomTbId).ConfigureAwait(false);
                if(room is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                FloorTb? FloorTB = await FloorInfoRepository.GetFloorInfo(room.FloorTbId);
                if (FloorTB is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(FloorTB.BuildingTbId);
                if (BuildingTB is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                FacilityDetailDTO dto = new FacilityDetailDTO();
                dto.Id = model.Id;
                dto.Category = model.Category;
                dto.Name = model.Name;
                dto.Type = model.Type;
                dto.Num = model.Num;
                dto.Unit = model.Unit;
                dto.EquipDT = model.EquipDt;
                dto.LifeSpan = model.Lifespan;
                dto.Standard_capacity = model.StandardCapacity;
                dto.ChangeDT = model.ChangeDt;
                dto.RoomId = model.RoomTbId;
                dto.RoomName = room.Name;
                dto.BuildingId = BuildingTB.Id;
                dto.BuildingName = BuildingTB.Name;

                ConstructFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Facility", "Construct");

                di = new DirectoryInfo(ConstructFileFolderPath);
                if (!di.Exists) di.Create();

                if(isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                    if(!String.IsNullOrWhiteSpace(model.Image))
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(ConstructFileFolderPath, model.Image).ConfigureAwait(false);

                        if(ImageBytes is not null)
                        {
                            IFormFile? files = FileService.ConvertFormFiles(ImageBytes, model.Image);
                            if(files is not null)
                            {
                                byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                if(ConvertFile is not null)
                                {
                                    dto.ImageName = model.Image;
                                    dto.Image = ConvertFile;
                                }
                                else
                                {
                                    dto.ImageName = null;
                                    dto.Image = null;
                                }
                            }
                            else
                            {
                                dto.ImageName = null;
                                dto.Image = null;
                            }
                        }
                        else
                        {
                            dto.ImageName = null;
                            dto.Image = null;
                        }
                    }
                    else
                    {
                        dto.ImageName = null;
                        dto.Image = null;
                    }

                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    if (!String.IsNullOrWhiteSpace(model.Image))
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(ConstructFileFolderPath, model.Image).ConfigureAwait(false);

                        if (ImageBytes is not null)
                        {
                            IFormFile? files = FileService.ConvertFormFiles(ImageBytes, model.Image);
                            if (files is not null)
                            {
                                byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

                                if (ConvertFile is not null)
                                {
                                    dto.ImageName = model.Image;
                                    dto.Image = ConvertFile;
                                }
                                else
                                {
                                    dto.ImageName = null;
                                    dto.Image = null;
                                }
                            }
                            else
                            {
                                dto.ImageName = null;
                                dto.Image = null;
                            }
                        }
                        else
                        {
                            dto.ImageName = null;
                            dto.Image = null;
                        }
                    }
                    else
                    {
                        dto.ImageName = null;
                        dto.Image = null;
                    }

                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<FacilityDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> UpdateConstructFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string? NewFileName = String.Empty;
                string? deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;

                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 이미지 변경 or 삭제
                //ConstructFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Construct", Common.FileServer, placeid);
                ConstructFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Facility", "Construct");

                di = new DirectoryInfo(ConstructFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(dto.ID!.Value).ConfigureAwait(false);
                if(model is null || model.Category.Trim() != "건축")
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Category = "건축"; // 카테고리
                model.Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!; // 설비명칭
                model.Type = dto.Type; // 타입
                model.Num = dto.Num; // 개수
                model.Unit = dto.Unit; // 단위
                model.EquipDt = dto.EquipDT; // 설치년월
                model.Lifespan = dto.LifeSpan; // 내용연수
                model.StandardCapacity = dto.Standard_capacity;
                model.ChangeDt = dto.ChangeDT;
                model.UpdateDt = ThisTime;
                model.UpdateUser = creater;
                model.RoomTbId = dto.RoomId!.Value;

                if(files is not null) // 파일이 공백이 아닌 경우
                {
                    if(files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다르면
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
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
                    ImageBytes = await FileService.GetImageFile(ConstructFileFolderPath, deleteFileName).ConfigureAwait(false);
                }
                 
                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if(ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(ConstructFileFolderPath, deleteFileName);
                }

                // 새 파일 저장
                if(files is not null)
                {
                    if(String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, ConstructFileFolderPath, files).ConfigureAwait(false);
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? updateBuilding = await FacilityInfoRepository.UpdateFacilityInfo(model).ConfigureAwait(false);
                if (updateBuilding == true)
                {
                    // 성공했으면 그걸로 끝
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을  원래대로 돌려놔야함.
                    if(AddTemp is not null)
                    {
                        try
                        {
                            if(FileService.IsFileExists(ConstructFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, ConstructFileFolderPath, files).ConfigureAwait(false);
                            }
                        }
                        catch(Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    if(!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(ConstructFileFolderPath, RemoveTemp);
                        }
                        catch(Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        public async Task<ResponseUnit<bool?>> DeleteConstructFacilityService(HttpContext context, List<int> delIdx)
        {
            try
            {
                if (context is null || delIdx is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 삭제가능여부 체크
                foreach (int id in delIdx)
                {
                    bool? DelCheck = await FacilityInfoRepository.DelFacilityCheck(id).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "참조하고있는 하위 정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                bool? DeleteResult = await FacilityInfoRepository.DeleteFacilityInfo(delIdx, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


    }
}
