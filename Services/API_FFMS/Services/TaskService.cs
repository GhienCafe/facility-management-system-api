using System.Linq.Expressions;
using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface ITaskService : IBaseService
{
    Task<ApiResponses<TaskBaseDto>> GetTasks(TaskQueryDto queryDto);
    Task<ApiResponse<TaskDetailDto>> GetTaskDetail(Guid id);
    Task<ApiResponse> UpdateTaskStatus(ReportCreateDto createDto);
    //Task<ApiResponse> InventoryCheckReport(InventoryCheckReport reportDto);
}

public class TaskService : BaseService, ITaskService
{
    private readonly ITaskRepository _taskRepository;
    public TaskService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                       IMapperRepository mapperRepository, ITaskRepository taskRepository)
                       : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<ApiResponse> UpdateTaskStatus(ReportCreateDto createDto)
    {
        //ASSET CHECK
        var assetCheckReport = await MainUnitOfWork.AssetCheckRepository.FindOneAsync(
            new Expression<Func<AssetCheck, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });
        if (assetCheckReport != null)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(assetCheckReport.AssetId);
            if (asset == null)
            {
                throw new ApiException("Không tìm thấy thiết bị của yêu cầu này", StatusCode.NOT_FOUND);
            }

            createDto.ItemId = assetCheckReport.Id;
        }

        //TRANSPORTATION
        var transportReport = await MainUnitOfWork.TransportationRepository.FindOneAsync(
            new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });
        if (transportReport != null)
        {
            createDto.ItemId = transportReport.Id;
        }

        //REPLACEMENT
        var replaceReport = await MainUnitOfWork.ReplacementRepository.FindOneAsync(
            new Expression<Func<Replacement, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });
        if (replaceReport != null)
        {
            createDto.ItemId = replaceReport.Id;
        }

        //REPAIRATION
        var repairReport = await MainUnitOfWork.RepairRepository.FindOneAsync(
            new Expression<Func<Repair, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });
        if (repairReport != null)
        {
            createDto.ItemId = repairReport.Id;
        }

        //MAINTENANCE
        var maintenReport = await MainUnitOfWork.MaintenanceRepository.FindOneAsync(
            new Expression<Func<Maintenance, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });
        if (maintenReport != null)
        {
            createDto.ItemId = maintenReport.Id;
        }

        //INVENTORY CHECK
        var inventoryCheckReport = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(
            new Expression<Func<InventoryCheck, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == createDto.ItemId,
                x => x.Status != RequestStatus.Cancelled
            });

        var inventoryDetails = new List<InventoryCheckDetail>();
        if (inventoryCheckReport != null)
        {
            if (createDto.Rooms != null)
            {
                foreach (var room in createDto.Rooms)
                {
                    if (room.Assets != null)
                    {
                        foreach (var assetReport in room.Assets)
                        {
                            var inventoryDetail = new InventoryCheckDetail
                            {
                                AssetId = assetReport.AssetId ?? Guid.Empty,
                                InventoryCheckId = inventoryCheckReport.Id,
                                RoomId = (Guid)room.RoomId,
                                Status = assetReport.Status,
                                Quantity = assetReport.Quantity
                            };

                            inventoryDetails.Add(inventoryDetail);
                        }
                    }
                }
            }
            createDto.ItemId = inventoryCheckReport.Id;
        }

        var mediaFiles = new List<MediaFile>();
        foreach (var uri in createDto.Uris!)
        {
            var newMediaFile = new MediaFile
            {
                FileName = createDto.FileName!,
                RawUri = createDto.RawUri!,
                Uri = uri,
                FileType = createDto.FileType!,
                Content = createDto.Content!,
                ItemId = createDto.ItemId,
                IsVerified = createDto.IsVerified ?? false
            };
            mediaFiles.Add(newMediaFile);
        }

        if (mediaFiles.Count() > 0 && inventoryCheckReport == null)
        {
            if (!await _taskRepository.UpdateStatus(mediaFiles, createDto.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Báo cáo thất bại", StatusCode.SERVER_ERROR);
            }
        }
        else if (mediaFiles.Count() > 0 && inventoryCheckReport != null)
        {
            if (!await _taskRepository.InventoryCheckReport(mediaFiles, inventoryDetails, createDto.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Báo cáo thất bại", StatusCode.SERVER_ERROR);
            }
        }
        else
        {
            throw new ApiException("Báo cáo thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Báo cáo thành công");
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
                x => x.Id == id,
                x => x.Status != RequestStatus.Cancelled
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
                    assetCheckTask.CurrentRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                            new Expression<Func<Room, bool>>[]
                            {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == location.RoomId
                            });
                }
            }
            assetCheckTask.PriorityObj = assetCheckTask.Priority.GetValue();
            assetCheckTask.StatusObj = assetCheckTask.Status?.GetValue();
            assetCheckTask.Type = RequestType.StatusCheck;
            assetCheckTask.TypeObj = assetCheckTask.Type.GetValue();
            taskDetail = assetCheckTask;
        }

        //TRANSPORTATION
        var trasportTask = await MainUnitOfWork.TransportationRepository.FindOneAsync<TaskDetailDto>(
            new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == id,
                x => x.Status != RequestStatus.Cancelled
            });
        if (trasportTask != null)
        {
            trasportTask.Type = RequestType.Transportation;
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
                                        Quantity = (double)td!.Quantity!,
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
                                    },
                                    Quantity = td!.Quantity
                                }).ToListAsync();
            var tranportation = new TaskDetailDto
            {
                NewAssetId = null,
                Type = RequestType.Transportation,
                TypeObj = trasportTask.Type.GetValue(),
                Id = trasportTask.Id,
                RequestCode = trasportTask.RequestCode,
                RequestDate = trasportTask.RequestDate,
                Description = trasportTask.Description,
                Notes = trasportTask.Notes,
                //Quantity = trasportTask.Quantity,
                Priority = trasportTask.Priority,
                PriorityObj = trasportTask.Priority.GetValue(),
                Status = trasportTask.Status,
                StatusObj = trasportTask.Status!.GetValue(),
                CreatedAt = trasportTask.CreatedAt,
                EditedAt = trasportTask.EditedAt,
                CreatorId = trasportTask.CreatorId,
                EditorId = trasportTask.EditorId,
                Assets = assets,
                ToRoom = toRoom
            };
            taskDetail = tranportation;
        }

        //INVENTORY CHECK
        var inventoryCheckTask = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync<TaskDetailDto>(
            new Expression<Func<InventoryCheck, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.AssignedTo == AccountId,
                x => x.Id == id,
                x => x.Status != RequestStatus.Cancelled
            });
        if (inventoryCheckTask != null)
        {
            inventoryCheckTask.Type = RequestType.InventoryCheck;
            inventoryCheckTask.TypeObj = inventoryCheckTask.Type.GetValue();
            inventoryCheckTask.PriorityObj = inventoryCheckTask.Priority.GetValue();
            inventoryCheckTask.StatusObj = inventoryCheckTask.Status.GetValue();

            var inventoryCheckDetails = await MainUnitOfWork.InventoryCheckDetailRepository.GetQuery()
                                              .Include(x => x!.Asset)
                                              .Where(x => x!.InventoryCheckId == inventoryCheckTask.Id)
                                              .ToListAsync();
            var distinctRoomIds = inventoryCheckDetails.Select(detail => detail!.RoomId).Distinct();

            var roomAssetQuery = MainUnitOfWork.RoomAssetRepository.GetQuery();

            var rooms = await MainUnitOfWork.RoomRepository.GetQuery()
                             .Where(room => distinctRoomIds.Contains(room!.Id))
                             .ToListAsync();
            inventoryCheckTask.Rooms = distinctRoomIds.Select(roomId => new RoomInventoryCheckDto
            {
                Id = roomId,
                RoomName = rooms.FirstOrDefault(r => r!.Id == roomId)!.RoomName,
                Area = rooms.FirstOrDefault(r => r!.Id == roomId)!.Area,
                RoomCode = rooms.FirstOrDefault(r => r!.Id == roomId)!.RoomCode,
                FloorId = rooms.FirstOrDefault(r => r!.Id == roomId)!.FloorId,
                StatusId = rooms.FirstOrDefault(r => r!.Id == roomId)!.StatusId,
                Assets = inventoryCheckDetails
                    .Where(detail => detail!.RoomId == roomId)
                    .Select(detail => new AssetInventoryCheckDto
                    {
                        Id = detail!.AssetId,
                        AssetName = detail.Asset!.AssetName,
                        AssetCode = detail.Asset.AssetCode,
                        Quantity = inventoryCheckTask.Status != RequestStatus.Done ? roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == detail.AssetId && ra.RoomId == detail.RoomId)!.Quantity : detail.Quantity,
                        Status = inventoryCheckTask.Status != RequestStatus.Done ? detail.Asset.Status : detail.Status,
                        StatusObj = inventoryCheckTask.Status != RequestStatus.Done ? detail.Asset.Status.GetValue() : detail.Status.GetValue(),
                    }).ToList()
            }).ToList();

            taskDetail = inventoryCheckTask;
        }


        //REPLACEMENT
        var replacementTask = await MainUnitOfWork.ReplacementRepository.FindOneAsync<TaskDetailDto>(
                new Expression<Func<Replacement, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssignedTo == AccountId,
                    x => x.Id == id,
                    x => x.Status != RequestStatus.Cancelled
                });
        if (replacementTask != null)
        {
            replacementTask!.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                    new Expression<Func<Asset, bool>>[]
                    {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacementTask.AssetId
                    });

            var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == replacementTask.Asset!.Id,
                    x => x.ToDate == null
                });
            if (location != null)
            {
                replacementTask.CurrentRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                    new Expression<Func<Room, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == location.RoomId
                    });
            }

            replacementTask.NewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacementTask.NewAssetId
                });
            var toLocation = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == replacementTask.NewAsset!.Id,
                    x => x.ToDate == null
                });
            if (toLocation != null)
            {
                replacementTask.ToRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                    new Expression<Func<Room, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == toLocation.RoomId
                    });
            }

            replacementTask.RequestCode = replacementTask.RequestCode;
            replacementTask.RequestDate = replacementTask.RequestDate;
            replacementTask.Description = replacementTask.Description;
            replacementTask.PriorityObj = replacementTask.Priority.GetValue();
            replacementTask.Notes = replacementTask.Notes;
            replacementTask.Status = replacementTask.Status;
            replacementTask.StatusObj = replacementTask.Status!.GetValue();
            replacementTask.Type = RequestType.Replacement;
            replacementTask.TypeObj = replacementTask.Type.GetValue();
            taskDetail = replacementTask;
        }

        //REPAIR
        var repairation = await MainUnitOfWork.RepairRepository.FindOneAsync<TaskDetailDto>(
                new Expression<Func<Repair, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssignedTo == AccountId,
                    x => x.Id == id,
                    x => x.Status != RequestStatus.Cancelled
                });
        if (repairation != null)
        {
            repairation.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == repairation.AssetId
                });
            if (repairation.Asset != null)
            {
                repairation.Asset!.StatusObj = repairation.Asset.Status?.GetValue();
                var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.AssetId == repairation.Asset.Id,
                        x => x.ToDate == null
                    });
                if (location != null)
                {
                    repairation.CurrentRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                        new Expression<Func<Room, bool>>[]
                        {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == location.RoomId
                        });
                }
            }

            repairation.PriorityObj = repairation.Priority.GetValue();
            repairation.StatusObj = repairation.Status!.GetValue();
            repairation.Type = RequestType.Repairation;
            repairation.TypeObj = repairation.Type.GetValue();

            taskDetail = repairation;
        }

        //MAINTENANCE
        var maintenance = await MainUnitOfWork.MaintenanceRepository.FindOneAsync<TaskDetailDto>(
                new Expression<Func<Maintenance, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssignedTo == AccountId,
                    x => x.Id == id,
                    x => x.Status != RequestStatus.Cancelled
                });
        if (maintenance != null)
        {
            maintenance.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == maintenance.AssetId
                });
            if (maintenance.Asset != null)
            {
                maintenance.Asset!.StatusObj = maintenance.Asset.Status?.GetValue();
                var location = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.AssetId == maintenance.Asset.Id,
                        x => x.ToDate == null
                    });
                if (location != null)
                {
                    maintenance.CurrentRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                        new Expression<Func<Room, bool>>[]
                        {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == location.RoomId
                        });
                }
            }

            maintenance.PriorityObj = maintenance.Priority.GetValue();
            maintenance.StatusObj = maintenance.Status!.GetValue();
            maintenance.Type = RequestType.Maintenance;
            maintenance.TypeObj = maintenance.Type.GetValue();

            taskDetail = maintenance;
        }

        if (taskDetail == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
        }

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

        // Retrieve tasks from the Replacement table
        var replacementTasks = MainUnitOfWork.ReplacementRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Replacement,
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

        // Retrieve tasks from the Replair table
        var repairationTasks = MainUnitOfWork.RepairRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.Repairation,
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

        var inventoryCheckTask = MainUnitOfWork.InventoryCheckRepository.GetQuery()
            .Where(t => !t!.DeletedAt.HasValue && t.AssignedTo == AccountId)
            .Select(t => new TaskBaseDto
            {
                Type = RequestType.InventoryCheck,
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
        var combinedTasks = transportationTasks.Union(maintenanceTasks)
                                               .Union(assetCheckTasks)
                                               .Union(replacementTasks)
                                               .Union(repairationTasks)
                                               .Union(inventoryCheckTask);

        combinedTasks = combinedTasks.Where(x => x.Status != RequestStatus.Cancelled);

        if (queryDto.Type != null)
        {
            combinedTasks = combinedTasks.Where(x => x.Type == queryDto.Type);
        }

        if (queryDto.Status != null)
        {
            combinedTasks = combinedTasks.Where(x => x.Status == queryDto.Status);
        }

        // Sort
        var isDescending = queryDto.OrderBy.Split(' ').Last().ToLowerInvariant()
            .StartsWith("desc");

        var sortField = queryDto.OrderBy.Split(' ').First();

        // Sort
        if (!string.IsNullOrEmpty(sortField))
        {
            try
            {
                combinedTasks = combinedTasks.OrderBy(sortField, isDescending);
            }
            catch
            {
                throw new ApiException($"Không tồn tại trường thông tin {sortField}", StatusCode.BAD_REQUEST);
            }
        }

        var totalCount = await combinedTasks.CountAsync();

        combinedTasks = combinedTasks.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var items = await combinedTasks.Select(x => new TaskBaseDto
        {
            Type = x.Type,
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

    //public async Task<ApiResponse> InventoryCheckReport(InventoryCheckReport reportDto)
    //{
    //    //INVENTORY CHECK
    //    var inventoryCheckReport = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(
    //        new Expression<Func<InventoryCheck, bool>>[]
    //        {
    //            x => !x.DeletedAt.HasValue,
    //            x => x.AssignedTo == AccountId,
    //            x => x.Id == reportDto.ItemId,
    //            x => x.Status != RequestStatus.Cancelled
    //        });

    //    var mediaFiles = new List<MediaFile>();
    //    foreach (var uri in reportDto.Uris!)
    //    {
    //        var newMediaFile = new MediaFile
    //        {
    //            FileName = reportDto.FileName!,
    //            RawUri = reportDto.RawUri!,
    //            Uri = uri,
    //            FileType = reportDto.FileType!,
    //            Content = reportDto.Content!,
    //            ItemId = reportDto.ItemId
    //        };
    //        mediaFiles.Add(newMediaFile);
    //    }

    //    return ApiResponse.Created("Báo cáo thành công");
    //}
}