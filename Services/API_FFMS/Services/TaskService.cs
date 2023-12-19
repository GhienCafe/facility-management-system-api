using System.Linq.Expressions;
using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        // Handle report content
        var reports = new List<Report>();
        var listUrisJson = JsonConvert.SerializeObject(createDto.Uris);
        var report = new Report
        {
            FileName = createDto.FileName!,
            Uri = listUrisJson,
            Content = createDto.Content,
            FileType = createDto.FileType!,
            ItemId = createDto.ItemId,
            IsVerified = createDto.IsVerified ?? false,
            IsReported = true,
        };

        reports.Add(report);

        var reportedTask = await MainUnitOfWork.TaskRepository.FindOneAsync(createDto.ItemId ?? Guid.Empty);

        if (reportedTask != null)
        {
            createDto.ItemId = reportedTask.Id;
        }

        // For normal task
        if (reportedTask != null && reportedTask.Type != RequestType.InventoryCheck)
        {
            if (reports.Count > 0)
            {
                if (!await _taskRepository.UpdateStatus(reports, createDto.Status, AccountId, CurrentDate))
                {
                    throw new ApiException("Báo cáo thất bại", StatusCode.SERVER_ERROR);
                }
            }
        } // For inventory check
        else
        {
            var inventoryDetails = new List<InventoryCheckDetail>();
            if (reportedTask != null)
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
                                    InventoryCheckId = reportedTask.Id,
                                    RoomId = (Guid)room.RoomId,
                                    StatusReported = assetReport.Status,
                                    QuantityReported = assetReport.Quantity
                                };

                                inventoryDetails.Add(inventoryDetail);
                            }
                        }
                    }
                }

                createDto.ItemId = reportedTask.Id;
            }

            if (reports.Count > 0 && reportedTask != null)
            {
                if (!await _taskRepository.InventoryCheckReport(reports, inventoryDetails, createDto.Status,
                        AccountId, CurrentDate))
                {
                    throw new ApiException("Báo cáo thất bại", StatusCode.SERVER_ERROR);
                }
            }
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
            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
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
                            ModelId = asset.ModelId
                        },
                        FromRoom = new RoomBaseDto
                        {
                            Id = (Guid)td.FromRoomId!,
                            RoomCode = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.RoomCode,
                            RoomName = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.RoomName,
                            StatusId = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.StatusId,
                            FloorId = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.FloorId
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
                CreatorId = trasportTask.CreatorId,
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
                        QuantityBefore = detail.QuantityBefore,
                        StatusBefore = detail.StatusBefore,
                        StatusBeforeObj = detail.StatusBefore.GetValue(),
                        QuantityReported = detail.QuantityReported,
                        StatusReported = detail.StatusReported,
                        StatusReportedObj = detail.StatusReported.GetValue()
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

            replacementTask.CurrentRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                new Expression<Func<Room, bool>>[]
                {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == replacementTask.RoomId
                });

            replacementTask.NewAsset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == replacementTask.NewAssetId
                });

            replacementTask.ToRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                new Expression<Func<Room, bool>>[]
                {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == replacementTask.NewRoomId
                });


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

        var combinedTasks = MainUnitOfWork.TaskRepository.GetQuery()
            .Where(x => x.AssignedTo == AccountId);

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
            CreatorId = x.CreatorId ?? Guid.Empty,
            EditorId = x.EditorId ?? Guid.Empty,
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
            ToRoomId = x.ToRoomId,
            Priority = x.Priority
        }).ToListAsync();

        items.ForEach(x =>
        {
            x.StatusObj = x.Status?.GetValue();
            x.TypeObj = x.Type.GetValue();
            x.PriorityObj = x.Priority.GetValue();
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