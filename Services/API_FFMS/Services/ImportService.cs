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
        Task<ApiResponse<ImportError>> ImportAssets(IFormFile formFile);
    }
    public class ImportService : BaseService, IImportAssetService
    {
        private List<ImportError> validationErrors;
        public ImportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository)
            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            validationErrors = new List<ImportError>();
        }

        public async Task<ApiResponse<ImportError>> ImportAssets(IFormFile formFile)
        {
            validationErrors.Clear();

            if (formFile == null)
            {
                throw new ApiException("Not recognized file");
            }

            string[] extensions = { ".xlsx", ".xls" };
            if (!extensions.Contains(Path.GetExtension(formFile.FileName)))
            {
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

                // Filter out assets with validation errors
                var validAssets = assets.Where(a => !validationErrors.Any(e => e.Row == assets.IndexOf(a) + 2)).ToList();

                if (validationErrors.Count > 0 && validAssets.Count > 0)
                {
                    if (!await MainUnitOfWork.AssetRepository.InsertAsync(validAssets, AccountId, CurrentDate))
                    {
                        throw new ApiException("Import assets failed", StatusCode.SERVER_ERROR);
                    }
                    return ApiResponse<ImportError>.Failed("Import failed due to validation errors", StatusCode.BAD_REQUEST, validationErrors);
                }

                //if (validAssets.Count > 0)
                //{
                    
                //}

                return (ApiResponse<ImportError>)ApiResponse<ImportError>.Success();
            }
            catch (Exception exception)
            {
                throw new ApiException(exception.Message);
            }
        }

        private AssetCategory? GetAssetCategoryByCode(string? categoryCode)
        {
            var assetCategory = MainUnitOfWork.AssetCategoryRepository.GetQuery()
                                .Where(x => x.CategoryCode.Trim().ToLower()
                                .Contains(categoryCode.Trim().ToLower()))
                                .FirstOrDefault();
            if (assetCategory == null)
            {
                throw new ApiException("Category code does not exist", StatusCode.BAD_REQUEST);
            }

            return assetCategory;
        }

        private void CheckAllFieldsNotBlank(List<ImportAssetDto> assetDtos)
        {
            //for (var i = 0; i < assetDtos.Count; i++)
            //{
            //    var dto = assetDtos[i];
            //    if (string.IsNullOrWhiteSpace(dto.AssetName) ||
            //        string.IsNullOrWhiteSpace(dto.AssetCode) ||
            //        string.IsNullOrWhiteSpace(dto.CategoryCode) ||
            //        dto.Status == default(AssetStatus) ||
            //        dto.ManufacturingYear == default ||
            //        dto.Quantity <= 0)
            //    {
            //        //throw new ApiException($"All fields must be filled in for row {i + 2}");
            //        validationErrors.Add(new ImportError
            //        {
            //            ErrorMessage = $"All fields must be filled in row {i + 2}"
            //        });
            //    }
            //}
            foreach (var assetDto in assetDtos)
            {
                if (string.IsNullOrWhiteSpace(assetDto.AssetName) ||
                    string.IsNullOrWhiteSpace(assetDto.AssetCode) ||
                    string.IsNullOrWhiteSpace(assetDto.CategoryCode) ||
                    string.IsNullOrWhiteSpace(assetDto.Status.ToString()) ||
                    string.IsNullOrWhiteSpace(assetDto.ManufacturingYear.ToString()) ||
                    string.IsNullOrWhiteSpace(assetDto.Quantity.ToString()))
                {
                    var row = assetDtos.IndexOf(assetDto) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"All fields must be filled in row {row}"
                    });
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

                var duplicateError = string.Join(", ", duplicates.Select(d => $"'{d.AssetCode}' in row {d.Row}"));
                validationErrors.Add(new ImportError 
                { 
                    ErrorMessage = $"Duplicate AssetCodes: {duplicateError}" 
                });
                // Set the Row property for each validation error
                foreach (var error in validationErrors.Where(e => e.ErrorMessage.Contains("Duplicate AssetCodes")))
                {
                    var assetCode = error.ErrorMessage.Split('\'')[1];
                    error.Row = duplicates.First(d => d.AssetCode == assetCode).Row;
                }
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

                // Set the Row property for each validation error
                foreach (var error in validationErrors.Where(e => e.ErrorMessage.Contains("Duplicate AssetCategories")))
                {
                    var assetCode = error.ErrorMessage.Split('\'')[1];
                    error.Row = duplicates.First(d => d.CategoryCode == assetCode).Row;
                }
            }
        }

        private async Task CheckUniqueAssetCodesInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingAsset = await MainUnitOfWork.AssetRepository.GetQuery()
                                         .Where(a => a.AssetCode.Equals(asset.AssetCode))
                                         .FirstOrDefaultAsync();

                if (existingAsset != null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Duplicate AssetCode '{asset.AssetCode}' in row {row}"
                    });
                }
            }
        }

        private async Task CheckCategoryCodeExistInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingCategory = await MainUnitOfWork.AssetCategoryRepository.GetQuery()
                                             .Where(a => a.CategoryCode.Equals(asset.AssetCategory.CategoryCode))
                                             .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError 
                    { 
                        Row = row, 
                        ErrorMessage = $"Category Code '{asset.AssetCategory}' in row {row} does not exist" 
                    });
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
                    validationErrors.Add(new ImportError 
                    { 
                        Row = row, 
                        ErrorMessage = $"ManufacturingYear in row {row} is not before the current date" 
                    });
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
                    validationErrors.Add(new ImportError 
                    { 
                        Row = row,
                        ErrorMessage = $"Quantity in row {row} must be greater than 0" 
                    });
                }
            }
        }

        private void CheckStatusValueRange(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                if (asset.Status is < 0 or > (AssetStatus)10)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError 
                    { 
                        Row = row, 
                        ErrorMessage = $"Status value must be between 0 and 10 for row {row}" 
                    });
                }
            }
        }
    }
}
