using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services;

public interface IMaintenanceService : IBaseService
{
    Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto);
    Task<ApiResponse<MaintenanceDto>> GetItem(Guid id);
    Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto);
    Task<ApiResponse> CreateItems(List<MaintenanceCreateDto> createDtos);
    Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto);
    Task<ApiResponse> DeleteItem(Guid id);
    Task<ApiResponse> DeleteItems(DeleteMutilDto deleteDto);
    Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
}

public class MaintenanceService : BaseService, IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    public MaintenanceService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IMaintenanceRepository maintenanceRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }


    public async Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var maintenanceQueryable = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.AssetId != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.AssetId == queryDto.AssetId);
        }

        if (queryDto.IsInternal != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.IsInternal == queryDto.IsInternal);
        }

        if (queryDto.AssignedTo != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.Status == queryDto.Status);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            maintenanceQueryable = maintenanceQueryable.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                               || x.Notes!.ToLower().Contains(keyword) ||
                                                               x.RequestCode.ToLower().Contains(keyword));
        }

        var assetQueryable = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var userQueryable = MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var joinTables = from maintenance in maintenanceQueryable
                         join user in userQueryable on maintenance.AssignedTo equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         join asset in assetQueryable on maintenance.AssetId equals asset.Id into assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                         from assetType in assetTypeGroup.DefaultIfEmpty()
                         join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         select new
                         {
                             Maintenance = maintenance,
                             User = user,
                             Asset = asset,
                             AssetType = assetType,
                             Category = category
                         };

        var totalCount = await joinTables.CountAsync();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var items = await joinTables.Select(x => new MaintenanceDto
        {
            Id = x.Maintenance.Id,
            Status = x.Maintenance.Status,
            AssetId = x.Maintenance.AssetId,
            Notes = x.Maintenance.Notes,
            Description = x.Maintenance.Description,
            IsInternal = x.Maintenance.IsInternal,
            AssignedTo = x.Maintenance.AssignedTo,
            RequestCode = x.Maintenance.RequestCode,
            CompletionDate = x.Maintenance.CompletionDate,
            Checkin = x.Maintenance.Checkin,
            Checkout = x.Maintenance.Checkout,
            Result = x.Maintenance.Result,
            RequestDate = x.Maintenance.RequestDate,
            CategoryId = x.Maintenance.CategoryId,
            AssetTypeId = x.Maintenance.AssetTypeId,
            StatusObj = x.Maintenance.Status!.GetValue(),
            CreatedAt = x.Maintenance.CreatedAt,
            Priority = x.Maintenance.Priority,
            EditedAt = x.Maintenance.EditedAt,
            CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
            EditorId = x.Maintenance.EditorId ?? Guid.Empty,
            User = x.User.ProjectTo<User, UserBaseDto>(),
            Asset = x.Asset.ProjectTo<Asset, AssetBaseDto>(),
            AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
            Category = x.Category.ProjectTo<Category, CategoryDto>(),
            PriorityObj = x.Maintenance.Priority.GetValue()
        }).ToListAsync();

        foreach (var item in items)
        {
            if (item.User != null)
            {
                item.User.StatusObj = item.User.Status?.GetValue();
                item.User.RoleObj = item.User.Role?.GetValue();
            }

            if (item.Asset != null)
            {
                item.Asset.StatusObj = item.Asset.Status?.GetValue();
            }
        }

        items = await _mapperRepository.MapCreator(items);

        return ApiResponses<MaintenanceDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse<MaintenanceDto>> GetItem(Guid id)
    {
        var maintenanceDto = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Id == id);

        if (maintenanceDto == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

        var assetQueryable = MainUnitOfWork.AssetRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var userQueryable = MainUnitOfWork.UserRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var assetTypeQueryable = MainUnitOfWork.AssetTypeRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var categoryQueryable = MainUnitOfWork.CategoryRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        var joinTables = from maintenance in maintenanceDto
                         join user in userQueryable on maintenance.AssignedTo equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         join asset in assetQueryable on maintenance.AssetId equals asset.Id into assetGroup
                         from asset in assetGroup.DefaultIfEmpty()
                         join assetType in assetTypeQueryable on asset.TypeId equals assetType.Id into assetTypeGroup
                         from assetType in assetTypeGroup.DefaultIfEmpty()
                         join category in categoryQueryable on assetType.CategoryId equals category.Id into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         select new
                         {
                             Maintenance = maintenance,
                             User = user,
                             Asset = asset,
                             AssetType = assetType,
                             Category = category
                         };

        var item = await joinTables.Select(x => new MaintenanceDto
        {
            Id = x.Maintenance.Id,
            Status = x.Maintenance.Status,
            AssetId = x.Maintenance.AssetId,
            Notes = x.Maintenance.Notes,
            Description = x.Maintenance.Description,
            IsInternal = x.Maintenance.IsInternal,
            AssignedTo = x.Maintenance.AssignedTo,
            RequestCode = x.Maintenance.RequestCode,
            CompletionDate = x.Maintenance.CompletionDate,
            RequestDate = x.Maintenance.RequestDate,
            Result = x.Maintenance.Result,
            Checkout = x.Maintenance.Checkout,
            Checkin = x.Maintenance.Checkout,
            CategoryId = x.Maintenance.CategoryId,
            AssetTypeId = x.Maintenance.AssetTypeId,
            Priority = x.Maintenance.Priority,
            StatusObj = x.Maintenance.Status!.GetValue(),
            CreatedAt = x.Maintenance.CreatedAt,
            EditedAt = x.Maintenance.EditedAt,
            CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
            EditorId = x.Maintenance.EditorId ?? Guid.Empty,
            User = x.User.ProjectTo<User, UserBaseDto>(),
            Asset = x.Asset.ProjectTo<Asset, AssetBaseDto>(),
            AssetType = x.AssetType.ProjectTo<AssetType, AssetTypeDto>(),
            Category = x.Category.ProjectTo<Category, CategoryDto>(),
            PriorityObj = x.Maintenance.Priority.GetValue()
        }).FirstOrDefaultAsync();

        var mediaFileQuerys = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == id);

        item!.MediaFile = new MediaFileDto
        {
            FileType = mediaFileQuerys.Select(m => m!.FileType).FirstOrDefault(),
            Uri = mediaFileQuerys.Select(m => m!.Uri).ToList(),
            Content = mediaFileQuerys.Select(m => m!.Content).FirstOrDefault()
        };

        return ApiResponse<MaintenanceDto>.Success(item);
    }

    public async Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto)
    {
        try
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);
            var assetType = await MainUnitOfWork.AssetTypeRepository.FindOneAsync((Guid)createDto.AssetTypeId!);

            if (asset == null)
                throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);

            if (assetType!.Unit == Unit.Individual && asset.RequestStatus != RequestType.Operational)
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

            var maintenance = createDto.ProjectTo<MaintenanceCreateDto, Maintenance>();
            maintenance.RequestCode = GenerateRequestCode();

            var mediaFiles = new List<MediaFile>();
            if (createDto.RelatedFiles != null)
            {
                foreach (var file in createDto.RelatedFiles)
                {
                    var newMediaFile = new MediaFile
                    {
                        FileName = file.FileName ?? "",
                        Uri = file.Uri ?? ""
                    };
                    mediaFiles.Add(newMediaFile);
                }
            }

            if (!await _maintenanceRepository.InsertMaintenance(maintenance, mediaFiles , AccountId, CurrentDate))
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto)
    {
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

        if (maintenance.Status != RequestStatus.Done)
            throw new ApiException($"Không thế cập nhật với quy trình có trạng thái: {maintenance.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);

        maintenance.Description = updateDto.Description ?? maintenance.Description;
        maintenance.Notes = updateDto.Notes ?? maintenance.Notes;
        maintenance.CategoryId = updateDto.CategoryId ?? maintenance.CategoryId;
        maintenance.IsInternal = updateDto.IsInternal ?? maintenance.IsInternal;
        maintenance.AssetTypeId = updateDto.AssetTypeId ?? maintenance.AssetTypeId;
        maintenance.AssignedTo = updateDto.AssignedTo ?? maintenance.AssignedTo;
        maintenance.Priority = updateDto.Priority ?? maintenance.Priority;

        if (!await MainUnitOfWork.MaintenanceRepository.UpdateAsync(maintenance, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thành công");
    }

    public async Task<ApiResponse> CreateItems(List<MaintenanceCreateDto> createDtos)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDtos.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

        if (assets.Any(asset => asset!.RequestStatus != RequestType.Operational))
        {
            throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
        }

        var maintenances = createDtos.Select(dto => dto.ProjectTo<MaintenanceCreateDto, Maintenance>())
                                     .ToList();

        if (!await _maintenanceRepository.InsertMaintenances(maintenances, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }

    public string GenerateRequestCode()
    {
        var requests = MainUnitOfWork.MaintenanceRepository.GetQueryAll().ToList();

        var numbers = new List<int>();
        foreach (var t in requests)
        {
            int.TryParse(t!.RequestCode[3..], out int lastNumber);
            numbers.Add(lastNumber);
        }

        string newRequestCode = "MTN1";

        if (requests.Any())
        {
            var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
            if (requests.Any(x => x!.RequestCode.StartsWith("MTN")))
            {
                newRequestCode = $"MTN{lastCode + 1}";
            }
        }
        return newRequestCode;
    }

    public async Task<ApiResponse> DeleteItem(Guid id)
    {
        var existingMaintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(
                new Expression<Func<Maintenance, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
        if (existingMaintenance == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu bảo trì này", StatusCode.NOT_FOUND);
        }

        if (!await _maintenanceRepository.DeleteMaintenance(existingMaintenance, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> DeleteItems(DeleteMutilDto deleteDto)
    {
        var maintenDeleteds = await MainUnitOfWork.MaintenanceRepository.FindAsync(
            new Expression<Func<Maintenance, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
            }, null);

        var maintenances = maintenDeleteds.Where(m => m != null).ToList();
        if (!await _maintenanceRepository.DeleteMaintenances(maintenances, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto)
    {
        var existingMainten = MainUnitOfWork.MaintenanceRepository.GetQuery()
                                    .Include(t => t!.Asset)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
        if (existingMainten == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu bảo trì này", StatusCode.NOT_FOUND);
        }

        existingMainten.Status = updateStatusDto.Status ?? existingMainten.Status;

        if (!await _maintenanceRepository.UpdateStatus(existingMainten, updateStatusDto.Status, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật yêu cầu thành công");
    }
}