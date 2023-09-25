using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface IMaintenanceService : IBaseService
{
    Task<ApiResponses<MaintenanceDto>> GetMaintenances(MaintenanceQueryDto queryDto);
    Task<ApiResponse<MaintenanceDetailDto>> GetMaintenance(Guid id);
    Task<ApiResponse> DeleteMaintenance(Guid id);
    Task<ApiResponse> CreateMaintenance(MaintenanceCreateDto maintenanceCreateDto);
    Task<ApiResponse> UpdateMaintenance(Guid id, MaintenanceUpdateDto maintenanceUpdateDto);
    
}

public class MaintenanceService : BaseService, IMaintenanceService
{
    public MaintenanceService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
    
    public async Task<ApiResponses<MaintenanceDto>> GetMaintenances(MaintenanceQueryDto queryDto)
    {
        var maintenanceDataset = MainUnitOfWork.MaintenanceRepository.GetQuery();

        if (queryDto.AssetId != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.AssetId == queryDto.AssetId);
        }
        
        if (!string.IsNullOrEmpty(queryDto.Description))
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.Description!.ToLower().Contains(x.Description.Trim().ToLower()));
        }
        
        if (!string.IsNullOrEmpty(queryDto.Note))
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.Note!.ToLower().Contains(x.Note.Trim().ToLower()));
        }
        
        if (queryDto.Status != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.Status == queryDto.Status);
        }
        
        if (queryDto.Type != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.Type == queryDto.Type);
        }
        
        if (queryDto.AssignedTo != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }
        
        if (queryDto.CompletionDate != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.CompletionDate == queryDto.CompletionDate);
        }
        
        if (queryDto.RequestedDate != null)
        {
            maintenanceDataset = maintenanceDataset.Where(x => x!.RequestedDate == queryDto.RequestedDate);
        }

        var joinTables = from maintenance in maintenanceDataset
            join asset in MainUnitOfWork.AssetRepository.GetQuery() on maintenance.AssetId equals asset.Id
                into assetGroup
            from asset in assetGroup.DefaultIfEmpty()
            join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on maintenance.AssignedTo equals personInCharge.Id
                into personInChargeGroup
            from personInCharge in personInChargeGroup.DefaultIfEmpty()
            select new
            {
                Maintenance = maintenance,
                Asset = asset,
                PersonInCharge = personInCharge
            };

        var totalCount = joinTables.Count();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var maintenances = await joinTables.Select(
            x => new MaintenanceDto
            {
                AssetId = x.Maintenance.AssetId,
                Id = x.Maintenance.Id,
                AssignedTo = x.Maintenance.AssignedTo,
                CompletionDate = x.Maintenance.CompletionDate,
                Description = x.Maintenance.Description,
                Type = x.Maintenance.Type.GetValue(),
                Note = x.Maintenance.Note,
                RequestedDate = x.Maintenance.RequestedDate,
                Status = x.Maintenance.Status.GetValue(),
                CreatedAt = x.Maintenance.CreatedAt,
                EditedAt = x.Maintenance.EditedAt,
                CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
                EditorId = x.Maintenance.EditorId ?? Guid.Empty,
                Asset = x.Asset.ProjectTo<Asset, AssetDto>(),
                PersonInCharge = x.PersonInCharge.ProjectTo<User, UserDto>()
            }).ToListAsync();

        maintenances = await _mapperRepository.MapCreator(maintenances);

        return ApiResponses<MaintenanceDto>.Success(
            maintenances,
            totalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse<MaintenanceDetailDto>> GetMaintenance(Guid id)
    {
        var maintenance = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.Id == id)
            .Select(x => new MaintenanceDetailDto
            {
                AssetId = x.AssetId,
                Id = x.Id,
                AssignedTo = x.AssignedTo,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Type = x.Type.GetValue(),
                Note = x.Note,
                RequestedDate = x.RequestedDate,
                Status = x.Status.GetValue(),
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            }).FirstOrDefault();

        if (maintenance == null)
            throw new ApiException("Không tìm thấy yêu cầu");

        maintenance.PersonInCharge = await MainUnitOfWork.UserRepository.FindOneAsync<UserDto>(
            new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == maintenance.AssignedTo
            });
        
        maintenance.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetDto>(
            new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == maintenance.AssetId
            });

        maintenance = await _mapperRepository.MapCreator(maintenance);
        
        return ApiResponse<MaintenanceDetailDto>.Success(maintenance);
    }

    public async Task<ApiResponse> DeleteMaintenance(Guid id)
    {
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy yêu cầu");

        if (!await MainUnitOfWork.MaintenanceRepository.DeleteAsync(maintenance, AccountId, CurrentDate))
            throw new ApiException("Xóa yêu cầu thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success("Xóa yêu cầu thành công");
    }

    public async Task<ApiResponse> CreateMaintenance(MaintenanceCreateDto maintenanceCreateDto)
    {
        var maintenance = maintenanceCreateDto.ProjectTo<MaintenanceCreateDto, Maintenance>();

        maintenance.Status = MaintenanceStatus.NotStarted;
        maintenance.Type = MaintenanceType.Unexpected;

        //// check asset is rent or not

        if (!await MainUnitOfWork.MaintenanceRepository.InsertAsync(maintenance, AccountId, CurrentDate))
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
        
        var notification = new Notification
        {
            UserId = maintenance.AssignedTo,
            Status = NotificationStatus.Waiting,
            Content = "Bảo trì trang thiết bị ...",
            Title = "Bảo trì trang thiết bị",
            Type = NotificationType.Task,
            IsRead = false,
            ItemId = maintenance.Id,
            ShortContent = "Bảo trì trang thiết bị"
        };

        if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
            throw new ApiException("Thông báo tới nhân viên không thành công", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Tạo yêu cầu thành công");
    }

    public async Task<ApiResponse> UpdateMaintenance(Guid id, MaintenanceUpdateDto maintenanceUpdateDto)
    {
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(id);

        if (maintenance == null)
            throw new ApiException("Không tìm thấy yêu cầu");

        maintenance.Status = maintenanceUpdateDto.Status ?? maintenance.Status;
        maintenance.Description = maintenanceUpdateDto.Description ?? maintenance.Description;
        maintenance.AssetId = maintenanceUpdateDto.AssetId ?? maintenance.AssetId;
        maintenance.AssignedTo = maintenanceUpdateDto.AssignedTo ?? maintenance.AssignedTo;
        maintenance.Note = maintenanceUpdateDto.Note ?? maintenance.Note;
        maintenance.CompletionDate = maintenanceUpdateDto.CompletionDate ?? maintenance.CompletionDate;
        maintenance.RequestedDate = maintenanceUpdateDto.RequestedDate ?? maintenance.RequestedDate;

        if (!await MainUnitOfWork.MaintenanceRepository.UpdateAsync(maintenance, AccountId, CurrentDate))
            throw new ApiException("Cập nhật yêu cầu thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success("Cập nhật thành công");
    }
}