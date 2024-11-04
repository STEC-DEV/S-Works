

using ClosedXML.Excel;
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

        //월별 export
        public async Task ExportVocData(List<MonthVocListDTO> data, List<string> colName)
        {
            var workbook = new XLWorkbook();
            var properties = typeof(ListVocDTO).GetProperties();
            //월별 시트 생성
            foreach (var sheet  in data)
            {
                //월별 시트 생성
                var worksheet = workbook.Worksheets.Add(sheet.Dates + "민원");

                foreach(var header in colName.Select((value,idx) => (value, idx)))
                {
                    worksheet.Cell(1, header.idx+1).Value = header.value;
                }
                foreach(var row in sheet.VocList.Select((value,idx) => (value,idx)))
                {
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
                     
                    worksheet.Cell(row.idx + 2, 1).Value = row.value.BuildingName;
                    worksheet.Cell(row.idx + 2, 2).Value = type;
                    worksheet.Cell(row.idx + 2, 3).Value = row.value.Title;
                    worksheet.Cell(row.idx + 2, 4).Value = row.value.CreateDT;
                    worksheet.Cell(row.idx + 2, 5).Value = row.value.CompleteDT;
                    worksheet.Cell(row.idx + 2, 6).Value = row.value.DurationDT;
                    worksheet.Cell(row.idx + 2, 7).Value = status;
                }

                //헤더
                var range = worksheet.Range(1, 1, 1, colName.Count);
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                range.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

                worksheet.Columns().AdjustToContents();
            }
            

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            // 파일 다운로드
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream",
                "민원.xlsx", Convert.ToBase64String(content));

        }

    }
}
