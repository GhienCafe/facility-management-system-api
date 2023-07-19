using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheetId);
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
                continue;

            result.AddRange(sheetData.Elements<Row>().Select(mapFunction).Where(obj => obj != null));
        }

        return result;
    }
}