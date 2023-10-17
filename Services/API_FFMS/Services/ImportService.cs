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
        Task<ApiResponses<ImportError>> ImportAssets(IFormFile formFile);
    }
    public class ImportService : BaseService, IImportAssetService
    {
        private List<ImportError> validationErrors;
        public ImportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository)
            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            validationErrors = new List<ImportError>();
        }

        public async Task<ApiResponses<ImportError>> ImportAssets(IFormFile formFile)
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
                    Type = GetAssetTypeByCode(dto.TypeCode!),
                    Status = AssetStatus.Operational,
                    ManufacturingYear = dto.ManufacturingYear,
                    SerialNumber = dto.SerialNumber,
                    Quantity = dto.Quantity,
                    Description = dto.Description,
                    IsRented = IsTrueOrFalse(dto.IsRented!),
                    IsMovable = IsTrueOrFalse(dto.IsMovable!),
                    Model = GetModelByName(dto.Model!),
                    StartDateOfUse = DateTime.Now,
                    LastCheckedDate = DateTime.Now,
                    LastMaintenanceTime = DateTime.Now
                }).ToList();

                // Validation checks
                CheckAllFieldsNotBlank(assetDtos);
                CheckUniqueAssetCodes(assets);
                await CheckExistTypeCode(assets);
                await CheckUniqueAssetCodesInDatabase(assets);
                await CheckTypeCodeExistInDatabase(assets);
                CheckManufacturingYear(assets);
                CheckQuantity(assets);
                CheckStatusValueRange(assets);

                // Filter out assets with validation errors
                var validAssets = assets.Where(a => !validationErrors.Any(e => e.Row == assets.IndexOf(a) + 2)).ToList();

                if (validationErrors.Count >= 0 && validAssets.Count >= 0)
                {
                    if (!await MainUnitOfWork.AssetRepository.InsertAsync(validAssets, AccountId, CurrentDate))
                    {
                        throw new ApiException("Import assets failed", StatusCode.SERVER_ERROR);
                    }

                    var assetIds = validAssets.Select(x => x?.Id);
                    Guid wareHouseId = GetWareHouse("Kho")!.Id;
                    if(wareHouseId != Guid.Empty)
                    {
                        var roomAssets = assetIds.Select(x => new RoomAsset
                        {
                            FromDate = CurrentDate,
                            AssetId = x.Value,
                            RoomId = wareHouseId,
                            Status = AssetStatus.Operational
                        }).ToList();

                        if (!await MainUnitOfWork.RoomAssetRepository.InsertAsync(roomAssets, AccountId, CurrentDate))
                            throw new ApiException("Import assets to ware house failed", StatusCode.BAD_REQUEST);
                    }

                    return ApiResponses<ImportError>.Fail(validationErrors, StatusCode.UNPROCESSABLE_ENTITY, "Some Asset imports failed due to validation errors");
                }

                return ApiResponses<ImportError>.Success((IEnumerable<ImportError>)validAssets);


            }
            catch (Exception exception)
            {
                throw new ApiException(exception.Message);
            }
        }

        private AssetType? GetAssetTypeByCode(string typeCode)
        {
            var assetCategory = MainUnitOfWork.AssetTypeRepository.GetQuery()
                                .Where(x => x!.TypeCode.Trim().ToLower()
                                .Contains(typeCode.Trim().ToLower()))
                                .FirstOrDefault();
            return assetCategory;
        }

        private Model? GetModelByName(string modelName)
        {
            var model = MainUnitOfWork.ModelRepository.GetQuery()
                                .Where(x => x!.ModelName!.Trim().ToLower()
                                .Contains(modelName.Trim().ToLower()))
                                .FirstOrDefault();
            return model;
        }

        public Room? GetWareHouse(string roomName)
        {
            var wareHouse = MainUnitOfWork.RoomRepository.GetQuery()
                            .Where(x => x!.RoomName!.Trim()
                            .Equals(roomName.Trim()))
                            .FirstOrDefault();
            return wareHouse;
        }

        private bool IsTrueOrFalse(string value)
        {
            if (value.Trim().Equals("Có"))
            {
                return true;
            }
            else if (value.Trim().Equals("Không"))
            {
                return false;
            }
            else
            {
                throw new ApiException("Input must be 'Có' or 'Không'");
            }
        }

        private async Task CheckExistTypeCode(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingTypeCode = await MainUnitOfWork.AssetTypeRepository.GetQuery()
                                       .Where(t => t!.TypeCode.Equals(asset.Type!.TypeCode))
                                       .FirstOrDefaultAsync();
                if (existingTypeCode == null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Type code '{asset.Type!.TypeCode}' in row {row} does not exist"
                    });
                }
            }
        }

        private void CheckAllFieldsNotBlank(List<ImportAssetDto> assetDtos)
        {
            foreach (var assetDto in assetDtos)
            {
                if (string.IsNullOrWhiteSpace(assetDto.AssetName) ||
                    string.IsNullOrWhiteSpace(assetDto.AssetCode) ||
                    string.IsNullOrWhiteSpace(assetDto.TypeCode!.ToString()) ||
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
                foreach (var error in validationErrors.Where(e => e.ErrorMessage!.Contains("Duplicate AssetCodes")))
                {
                    var assetCode = error.ErrorMessage!.Split('\'')[1];
                    error.Row = duplicates.First(d => d.AssetCode == assetCode).Row;
                }
            }
        }

        private async Task CheckUniqueAssetCodesInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingAsset = await MainUnitOfWork.AssetRepository.GetQuery()
                                         .Where(a => a!.AssetCode!.Equals(asset.AssetCode))
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

        private async Task CheckTypeCodeExistInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingCategory = await MainUnitOfWork.AssetTypeRepository.GetQuery()
                                             .Where(a => a!.TypeCode.Equals(asset.Type!.TypeCode))
                                             .FirstOrDefaultAsync();

                if (existingCategory == null)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Type Code '{asset.Type}' in row {row} does not exist"
                    });
                }
            }
        }

        private void CheckManufacturingYear(List<Asset> assets)
        {
            var currentDate = DateTime.UtcNow.Year;
            var minManufacturingYear = 2000;

            foreach (var asset in assets)
            {
                if (!int.TryParse(asset.ManufacturingYear.ToString(), out int manufacturingYear) || manufacturingYear < minManufacturingYear || manufacturingYear > currentDate)
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"ManufacturingYear in row {row} is not a valid integer or is not within the range {minManufacturingYear}-{currentDate}"
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
                if (asset.Status is < 0 or > (AssetStatus)11)
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
