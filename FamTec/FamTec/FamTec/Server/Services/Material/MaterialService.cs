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

        private readonly IFileService FileService;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<MaterialService> CreateBuilderLogger;

        private DirectoryInfo? di;
        private string? MaterialFileFolderPath;

        public MaterialService(IMaterialInfoRepository _materialinforepository,
            IInventoryInfoRepository _inventoryinforepository,
            IRoomInfoRepository _roominforepository,
            IBuildingInfoRepository _buildinginforepository,
            IFloorInfoRepository _floorinforepository,
            IFileService _fileservice,
            ILogService _logservice,
            ConsoleLogService<MaterialService> _createbuilderlogger)
        {
            this.MaterialInfoRepository = _materialinforepository;
            this.InventoryInfoRepository = _inventoryinforepository;
            this.RoomInfoRepository = _roominforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.FloorInfoRepository = _floorinforepository;

            this.FileService = _fileservice;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        private IXLWorksheet CreateCell(IXLWorksheet sheet, List<string> title, List<RoomTb> RoomList)
        {
            try
            {
                IXLRange mergerange = sheet.Range(sheet.Cell("A1"), sheet.Cell("B1"));
                mergerange.Merge(); // Merge() 에서 범위의 셀의 결합

                mergerange.Value = "위치정보";
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
            catch (Exception ex)
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
                IXLRange mergerange = sheet.Range(sheet.Cell("A1"), sheet.Cell("G1"));
                mergerange.Merge(); // Merge

                mergerange.Value = "품목정보";
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
        /// 품목 엑셀양식 다운로드
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<byte[]?> DownloadMaterialForm(HttpContext context)
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

                // 첫번째 시트작성
                var worksheet1 = workbook.Worksheets.Add("위치정보");
                List<string> title1 = new List<string>
                {
                    "번호",
                    "위치명칭"
                };
                worksheet1 = CreateCell(worksheet1, title1, RoomList);

                // 두 번째 시트 생성
                var worksheet2 = workbook.Worksheets.Add("품목정보");
                List<string> title2 = new List<string>
                {
                    "*위치번호",
                    "*품목코드",
                    "*품목명",
                    "제조사",
                    "규격",
                    "안전재고",
                    "단위"
                };
                worksheet2 = CreateCell_2(worksheet2, title2);

                using MemoryStream xlsStream = new();
                workbook.SaveAs(xlsStream, false);
                return xlsStream.ToArray();
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
        /// 품목 엑셀 IMPORT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> ImportMaterialService(HttpContext context, IFormFile? file)
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
                    return new ResponseUnit<bool>() { message = "위치정보가 존재하지 않습니다.", data = false, code = 204 };

                List<ExcelMaterialInfo> Materiallist = new List<ExcelMaterialInfo>();

                using (var stream = new MemoryStream())
                {
                    await file!.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        // 두번째 시트 읽음
                        var worksheet = workbook.Worksheet(2);

                        if (worksheet.Cell("A2").GetValue<string>().Trim() != "*위치번호")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("B2").GetValue<string>().Trim() != "*품목코드")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("C2").GetValue<string>().Trim() != "*품목명")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("D2").GetValue<string>().Trim() != "제조사")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("E2").GetValue<string>().Trim() != "규격")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("F2").GetValue<string>().Trim() != "안전재고")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };
                        if (worksheet.Cell("G2").GetValue<string>().Trim() != "단위")
                            return new ResponseUnit<bool>() { message = "잘못된 양식입니다.", data = false, code = 204 };

                        int total = worksheet.LastRowUsed().RowNumber(); // Row 개수 반환

                        for (int i = 3; i <= total; i++)
                        {
                            var Data = new ExcelMaterialInfo();

                            // 공간인덱스
                            string? DataTypeCheck = worksheet.Cell("A" + i).GetValue<string>().Trim();
                            if (String.IsNullOrWhiteSpace(DataTypeCheck))
                                return new ResponseUnit<bool>() { message = "품목의 위치번호가 유효하지 않습니다.", data = false, code = 204 };

                            Data.RoomId = int.TryParse(DataTypeCheck, out int parsedValue) ? parsedValue : (int?)null;
                            if (Data.RoomId is null)
                                return new ResponseUnit<bool>() { message = "데이터의 형식이 올바르지 않습니다.", data = false, code = 204 };

                            // 품목코드
                            Data.Code = Convert.ToString(worksheet.Cell("B" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Code))
                            {
                                return new ResponseUnit<bool>() { message = "품목의 코드는 공백이 될 수 없습니다.", data = false, code = 204 };
                            }

                            // 품목명
                            Data.Name = Convert.ToString(worksheet.Cell("C" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Name))
                            {
                                return new ResponseUnit<bool>() { message = "품목의 이름은 공백이 될 수 없습니다.", data = false, code = 204 };
                            }

                            // 제조사
                            Data.MFC = Convert.ToString(worksheet.Cell("D" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.MFC))
                            {
                                Data.MFC = null;
                            }

                            // 규격
                            Data.Standard = Convert.ToString(worksheet.Cell("E" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Standard))
                            {
                                Data.Standard = null;
                            }

                            // 안전재고 수량
                            DataTypeCheck = worksheet.Cell("F" + i).GetValue<string>().Trim();
                            if(!String.IsNullOrWhiteSpace(DataTypeCheck))
                            {
                                Data.SafeNum = int.TryParse(DataTypeCheck, out int parsedValue1) ? parsedValue1 : (int?)null;
                                if (Data.SafeNum is null)
                                    return new ResponseUnit<bool>() { message = "데이터의 형식이 올바르지 않습니다.", data = false, code = 204 };
                            }
                            Data.SafeNum = null;

                            // 단위
                            Data.Unit = Convert.ToString(worksheet.Cell("G" + i).GetValue<string>().Trim());
                            if(String.IsNullOrWhiteSpace(Data.Unit))
                            {
                                Data.Unit = null;
                            }

                            Materiallist.Add(Data);
                        }

                        bool hasDuplicateNames = Materiallist.GroupBy(m => m.Name).Any(g => g.Count() > 1);
                        if(hasDuplicateNames)
                        {
                            return new ResponseUnit<bool>() { message = "중복된 품목명이 존재합니다.", data = false, code = 204 };
                        }

                        List<MaterialTb> model = Materiallist.Select(m => new MaterialTb
                        {
                            Code = m.Code!,
                            Name = m.Name!,
                            Unit = m.Unit,
                            Standard = m.Standard,
                            ManufacturingComp = m.MFC,
                            SafeNum = m.SafeNum,
                            CreateDt = ThisDate,
                            CreateUser = creater,
                            UpdateDt = ThisDate,
                            UpdateUser = creater,
                            RoomTbId = m.RoomId!.Value,
                            PlaceTbId = Convert.ToInt32(placeidx)
                        }).ToList();

                        bool? AddResult = await MaterialInfoRepository.AddMaterialList(model).ConfigureAwait(false);
                        return AddResult switch
                        {
                            true => new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                            false => new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                            _ => new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 }
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
        /// 자재추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddMaterialDTO>> AddMaterialService(HttpContext context, AddMaterialDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                string? Creater = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                DateTime ThisDate = DateTime.Now;

                if (String.IsNullOrWhiteSpace(placeidx) || String.IsNullOrWhiteSpace(Creater) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                // 중복코드 검사
                bool? MaterialCheck = await MaterialInfoRepository.GetPlaceMaterialCheck(Int32.Parse(placeidx), dto.Code).ConfigureAwait(false);
                if(MaterialCheck != true)
                    return new ResponseUnit<AddMaterialDTO>() { message = "이미 존재하는 코드입니다.", data = null, code = 201 };

                //MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeidx);
                MaterialFileFolderPath = Path.Combine(Common.FileServer, placeidx.ToString(), "Material");

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
                    CreateDt = ThisDate,
                    CreateUser = Creater,
                    UpdateDt = ThisDate,
                    UpdateUser = Creater,
                    Image = NewFileName,
                    RoomTbId = dto.RoomID!.Value, // 공간위치 인덱스
                    PlaceTbId = Int32.Parse(placeidx) // 사업장ID
                };

                MaterialTb? model = await MaterialInfoRepository.AddAsync(matertialtb).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 처리되지 않았습니다.", data = new AddMaterialDTO(), code = 404 };

               
                if(files is not null)
                {
                    // 파일 넣기
                    await FileService.AddResizeImageFile(NewFileName, MaterialFileFolderPath, files).ConfigureAwait(false);
                }

                return new ResponseUnit<AddMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddMaterialDTO()
                {
                    Code = model.Code, // 품목코드
                    Name = model.Name, // 자재명
                    Unit = model.Unit, // 단위
                    Standard = model.Standard, // 규격
                    ManufacturingComp = model.ManufacturingComp, // 제조사
                    SafeNum = model.SafeNum, // 안전재고수량
                    RoomID = model.RoomTbId, // 기본위치
                }, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<MaterialListDTO>> GetPlaceMaterialListService(HttpContext context, bool isMobile)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                //MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                MaterialFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Material");


                List<MaterialTb>? model = await MaterialInfoRepository.GetPlaceAllMaterialList(Int32.Parse(placeid)).ConfigureAwait(false);
                if(model is null || !model.Any())
                    return new ResponseList<MaterialListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MaterialListDTO>(), code = 200 };
                
                if(isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                    List<MaterialListDTO> ListDTO = new List<MaterialListDTO>();

                    foreach (MaterialTb MaterialTB in model)
                    {
                        MaterialListDTO DTO = new MaterialListDTO();
                        DTO.ID = MaterialTB.Id; // 품목 인덱스
                        DTO.Code = MaterialTB.Code; // 품목 코드
                        DTO.Name = MaterialTB.Name; // 품목명
                        DTO.Unit = MaterialTB.Unit; // 단위
                        DTO.Standard = MaterialTB.Standard; // 규격
                        DTO.ManufacturingComp = MaterialTB.ManufacturingComp; // 제조사
                        DTO.SafeNum = MaterialTB.SafeNum; // 안전재고수량

                        if (!String.IsNullOrWhiteSpace(MaterialTB.Image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(MaterialFileFolderPath, MaterialTB.Image).ConfigureAwait(false);
                            if (ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, MaterialTB.Image);
                                if (files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                    if (ConvertFile is not null)
                                    {
                                        DTO.Image = ConvertFile;
                                    }
                                    else
                                    {
                                        DTO.Image = null;
                                    }
                                }
                                else
                                {
                                    DTO.Image = null;
                                }
                            }
                            else
                            {
                                DTO.Image = null;
                            }
                        }
                        else
                        {
                            DTO.Image = null;
                        }

                        ListDTO.Add(DTO);
                    }
                    return new ResponseList<MaterialListDTO>() { message = "요청이 정상 처리되었습니다.", data = ListDTO, code = 200 };
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    List<MaterialListDTO> ListDTO = new List<MaterialListDTO>();
                    
                    foreach (MaterialTb MaterialTB in model)
                    {
                        MaterialListDTO DTO = new MaterialListDTO();
                        DTO.ID = MaterialTB.Id; // 품목 인덱스
                        DTO.Code = MaterialTB.Code; // 품목 코드
                        DTO.Name = MaterialTB.Name; // 품목명
                        DTO.Unit = MaterialTB.Unit; // 단위
                        DTO.Standard = MaterialTB.Standard; // 규격
                        DTO.ManufacturingComp = MaterialTB.ManufacturingComp; // 제조사
                        DTO.SafeNum = MaterialTB.SafeNum; // 안전재고수량

                        if (!String.IsNullOrWhiteSpace(MaterialTB.Image))
                        {
                            byte[]? ImageBytes = await FileService.GetImageFile(MaterialFileFolderPath, MaterialTB.Image).ConfigureAwait(false);
                            if(ImageBytes is not null)
                            {
                                IFormFile? files = FileService.ConvertFormFiles(ImageBytes, MaterialTB.Image);
                                if(files is not null)
                                {
                                    byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);

                                    if(ConvertFile is not null)
                                    {
                                        DTO.Image = ConvertFile;
                                    }
                                    else
                                    {
                                        DTO.Image = null;
                                    }
                                }
                                else
                                {
                                    DTO.Image = null;
                                }
                            }
                            else
                            {
                                DTO.Image = null;
                            }
                        }
                        else
                        {
                            DTO.Image = null;
                        }

                        ListDTO.Add(DTO);
                    }
                    return new ResponseList<MaterialListDTO>() { message = "요청이 정상 처리되었습니다.", data = ListDTO, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<MaterialListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<MaterialListDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력 - Search용
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseList<MaterialSearchListDTO>> GetAllPlaecMaterialSearchService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialSearchListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialSearchListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaterialTb>? model = await MaterialInfoRepository.GetPlaceAllMaterialList(Int32.Parse(placeid)).ConfigureAwait(false);
                
                if (model is null || !model.Any())
                    return new ResponseList<MaterialSearchListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MaterialSearchListDTO>(), code = 200 };

                List<MaterialSearchListDTO> dto = model.Select(e => new MaterialSearchListDTO
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    Mfr = e.ManufacturingComp,
                    Standard = e.Standard
                }).ToList();
                return new ResponseList<MaterialSearchListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<MaterialSearchListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재리스트 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> GetPlaceMaterialCountService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int count = await MaterialInfoRepository.GetPlaceAllMaterialCount(Int32.Parse(placeid)).ConfigureAwait(false);

                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트 페이지네이션
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<ResponseList<MaterialListDTO>> GetPlaceMaterialPageNationListService(HttpContext context, int pagenumber, int pagesize)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                //MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                MaterialFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Material");

                List<MaterialTb>? model = await MaterialInfoRepository.GetPlaceAllMaterialPageNationList(Int32.Parse(placeid), pagenumber, pagesize).ConfigureAwait(false);
                if (model is null || !model.Any())
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
                        Image = !String.IsNullOrWhiteSpace(MaterialTB.Image) ? await FileService.GetImageFile(MaterialFileFolderPath, MaterialTB.Image).ConfigureAwait(false) : null
                    };

                    ListDTO.Add(DTO);
                }
                return new ResponseList<MaterialListDTO>() { message = "요청이 정상 처리되었습니다.", data = ListDTO, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<MaterialListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

            }
        }

        /// <summary>
        /// 자재 상세정보 보기
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<DetailMaterialDTO>> GetDetailMaterialService(HttpContext context, int materialid, bool isMobile)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                MaterialTb? materialTB = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), materialid).ConfigureAwait(false);
                if(materialTB is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                RoomTb? RoomTB = await RoomInfoRepository.GetRoomInfo(materialTB.RoomTbId).ConfigureAwait(false);
                if (RoomTB is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                FloorTb? FloorTB = await FloorInfoRepository.GetFloorInfo(RoomTB.FloorTbId).ConfigureAwait(false);
                if (FloorTB is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(FloorTB.BuildingTbId);
                if (BuildingTB is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                DetailMaterialDTO dto = new DetailMaterialDTO();
                dto.Id = materialTB.Id; // 품목 ID
                dto.Code = materialTB.Code; // 품목코드
                dto.Name = materialTB.Name; // 품목명
                dto.Unit = materialTB.Unit; // 품목단위
                dto.Standard = materialTB.Standard; // 규격
                dto.ManufacturingComp = materialTB.ManufacturingComp; // 제조사
                dto.SafeNum = materialTB.SafeNum; // 안전재고 수량
                dto.RoomID = materialTB.RoomTbId; // 기본위치
                dto.RoomName = RoomTB.Name;
                dto.BuildingID = BuildingTB.Id;
                dto.BuildingName = BuildingTB.Name;

                //MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                MaterialFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Material");

                di = new DirectoryInfo(MaterialFileFolderPath);
                if (!di.Exists) di.Create();

                if(isMobile)
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== 모바일 ====");
#endif
                    if (!String.IsNullOrWhiteSpace(materialTB.Image))
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(MaterialFileFolderPath, materialTB.Image).ConfigureAwait(false);

                        if (ImageBytes is not null)
                        {
                            IFormFile? files = FileService.ConvertFormFiles(ImageBytes, materialTB.Image);
                            if (files is not null)
                            {
                                byte[]? ConvertFile = await FileService.AddResizeImageFile_2(files);

                                if (ConvertFile is not null)
                                {
                                    dto.ImageName = materialTB.Name;
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
                }
                else
                {
#if DEBUG
                    CreateBuilderLogger.ConsoleText("==== PC ====");
#endif
                    if (!String.IsNullOrWhiteSpace(materialTB.Image))
                    {
                        byte[]? ImageBytes = await FileService.GetImageFile(MaterialFileFolderPath, materialTB.Image).ConfigureAwait(false);

                        if(ImageBytes is not null)
                        {
                            IFormFile? files = FileService.ConvertFormFiles(ImageBytes, materialTB.Image);
                            if(files is not null)
                            {
                                byte[]? ConvertFile = await FileService.AddResizeImageFile_3(files);
                                
                                if(ConvertFile is not null)
                                {
                                    dto.ImageName = materialTB.Name;
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
                }

                return new ResponseUnit<DetailMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<DetailMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseUnit<bool?>> UpdateMaterialService(HttpContext context, UpdateMaterialDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;

                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                //MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                MaterialFileFolderPath = Path.Combine(Common.FileServer, placeid.ToString(), "Material");

                di = new DirectoryInfo(MaterialFileFolderPath);
                if (!di.Exists) di.Create();


                MaterialTb? model = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), dto.Id!.Value).ConfigureAwait(false);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                //model.Code = dto.Code!; // 품목코드
                model.Name = dto.Name!; // 품목명
                model.Unit = dto.Unit; // 단위
                //model.Standard = dto.Standard; // 규격
                //model.ManufacturingComp = dto.ManufacturingComp; // 제조사
                model.SafeNum = dto.SafeNum; // 안전재고수량
                model.RoomTbId = dto.RoomID!.Value; // 공간
                model.UpdateDt = ThisDate;
                model.UpdateUser = creater;

                if(files is not null) // 파일이 공백이 아닌 경우
                {
                    if(files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다른 경우
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
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB의 이밎지가 공백이 아니면
                    {
                        deleteFileName = model.Image; // 기존 파일 삭제 목록에 추가
                        model.Image = null; // 모델의 파일명 비우기
                    }
                }

                // 먼저 파일 삭제 처리
                // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                byte[]? ImageBytes = null;
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    ImageBytes = await FileService.GetImageFile(MaterialFileFolderPath, deleteFileName).ConfigureAwait(false);
                }

                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if(ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if(!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(MaterialFileFolderPath, deleteFileName);
                }

                if(files is not null)
                {
                    if(String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, MaterialFileFolderPath, files).ConfigureAwait(false);
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? updateMaterial = await MaterialInfoRepository.UpdateMaterialInfo(model).ConfigureAwait(false);
                if(updateMaterial == true)
                {
                    // 성공했으면 그걸로 끝
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을 원래대로 돌려놔야함.
                    if (AddTemp is not null)
                    {
                        try
                        {
                            if(FileService.IsFileExists(MaterialFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, MaterialFileFolderPath, files).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(MaterialFileFolderPath, RemoveTemp);
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
#if DEBUG
                            CreateBuilderLogger.ConsoleLog(ex);
#endif
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 품목 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> DeleteMaterialService(HttpContext context, List<int> delIdx)
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
                    bool? delCheck = await MaterialInfoRepository.DelMaterialCheck(index);
                    if (delCheck == true)
                        return new ResponseUnit<bool?>() { message = "참조하고있는 하위 정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };


                    List<InventoryTb>? Inventory = await InventoryInfoRepository.GetPlaceMaterialInventoryList(Convert.ToInt32(placeid), index).ConfigureAwait(false);
                    
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
                bool? DeleteResult = await MaterialInfoRepository.DeleteMaterialInfo(delIdx, creater).ConfigureAwait(false);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

    
     

        /// <summary>
        /// 품목검색
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public async Task<ResponseList<MaterialSearchListDTO>> GetMaterialSearchService(HttpContext context, string searchData)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialSearchListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialSearchListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaterialSearchListDTO> model = await MaterialInfoRepository.GetMaterialSearchInfo(Int32.Parse(placeid), searchData).ConfigureAwait(false);
                if (model is not null)
                    return new ResponseList<MaterialSearchListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<MaterialSearchListDTO>() { message = "요청이 정상 처리되었습니다.", data = null, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<MaterialSearchListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

     
    }
}
