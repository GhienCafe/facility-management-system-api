using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IMaintenanceService : IBaseService
{
    Task<ApiResponses<MaintenanceDto>> GetItems(MaintenanceQueryDto queryDto);
    Task<ApiResponse<MaintenanceDto>> GetItem(Guid id);
    Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto);
    Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto);
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

        var joinTables = from maintenance in maintenanceQueryable
            join user in userQueryable on maintenance.AssignedTo equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty()
            join asset in assetQueryable on maintenance.AssetId equals asset.Id into assetGroup
            from asset in assetGroup.DefaultIfEmpty()
            select new
            {
                Maintenance = maintenance,
                User = user,
                Asset = asset
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
            RequestDate = x.Maintenance.RequestDate,
            StatusObj = x.Maintenance.Status!.GetValue(),
            CreatedAt = x.Maintenance.CreatedAt,
            EditedAt = x.Maintenance.EditedAt,
            CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
            EditorId = x.Maintenance.EditorId ?? Guid.Empty,
            User = x.User.ProjectTo<User, UserBaseDto>(),
            Asset = x.Asset.ProjectTo<Asset, AssetBaseDto>()
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
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);
        
        var maintenanceDto = maintenance.ProjectTo<Maintenance, MaintenanceDto>();
        maintenanceDto.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(new Expression<Func<Asset, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == maintenance.AssetId
        });

        if (maintenanceDto.Asset != null)
        {
            maintenanceDto.Asset.StatusObj = maintenance.Asset?.Status.GetValue();
        }

        maintenanceDto.StatusObj = maintenance.Status?.GetValue();
        
        maintenanceDto.User = await MainUnitOfWork.UserRepository.FindOneAsync<UserBaseDto>(new Expression<Func<User, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.Id == maintenance.AssignedTo
        });

        if (maintenanceDto.User != null)
        {
            maintenanceDto.User.StatusObj = maintenanceDto.User.Status?.GetValue();
            maintenanceDto.User.RoleObj = maintenanceDto.User.Role?.GetValue();
        }

        maintenanceDto = await _mapperRepository.MapCreator(maintenanceDto);

        return ApiResponse<MaintenanceDto>.Success(maintenanceDto);
    }

    public async Task<ApiResponse> CreateItem(MaintenanceCreateDto createDto)
    {
        var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);

        if (asset == null)
            throw new ApiException("Không cần tồn tại trang thiết bị", StatusCode.NOT_FOUND);
        
        if (asset.Status != AssetStatus.Operational)
            throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);

        var maintenance = createDto.ProjectTo<MaintenanceCreateDto, Maintenance>();

        if (!await _maintenanceRepository.InsertMaintenance(maintenance, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }

    public async Task<ApiResponse> UpdateItem(Guid id, MaintenanceUpdateDto updateDto)
    {
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy nội dung", StatusCode.NOT_FOUND);

        if(maintenance.Status != RequestStatus.NotStarted)
            throw new ApiException($"Không thế cập nhật với quy trình có trạng thái: {maintenance.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);
        
        maintenance.Description = updateDto.Description ?? maintenance.Description;
        maintenance.Status = updateDto.Status ?? maintenance.Status;
        maintenance.Notes = updateDto.Notes ?? maintenance.Notes;
        maintenance.AssignedTo = updateDto.AssignedTo ?? maintenance.AssignedTo;
        maintenance.CompletionDate = updateDto.CompletionDate ?? maintenance.CompletionDate;
        maintenance.RequestDate = updateDto.RequestDate ?? maintenance.RequestDate;
        
        if(!await MainUnitOfWork.MaintenanceRepository.UpdateAsync(maintenance, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thất bại", StatusCode.SERVER_ERROR);

        return ApiResponse.Success("Cập nhật thành công");
    }
}