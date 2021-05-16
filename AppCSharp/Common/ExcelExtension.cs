using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocatingApp.Common
{
    public static class ExcelExtension
    {
        public static void GenerateWorksheet(this ExcelPackage excelPackage, string SheetName, List<string[]> headers, IEnumerable<object[]> data)
        {
            var worksheet = excelPackage.Workbook.Worksheets.Add(SheetName);
            SetHeader(worksheet, headers);
            SetData(worksheet, headers, data);
        }
        private static void SetHeader(this ExcelWorksheet worksheet, List<string[]> headers)
        {
            int headerLine = headers.Count;
            string headerRange = $"A{headerLine}:" + Char.ConvertFromUtf32(headers[0].Length + 64) + headerLine;
            if (headers.FirstOrDefault().Length > 26)
            {
                headerRange = $"A{headerLine}:A" + Char.ConvertFromUtf32((headers[0].Length + 64) - 26) + headerLine;
            }
            
            worksheet.Cells[headerRange].LoadFromArrays(headers);
            worksheet.Cells[headerRange].Style.Font.Bold = true;
            worksheet.Cells[headerRange].Style.Font.Size = 13;
            worksheet.Cells[headerRange].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Red);
            worksheet.Cells[headerRange].AutoFitColumns();
            worksheet.Cells[headerRange].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }
        private static void SetData(this ExcelWorksheet worksheet, List<string[]> headers, IEnumerable<object[]> data)
        {
            int startFromLine = headers.Count + 1;
            int headerLine = headers.Count;
            var headerRange = $"A{startFromLine}:" + Char.ConvertFromUtf32(headers[0].Length + 64) + startFromLine;
            if (headers.FirstOrDefault().Length > 26)
            {
                headerRange = $"A{startFromLine}:A" + Char.ConvertFromUtf32((headers[0].Length + 64) - 26) + startFromLine;
            }
            worksheet.Cells[headerRange].LoadFromArrays(data);
        }
    }
}
