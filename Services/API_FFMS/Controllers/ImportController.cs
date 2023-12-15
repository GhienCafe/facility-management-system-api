using API_FFMS.Dtos;
using API_FFMS.Services;
using AppCore.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace API_FFMS.Controllers
{
    public class ImportController : BaseController
    {
        private readonly IImportAssetService _importService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IModelService _modelService;

        public ImportController(IImportAssetService assetService,
                                IAssetTypeService assetTypeService,
                                IModelService modelService)
        {
            _importService = assetService;
            _assetTypeService = assetTypeService;
            _modelService = modelService;
        }

        [HttpGet]
        [SwaggerOperation("Download template import asset")]
        public async Task<IActionResult> GetAssetImportTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Thiết bị");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Danh sách trang thiết bị";
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 18;
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(1, 1, 1, 11).Row(1).Merge();
                worksheet.Range(1, 1, 1, 11).Style.Font.Bold = true;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Tên thiết bị*";
                worksheet.Cell(currentRow, 2).Value = "Mã thiết bị*";
                worksheet.Cell(currentRow, 3).Value = "Mã nhóm thiết bị*";
                worksheet.Cell(currentRow, 4).Value = "Mã dòng sản phẩm*";
                worksheet.Cell(currentRow, 5).Value = "Năm sản xuất*";
                worksheet.Cell(currentRow, 6).Value = "Số Seri";
                worksheet.Cell(currentRow, 7).Value = "Số lượng*";
                worksheet.Cell(currentRow, 8).Value = "Mô tả";
                worksheet.Cell(currentRow, 9).Value = "Thuê ngoài(Có/Không)";
                worksheet.Cell(currentRow, 10).Value = "Có thể di chuyển(Có/Không)";
                worksheet.Cell(currentRow, 11).Value = "Mã phòng";

                var headerRange = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 11));
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();

                var assetTypesWorksheet = workbook.Worksheets.Add("Nhóm thiết bị");
                var assetTypesCurrentRow = 1;
                var assetTypes = await _assetTypeService.GetAssetTypes();

                //Asset types
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = "Danh sách loại thiết bị";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Font.FontSize = 18;
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                assetTypesWorksheet.Range(1, 1, 1, 4).Row(1).Merge();
                assetTypesWorksheet.Range(1, 1, 1, 4).Style.Font.Bold = true;
                assetTypesCurrentRow++;

                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = "Tên nhóm thiết bị";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = "Mã nhóm thiết bị";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = "Mô tả";

                var assetTypesHeaderRange = assetTypesWorksheet.Range(assetTypesWorksheet.Cell(assetTypesCurrentRow, 1), assetTypesWorksheet.Cell(assetTypesCurrentRow, 3));
                assetTypesHeaderRange.Style.Font.Bold = true;
                assetTypesHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                assetTypesHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                assetTypesHeaderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                foreach (var assetType in assetTypes.Data)
                {
                    assetTypesCurrentRow++;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = assetType.TypeName;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = assetType.TypeCode;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = assetType.Description;
                }
                assetTypesCurrentRow++;
                //Models
                assetTypesCurrentRow++;

                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = "Danh sách dòng sản phẩm";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Font.FontSize = 18;
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                assetTypesWorksheet.Range(assetTypesCurrentRow, 1, assetTypesCurrentRow, 4).Row(1).Merge();
                assetTypesWorksheet.Range(assetTypesCurrentRow, 1, assetTypesCurrentRow, 4).Style.Font.Bold = true;

                assetTypesCurrentRow++;
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = "Tên Dòng sản phẩm";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = "Mã dòng sản phẩm";
                assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = "Mô tả";

                var modelsHeaderRange = assetTypesWorksheet.Range(assetTypesWorksheet.Cell(assetTypesCurrentRow, 1), assetTypesWorksheet.Cell(assetTypesCurrentRow, 3));
                modelsHeaderRange.Style.Font.Bold = true;
                modelsHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                modelsHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                modelsHeaderRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                assetTypesCurrentRow++;

                var models = await _modelService.GetModels();
                foreach ( var model in models.Data)
                {
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 1).Value = model.ModelName;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 2).Value = model.ModelCode;
                    assetTypesWorksheet.Cell(assetTypesCurrentRow, 3).Value = model.Description;
                    assetTypesCurrentRow++;
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
        
        [HttpGet("template")]
        [SwaggerOperation("Get import template")]
        public IActionResult   GetTemplate([FromQuery]ImportTemplate template)
        {
            var fileStream = _importService.GetTemplate(template);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileStream, contentType, $"{template.GetDisplayName()}.xlsx");
        }
    }
}
