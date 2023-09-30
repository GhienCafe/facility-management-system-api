using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Office2021.Excel.Pivot;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API_FFMS.Services;

public interface ITaskService : IBaseService
{
    Task<ApiResponses<TaskDto>> GetTasks(TaskQueryDto queryDto);

    Task<ApiResponse<DetailTaskDto>> GetTask(Guid id);

    // Task<ApiResponses<TaskCommonDto>> GetTasksV2(TaskQueryDto queryDto);
}

public class TaskService : BaseService, ITaskService
{
    public TaskService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<TaskDto>> GetTasks(TaskQueryDto queryDto)
    {
        var maintenanceDataset = MainUnitOfWork.MaintenanceRepository.GetQuery()
         .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
         .Select(x => new TaskDataSetDto
         {
             Id = x.Id,
             RequestedDate = x.RequestedDate,
             CompletionDate = x.CompletionDate,
             Description = x.Description,
             Note = x.Note,
             Reason = null,
             Status = x.Status,
             Type = ActionType.Maintenance,
             AssignedTo = x.AssignedTo,
             AssetId = x.AssetId,
             Quantity = null,
             NewAssetId = null,
             ToRoomId = null,
             CreatedAt = x.CreatedAt,
             EditedAt = x.EditedAt,
             CreatorId = x.CreatorId ?? Guid.Empty,
             EditorId = x.EditorId ?? Guid.Empty
         });

        var replacementDataset = MainUnitOfWork.ReplacementRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = x.Reason,
                Status = x.Status,
                Type = ActionType.Replacement,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = x.NewAssetId,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var repairationDataset = MainUnitOfWork.RepairationRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = x.Reason,
                Status = x.Status,
                Type = ActionType.Repairation,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = null,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var transportationDataset = MainUnitOfWork.TransportationRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = null,
                Status = x.Status,
                Type = ActionType.Repairation,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = null,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var allTasks = maintenanceDataset
            .Concat(repairationDataset)
            .Concat(replacementDataset)
            .Concat(transportationDataset);

        var taskQuery = from task in allTasks
                        join notification in MainUnitOfWork.NotificationRepository.GetQuery() on task.Id equals notification.ItemId into notificationGroup
                        from notification in notificationGroup.DefaultIfEmpty()
                        join room in MainUnitOfWork.RoomRepository.GetQuery() on task.ToRoomId equals room.Id into roomGroup
                        from room in roomGroup.DefaultIfEmpty()
                        join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on task.AssignedTo equals personInCharge.Id into personInChargeGroup
                        from personInCharge in personInChargeGroup.DefaultIfEmpty()
                        join asset in MainUnitOfWork.AssetRepository.GetQuery() on task.AssetId equals asset.Id into assetGroup
                        from asset in assetGroup.DefaultIfEmpty()
                        join newAsset in MainUnitOfWork.AssetRepository.GetQuery() on task.NewAssetId equals newAsset.Id into newAssetGroup
                        from newAsset in newAssetGroup.DefaultIfEmpty()
                        join roomAsset in MainUnitOfWork.RoomAssetRepository.GetQuery() on asset.Id equals roomAsset.AssetId into roomAssetGroup
                        from roomAsset in roomAssetGroup.DefaultIfEmpty()
                        join location in MainUnitOfWork.RoomRepository.GetQuery() on roomAsset.RoomId equals location.Id into locationGroup
                        from location in locationGroup.DefaultIfEmpty()
                        select new TaskDto
                        {
                            Id = task.Id,
                            Title = notification.Title,
                            Content = notification.Content,
                            NotificationDate = notification.CreatedAt,
                            RequestedDate = task.RequestedDate,
                            CompletionDate = task.CompletionDate,
                            Description = task.Description,
                            Note = task.Note,
                            Reason = task.Reason,
                            Status = task.Status.GetValue(),
                            Type = task.Type.GetValue(),
                            AssignedTo = task.AssignedTo,
                            AssetId = task.AssetId,
                            // Quantity = task.Quantity,
                            // NewAssetId = task.NewAssetId,
                            ToRoomId = task.ToRoomId,
                            CreatedAt = task.CreatedAt,
                            EditedAt = task.EditedAt,
                            CreatorId = task.CreatorId,
                            EditorId = task.EditorId,
                            // Asset = asset.ProjectTo<Asset, AssetDto>(),
                            // NewAsset = newAsset.ProjectTo<Asset, AssetDto>(),
                            ToRoom = room.ProjectTo<Room, RoomDto>(),
                            Location = location.ProjectTo<Room, RoomDto>()
                            //PersonInCharge = personInCharge.ProjectTo<User, UserDto>()
                        };

        var totalCount = await taskQuery.CountAsync();
        var tasks = taskQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize).ToList();
        tasks = await _mapperRepository.MapCreator(tasks);

        return ApiResponses<TaskDto>.Success(
            tasks,
            totalCount,
            queryDto.PageSize,
            queryDto.Skip(),
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        );
    }

    public async Task<ApiResponse<DetailTaskDto>> GetTask(Guid id)
    {
        var maintenanceDataset = MainUnitOfWork.MaintenanceRepository.GetQuery()
        .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
        .Select(x => new TaskDataSetDto
        {
            Id = x.Id,
            RequestedDate = x.RequestedDate,
            CompletionDate = x.CompletionDate,
            Description = x.Description,
            Note = x.Note,
            Reason = null,
            Status = x.Status,
            Type = ActionType.Maintenance,
            AssignedTo = x.AssignedTo,
            AssetId = x.AssetId,
            Quantity = null,
            NewAssetId = null,
            ToRoomId = null,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            CreatorId = x.CreatorId ?? Guid.Empty,
            EditorId = x.EditorId ?? Guid.Empty
        });

        var replacementDataset = MainUnitOfWork.ReplacementRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = x.Reason,
                Status = x.Status,
                Type = ActionType.Replacement,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = x.NewAssetId,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var repairationDataset = MainUnitOfWork.RepairationRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = x.Reason,
                Status = x.Status,
                Type = ActionType.Repairation,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = null,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var transportationDataset = MainUnitOfWork.TransportationRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId)
            .Select(x => new TaskDataSetDto
            {
                Id = x.Id,
                RequestedDate = x.RequestedDate,
                CompletionDate = x.CompletionDate,
                Description = x.Description,
                Note = x.Note,
                Reason = null,
                Status = x.Status,
                Type = ActionType.Repairation,
                AssignedTo = x.AssignedTo,
                AssetId = x.AssetId,
                Quantity = null,
                NewAssetId = null,
                ToRoomId = null,
                CreatedAt = x.CreatedAt,
                EditedAt = x.EditedAt,
                CreatorId = x.CreatorId ?? Guid.Empty,
                EditorId = x.EditorId ?? Guid.Empty
            });

        var allTasks = maintenanceDataset
            .Concat(repairationDataset)
            .Concat(replacementDataset)
            .Concat(transportationDataset);

        var taskQuery = from task in allTasks
                        where task.Id == id
                        join notification in MainUnitOfWork.NotificationRepository.GetQuery() on task.Id equals notification.ItemId into notificationGroup
                        from notification in notificationGroup.DefaultIfEmpty()
                        join room in MainUnitOfWork.RoomRepository.GetQuery() on task.ToRoomId equals room.Id into roomGroup
                        from room in roomGroup.DefaultIfEmpty()
                        join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on task.AssignedTo equals personInCharge.Id into personInChargeGroup
                        from personInCharge in personInChargeGroup.DefaultIfEmpty()
                        join asset in MainUnitOfWork.AssetRepository.GetQuery() on task.AssetId equals asset.Id into assetGroup
                        from asset in assetGroup.DefaultIfEmpty()
                        join newAsset in MainUnitOfWork.AssetRepository.GetQuery() on task.NewAssetId equals newAsset.Id into newAssetGroup
                        from newAsset in newAssetGroup.DefaultIfEmpty()
                        join roomAsset in MainUnitOfWork.RoomAssetRepository.GetQuery() on asset.Id equals roomAsset.AssetId into roomAssetGroup
                        from roomAsset in roomAssetGroup.DefaultIfEmpty()
                        join location in MainUnitOfWork.RoomRepository.GetQuery() on roomAsset.RoomId equals location.Id into locationGroup
                        from location in locationGroup.DefaultIfEmpty()
                        select new DetailTaskDto
                        {
                            Id = task.Id,
                            Title = notification.Title,
                            Content = notification.Content,
                            NotificationDate = notification.CreatedAt,
                            RequestedDate = task.RequestedDate,
                            CompletionDate = task.CompletionDate,
                            Description = task.Description,
                            Note = task.Note,
                            Reason = task.Reason,
                            Status = task.Status.GetValue(),
                            Type = task.Type.GetValue(),
                            AssignedTo = task.AssignedTo,
                            AssetId = task.AssetId,
                            Quantity = task.Quantity,
                            NewAssetId = task.NewAssetId,
                            ToRoomId = task.ToRoomId,
                            CreatedAt = task.CreatedAt,
                            EditedAt = task.EditedAt,
                            CreatorId = task.CreatorId,
                            EditorId = task.EditorId,
                            Asset = new AssetDto
                            {
                                Id = asset.Id,
                                Status = asset.Status.GetValue(),
                                Description = asset.Description,
                                TypeId = asset.TypeId,
                                Quantity = asset.Quantity,
                                CreatedAt = asset.CreatedAt,
                                IsRented = asset.IsRented,
                                CreatorId = asset.CreatorId ?? Guid.Empty,
                                EditorId = asset.EditorId ?? Guid.Empty,
                                EditedAt = asset.EditedAt,
                                ModelId = asset.ModelId,
                                AssetName = asset.AssetName,
                                IsMovable = asset.IsMovable,
                                ManufacturingYear = asset.ManufacturingYear,
                                SerialNumber = asset.SerialNumber,
                                LastMaintenanceTime = asset.LastMaintenanceTime,
                                AssetCode = asset.AssetCode
                            },//asset.ProjectTo<Asset, AssetDto>(),
                            NewAsset = newAsset != null ? new AssetDto
                            {
                                Id = newAsset.Id,
                                Status = newAsset.Status.GetValue(),
                                Description = newAsset.Description,
                                TypeId = newAsset.TypeId,
                                Quantity = newAsset.Quantity,
                                CreatedAt = newAsset.CreatedAt,
                                IsRented = newAsset.IsRented,
                                CreatorId = newAsset.CreatorId ?? Guid.Empty,
                                EditorId = newAsset.EditorId ?? Guid.Empty,
                                EditedAt = newAsset.EditedAt,
                                ModelId = newAsset.ModelId,
                                AssetName = newAsset.AssetName,
                                IsMovable = newAsset.IsMovable,
                                ManufacturingYear = newAsset.ManufacturingYear,
                                SerialNumber = newAsset.SerialNumber,
                                LastMaintenanceTime = newAsset.LastMaintenanceTime,
                                AssetCode = newAsset.AssetCode
                            } : null,//newAsset.ProjectTo<Asset, AssetDto>(),
                            ToRoom = room.ProjectTo<Room, RoomDto>(),
                            Location = location.ProjectTo<Room, RoomDto>()
                            //PersonInCharge = personInCharge.ProjectTo<User, UserDto>()
                        };

        var detailTask = await taskQuery.FirstOrDefaultAsync();
        if (detailTask == null)
        {
            throw new ApiException("Không tìm thấy nhiệm vụ", StatusCode.NOT_FOUND);
        }

        detailTask = await _mapperRepository.MapCreator(detailTask);

        return ApiResponse<DetailTaskDto>.Success(detailTask);
    }

}