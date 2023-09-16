using API_FFMS.Services;
using AppCore.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
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
                worksheet.Cell(currentRow, 1).Value = "Asset Name";
                worksheet.Cell(currentRow, 2).Value = "Asset Code";
                worksheet.Cell(currentRow, 3).Value = "Category Code";
                worksheet.Cell(currentRow, 4).Value = "Status";
                worksheet.Cell(currentRow, 5).Value = "Manufacturing Year";
                worksheet.Cell(currentRow, 6).Value = "SerialNumber";
                worksheet.Cell(currentRow, 7).Value = "Quantity";
                worksheet.Cell(currentRow, 8).Value = "Description";

                // Apply styles to the header row
                var headerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 8));
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
        public async Task<ApiResponse<ImportError>> ImportAsset(IFormFile file)
        {
            return await _importService.ImportAssets(file);
        }
    }
}
