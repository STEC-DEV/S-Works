

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using FamTec.Shared.Client.DTO.Normal.Buildings;
using FamTec.Shared.Client.DTO.Normal.Buildings.Group;
using FamTec.Shared.Client.DTO.Normal.Facility;
using FamTec.Shared.Client.DTO.Normal.Facility.Group;
using FamTec.Shared.Client.DTO.Normal.Facility.Maintenance;
using FamTec.Shared.Client.DTO.Normal.Location.Inventory;
using FamTec.Shared.Client.DTO.Normal.Material.Detail;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Client.DTO.Normal.Voc;
using Microsoft.JSInterop;

namespace FamTec.Client.Pages.Normal.Voc.VocMain.utill
{
    public class ExcelService
    {
        private static ExcelService excel;
        private readonly IJSRuntime _jsRuntime;
        

        // IJSRuntime 의존성 주입을 위한 생성자
        private ExcelService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public static ExcelService getInstance(IJSRuntime jsRuntime)
        {
            if (excel == null)
            {
                excel = new ExcelService(jsRuntime);
            }

            return excel;
        }

        /// <summary>
        /// 민원
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="date"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportVocData(List<ListVocDTO> data, List<string> colName,string date,string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();
            
            
            var worksheet = workbook.Worksheets.Add(date + "민원");

            foreach(var header in colName.Select((value,idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx+1).Value = header.value;
            }


            foreach(var row in data.Select((value,idx) => (value,idx)))
            {
                string division = row.value.Division switch
                {
                    0 => "모바일",
                    1 => "수기입력",
                };
                string status = row.value.Status switch
                {
                    0 => "미처리",
                    1 => "처리중",
                    2 => "처리완료",
                };

                string type = row.value.Type switch
                {
                    0 => "미분류",
                    1 => "기계",
                    2 => "전기",
                    3 => "승강",
                    4 => "소방",
                    5 => "건축",
                    6 => "통신",
                    7 => "미화",
                    8 => "보안",
                };

                worksheet.Cell(row.idx + 2, 1).Value = division;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.BuildingName;
                worksheet.Cell(row.idx + 2, 3).Value = type;
                worksheet.Cell(row.idx + 2, 4).Value = row.value.CreateUser;
                worksheet.Cell(row.idx + 2, 5).Value = row.value.Phone;
                worksheet.Cell(row.idx + 2, 6).Value = row.value.Title;
                worksheet.Cell(row.idx + 2, 7).Value = row.value.CreateDT;
                worksheet.Cell(row.idx + 2, 8).Value = row.value.CompleteDT;
                worksheet.Cell(row.idx + 2, 9).Value = row.value.DurationDT;
                worksheet.Cell(row.idx + 2, 10).Value = status;

            }

            // 전체 데이터 범위 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);

            // 전체 범위에 테두리 적용
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            //헤더
            var range = worksheet.Range(1, 1, 1, colName.Count);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            


            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream",$"{title}.xlsx", Convert.ToBase64String(content));

        }

        public async Task ExportMaintenanceData(List<MaintanceHistoryDTO> data, List<string> colName, string date, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add(date + "유지보수 이력");

            foreach (var header in colName.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx + 1).Value = header.value;
            }


            foreach (var row in data.Select((value, idx) => (value, idx)))
            {

                worksheet.Cell(row.idx + 2, 1).Value = row.value.Category;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.WorkDT;
                worksheet.Cell(row.idx + 2, 3).Value = row.value.HistoryTitle;
                worksheet.Cell(row.idx + 2, 4).Value = row.value.Type == 0 ? "자체 작업" : "외주 작업";
                worksheet.Cell(row.idx + 2, 5).Value = row.value.Worker;
                worksheet.Cell(row.idx + 2, 6).Value = row.value.HistoryMaterialList.Count;
                worksheet.Cell(row.idx + 2, 7).Value = row.value.TotalPrice;
            }

            // 전체 데이터 범위 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);

            // 전체 범위에 테두리 적용
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            //헤더
            var range = worksheet.Range(1, 1, 1, colName.Count);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);




            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }

        /// <summary>
        /// 사용자
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportUser(List<ListUser> data, List<string> colName, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add("직원목록");

            foreach (var header in colName.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx + 1).Value = header.value;
            }


            foreach (var row in data.Select((value, idx) => (value, idx)))
            {

                string state = row.value.Status switch
                {
                    0 => "퇴직",
                    1 => "휴직",
                    2 => "재직",
                    _ => "",
                };

                worksheet.Cell(row.idx + 2, 1).Value = row.value.Name;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.UserId;
                worksheet.Cell(row.idx + 2, 3).Value = row.value.Type;
                worksheet.Cell(row.idx + 2, 4).Value = row.value.Email;
                worksheet.Cell(row.idx + 2, 5).Value = row.value.Created;
                worksheet.Cell(row.idx + 2, 6).Value = state;
            }

            // 전체 데이터 범위 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);

            // 전체 범위에 테두리 적용
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            //헤더
            var range = worksheet.Range(1, 1, 1, colName.Count);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);




            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }

        /// <summary>
        /// 건물
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportBuilding(List<BuildingListDTO> data, List<string> colName, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add("건물목록");

            foreach (var header in colName.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx + 1).Value = header.value;
            }


            foreach (var row in data.Select((value, idx) => (value, idx)))
            {


                worksheet.Cell(row.idx + 2, 1).Value = row.value.Name;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.Address;
                worksheet.Cell(row.idx + 2, 3).Value = row.value.Tel;
                worksheet.Cell(row.idx + 2, 4).Value = row.value.TotalFloor;
                worksheet.Cell(row.idx + 2, 5).Value = row.value.CompletionDt;
                worksheet.Cell(row.idx + 2, 6).Value = row.value.CreateDt;
            }

            // 전체 데이터 범위 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);

            // 전체 범위에 테두리 적용
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            //헤더
            var range = worksheet.Range(1, 1, 1, colName.Count);
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);




            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }


        /// <summary>
        /// 입출고
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportInOutDetail(List<DetailMaterialListDTO> data, List<string> colName, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            foreach(var m in data)
            {
                var worksheet = workbook.Worksheets.Add($"{m.Name} 입출고 내역");

                foreach (var header in colName.Select((value, idx) => (value, idx)))
                {
                    worksheet.Cell(1, header.idx + 1).Value = header.value;
                }
                worksheet.Cell(2, 1).Value = "기준월 이전재고";
                worksheet.Cell(2, 2).Value = m.Code;
                worksheet.Cell(2, 3).Value = m.Name;
                worksheet.Cell(2, 4).Value = 0;
                worksheet.Cell(2, 5).Value = 0;
                worksheet.Cell(2, 6).Value = 0;
                worksheet.Cell(2, 7).Value = 0;
                worksheet.Cell(2, 8).Value = 0;
                worksheet.Cell(2, 9).Value = 0;
                worksheet.Cell(2, 10).Value = 0;                
                worksheet.Cell(2, 11).Value = m.LastMonthStock;

                int lastRow = 0;
                int totalInQty = 0;
                float totalInPrice = 0;
                float totalInAmount = 0;
                int totalOutQty = 0;
                float totalOutPrice = 0;
                float totalOutAmount = 0;
                foreach (var row in m.InventoryList.Select((value, idx) => (value, idx)))
                {
                    worksheet.Cell(row.idx + 3, 1).Value = row.value.INOUT_DATE;
                    worksheet.Cell(row.idx + 3, 2).Value = row.value.Code;
                    worksheet.Cell(row.idx + 3, 3).Value = row.value.Name;
                    worksheet.Cell(row.idx + 3, 4).Value = row.value.MaterialUnit;
                    worksheet.Cell(row.idx + 3, 5).Value = row.value.Type == 1 ?row.value.InOutNum : 0;
                    worksheet.Cell(row.idx + 3, 6).Value = row.value.Type == 1 ? row.value.InOutUnitPrice : 0;
                    worksheet.Cell(row.idx + 3, 7).Value = row.value.Type == 1 ? row.value.InOutTotalPrice : 0;
                    worksheet.Cell(row.idx + 3, 8).Value = row.value.Type == 0 ? row.value.InOutNum : 0;
                    worksheet.Cell(row.idx + 3, 9).Value = row.value.Type == 0 ? row.value.InOutUnitPrice : 0;
                    worksheet.Cell(row.idx + 3, 10).Value = row.value.Type == 0 ? row.value.InOutTotalPrice : 0;
                    worksheet.Cell(row.idx + 3, 11).Value = row.value.CurrentNum;
                    worksheet.Cell(row.idx + 3, 12).Value = row.value.Note;
                    lastRow = row.idx + 3+1;
                    if(row.value.Type == 1)
                    {
                        totalInQty += row.value.InOutNum.Value;
                        totalInPrice += row.value.InOutUnitPrice.Value;
                        totalInAmount += row.value.InOutTotalPrice.Value;
                    }
                    if(row.value.Type == 0)
                    {
                        totalOutQty += row.value.InOutNum.Value;
                        totalOutPrice += row.value.InOutUnitPrice.Value;
                        totalOutAmount += row.value.InOutTotalPrice.Value;
                    }

                }
                worksheet.Cell(lastRow, 1).Value = "소계";
                worksheet.Cell(lastRow, 2).Value = "소계";
                worksheet.Cell(lastRow, 3).Value = "소계";
                worksheet.Cell(lastRow, 4).Value = "소계";
                // 1번부터 3번까지 셀을 그룹화
                var range = worksheet.Range(lastRow, 1, lastRow, 4);
                range.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Font.Bold = true;
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Cell(lastRow, 5).Value = totalInQty;
                worksheet.Cell(lastRow, 6).Value = totalInPrice/(m.InventoryList.Count);
                worksheet.Cell(lastRow, 7).Value = totalInAmount;
                worksheet.Cell(lastRow, 8).Value = totalOutQty;
                worksheet.Cell(lastRow, 9).Value = totalOutPrice / (m.InventoryList.Count);
                worksheet.Cell(lastRow, 10).Value = totalOutAmount;
                worksheet.Cell(lastRow, 11).Value = m.InventoryList[m.InventoryList.Count - 1].CurrentNum;

               

                //헤더
                range = worksheet.Range(1, 1, 1, colName.Count);
                range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

                // 전체 테두리 설정
                int totalRows = lastRow; // 데이터가 포함된 마지막 행
                int totalCols = colName.Count;
                var fullRange = worksheet.Range(1, 1, totalRows, totalCols);

                // 테두리 적용
                fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            }
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }



        /// <summary>
        /// 품목별 재고 현황
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportInventory(List<MaterialHistoryDTO> data, List<string> colName, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add("품목별 재고현황");

            foreach (var header in colName.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx + 1).Value = header.value;
            }


            foreach (var row in data.Select((value, idx) => (value, idx)))
            {
                int lastCol = 0;
                int totalStock = 0;

                worksheet.Cell(row.idx + 2, 1).Value = row.value.Code;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.Name;
                foreach(var roomData in row.value.RoomHistory.Select((value,idx) => (value, idx)))
                {
                    lastCol = roomData.idx + 1;
                    totalStock += roomData.value.Num.Value;
                    worksheet.Cell(row.idx + 2, roomData.idx+3).Value = roomData.value.Num;
                }
                
                worksheet.Cell(row.idx + 2, lastCol+3).Value = totalStock;

            }


            // 전체 범위의 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            
            
            worksheet.Columns().AdjustToContents(1, totalRows, 10,200); // 최소 10, 최대 50

            // 헤더 스타일 설정
            var headerRange = worksheet.Range(1, 1, 1, colName.Count);
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            // 3번째 컬럼부터 마지막 전 컬럼까지 Bold 설정
            int startColumn = 3;
            int endColumn = colName.Count - 1;

            var boldRange = worksheet.Range(1, startColumn, 1, endColumn);
            boldRange.Style.Font.Bold = true;


            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }

        /// <summary>
        /// 설비 제원
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportFacility(FacilityDetailDTO data, List<FamTec.Shared.Client.DTO.Normal.Facility.Group.GroupDTO> groups, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add($"{title} 제원");

            worksheet.Range("A1:H1").Value = "설비제원";

            // 1번부터 3번까지 셀을 그룹화
            var titleRange = worksheet.Range("A1:H1");
            titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            worksheet.Cell(2, 1).Value = "설비이름";
            worksheet.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 2).Value = data.Name;
            worksheet.Cell(2, 3).Value = "형식";
            worksheet.Cell(2, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 4).Value = data.Type;
            worksheet.Cell(2, 5).Value = "위치";
            worksheet.Cell(2, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 6).Value = data.RoomName;
            worksheet.Cell(2, 7).Value = "규격용량";
            worksheet.Cell(2, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 8).Value = data.Standard_capacity;
            worksheet.Cell(3, 1).Value = "수량";
            worksheet.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 2).Value = data.Num;
            worksheet.Cell(3, 3).Value = "내용년수";
            worksheet.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 4).Value = data.LifeSpan;
            worksheet.Cell(3, 5).Value = "설치년월";
            worksheet.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 6).Value = data.EquipDT;
            worksheet.Cell(3, 7).Value = "교체년월";
            worksheet.Cell(3, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 8).Value = data.ChangeDT;

            var bodyRange = worksheet.Range("A2:H3");
            bodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            bodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            worksheet.Range("A6:F6").Value = "추가항목";

            var subTitleRange = worksheet.Range("A6:F6");
            subTitleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            subTitleRange.Style.Font.Bold = true;
            subTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subTitleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            List<string> subTitle = new List<string>()
            {
                "그룹명",
                "제원명",
                "단위",
                "값",
            };

            foreach (var item in subTitle.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(7, item.idx+1).Value = item.value;
            }
            var subItemRange = worksheet.Range("D7:F7");
            subItemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var subHeaderRange = worksheet.Range("A7:F7");
            subHeaderRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            subHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subHeaderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int groupRowStartCount = 8;
            int groupRowEndCount = 8;
            if (groups.Count > 0)
            {
                foreach(var group in groups.Select((value, idx) => (value, idx)))
                {
                    if(group.value.keyListDTO.Count > 0)
                    {
                        foreach (var item in group.value.keyListDTO.Select((value, idx) => (value, idx)))
                        {
                            worksheet.Cell(item.idx + groupRowStartCount, 1).Value = group.value.Name;
                            worksheet.Cell(item.idx + groupRowStartCount, 2).Value = item.value.ItemKey;
                            worksheet.Cell(item.idx + groupRowStartCount, 3).Value = item.value.Unit;
                            foreach (var itemValue in item.value.valueList.Select((value, idx) => (value, idx)))
                            {
                                worksheet.Cell(item.idx + 8, itemValue.idx + 4).Value = itemValue.value.itemValue;
                            }
                            groupRowEndCount++;
                        }
                        var groupRange = worksheet.Range($"A{groupRowStartCount}:A{groupRowEndCount - 1}");
                        groupRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        groupRowStartCount = groupRowEndCount;
                    }
                    
                }
                if(groupRowEndCount != 8)
                {
                    var groupBodyRange = worksheet.Range($"A8:F{groupRowEndCount - 1}");

                    groupBodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    groupBodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
                
            }

         

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }


        /// <summary>
        /// 설비 상세 유지보수이력
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportFacMaintenanace(List<MaintenanceListDTO> data, List<string> colName, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add(title);

            foreach (var header in colName.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(1, header.idx + 1).Value = header.value;
            }


            foreach (var row in data.Select((value, idx) => (value, idx)))
            {
                int lastCol = 0;
                int totalStock = 0;

                worksheet.Cell(row.idx + 2, 1).Value = row.value.WorkDT;
                worksheet.Cell(row.idx + 2, 2).Value = row.value.Name;
                worksheet.Cell(row.idx + 2, 3).Value = row.value.Type == 0 ? "자체 작업" : "외주 작업";
                worksheet.Cell(row.idx + 2, 4).Value = row.value.Worker;
                worksheet.Cell(row.idx + 2, 5).Value = row.value.TotalPrice;
            }


            // 전체 범위의 테두리 설정
            int totalRows = data.Count + 1; // 헤더 포함
            int totalCols = colName.Count;
            var fullRange = worksheet.Range(1, 1, totalRows, totalCols);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            worksheet.Columns().AdjustToContents(1, totalRows, 10, 200); // 최소 10, 최대 50

            // 헤더 스타일 설정
            var headerRange = worksheet.Range(1, 1, 1, colName.Count);
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            //// 3번째 컬럼부터 마지막 전 컬럼까지 Bold 설정
            //int startColumn = 3;
            //int endColumn = colName.Count - 1;

            //var boldRange = worksheet.Range(1, startColumn, 1, endColumn);
            //boldRange.Style.Font.Bold = true;


            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }

        /// <summary>
        /// 상세 유지보수 정보
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportFacMaintenance(DetailMaintenanceDTO data, List<UpdateUseMaterialDTO> usematerial,string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add(title);

            worksheet.Range("A1:F1").Value = "유지보수 정보";

            // 1번부터 3번까지 셀을 그룹화
            var titleRange = worksheet.Range("A1:F1");
            titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            worksheet.Cell(2, 1).Value = "작업년월";
            worksheet.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 2).Value = data.WorkDT;
            worksheet.Cell(2, 3).Value = "작업명";
            worksheet.Cell(2, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 4).Value = data.WorkName;
            worksheet.Cell(2, 5).Value = "작업구분";
            worksheet.Cell(2, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(2, 6).Value = data.Type == 0 ? "자체작업" : "외주작업";
            worksheet.Cell(3, 1).Value = "작업자";
            worksheet.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 2).Value = data.Worker;
            worksheet.Cell(3, 3).Value = "소요비용";
            worksheet.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(3, 4).Value = data.TotalPrice;
            

            var bodyRange = worksheet.Range("A2:F3");
            bodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            bodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            worksheet.Range("A6:G6").Value = "사용자재 목록";

            var subTitleRange = worksheet.Range("A6:G6");
            subTitleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            subTitleRange.Style.Font.Bold = true;
            subTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subTitleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            List<string> subTitle = new List<string>()
            {
                "품목코드",
                "품목명",
                "위치",
                "수량",
                "단위",
                "금액",
                "비고",
            };

            foreach (var item in subTitle.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(7, item.idx + 1).Value = item.value;
            }

            var subHeaderRange = worksheet.Range("A7:G7");
            subHeaderRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            subHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subHeaderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            if (usematerial.Count > 0)
            {

                foreach (var item in usematerial.Select((value, idx) => (value, idx)))
                {
                    worksheet.Cell(item.idx + 8, 1).Value = item.value.MaterialCode;
                    worksheet.Cell(item.idx + 8, 2).Value = item.value.MaterialName;
                    worksheet.Cell(item.idx + 8, 3).Value = item.value.RoomName;
                    worksheet.Cell(item.idx + 8, 4).Value = item.value.Num;
                    worksheet.Cell(item.idx + 8, 5).Value = item.value.Unit;
                    worksheet.Cell(item.idx + 8, 6).Value = item.value.Price;
                    worksheet.Cell(item.idx + 8, 7).Value = item.value.Note;

                }
                var useBodyRange = worksheet.Range($"A8:G{7 + usematerial.Count}");
                useBodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                useBodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }
            

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }


        /// <summary>
        /// 건물 상세
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task ExportDetailBuilding(BuildingDTO data, List<FamTec.Shared.Client.DTO.Normal.Buildings.Group.GroupDTO> groups, string title)
        {//option 0은 월간 조회, 1은 기간조회


            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();


            var worksheet = workbook.Worksheets.Add($"{title} 제원");

            worksheet.Range("A1:G1").Value = "건물정보";

            // 1번부터 3번까지 셀을 그룹화
            var titleRange = worksheet.Range("A1:G1");
            titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            worksheet.Cell(2, 1).Value = "건물이름";
            worksheet.Cell(2, 2).Value = data.Name;
            worksheet.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            var itemRange = worksheet.Range("B2:G2");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3, 1).Value = "건물주소";
            worksheet.Cell(3, 2).Value = data.Address;
            itemRange = worksheet.Range("B3:G3");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(4, 1).Value = "대표전화";
            worksheet.Cell(4, 2).Value = data.Tel;
            itemRange = worksheet.Range("B4:G4");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(5, 1).Value = "건물용도";
            worksheet.Cell(5, 2).Value = data.Usage;
            itemRange = worksheet.Range("B5:G5");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(5, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(6, 1).Value = "시공업체";
            worksheet.Cell(6, 2).Value = data.ConstCompany;
            itemRange = worksheet.Range("B6:G6");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(6, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(7, 1).Value = "준공년월";
            worksheet.Cell(7, 2).Value = data.CompletionDT;
            itemRange = worksheet.Range("B7:G7");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(7, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(8, 1).Value = "건물구조";
            worksheet.Cell(8, 2).Value = data.BuildingStruct;
            itemRange = worksheet.Range("B8:D8");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(8, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(8, 5).Value = "지붕구조";
            worksheet.Cell(8, 6).Value = data.RoofStruct;
            itemRange = worksheet.Range("F8:G8");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(8, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            worksheet.Cell(9, 1).Value = "연면적";
            worksheet.Cell(9, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(9, 2).Value = data.GrossFloorArea;
            worksheet.Cell(9, 3).Value = "㎡";
            worksheet.Cell(9, 4).Value = "대지면적";
            worksheet.Cell(9, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(9, 5).Value = data.LandArea;
            worksheet.Cell(9, 6).Value = "건축면적";
            worksheet.Cell(9, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(9, 7).Value = data.BuildingArea;

            worksheet.Cell(10, 1).Value = "건물층수";
            worksheet.Cell(10, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(10, 2).Value = data.GrossFloorArea;
            worksheet.Cell(10, 3).Value = "층";
            worksheet.Cell(10, 4).Value = "지상";
            worksheet.Cell(10, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(10, 5).Value = data.LandArea;
            worksheet.Cell(10, 6).Value = "지하";
            worksheet.Cell(10, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(10, 7).Value = data.BuildingArea;

            worksheet.Cell(11, 1).Value = "건물높이";
            worksheet.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(11, 2).Value = data.GrossFloorArea;
            worksheet.Cell(11, 3).Value = "M";
            worksheet.Cell(11, 4).Value = "지상";
            worksheet.Cell(11, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(11, 5).Value = data.LandArea;
            worksheet.Cell(11, 6).Value = "지하";
            worksheet.Cell(11, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(11, 7).Value = data.BuildingArea;

            worksheet.Cell(12, 1).Value = "주차대수";
            worksheet.Cell(12, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(12, 2).Value = data.GrossFloorArea;
            worksheet.Cell(12, 3).Value = "대";
            worksheet.Cell(12, 4).Value = "옥내";
            worksheet.Cell(12, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(12, 5).Value = data.LandArea;
            worksheet.Cell(12, 6).Value = "옥외";
            worksheet.Cell(12, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(12, 7).Value = data.BuildingArea;

            worksheet.Cell(13, 1).Value = "전기용량";
            worksheet.Cell(13, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(13, 2).Value = data.GrossFloorArea;
            worksheet.Cell(13, 3).Value = "kw";
            worksheet.Cell(13, 4).Value = "수전용량";
            worksheet.Cell(13, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(13, 5).Value = data.LandArea;
            worksheet.Cell(13, 6).Value = "발전용량";
            worksheet.Cell(13, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(13, 7).Value = data.BuildingArea;

            worksheet.Cell(14, 1).Value = "급수용량";
            worksheet.Cell(14, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(14, 2).Value = data.GrossFloorArea;
            worksheet.Cell(14, 3).Value = "Ton";
            worksheet.Cell(14, 4).Value = "고가수조";
            worksheet.Cell(14, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(14, 5).Value = data.LandArea;
            worksheet.Cell(14, 6).Value = "저수조";
            worksheet.Cell(14, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(14, 7).Value = data.BuildingArea;

            worksheet.Cell(15, 1).Value = "가스용량";
            worksheet.Cell(15, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(15, 2).Value = data.GrossFloorArea;
            worksheet.Cell(15, 3).Value = "N㎥";
            worksheet.Cell(15, 4).Value = "보일러";
            worksheet.Cell(15, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(15, 5).Value = data.LandArea;
            worksheet.Cell(15, 6).Value = "냉온수기";
            worksheet.Cell(15, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(15, 7).Value = data.BuildingArea;

            worksheet.Cell(16, 1).Value = "승강기대수";
            worksheet.Cell(16, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(16, 2).Value = data.GrossFloorArea;
            worksheet.Cell(16, 3).Value = "대";
            worksheet.Cell(16, 4).Value = "인승용";
            worksheet.Cell(16, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(16, 5).Value = data.LandArea;
            worksheet.Cell(16, 6).Value = "화물용";
            worksheet.Cell(16, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(16, 7).Value = data.BuildingArea;

            worksheet.Cell(17, 1).Value = "냉난방용량";
            worksheet.Cell(17, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(17, 2).Value = data.GrossFloorArea;
            worksheet.Cell(17, 3).Value = "kcal/h";
            worksheet.Cell(17, 4).Value = "난방용량";
            worksheet.Cell(17, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(17, 5).Value = data.LandArea;
            worksheet.Cell(17, 6).Value = "냉방용량";
            worksheet.Cell(17, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(17, 7).Value = data.BuildingArea;

            worksheet.Cell(18, 1).Value = "조경면적";
            worksheet.Cell(18, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(18, 2).Value = data.GrossFloorArea;
            worksheet.Cell(18, 3).Value = "㎡";
            worksheet.Cell(18, 4).Value = "지상";
            worksheet.Cell(18, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(18, 5).Value = data.LandArea;
            worksheet.Cell(18, 6).Value = "지하";
            worksheet.Cell(18, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(18, 7).Value = data.BuildingArea;

            worksheet.Cell(19, 1).Value = "화장실";
            worksheet.Cell(19, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(19, 2).Value = data.GrossFloorArea;
            worksheet.Cell(19, 3).Value = "개소";
            worksheet.Cell(19, 4).Value = "남자";
            worksheet.Cell(19, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(19, 5).Value = data.LandArea;
            worksheet.Cell(19, 6).Value = "여자";
            worksheet.Cell(19, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Cell(19, 7).Value = data.BuildingArea;

            worksheet.Cell(20, 1).Value = "소방등급";
            worksheet.Cell(20, 2).Value = data.FireRating;
            itemRange = worksheet.Range("B20:G20");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(20, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            worksheet.Cell(21, 1).Value = "정화조용량";
            worksheet.Cell(21, 2).Value = data.SeptictankCapacity;
            itemRange = worksheet.Range("B21:G21");
            itemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(21, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


            var fullRange = worksheet.Range("A1:G21");
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;



            worksheet.Range("A24:F24").Value = "추가항목";

            var subTitleRange = worksheet.Range("A24:F24");
            subTitleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            subTitleRange.Style.Font.Bold = true;
            subTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subTitleRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

            List<string> subTitle = new List<string>()
            {
                "그룹명",
                "제원명",
                "단위",
                "값",
            };

            foreach (var item in subTitle.Select((value, idx) => (value, idx)))
            {
                worksheet.Cell(25, item.idx + 1).Value = item.value;
            }
            var subItemRange = worksheet.Range("D25:F25");
            subItemRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var subHeaderRange = worksheet.Range("A25:F25");
            subHeaderRange.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            subHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            subHeaderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int groupRowStartCount = 26;
            int groupRowEndCount = 26;
            if (groups.Count > 0)
            {
                foreach (var group in groups.Select((value, idx) => (value, idx)))
                {
                    foreach (var item in group.value.keyListDTO.Select((value, idx) => (value, idx)))
                    {
                        worksheet.Cell(item.idx + groupRowStartCount, 1).Value = group.value.Name;
                        worksheet.Cell(item.idx + groupRowStartCount, 2).Value = item.value.ItemKey;
                        worksheet.Cell(item.idx + groupRowStartCount, 3).Value = item.value.Unit;
                        foreach (var itemValue in item.value.valueList.Select((value, idx) => (value, idx)))
                        {
                            worksheet.Cell(item.idx + 26, itemValue.idx + 4).Value = itemValue.value.itemValue;
                        }
                        groupRowEndCount++;
                    }
                    var groupRange = worksheet.Range($"A{groupRowStartCount}:A{groupRowEndCount - 1}");
                    groupRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    groupRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    groupRowStartCount = groupRowEndCount;
                }
                var groupBodyRange = worksheet.Range($"A24:F{groupRowEndCount - 1}");
                groupBodyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                groupBodyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }



            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{title}.xlsx", Convert.ToBase64String(content));

        }
    }
}
