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

        //   public async Task<ApiResponses<TaskCommonDto>> GetTasksV2(TaskQueryDto queryDto)
        // {
        //     var notifications = MainUnitOfWork.NotificationRepository.GetQuery()
        //         .Where(n => !n!.DeletedAt.HasValue && n.UserId == AccountId);
        //     
        //     var assetRooms = MainUnitOfWork.RoomAssetRepository.GetQuery()
        //         .Where(n => !n!.DeletedAt.HasValue);
        //     
        //     var rooms = MainUnitOfWork.RoomRepository.GetQuery()
        //         .Where(n => !n!.DeletedAt.HasValue);
        //     
        //     var maintenanceTasks = MainUnitOfWork.MaintenanceRepository.GetQuery()
        //         .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId);
        //     var replaceTasks = MainUnitOfWork.ReplacementRepository.GetQuery()
        //         .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId);
        //     var transportTasks = MainUnitOfWork.TransportationRepository.GetQuery()
        //         .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId);
        //     var repairTasks = MainUnitOfWork.RepairationRepository.GetQuery()
        //         .Where(x => !x!.DeletedAt.HasValue && x.AssignedTo == AccountId);
        //
        //     var maintenanceDataset = (from maintenance in maintenanceTasks
        //         join notification in notifications on maintenance.AssignedTo equals notification.UserId
        //         join assetRoom in assetRooms on maintenance.AssetId equals assetRoom.AssetId
        //         join room in rooms on assetRoom.RoomId equals room.Id
        //         where assetRoom.ToDate == null
        //         select new
        //         {
        //             Notification = notification,
        //             Maintenance = maintenance,
        //             Room = room
        //         }).Select(x => new TaskCommonQueryDto
        //     {
        //         Id = x.Maintenance.Id,
        //         RequestedDate = x.Maintenance.RequestedDate,
        //         Content = x.Notification.Content,
        //         Title = x.Notification.Title,
        //         AssetId = x.Maintenance.AssetId,
        //         Type = ActionType.Maintenance,
        //         Status = x.Maintenance.Status,
        //         RoomId = x.Room.Id,
        //         Room = x.Room.ProjectTo<Room, RoomDto>(),
        //         CreatedAt = x.Maintenance.CreatedAt,
        //         EditedAt = x.Maintenance.EditedAt,
        //         CreatorId = x.Maintenance.CreatorId ?? Guid.Empty,
        //         EditorId = x.Maintenance.EditorId ?? Guid.Empty
        //     });
        //     
        //     // Replace tasks
        //     var replaceDataset = (from replace in replaceTasks
        //         join notification in notifications on replace.AssignedTo equals notification.UserId
        //         join assetRoom in assetRooms on replace.AssetId equals assetRoom.AssetId
        //         join room in rooms on assetRoom.RoomId equals room.Id
        //         where assetRoom.ToDate == null
        //         select new
        //         {
        //             Notification = notification,
        //             Replacement = replace,
        //             Room = room
        //         }).Select(x => new TaskCommonQueryDto
        //     {
        //         Id = x.Replacement.Id,
        //         RequestedDate = x.Replacement.RequestedDate,
        //         Content = x.Notification.Content,
        //         Title = x.Notification.Title,
        //         AssetId = x.Replacement.AssetId,
        //         Type = ActionType.Replacement,
        //         Status = x.Replacement.Status,
        //         RoomId = x.Room.Id,
        //         Room = x.Room.ProjectTo<Room, RoomDto>(),
        //         CreatedAt = x.Replacement.CreatedAt,
        //         EditedAt = x.Replacement.EditedAt,
        //         CreatorId = x.Replacement.CreatorId ?? Guid.Empty,
        //         EditorId = x.Replacement.EditorId ?? Guid.Empty
        //     });
        //     
        //     // Repair tasks
        //     var repairDataset = (from repair in repairTasks
        //         join notification in notifications on repair.AssignedTo equals notification.UserId
        //         join assetRoom in assetRooms on repair.AssetId equals assetRoom.AssetId
        //         join room in rooms on assetRoom.RoomId equals room.Id
        //         where assetRoom.ToDate == null
        //         select new
        //         {
        //             Notification = notification,
        //             Repair = repair,
        //             Room = room
        //         }).Select(x => new TaskCommonQueryDto
        //     {
        //         Id = x.Repair.Id,
        //         RequestedDate = x.Repair.RequestedDate,
        //         Content = x.Notification.Content,
        //         Title = x.Notification.Title,
        //         AssetId = x.Repair.AssetId,
        //         Type = ActionType.Repairation,
        //         Status = x.Repair.Status,
        //         RoomId = x.Room.Id,
        //         Room = x.Room.ProjectTo<Room, RoomDto>(),
        //         CreatedAt = x.Repair.CreatedAt,
        //         EditedAt = x.Repair.EditedAt,
        //         CreatorId = x.Repair.CreatorId ?? Guid.Empty,
        //         EditorId = x.Repair.EditorId ?? Guid.Empty
        //     });
        //     
        //     // Transport tasks
        //     var transportDataset = (from transport in transportTasks
        //         join notification in notifications on transport.AssignedTo equals notification.UserId
        //         join assetRoom in assetRooms on transport.AssetId equals assetRoom.AssetId
        //         join room in rooms on assetRoom.RoomId equals room.Id
        //         where assetRoom.ToDate == null
        //         select new
        //         {
        //             Notification = notification,
        //             Transport = transport,
        //             Room = room
        //         }).Select(x => new TaskCommonQueryDto
        //     {
        //         Id = x.Transport.Id,
        //         RequestedDate = x.Transport.RequestedDate,
        //         Content = x.Notification.Content,
        //         Title = x.Notification.Title,
        //         AssetId = x.Transport.AssetId,
        //         Type = ActionType.Transportation,
        //         Status = x.Transport.Status,
        //         RoomId = x.Room.Id,
        //         Room = x.Room.ProjectTo<Room, RoomDto>(),
        //         CreatedAt = x.Transport.CreatedAt,
        //         EditedAt = x.Transport.EditedAt,
        //         CreatorId = x.Transport.CreatorId ?? Guid.Empty,
        //         EditorId = x.Transport.EditorId ?? Guid.Empty
        //     });
        //
        //     // var query = maintenanceDataset.Concat(repairDataset);
        //     //
        //     // var taskQuery = from task in query
        //     //     select new TaskCommonDto
        //     //     {
        //     //         Id = task.Id,
        //     //         RequestedDate = task.RequestedDate,
        //     //         Status = task.Status.GetValue(),
        //     //         Type = task.Type.GetValue(),
        //     //         AssetId = task.AssetId,
        //     //         CreatedAt = task.CreatedAt,
        //     //         EditedAt = task.EditedAt,
        //     //         CreatorId = task.CreatorId,
        //     //         EditorId = task.EditorId,
        //     //         Title = task.Title,
        //     //         Content = task.Content,
        //     //         RoomId = task.RoomId,
        //     //         Room = task.Room,
        //     //     };
        //     
        //     var listMaintenanceTasks = await maintenanceDataset.ToListAsync();
        //     var listReplaceTasks = await replaceDataset.ToListAsync();
        //     var listRepairTasks = await repairDataset.ToListAsync();
        //     var listTransportTasks = await transportDataset.ToListAsync();
        //
        //     var concatenatedTasks = listMaintenanceTasks.Concat(listReplaceTasks)
        //         .Concat(listRepairTasks)
        //         .Concat(listTransportTasks);
        //
        //     var taskQuery = from task in concatenatedTasks
        //         select new TaskCommonDto
        //         {
        //             Id = task.Id,
        //             RequestedDate = task.RequestedDate,
        //             Status = task.Status.GetValue(),
        //             Type = task.Type.GetValue(),
        //             AssetId = task.AssetId,
        //             CreatedAt = task.CreatedAt,
        //             EditedAt = task.EditedAt,
        //             CreatorId = task.CreatorId,
        //             EditorId = task.EditorId,
        //             Title = task.Title,
        //             Content = task.Content,
        //             RoomId = task.RoomId,
        //             Room = task.Room,
        //         };
        //
        //     var totalCount = await taskQuery.CountAsync();
        //
        //     var tasks = taskQuery!.Skip(queryDto.Skip()).Take(queryDto.PageSize);
        //
        //     tasks = await _mapperRepository.MapCreator(tasks.ToList());
        //
        //     return ApiResponses<TaskCommonDto>.Success(
        //         tasks,
        //         totalCount,
        //         queryDto.PageSize,
        //         queryDto.Skip(),
        //         (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
        //     );
        // }
}