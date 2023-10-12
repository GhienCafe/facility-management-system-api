using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2022.M08.Main;
using DocumentFormat.OpenXml.Office2021.Excel.Pivot;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API_FFMS.Services;

public interface ITaskService : IBaseService
{
    Task<ApiResponses<TaskBaseDto>> GetTasks(TaskQueryDto queryDto);
    Task<ApiResponse<TaskDetailDto>> GetTaskDetail(Guid id);
}

public class TaskService : BaseService, ITaskService
{
    public TaskService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponse<TaskDetailDto>> GetTaskDetail(Guid id)
    {
        var taskDetail = new TaskDetailDto();
        //ASSET CHECK
        var assetCheckTask = await MainUnitOfWork.AssetCheckRepository.FindOneAsync<TaskDetailDto>(
            new Expression<Func<AssetCheck, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
            });
        if (assetCheckTask != null)
        {
            assetCheckTask!.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                    new Expression<Func<Asset, bool>>[]
                    {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == assetCheckTask.AssetId
                    });
            if (assetCheckTask.Asset != null)
            {
                assetCheckTask.Asset!.StatusObj = assetCheckTask.Asset.Status?.GetValue();
                var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == assetCheckTask.Asset.Id,
                    x => x.ToDate == null
                    });
                if (location != null)
                {
                    assetCheckTask.ToRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                            new Expression<Func<Room, bool>>[]
                            {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == location.RoomId
                            });
                }
            }
            taskDetail = assetCheckTask;
        }

        //TRANSPORTATION
        var trasportTask = await MainUnitOfWork.TransportationRepository.FindOneAsync<TaskDetailDto>(
            new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
            });
        if (trasportTask != null)
        {
            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();
            var toRoom = await roomDataset
                            .Where(r => r!.Id == trasportTask!.ToRoomId)
                            .Select(r => new RoomBaseDto
                            {
                                Id = r!.Id,
                                RoomCode = r.RoomCode,
                                RoomName = r.RoomName,
                                StatusId = r.StatusId,
                                FloorId = r.FloorId,
                                CreatedAt = r.CreatedAt,
                                EditedAt = r.EditedAt
                            }).FirstOrDefaultAsync();
            var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery();
            var assets = await transportDetails
                        .Where(td => td!.TransportationId == id)
                        .Join(MainUnitOfWork.AssetRepository.GetQuery(),
                                td => td!.AssetId,
                                asset => asset!.Id, (td, asset) => new FromRoomAssetDto
                                {
                                    Asset = new AssetBaseDto
                                    {
                                        Id = asset!.Id,
                                        Description = asset.Description,
                                        AssetCode = asset.AssetCode,
                                        AssetName = asset.AssetName,
                                        Quantity = asset.Quantity,
                                        IsMovable = asset.IsMovable,
                                        IsRented = asset.IsRented,
                                        ManufacturingYear = asset.ManufacturingYear,
                                        StatusObj = asset.Status.GetValue(),
                                        Status = asset.Status,
                                        StartDateOfUse = asset.StartDateOfUse,
                                        SerialNumber = asset.SerialNumber,
                                        LastCheckedDate = asset.LastCheckedDate,
                                        LastMaintenanceTime = asset.LastMaintenanceTime,
                                        TypeId = asset.TypeId,
                                        ModelId = asset.ModelId,
                                        CreatedAt = asset.CreatedAt,
                                        EditedAt = asset.EditedAt,
                                        CreatorId = asset.CreatorId ?? Guid.Empty,
                                        EditorId = asset.EditorId ?? Guid.Empty
                                    },
                                    FromRoom = new RoomBaseDto
                                    {
                                        Id = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.Id,
                                        RoomCode = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.RoomCode,
                                        RoomName = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.RoomName,
                                        StatusId = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.StatusId,
                                        FloorId = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.FloorId,
                                        CreatedAt = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.CreatedAt,
                                        EditedAt = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.EditedAt
                                    }
                                }).ToListAsync();
            var tranportation = new TaskDetailDto
            {
                NewAssetId = Guid.Empty,
                Id = trasportTask!.Id,
                RequestDate = trasportTask.RequestDate,
                Quantity = trasportTask.Quantity,
                CreatedAt = trasportTask.CreatedAt,
                EditedAt = trasportTask.EditedAt,
                CreatorId = trasportTask.CreatorId,
                EditorId = trasportTask.EditorId,
                Assets = assets,
                ToRoom = toRoom
            };
            taskDetail = tranportation;
        }

        //REPLAEMENT
        var replacementTask = await MainUnitOfWork.ReplacementRepository.FindOneAsync<TaskDetailDto>(
                new Expression<Func<Replacement, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
        if (replacementTask != null)
        {
            replacementTask!.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                    new Expression<Func<Asset, bool>>[]
                    {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacementTask.AssetId
                    });

            replacementTask.NewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacementTask.NewAssetId
                });
            taskDetail = replacementTask;
        }
        //var taskDetail = new TaskDetailDto();
        //if (assetCheckTask != null)
        //{
        //    taskDetail = assetCheckTask;
        //}
        //else if (trasportTask != null)
        //{
        //    taskDetail = trasportTask;
        //}
        //else if (replacementTask != null)
        //{
        //    taskDetail = replacementTask;
        //}

        taskDetail = await _mapperRepository.MapCreator(taskDetail);

        return ApiResponse<TaskDetailDto>.Success(taskDetail);

    }

    public async Task<ApiResponses<TaskBaseDto>> GetTasks(TaskQueryDto queryDto)
    {
        // Retrieve tasks from the Transportation table
        var transportationTasks = MainUnitOfWork.TransportationRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Transportation,
                Id = t!.Id,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                ToRoomId = t.ToRoomId,
                Quantity = t.Quantity,
                AssignedTo = t.AssignedTo,
                CompletionDate = t.CompletionDate,
                Description = t.Description,
                IsInternal = t.IsInternal,
                Notes = t.Notes,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                Status = t.Status,
            });

        // Retrieve tasks from the Maintenance table
        var maintenanceTasks = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Maintenance,
                Id = t!.Id,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                ToRoomId = null,
                Quantity = null,
                AssignedTo = t.AssignedTo,
                CompletionDate = t.CompletionDate,
                Description = t.Description,
                IsInternal = t.IsInternal,
                Notes = t.Notes,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                Status = t.Status,
            });

        // Retrieve tasks from the AssetCheck table
        var assetCheckTasks = MainUnitOfWork.AssetCheckRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.StatusCheck,
                Id = t!.Id,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                ToRoomId = null,
                Quantity = null,
                AssignedTo = t.AssignedTo,
                CompletionDate = t.CompletionDate,
                Description = t.Description,
                IsInternal = t.IsInternal,
                Notes = t.Notes,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                Status = t.Status,
            });

        // Concatenate the results from both tables
        var combinedTasks = transportationTasks.Union(maintenanceTasks).Union(assetCheckTasks);

        var totalCount = await combinedTasks.CountAsync();

        combinedTasks = combinedTasks.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var items = await combinedTasks.Select(x => new TaskBaseDto
        {
            Type = RequestType.Maintenance,
            Id = x.Id,
            CreatorId = x.CreatorId,
            EditorId = x.EditorId,
            CreatedAt = x.CreatedAt,
            EditedAt = x.EditedAt,
            AssignedTo = x.AssignedTo,
            CompletionDate = x.CompletionDate,
            Description = x.Description,
            Notes = x.Notes,
            IsInternal = x.IsInternal,
            RequestCode = x.RequestCode,
            RequestDate = x.RequestDate,
            Status = x.Status,
            Quantity = x.Quantity,
            ToRoomId = x.ToRoomId
        }).ToListAsync();

        items.ForEach(x =>
        {
            x.StatusObj = x.Status?.GetValue();
            x.TypeObj = x.Type.GetValue();
        });

        items = await _mapperRepository.MapCreator(items);

        return ApiResponses<TaskBaseDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}