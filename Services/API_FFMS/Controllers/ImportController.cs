using API_FFMS.Services;
using AppCore.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class ImportController : BaseController
    {
        private readonly IImportAssetService _importService;

        public ImportController(IImportAssetService assetService)
        {
            _importService = assetService;
        }

        [HttpGet]
        [SwaggerOperation("Download template import asset")]
        public async Task<IActionResult> GetAssetImportTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AssetTemplate");
                var currentRow = 1;

                // Define the column headers
                worksheet.Cell(currentRow, 1).Value = "Tên thiết bị";
                worksheet.Cell(currentRow, 2).Value = "Mã thiết bị";
                worksheet.Cell(currentRow, 3).Value = "Nhóm thiết bị";
                worksheet.Cell(currentRow, 4).Value = "Nhãn hiệu";
                worksheet.Cell(currentRow, 5).Value = "Trạng thái";
                worksheet.Cell(currentRow, 6).Value = "Năm sản xuất";
                worksheet.Cell(currentRow, 7).Value = "Số định danh";
                worksheet.Cell(currentRow, 8).Value = "Số lượng";
                worksheet.Cell(currentRow, 9).Value = "Mô tả";
                worksheet.Cell(currentRow, 10).Value = "Thuộc sở hữu";
                worksheet.Cell(currentRow, 11).Value = "Có thể di chuyển";

                // Apply styles to the header row
                var headerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 11));
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Adjust column width
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "AssetTemplate.xlsx");
                }
            }
        }

        [HttpPost]
        [SwaggerOperation("Import asset")]
        public async Task<ApiResponses<ImportError>> ImportAsset(IFormFile file)
        {
            return await _importService.ImportAssets(file);
        }
    }
}
