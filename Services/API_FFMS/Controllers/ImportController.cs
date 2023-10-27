using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using ClosedXML.Excel;
using MainData;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class ImportController : BaseController
    {
        private readonly IImportAssetService _importService;
        private readonly IAssetTypeService _assetTypeService;

        public ImportController(IImportAssetService assetService, IAssetTypeService assetTypeService)
        {
            _importService = assetService;
            _assetTypeService = assetTypeService;
        }

        [HttpGet]
        [SwaggerOperation("Download template import asset")]
        public async Task<IActionResult> GetAssetImportTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AssetTemplate");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Tên thiết bị";
                worksheet.Cell(currentRow, 2).Value = "Mã thiết bị";
                worksheet.Cell(currentRow, 3).Value = "Nhóm thiết bị";
                worksheet.Cell(currentRow, 4).Value = "Nhãn hiệu";
                worksheet.Cell(currentRow, 5).Value = "Năm sản xuất";
                worksheet.Cell(currentRow, 6).Value = "Số định danh";
                worksheet.Cell(currentRow, 7).Value = "Số lượng";
                worksheet.Cell(currentRow, 8).Value = "Mô tả";
                worksheet.Cell(currentRow, 9).Value = "Thuộc sở hữu";
                worksheet.Cell(currentRow, 10).Value = "Có thể di chuyển";

                var headerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 10));
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Columns().AdjustToContents();


                var assetTypesWorksheet = workbook.Worksheets.Add("AssetTypes");
                var assetTypesCurrentRow = 1;
                var assetTypes = await _assetTypeService.GetAssetTypes();

                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = "Tên nhóm thiết bị";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = "Mã nhóm thiết bị";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = "Mô tả";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 4).Value = "Hình ảnh";

                var assetTypesHeaderRange = assetTypesWorksheet.Range(assetTypesWorksheet.Cell(assetTypesCurrentRow, 1), assetTypesWorksheet.Cell(assetTypesCurrentRow, 4));
                assetTypesHeaderRange.Style.Font.Bold = true;
                assetTypesHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                foreach (var assetType in assetTypes.Data)
                {
                    assetTypesCurrentRow++;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = assetType.TypeName;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = assetType.TypeCode;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = assetType.Description;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 4).Value = assetType.ImageUrl;
                }

                assetTypesWorksheet.Columns().AdjustToContents();

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

        [HttpGet("transport-template")]
        [SwaggerOperation("Download template import asset for transport")]
        public async Task<IActionResult> GetAssetTransportImportTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AssetTemplate");
                var currentRow = 1;

                // Define the column headers
                worksheet.Cell(currentRow, 1).Value = "Tên thiết bị";
                worksheet.Cell(currentRow, 2).Value = "Mã thiết bị";
                worksheet.Cell(currentRow, 3).Value = "Nhóm thiết bị";
                worksheet.Cell(currentRow, 4).Value = "Số lượng";

                // Apply styles to the header row
                var headerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 4));
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
                                "AssetTransport.xlsx");
                }
            }
        }

        [HttpPost]
        [SwaggerOperation("Import asset")]
        public async Task<ApiResponses<ImportError>> ImportAsset(IFormFile file)
        {
            return await _importService.ImportAssets(file);
        }

        [HttpPost("asset-transport")]
        [SwaggerOperation("Import asset for transport")]
        public async Task<ApiResponses<ImportTransportError>> ImportAssetTransport(IFormFile file)
        {
            return await _importService.ImportAssetsTransport(file);
        }
    }
}
