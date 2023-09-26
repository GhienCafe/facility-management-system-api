using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;
namespace AppCore.Extensions;

public static class ExcelExtension
{
    public static List<T> ReadExcelFile<T>(this Stream stream, Func<Row, T> mapFunction)
    {
        var result = new List<T>();
        using var document = SpreadsheetDocument.Open(stream, false);
        var workbookPart = document.WorkbookPart;
        if (workbookPart == null)
            return new List<T>();
        var sheetIds = workbookPart.Workbook.GetFirstChild<Sheets>()?.Elements<Sheet>()
            .Select(x => x.Id)
            .Where(x => x != null)
            .ToList();

        if (sheetIds == null)
            return new List<T>();

        foreach (var sheetId in sheetIds)
        {
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheetId!);
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
                continue;

            result.AddRange(sheetData.Elements<Row>().Select(mapFunction).Where(obj => obj != null));
        }

        return result;
    }

    public class ExportField
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Index { get; set; }
    }

    public static class ExportHelperList<T>
    {
        public static Stream Export(List<T> items, string sheetName, string title, int startRow = 6,
            Dictionary<string, string>? otherFields = null!)
        {
            var now = DateTime.Now;
            var defaultColumns = new Dictionary<string, ExportField>();
            var valueType = typeof(T);
            var propertyInfos = valueType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var name = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name;
                var other = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Order ?? 0;
                if (!string.IsNullOrEmpty(name))
                    defaultColumns.Add(propertyInfo.Name, new ExportField
                    {
                        Name = name,
                        Type = propertyInfo.PropertyType.Name,
                        Index = other
                    });
            }

            var columnCount = defaultColumns.Count + 1;

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(sheetName);

            // Title
            worksheet.Cell(1, 1).SetValue(title.ToUpper());
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(1, 1, 1, columnCount).Row(1).Merge();
            worksheet.Range(1, 1, 1, columnCount).Style.Font.Bold = true;
            // Time export
            worksheet.Cell(2, 1).SetValue($"Thời gian xuất file: {now:yy-MM-dd hh:mm:ss}");
            worksheet.Cell(2, 1).Style.Font.FontSize = 12;
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(2, 1, 2, columnCount).Row(1).Merge();

            // Header
            worksheet.Cell(5, 1).Value = "STT";
            var currentColumn = 2;
            var columnsConvert = defaultColumns.Count(x => x.Value.Index == 0) > 1
                ? defaultColumns.ToList()
                : defaultColumns.OrderBy(x => x.Value.Index).ToList();
            foreach (var column in columnsConvert)
            {
                worksheet.Cell(5, currentColumn).SetValue(column.Value.Name);
                currentColumn++;
            }

            worksheet.Range(5, 1, 5, columnCount).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 168, 83);
            worksheet.Range(5, 1, 5, columnCount).Style.Font.FontColor = XLColor.White;

            // Othder fields
            foreach (var otherField in otherFields ?? new Dictionary<string, string>())
            {
                worksheet.Cell(otherField.Key).Value = otherField.Value;
            }

            // Content Start col = 6
            var currentRow = startRow >= 6 ? startRow : 6;
            foreach (var item in CollectionsMarshal.AsSpan(items))
            {
                currentColumn = 2;
                foreach (var column in columnsConvert)
                {
                    var value = valueType.GetProperty(column.Key)?.GetValue(item);
                    worksheet.Cell(currentRow, currentColumn).SetValue(value?.ToString());

                    if ((value?.ToString() ?? string.Empty).Length > 100)
                        worksheet.Cell(currentRow, currentColumn).Style.Alignment.WrapText = true;

                    currentColumn++;
                }

                currentRow++;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            workbook.Dispose();
            stream.Position = 0;
            return stream;
        }

        public static Stream ExportV2(List<T> items, string sheetName, string title, int startRow = 6,
            Dictionary<string, string>? otherFields = null!)
        {
            var now = DateTime.Now;
            var defaultColumns = new Dictionary<string, ExportField>();
            var valueType = typeof(T);
            var propertyInfos = valueType.GetProperties()
                .Where(x =>
                    x.GetCustomAttribute<DisplayAttribute>() != null
                ).OrderBy(x =>
                    x.GetCustomAttribute<DisplayAttribute>()?.Order ?? 0
                ).ToArray();

            foreach (var propertyInfo in propertyInfos)
            {
                var name = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name;
                var other = propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Order ?? 0;
                if (!string.IsNullOrEmpty(name))
                    defaultColumns.Add(propertyInfo.Name, new ExportField
                    {
                        Name = name,
                        Type = propertyInfo.PropertyType.Name,
                        Index = other
                    });
            }

            var columnCount = defaultColumns.Count + 1;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Title
            worksheet.Cell(1, 1).SetValue(title.ToUpper());
            worksheet.Cell(1, 1).Style.Font.FontSize = 18;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(1, 1, 1, columnCount).Row(1).Merge();
            worksheet.Range(1, 1, 1, columnCount).Style.Font.Bold = true;
            // Time export
            worksheet.Cell(2, 1).SetValue($"Thời gian xuất file: {now:yy-MM-dd hh:mm:ss}");
            worksheet.Cell(2, 1).Style.Font.FontSize = 12;
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range(2, 1, 2, columnCount).Row(1).Merge();

            // Header
            worksheet.Cell(5, 1).Value = "STT";
            var currentColumn = 2;
            var columnsConvert = defaultColumns.Count(x => x.Value.Index == 0) > 1
                ? defaultColumns.ToList()
                : defaultColumns.OrderBy(x => x.Value.Index).ToList();
            foreach (var column in columnsConvert)
            {
                worksheet.Cell(5, currentColumn).SetValue(column.Value.Name);
                currentColumn++;
            }

            worksheet.Range(5, 1, 5, columnCount).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 168, 83);
            worksheet.Range(5, 1, 5, columnCount).Style.Font.FontColor = XLColor.White;

            // Othder fields
            foreach (var otherField in otherFields ?? new Dictionary<string, string>())
            {
                worksheet.Cell(otherField.Key).Value = otherField.Value;
            }

            // Content Start col = 6
            var currentRow = startRow >= 6 ? startRow : 6;
            foreach (var item in CollectionsMarshal.AsSpan(items))
            {
                currentColumn = 2;
                worksheet.Cell(currentRow, 1).SetValue(currentRow - startRow + 1);
                var values = propertyInfos.Select(x => x.GetValue(item, null));
                foreach (var value in values)
                {
                    worksheet.Cell(currentRow, currentColumn).SetValue(value?.ToString());
                    if ((value?.ToString() ?? string.Empty).Length > 100)
                        worksheet.Cell(currentRow, currentColumn).Style.Alignment.WrapText = true;

                    currentColumn++;
                }

                Console.WriteLine(currentRow);
                currentRow++;
            }

            var stream = new MemoryStream();
            worksheet.Columns();
            workbook.SaveAs(stream);
            workbook.Dispose();
            stream.Position = 0;
            return stream;
        }
    }
}