// using API_FFMS.Dtos;
// using AppCore.Models;
// using MainData;
// using MainData.Entities;
// using MainData.Repositories;
// using System.Linq.Expressions;
// using API_FFMS.Repositories;
// using AppCore.Extensions;
// using Microsoft.EntityFrameworkCore;
//
// namespace API_FFMS.Services
// {
//     public interface IMaintenanceScheduleService : IBaseService
//     {
//         Task<ApiResponse> CreateMaintenanceSchedule(MaintenanceScheduleConfigCreateDto createDto);
//         Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems(MaintenanceScheduleConfigQueryDto queryDto);
//         Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id);
//         Task<ApiResponse> UpdateItem(Guid id, MaintenanceScheduleConfigUpdateDto updateDto);
//         Task<ApiResponse> DeleteItem(Guid id);
//         Task<ApiResponse> DeleteItems(List<Guid> ids);
//         Task<ApiResponses<AssetMaintenanceDto>> GetMaintenanceItems(AssetQueryDto queryDto);
//     }
//
//     public class MaintenanceScheduleService : BaseService, IMaintenanceScheduleService
//     {
//         private readonly IMaintenanceScheduleRepository _maintenanceScheduleRepository;
//
//         public MaintenanceScheduleService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
//             IMapperRepository mapperRepository, IMaintenanceScheduleRepository maintenanceScheduleRepository) : base(
//             mainUnitOfWork, httpContextAccessor, mapperRepository)
//         {
//             _maintenanceScheduleRepository = maintenanceScheduleRepository;
//         }
//
//         public async Task<ApiResponse> CreateMaintenanceSchedule(MaintenanceScheduleConfigCreateDto createDto)
//         {
//             // var isExists = await MainUnitOfWork.MaintenanceScheduleRepository.GetQuery()
//             //     .Where(x => x.RepeatIntervalInMonths == createDto.RepeatIntervalInMonths).AnyAsync();
//             //
//             // if (isExists)
//             //     throw new ApiException("Đã tồn tại cài đặt", StatusCode.ALREADY_EXISTS);
//
//             var maintenanceConfig =
//                 createDto.ProjectTo<MaintenanceScheduleConfigCreateDto, MaintenanceScheduleConfig>();
//
//             var assets = new List<Asset?>();
//
//             if (createDto.AssetIds != null)
//             {
//                 var assetIds = createDto.AssetIds.Select(id => id.Id).ToList();
//                 assets = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
//                 {
//                     x => !x.DeletedAt.HasValue,
//                     x => assetIds.Contains(x.Id)
//                 }, null);
//             }
//
//             if (!await _maintenanceScheduleRepository.InsertMaintenanceScheduleConfig(maintenanceConfig, assets,
//                     AccountId, CurrentDate))
//                 throw new ApiException("Thêm mới thất bại", StatusCode.SERVER_ERROR);
//
//             return ApiResponse.Created("Thêm mới thành công");
//         }
//
//         public async Task<ApiResponses<MaintenanceScheduleConfigDto>> GetItems(
//             MaintenanceScheduleConfigQueryDto queryDto)
//         {
//             var keyword = queryDto.Keyword?.Trim().ToLower();
//             var maintenanceConfigQueryable = MainUnitOfWork.MaintenanceScheduleRepository.GetQuery();
//
//             if (!string.IsNullOrEmpty(keyword))
//             {
//                 maintenanceConfigQueryable = maintenanceConfigQueryable
//                     .Where(x => x!.Description!.ToLower().Contains(keyword)
//                                 || x.RepeatIntervalInMonths.ToString().Contains(keyword)
//                                 || x.Code.ToLower().Contains(keyword));
//             }
//
//             // Sort
//             var isDescending = queryDto.OrderBy.Split(' ').Last().ToLowerInvariant()
//                 .StartsWith("desc");
//
//             var sortField = queryDto.OrderBy.Split(' ').First();
//
//             // Sort
//             if (!string.IsNullOrEmpty(sortField))
//             {
//                 try
//                 {
//                     maintenanceConfigQueryable = maintenanceConfigQueryable.OrderBy(sortField, isDescending);
//                 }
//                 catch
//                 {
//                     throw new ApiException($"Không tồn tại trường thông tin {sortField}", StatusCode.BAD_REQUEST);
//                 }
//             }
//
//             var totalCount = await maintenanceConfigQueryable.CountAsync();
//
//             maintenanceConfigQueryable = maintenanceConfigQueryable.Skip(queryDto.Skip())
//                 .Take(queryDto.PageSize);
//
//             var items = (await maintenanceConfigQueryable.ToListAsync())!
//                 .ProjectTo<MaintenanceScheduleConfig, MaintenanceScheduleConfigDto>();
//
//             items = await _mapperRepository.MapCreator(items);
//
//             return ApiResponses<MaintenanceScheduleConfigDto>.Success(
//                 items,
//                 totalCount,
//                 queryDto.PageSize,
//                 queryDto.Page,
//                 (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
//         }
//
//         public async Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetItem(Guid id)
//         {
//             var item = (await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id))?
//                 .ProjectTo<MaintenanceScheduleConfig, MaintenanceScheduleConfigDetailDto>();
//
//             if (item == null)
//                 throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
//
//             var assets = (await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
//             {
//                 x => !x.DeletedAt.HasValue,
//                 x => x.MaintenanceConfigId == id
//             }, null))!.ProjectTo<Asset, AssetBaseDto>();
//
//             foreach (var asset in assets)
//             {
//                 asset.StatusObj = asset.Status.GetValue();
//             }
//
//             item.Assets = assets;
//             item = await _mapperRepository.MapCreator(item);
//
//             return ApiResponse<MaintenanceScheduleConfigDetailDto>.Success(item);
//         }
//
//         public async Task<ApiResponse> UpdateItem(Guid id, MaintenanceScheduleConfigUpdateDto updateDto)
//         {
//             // var checkExist = await MainUnitOfWork.MaintenanceScheduleRepository.FindAsync(
//             //     new Expression<Func<MaintenanceScheduleConfig, bool>>[]
//             //     {
//             //         x => !x.DeletedAt.HasValue,
//             //         x => x.RepeatIntervalInMonths == updateDto.RepeatIntervalInMonths
//             //     }, null);
//             //
//             // if (checkExist != null && checkExist.Any())
//             //     throw new ApiException("Đã tồn tại cài đặt", StatusCode.ALREADY_EXISTS);
//
//             var item = await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id);
//
//             if (item == null)
//                 throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
//
//             item.Description = updateDto.Description ?? item.Description;
//             item.RepeatIntervalInMonths = updateDto.RepeatIntervalInMonths ?? item.RepeatIntervalInMonths;
//
//             if (updateDto.AssetIds != null && updateDto.AssetIds.Any())
//             {
//                 var oldData = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
//                 {
//                     x => !x.DeletedAt.HasValue,
//                     x => x.MaintenanceConfigId == id
//                 }, null);
//
//                 var assetIds = updateDto.AssetIds?.Select(x => x.Id);
//                 var newData = new List<Asset?>();
//
//                 if (assetIds != null)
//                 {
//                     newData = await MainUnitOfWork.AssetRepository.FindAsync(new Expression<Func<Asset, bool>>[]
//                     {
//                         x => !x.DeletedAt.HasValue,
//                         x => assetIds.Contains(x.Id)
//                     }, null);
//                 }
//
//                 if (!await _maintenanceScheduleRepository.UpdateMaintenanceScheduleConfig(item, oldData, newData,
//                         AccountId, CurrentDate))
//                     throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
//             }
//             else
//             {
//                 if (!await MainUnitOfWork.MaintenanceScheduleRepository.UpdateAsync(item, AccountId, CurrentDate))
//                     throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);
//             }
//
//             return ApiResponse.Success("Cập nhật thành công");
//         }
//
//         public async Task<ApiResponse> DeleteItem(Guid id)
//         {
//             var item = await MainUnitOfWork.MaintenanceScheduleRepository.FindOneAsync(id);
//
//             if (item == null)
//                 throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
//
//             if (!await MainUnitOfWork.MaintenanceScheduleRepository.DeleteAsync(item, AccountId, CurrentDate))
//                 throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
//
//             return ApiResponse.Success("Xóa nội dung thành công");
//         }
//
//         public async Task<ApiResponse> DeleteItems(List<Guid> ids)
//         {
//             var items = await MainUnitOfWork.MaintenanceScheduleRepository.FindAsync(
//                 new Expression<Func<MaintenanceScheduleConfig, bool>>[]
//                 {
//                     x => !x.DeletedAt.HasValue,
//                     x => ids.Contains(x.Id)
//                 }, null);
//
//             if (items == null || !items.Any())
//                 throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
//
//             if (!await MainUnitOfWork.MaintenanceScheduleRepository.DeleteAsync(items, AccountId, CurrentDate))
//                 throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
//
//             return ApiResponse.Success("Xóa nội dung thành công");
//         }
//
//         public async Task<ApiResponses<AssetMaintenanceDto>> GetMaintenanceItems(AssetQueryDto queryDto)
//         {
//             var keyword = queryDto.Keyword?.Trim().ToLower();
//
//             var assetsQueryable = MainUnitOfWork.AssetRepository.GetQuery();
//             var maintenanceScheduleConfigQueryable = MainUnitOfWork.MaintenanceScheduleRepository.GetQuery();
//
//             var maintenanceItems = (from asset in assetsQueryable
//                 join maintenanceScheduleConfig in maintenanceScheduleConfigQueryable
//                     on asset.MaintenanceConfigId equals maintenanceScheduleConfig.Id into maintenanceScheduleConfigGroup
//                 from maintenanceScheduleConfig in maintenanceScheduleConfigGroup.DefaultIfEmpty()
//                 select new AssetMaintenanceDto
//                 {
//                     // Map other properties from your entities to the DTO
//                     Id = asset.Id,
//                     AssetName = asset.AssetName,
//                     AssetCode = asset.AssetCode,
//                     IsMovable = asset.IsMovable,
//                     Status = asset.Status,
//                     StatusObj = asset.Status.GetValue(),
//                     ManufacturingYear = asset.ManufacturingYear,
//                     SerialNumber = asset.SerialNumber,
//                     Quantity = asset.Quantity,
//                     Description = asset.Description,
//                     LastMaintenanceTime = asset.LastMaintenanceTime,
//                     LastCheckedDate = asset.LastCheckedDate,
//                     TypeId = asset.TypeId,
//                     ModelId = asset.ModelId,
//                     IsRented = asset.IsRented,
//                     StartDateOfUse = asset.StartDateOfUse,
//                     ImageUrl = asset.ImageUrl,
//                     MaintenanceConfigId = asset.MaintenanceConfigId,
//                     // Calculate the NextMaintenanceDate
//                     NextMaintenanceDate = asset.LastMaintenanceTime != null
//                         ? asset.LastMaintenanceTime.Value.AddMonths(maintenanceScheduleConfig.RepeatIntervalInMonths)
//                         : asset.StartDateOfUse.Value.AddMonths(maintenanceScheduleConfig.RepeatIntervalInMonths)
//                 });
//
//             maintenanceItems = maintenanceItems.Where(x => x.NextMaintenanceDate <= CurrentDate);
//
//             if (!string.IsNullOrEmpty(keyword))
//             {
//                 maintenanceItems = maintenanceItems.Where(x => x.AssetName.ToLower().Contains(keyword)
//                                                                || x.AssetCode.Contains(keyword)
//                                                                || x.Description.Contains(keyword));
//             }
//
//             if (queryDto.TypeId != null)
//             {
//                 maintenanceItems = maintenanceItems.Where(x => x.TypeId == queryDto.TypeId);
//             }
//
//             if (queryDto.Status != null)
//             {
//                 maintenanceItems = maintenanceItems.Where(x => x.Status == queryDto.Status);
//             }
//
//             if (queryDto.ModelId != null)
//             {
//                 maintenanceItems = maintenanceItems.Where(x => x.ModelId == queryDto.ModelId);
//             }
//
//             if (queryDto.IsMovable != null)
//             {
//                 maintenanceItems = maintenanceItems.Where(x => x.IsMovable == queryDto.IsMovable);
//             }
//
//             var totalCount = await maintenanceItems.CountAsync();
//             maintenanceItems = maintenanceItems.Skip(queryDto.Skip()).Take(queryDto.PageSize);
//
//             var items = await maintenanceItems.ToListAsync();
//
//             items = await _mapperRepository.MapCreator(items);
//
//             return ApiResponses<AssetMaintenanceDto>.Success(
//                 items,
//                 totalCount,
//                 queryDto.PageSize,
//                 queryDto.Page,
//                 (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
//             );
//         }
//     }
// }