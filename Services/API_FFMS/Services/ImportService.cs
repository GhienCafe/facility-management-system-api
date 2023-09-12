using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface IImportAssetService : IBaseService
    {
        Task<ApiResponse> ImportAssets(IFormFile formFile);
    }
    public class ImportService : BaseService, IImportAssetService
    {
        public ImportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> ImportAssets(IFormFile formFile)
        {
            if (formFile == null) {
                throw new ApiException("Not recognized file");
            }

            string[] extensions = { ".xlsx", ".xls" };
            if (!extensions.Contains(Path.GetExtension(formFile.FileName))) {
                throw new ApiException("Not supported file extension");
            }

            try
            {
                var assetDtos = ExcelReader.AssetReader(formFile.OpenReadStream());

                var assets = assetDtos.Select(dto => new Asset
                {
                    AssetName = dto.AssetName,
                    AssetCode = dto.AssetCode,
                    AssetCategory = GetAssetCategoryByCode(dto.CategoryCode),
                    Status = dto.Status,
                    ManufacturingYear = dto.ManufacturingYear,
                    SerialNumber = dto.SerialNumber,
                    Quantity = dto.Quantity,
                    Description = dto.Description
                }).ToList();

                // Validation checks
                CheckAllFieldsNotBlank(assetDtos);
                CheckUniqueAssetCodes(assets);
                await CheckUniqueAssetCodesInDatabase(assets);
                await CheckCategoryCodeExistInDatabase(assets);
                CheckManufacturingYear(assets);
                CheckQuantity(assets);
                CheckStatusValueRange(assets);


                if (!await MainUnitOfWork.AssetRepository.InsertAsync(assets, AccountId, CurrentDate))
                {
                    throw new ApiException("Import assets failed", StatusCode.SERVER_ERROR);
                }

                return ApiResponse.Success();
            }
            catch (Exception exception)
            {
                throw new ApiException(exception.Message);
            }
        }

        private AssetCategory? GetAssetCategoryByCode(string? categoryCode)
        {
            var assetCategory = MainUnitOfWork.AssetCategoryRepository.GetQuery()
                                .Where(x => x.CategoryCode.Trim().ToLower().Contains(categoryCode.Trim().ToLower()))
                                .FirstOrDefault();
            if(assetCategory == null)
            {
                throw new ApiException("Category code does not exist", StatusCode.BAD_REQUEST);
            }

            return assetCategory;
        }

        private void CheckAllFieldsNotBlank(List<ImportAssetDto> assetDtos)
        {
            for (var i = 0; i < assetDtos.Count; i++)
            {
                var dto = assetDtos[i];
                if (string.IsNullOrWhiteSpace(dto.AssetName) ||
                    string.IsNullOrWhiteSpace(dto.AssetCode) ||
                    string.IsNullOrWhiteSpace(dto.CategoryCode) ||
                    dto.ManufacturingYear == default ||
                    dto.Quantity <= 0)
                {
                    throw new ApiException($"All fields must be filled in for row {i + 2}");
                }
            }
        }

        private void CheckUniqueAssetCodes(List<Asset> assets)
        {
            var duplicateCodes = assets
                .GroupBy(a => a.AssetCode)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCodes.Any())
            {
                var duplicates = assets
                    .Where(a => duplicateCodes.Contains(a.AssetCode))
                    .Select(a => new
                    {
                        AssetCode = a.AssetCode,
                        Row = assets.IndexOf(a) + 2 // +2 because Excel rows are 1-based, and we skip the header row
                    })
                    .ToList();

                var duplicateError = string.Join(", ", duplicates.Select(d => $"AssetCode '{d.AssetCode}' in row {d.Row}"));
                throw new ApiException($"Duplicate AssetCodes: {duplicateError}");
            }
        }

        private void CheckUniqueAssetCategories(List<Asset> assets)
        {
            var duplicateCategories = assets
                .GroupBy(a => a.AssetCategory?.CategoryCode)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCategories.Any())
            {
                var duplicates = assets
                    .Where(a => duplicateCategories.Contains(a.AssetCategory?.CategoryCode))
                    .Select(a => new
                    {
                        CategoryCode = a.AssetCategory?.CategoryCode,
                        Row = assets.IndexOf(a) + 2
                    })
                    .ToList();

                var duplicateError = string.Join(", ", duplicates.Select(d => $"AssetCategory '{d.CategoryCode}' in row {d.Row}"));
                throw new ApiException($"Duplicate AssetCategories: {duplicateError}");
            }
        }

        private async Task CheckUniqueAssetCodesInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingAsset = await MainUnitOfWork.AssetRepository.GetQuery()
                                         .Where(a => a.AssetCode.Trim().ToLower()
                                         .Contains(asset.AssetCode.Trim().ToLower()))
                                         .FirstOrDefaultAsync();

                if (existingAsset != null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    throw new ApiException($"Duplicate AssetCode '{asset.AssetCode}' in row {row}");
                }
            }
        }

        private async Task CheckCategoryCodeExistInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingCategory = await MainUnitOfWork.AssetCategoryRepository.GetQuery()
                                             .Where(a => a.CategoryCode.Trim().ToLower().Equals(asset.AssetCategory.ToString().Trim().ToLower()))
                                             .FirstOrDefaultAsync();

                if(existingCategory == null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    throw new ApiException($"Category Code '{asset.AssetCategory}' in row {row} does not exist");
                }
            }
        }

        private void CheckManufacturingYear(List<Asset> assets)
        {
            var currentDate = DateTime.UtcNow;

            foreach (var asset in assets)
            {
                if (asset.ManufacturingYear >= currentDate)
                {
                    var row = assets.IndexOf(asset) + 2;
                    throw new ApiException($"ManufacturingYear in row {row} is not before the current date");
                }
            }
        }

        private void CheckQuantity(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                if (asset.Quantity <= 0)
                {
                    var row = assets.IndexOf(asset) + 2;
                    throw new ApiException($"Quantity in row {row} must be greater than 0");
                }
            }
        }

        private void CheckStatusValueRange(List<Asset> assets)
        {
            for (var i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                if (asset.Status is < 0 or > (AssetStatus)10)
                {
                    throw new ApiException($"Status value must be between 0 and 10 for row {i + 2}");
                }
            }
        }
    }
}
