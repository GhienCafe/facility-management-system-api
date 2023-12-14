using API_FFMS.Dtos;
using API_FFMS.Repositories;
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
        private readonly IRoomAssetRepository _roomAssetRepository;
        private readonly IAssetRepository _assetRepository;
        private List<ImportError> validationErrors;
        private List<ImportTransportError> validationAssetErrors;

        public ImportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                             IMapperRepository mapperRepository, IAssetRepository assetRepository,
                             IRoomAssetRepository roomAssetRepository)
                             : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            validationErrors = new List<ImportError>();
            validationAssetErrors = new List<ImportTransportError>();
            _assetRepository = assetRepository;
            _roomAssetRepository = roomAssetRepository;
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

                return ApiResponses<ImportTransportError>.Response(validationAssetErrors, StatusCode.UNPROCESSABLE_ENTITY, "Một vài thiết bị được thêm mới có thể lỗi trong quá trình xác thực");

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
                    var row = assetDtos.IndexOf(assetDto) + 3;

                    validationAssetErrors.Add(new ImportTransportError
                    {
                        Row = row,
                        ErrorMessage = $"Thiết bị '{assetDto.AssetName}' tại dòng '{row}' không tồn tại"
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

            var importDtos = ExcelReader.AssetReader(formFile.OpenReadStream());
            await ValidationImport(importDtos);

            var validDtos = importDtos.Where(a => !validationErrors.Any(e => e.Row == importDtos.IndexOf(a) + 3)).ToList(); ;

            var assets = validDtos.Select(dto => new Asset
            {
                Id = dto.Id,
                AssetName = dto.AssetName,
                AssetCode = dto.AssetCode,
                TypeId = GetAssetTypeByCode(dto.TypeCode),
                Status = AssetStatus.Operational,
                ManufacturingYear = int.Parse(dto.ManufacturingYear),
                SerialNumber = dto.SerialNumber,
                Quantity = double.Parse(dto.Quantity),
                Description = dto.Description ?? "",
                IsRented = IsTrueOrFalse(dto.IsRented!),
                IsMovable = IsTrueOrFalse(dto.IsMovable!),
                ModelId = GetModelByCode(dto.ModelCode),
                ImageUrl = "",
                StartDateOfUse = DateTime.Now,
                LastCheckedDate = null,
                LastMaintenanceTime = null
            }).ToList();

            var totalCount = assets.Count();

            var roomAssets = validDtos.Where(dto => assets.Any(v => v.Id == dto.Id))
                                          .Select(dto => new RoomAsset
                                            {
                                                RoomId = GetRoomId(dto.RoomCode),
                                                AssetId = dto.Id,
                                                Quantity = double.Parse(dto.Quantity)
                                          }).ToList();

            if (validationErrors.Count >= 0 && validDtos.Count >= 0)
            {
                if (!await _assetRepository.InsertAssets(assets, AccountId, CurrentDate))
                {
                    throw new ApiException("Nhập danh sách thiết bị thất bại", StatusCode.SERVER_ERROR);
                }

                if (!await _roomAssetRepository.AddAssetToRooms(roomAssets, AccountId, CurrentDate))
                {
                    throw new ApiException("Thêm thiết bị vào phòng thất bại", StatusCode.SERVER_ERROR);
                }

                return ApiResponses<ImportError>.Response(validationErrors, StatusCode.MULTI_STATUS, "Hoàn tất nhập trang thiết bị vui lòng kiểm tra lại");
            }

            return ApiResponses<ImportError>.Success((IEnumerable<ImportError>)assets);
        }

        private Guid GetAssetTypeByCode(string typeCode)
        {
            var assetCategory = MainUnitOfWork.AssetTypeRepository.GetQuery()
                                .Where(x => x!.TypeCode.Trim().ToLower()
                                .Contains(typeCode.Trim().ToLower()))
                                .FirstOrDefault();
            if (assetCategory == null)
            {
                return Guid.NewGuid();
            }

            return assetCategory.Id;

        }

        private Guid GetModelByCode(string modelCode)
        {
            var model = MainUnitOfWork.ModelRepository.GetQuery()
                                .Where(x => x!.ModelCode!.Trim().ToLower()
                                .Contains(modelCode.Trim().ToLower()))
                                .FirstOrDefault();
            if (model == null)
            {
                return Guid.NewGuid();
            }
            return model.Id;
        }

        public Guid GetRoomId(string roomCode)
        {
            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();

            var room = roomQuery.Where(x => x!.RoomName!.Trim()
                                .Contains(roomCode.Trim()))
                                .FirstOrDefault();

            room ??= roomQuery.Where(x => x!.RoomName!.Trim()
                                .Contains("207".Trim()))
                                .FirstOrDefault();

            return room.Id;
        }

        private static bool IsTrueOrFalse(string value)
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

        private async Task ValidationImport(List<ImportAssetDto> assetDtos)
        {
            var currentDate = DateTime.UtcNow.Year;
            var minManufacturingYear = 2000;

            var typeCodes = await MainUnitOfWork.AssetTypeRepository.GetQuery()
                .Select(x => x!.Id).ToListAsync();

            var modelCodes = await MainUnitOfWork.ModelRepository.GetQuery()
                .Select(x => x!.Id).ToListAsync();

            var assetCodes = await MainUnitOfWork.AssetRepository.GetQuery()
                .Select(x => x!.AssetCode)
                .ToListAsync();

            var roomCodes = await MainUnitOfWork.RoomRepository.GetQuery()
                .Select(x => x!.RoomCode)
                .ToListAsync();

            foreach (var assetDto in assetDtos)
            {
                //Check blank
                if (string.IsNullOrWhiteSpace(assetDto.AssetName) ||
                    string.IsNullOrWhiteSpace(assetDto.TypeCode) ||
                    string.IsNullOrWhiteSpace(assetDto.ManufacturingYear.ToString()) ||
                    string.IsNullOrWhiteSpace(assetDto.Quantity.ToString())
                    )
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Tất cả các thông tin phải được điền, đang trống tại dòng {row}"
                    });
                }

                //Check Exist Model
                if (!modelCodes.Contains(GetModelByCode(assetDto.ModelCode)))
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Dòng sản phẩm ở dòng {row} không tồn tại"
                    });
                }

                //Check Unique Asset Codes In Database
                if (assetCodes.Contains(assetDto.AssetCode))
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Duplicate AssetCode '{assetDto.AssetCode}' tại dòng {row}"
                    });
                }

                //Check Exist Type Code
                if (!typeCodes.Contains(GetAssetTypeByCode(assetDto.TypeCode)))
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Mã nhóm thiết bị ở dòng {row} không tồn tại"
                    });
                }

                //Check Manufacturing Year
                if (!int.TryParse(assetDto.ManufacturingYear.ToString(), out int manufacturingYear) || manufacturingYear < minManufacturingYear || manufacturingYear > currentDate)
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Năm sản xuất tại dòng {row} không phù hợp hoặc không trong phạm vi {minManufacturingYear}-{currentDate}"
                    });
                }

                //Check Quantity
                if (!double.TryParse(assetDto.Quantity.ToString(), out double quantity) || double.Parse(assetDto.Quantity) <= 0)
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = row,
                        ErrorMessage = $"Số lượng tại dòng {row} sai định dạng"
                    });
                }

                //Check Room Code
                if (!roomCodes.Contains(assetDto.RoomCode))
                {
                    var row = assetDtos.IndexOf(assetDto) + 3;
                    validationErrors.Add(new ImportError
                    {
                        Row = 0,
                        ErrorMessage = $"Mã phòng tại dòng {row} không tồn tại, thiết bị được chuyển vào 'kho'"
                    });
                }
            }

            //Check unique asset code
            var duplicateCodes = assetDtos
            .GroupBy(a => a.AssetCode)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            if (duplicateCodes.Any())
            {
                var duplicates = assetDtos
                    .Where(a => duplicateCodes.Contains(a.AssetCode))
                    .Select(a => new
                    {
                        AssetCode = a.AssetCode,
                        Row = assetDtos.IndexOf(a) + 3 // +2 because Excel rows are 1-based, and we skip the header row
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

        //private async Task ValidationImport(List<Asset> assets)
        //{
        //    var currentDate = DateTime.UtcNow.Year;
        //    var minManufacturingYear = 2000;

        //    var typeCodes = await MainUnitOfWork.AssetTypeRepository.GetQuery()
        //        .Select(x => x!.Id).ToListAsync();

        //    var modelCodes = await MainUnitOfWork.ModelRepository.GetQuery()
        //        .Select(x => x!.Id).ToListAsync();

        //    var assetCodes = await MainUnitOfWork.AssetRepository.GetQuery()
        //        .Select(x => x!.AssetCode)
        //        .ToListAsync();

            

        //    foreach (var asset in assets)
        //    {
        //        //Check Exist Model
        //        if (!modelCodes.Contains((Guid)asset.ModelId))
        //        {
        //            var row = assets.IndexOf(asset) + 3;
        //            validationErrors.Add(new ImportError
        //            {
        //                Row = row,
        //                ErrorMessage = $"Dòng sản phẩm ở dòng {row} không tồn tại"
        //            });
        //        }

        //        //Check Unique Asset Codes In Database
        //        if (assetCodes.Contains(asset.AssetCode))
        //        {
        //            var row = assets.IndexOf(asset) + 3;
        //            validationErrors.Add(new ImportError
        //            {
        //                Row = row,
        //                ErrorMessage = $"Duplicate AssetCode '{asset.AssetCode}' tại dòng {row}"
        //            });
        //        }

        //        //Check Exist Type Code
        //        if (!typeCodes.Contains((Guid)asset.TypeId))
        //        {
        //            var row = assets.IndexOf(asset) + 3;
        //            validationErrors.Add(new ImportError
        //            {
        //                Row = row,
        //                ErrorMessage = $"Mã nhóm thiết bị ở dòng {row} không tồn tại"
        //            });
        //        }

        //        //Check Manufacturing Year
        //        if (!int.TryParse(asset.ManufacturingYear.ToString(), out int manufacturingYear) || manufacturingYear < minManufacturingYear || manufacturingYear > currentDate)
        //        {
        //            var row = assets.IndexOf(asset) + 3;
        //            validationErrors.Add(new ImportError
        //            {
        //                Row = row,
        //                ErrorMessage = $"Năm sản xuất tại dòng {row} không phù hợp hoặc không trong phạm vi {minManufacturingYear}-{currentDate}"
        //            });
        //        }

        //        //Check Quantity
        //        if (!int.TryParse(asset.Quantity.ToString(), out int quantity) || asset.Quantity <= 0)
        //        {
        //            var row = assets.IndexOf(asset) + 3;
        //            validationErrors.Add(new ImportError
        //            {
        //                Row = row,
        //                ErrorMessage = $"Số lượng tại dòng {row} sai định dạng"
        //            });
        //        }
        //    }

        //    //Check unique asset code
        //    var duplicateCodes = assets
        //    .GroupBy(a => a.AssetCode)
        //    .Where(g => g.Count() > 1)
        //    .Select(g => g.Key)
        //    .ToList();

        //    if (duplicateCodes.Any())
        //    {
        //        var duplicates = assets
        //            .Where(a => duplicateCodes.Contains(a.AssetCode))
        //            .Select(a => new
        //            {
        //                AssetCode = a.AssetCode,
        //                Row = assets.IndexOf(a) + 3 // +2 because Excel rows are 1-based, and we skip the header row
        //            })
        //            .ToList();

        //        var duplicateError = string.Join(", ", duplicates.Select(d => $"'{d.AssetCode}' tại dòng {d.Row}"));
        //        validationErrors.Add(new ImportError
        //        {
        //            ErrorMessage = $"Duplicate AssetCodes: {duplicateError}"
        //        });
        //        // Set the Row property for each validation error
        //        foreach (var error in validationErrors.Where(e => e.ErrorMessage!.Contains("Duplicate AssetCodes")))
        //        {
        //            var assetCode = error.ErrorMessage!.Split('\'')[1];
        //            error.Row = duplicates.First(d => d.AssetCode == assetCode).Row;
        //        }
        //    }
        //}
    }
}
