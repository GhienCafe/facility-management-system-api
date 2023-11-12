using API_FFMS.Dtos;
using AppCore.Extensions;
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
        Task<ApiResponses<ImportTransportError>> ImportAssetsTransport(IFormFile formFile);
        Stream GetTemplate(ImportTemplate importTemplate);
    }
    public class ImportService : BaseService, IImportAssetService
    {
        private List<ImportError> validationErrors;
        private List<ImportTransportError> validationAssetErrors;

        public ImportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository)
            : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            validationErrors = new List<ImportError>();
            validationAssetErrors = new List<ImportTransportError>();
        }

        public async Task<ApiResponses<ImportTransportError>> ImportAssetsTransport(IFormFile formFile)
        {
            validationAssetErrors.Clear();
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
                var assetDtos = ExcelReader.AssetTransportReader(formFile.OpenReadStream());
                var assets = ValidateAssetTransportImportDto(assetDtos!);


                var quantityByAssetCode = assets.ToDictionary(a => a!.AssetCode, a => a!.Quantity);

                if (validationAssetErrors.Count >= 0)
                {
                    var assetImport = assets.Select(asset => new AssetTransportDto
                    {
                        AssetId = asset!.Id,
                        AssetCode = asset.AssetCode,
                        AssetName = asset.AssetName,
                        AssetType = asset.Type!.TypeName,
                        Quantity = quantityByAssetCode.ContainsKey(asset.AssetCode) ? quantityByAssetCode[asset.AssetCode] : 0.0,
                        FromRoom = asset.RoomAssets!.Where(ra => ra.AssetId.Equals(asset.Id) && ra.ToDate == null).Select(ra => ra.Room!.RoomName).FirstOrDefault()
                    }).ToList();

                    validationAssetErrors.Add(new ImportTransportError
                    {
                        AssetTransportImportDtos = assetImport
                    });

                }

                return ApiResponses<ImportTransportError>.Fail(validationAssetErrors, StatusCode.UNPROCESSABLE_ENTITY, "Some Asset imports failed due to validation errors");

            }
            catch (Exception exception)
            {
                throw new ApiException(exception.Message);
            }
        }

        public Stream GetTemplate(ImportTemplate importTemplate)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TemplateImport", importTemplate.GetDisplayName());

            if (!File.Exists(path))
                throw new ApiException("File không tồn tại", StatusCode.NOT_FOUND);
            // Open the file as a stream
            var fileStream = File.OpenRead(path);

            return fileStream;
        }

        private List<Asset> ValidateAssetTransportImportDto(List<AssetTransportImportDto> assetDtos)
        {
            var assets = new List<Asset>();

            var assetCodes = assetDtos.Select(dto => dto.AssetCode).ToList();
            var assetNames = assetDtos.Select(dto => dto.AssetName).ToList();
            var assetTypes = assetDtos.Select(dto => dto.AssetType).ToList();

            var assetQuery = MainUnitOfWork.AssetRepository
                             .GetQuery()
                             .Include(a => a!.Type)
                             .Include(a => a.RoomAssets).ThenInclude(ra => ra.Room);

            foreach (var assetDto in assetDtos)
            {
                var existingAsset = assetQuery
                    .Where(a => a!.AssetName.Contains(assetDto.AssetName!) &&
                                a.AssetCode!.Contains(assetDto.AssetCode!) &&
                                a.Type!.TypeName.Contains(assetDto.AssetType!))
                    .FirstOrDefault();

                if (existingAsset == null)
                {
                    var row = assetDtos.IndexOf(assetDto) + 2;

                    validationAssetErrors.Add(new ImportTransportError
                    {
                        Row = row,
                        ErrorMessage = $"Asset '{assetDto.AssetName}' in row '{row}' does not exist"
                    });
                }
                else
                {
                    assets.Add(existingAsset!);
                }
            }
            return assets;
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
                    Model = GetModelByName(dto.ModelCode!),
                    ImageUrl = "",
                    StartDateOfUse = DateTime.Now,
                    LastCheckedDate = null,
                    LastMaintenanceTime = null
                }).ToList();

                // Validation checks
                CheckAllFieldsNotBlank(assetDtos);
                CheckUniqueAssetCodes(assets);
                await CheckExistTypeCode(assets);
                await CheckUniqueAssetCodesInDatabase(assets);
                await CheckExistModel(assets);
                //await CheckTypeCodeExistInDatabase(assets);
                CheckManufacturingYear(assets);
                CheckQuantity(assets);
                //CheckStatusValueRange(assets);

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
                    if (wareHouseId != Guid.Empty)
                    {
                        var roomAssets = assetIds.Select(x => new RoomAsset
                        {
                            FromDate = CurrentDate,
                            AssetId = x!.Value,
                            RoomId = wareHouseId,
                            Status = AssetStatus.Operational
                        }).ToList();

                        if (!await MainUnitOfWork.RoomAssetRepository.InsertAsync(roomAssets, AccountId, CurrentDate))
                            throw new ApiException("Import assets failed", StatusCode.SERVER_ERROR);
                    }

                    return ApiResponses<ImportError>.Fail(validationErrors, StatusCode.MULTI_STATUS, "Hoàn tất nhập trang thiết bị vui lòng kiểm tra lại");
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
                            .Contains(roomName.Trim()))
                            .FirstOrDefault();
            return wareHouse;
        }

        private bool IsTrueOrFalse(string value)
        {
            if (value.Trim().Contains("Có"))
            {
                return true;
            }
            else if (value.Trim().Contains("Không"))
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
            var typeCodes = await MainUnitOfWork.AssetTypeRepository.GetQuery()
                .Select(x => x!.TypeCode).ToListAsync();
            foreach (var asset in assets)
            {
                if (!typeCodes.Contains(asset.Type!.TypeCode))
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Mã nhóm thiết bị '{asset.Type!.TypeCode}' ở dòng {row} không tồn tại"
                    });
                }
            }
        }

        private async Task CheckExistModel(List<Asset> assets)
        {
            var modelCodes = await MainUnitOfWork.ModelRepository.GetQuery()
                .Select(x => x!.ModelName).ToListAsync();
            foreach (var asset in assets)
            {
                if(!modelCodes.Contains(asset.Model!.ModelName))
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Nhãn hiệu '{asset.Model!.ModelName}' ở dòng {row} không tồn tại"
                    });
                }
            }
        }

        private void CheckAllFieldsNotBlank(List<ImportAssetDto> assetDtos)
        {
            foreach (var assetDto in assetDtos)
            {
                if (string.IsNullOrWhiteSpace(assetDto.AssetName) ||
                    string.IsNullOrWhiteSpace(assetDto.TypeCode) ||
                    string.IsNullOrWhiteSpace(assetDto.ManufacturingYear.ToString()) ||
                    string.IsNullOrWhiteSpace(assetDto.Quantity.ToString()))
                {
                    var row = assetDtos.IndexOf(assetDto) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Tất cả các thông tin phải được điền, đang trống tại dòng {row}"
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

                var duplicateError = string.Join(", ", duplicates.Select(d => $"'{d.AssetCode}' tại dòng {d.Row}"));
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
            var assetCodes = await MainUnitOfWork.AssetRepository.GetQuery()
                .Select(x => x!.AssetCode)
                .ToListAsync();
            foreach (var asset in assets)
            {
                if (assetCodes.Contains(asset.AssetCode))
                {
                    var row = assets.IndexOf(asset) + 2;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Duplicate AssetCode '{asset.AssetCode}' tại dòng {row}"
                    });
                }
            }
        }

        private async Task CheckTypeCodeExistInDatabase(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                var existingCategory = await MainUnitOfWork.AssetTypeRepository.GetQuery()
                                             .Where(a => a!.TypeCode.Contains(asset.Type!.TypeCode))
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
                        ErrorMessage = $"Năm sản xuất tại dòng {row} không phù hợp hoặc không trong phạm vi {minManufacturingYear}-{currentDate}"
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
                        ErrorMessage = $"Số lượng tại dòng {row} phải lớn hơn 0"
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
