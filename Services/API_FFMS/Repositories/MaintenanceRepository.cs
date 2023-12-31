﻿using API_FFMS.Dtos;
using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IMaintenanceRepository
{
    Task<bool> InsertMaintenance(Maintenance maintenance, List<Report> mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> ConfirmOrReject(Maintenance maintenance, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null);
    Task<bool> InsertMaintenances(List<Maintenance> maintenances, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> DeleteMulti(List<Maintenance?> maintenances, Guid? deleterId, DateTime? now = null);
    Task<bool> DeleteMaintenance(Maintenance maintenance, Guid? deleterId, DateTime? now = null);
    Task<bool> UpdateMaintenance(Maintenance maintenance, List<Report?> additionMediaFiles, List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null);
}

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly DatabaseContext _context;

    public MaintenanceRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertMaintenances(List<Maintenance> entities, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        var requestCodes = GetRequestCodes();
        List<int> numbers = requestCodes.Where(x => x.StartsWith("MTN"))
                                        .Select(x => int.TryParse(x[3..], out var lastNumber) ? lastNumber : 0)
                                        .ToList();
        try
        {
            foreach (var entity in entities)
            {
                entity.CreatedAt = now.Value;
                entity.CreatorId = creatorId;
                entity.Status = RequestStatus.NotStart;
                entity.RequestDate = now.Value;
                entity.RequestCode = "MTN" + GenerateRequestCode(ref numbers);
                await _context.Maintenances.AddAsync(entity);

                var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == entity.AssetId)
                        .FirstOrDefaultAsync();
                if (asset != null)
                {
                    asset.RequestStatus = RequestType.Maintenance;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = entity.Description ?? "Yêu cầu bảo trì",
                    Title = RequestType.Maintenance.GetDisplayName(),
                    Type = NotificationType.Task,
                    CreatorId = creatorId,
                    IsRead = false,
                    ItemId = entity.Id,
                    UserId = entity.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);

                if (mediaFiles != null && mediaFiles.Count > 0)
                {
                    await _context.MediaFiles.AddRangeAsync(mediaFiles);
                }

            }
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    private int GenerateRequestCode(ref List<int> numbers)
    {
        int newRequestNumber = numbers.Any() ? numbers.Max() + 1 : 1;
        numbers.Add(newRequestNumber); // Add the new number to the list
        return newRequestNumber;
    }

    private List<string> GetRequestCodes()
    {
        var requests = _context.Maintenances.Where(x => x.RequestCode.StartsWith("MTN"))
                                            .Select(x => x.RequestCode)
                                            .ToList();
        return requests;
    }

    public async Task<bool> ConfirmOrReject(Maintenance maintenance, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            maintenance.EditedAt = now.Value;
            maintenance.EditorId = editorId;
            maintenance.CompletionDate = now.Value;
            _context.Entry(maintenance).State = EntityState.Modified;

            var asset = await _context.Assets
                            .Include(a => a.Type)
                            .Where(a => a.Id == maintenance.AssetId)
                            .FirstOrDefaultAsync();

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
            var reports = await _context.MediaFiles.FirstOrDefaultAsync(x => x.ItemId == maintenance.Id && !x.IsReject && x.IsReported);

            if (maintenance.IsInternal == true)
            {
                if (confirmOrRejectDto?.Status == RequestStatus.Done)
                {
                    asset!.RequestStatus = RequestType.Operational;
                    asset.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.LastMaintenanceTime = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.EditorId = editorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    location!.State = RoomState.Operational;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = "Đã xác nhận",
                        Title = "Đã xác nhận",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.NeedAdditional)
                {
                    maintenance.Status = RequestStatus.InProgress;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    if (reports != null)
                    {
                        reports.IsReject = true;
                        reports.RejectReason = confirmOrRejectDto.Reason;
                        _context.Entry(reports).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = confirmOrRejectDto.Reason ?? "Cần bổ sung",
                        Title = "Báo cáo lại",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.Cancelled)
                {
                    maintenance.Status = RequestStatus.Cancelled;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    if (reports != null)
                    {
                        reports.IsReject = true;
                        reports.RejectReason = confirmOrRejectDto.Reason;
                        _context.Entry(reports).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = confirmOrRejectDto.Reason ?? "Hủy yêu cầu",
                        Title = "Hủy yêu cầu",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);

                    asset!.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;
                }
            }
            else if (maintenance.IsInternal == false)
            {
                if (confirmOrRejectDto?.Status == RequestStatus.Done)
                {
                    maintenance.CompletionDate = now.Value;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    asset!.RequestStatus = RequestType.Operational;
                    asset.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.ToDate = now.Value;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.EditorId = editorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    var addRoomAsset = new RoomAsset
                    {
                        FromDate = now.Value,
                        AssetId = asset.Id,
                        RoomId = GetWareHouse("Kho")!.Id,
                        Status = AssetStatus.Operational,
                        ToDate = null,
                        EditedAt = now.Value,
                        CreatedAt = now.Value,
                        CreatorId = editorId,
                    };
                    await _context.RoomAssets.AddAsync(addRoomAsset);

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = "Đã xác nhận",
                        Title = "Đã xác nhận",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.NeedAdditional)
                {
                    maintenance.Status = RequestStatus.InProgress;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    if (reports != null)
                    {
                        reports.IsReject = true;
                        reports.RejectReason = confirmOrRejectDto.Reason;
                        _context.Entry(reports).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = confirmOrRejectDto.Reason ?? "Cần bổ sung",
                        Title = "Báo cáo lại",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.Cancelled)
                {
                    maintenance.Status = RequestStatus.Cancelled;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    if (reports != null)
                    {
                        reports.IsReject = true;
                        reports.RejectReason = confirmOrRejectDto.Reason;
                        _context.Entry(reports).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = confirmOrRejectDto.Reason ?? "Hủy yêu cầu",
                        Title = "Hủy yêu cầu",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);

                    asset!.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> DeleteMulti(List<Maintenance?> maintenances, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            foreach (var maintenance in maintenances)
            {
                if (maintenance == null) continue;

                maintenance.DeletedAt = now.Value;
                maintenance.DeleterId = deleterId;
                _context.Entry(maintenance).State = EntityState.Modified;

                 var asset = await _context.Assets.FindAsync(maintenance.AssetId);

                var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == maintenance.AssetId && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == maintenance.Id);
                if (notification != null)
                {
                    notification.DeletedAt = now.Value;
                    notification.DeleterId = deleterId;
                    _context.Entry(notification).State = EntityState.Modified;
                }

                if (asset != null)
                {
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                if (location != null)
                {
                    location!.State = RoomState.Operational;
                    location.EditedAt = now.Value;
                    location.EditorId = deleterId;
                    _context.Entry(location).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> DeleteMaintenance(Maintenance maintenance, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            maintenance.DeletedAt = now.Value;
            maintenance.DeleterId = deleterId;
            _context.Entry(maintenance).State = EntityState.Modified;

            var asset = await _context.Assets
                            .Include(a => a.Type)
                            .Where(a => a.Id == maintenance.AssetId)
                            .FirstOrDefaultAsync();

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == maintenance.Id);

            if (notification != null)
            {
                notification.DeletedAt = now.Value;
                notification.DeleterId = deleterId;
                _context.Entry(notification).State = EntityState.Modified;
            }

            if (asset != null)
            {
                asset.RequestStatus = RequestType.Operational;
                asset.EditedAt = now.Value;
                asset.EditorId = deleterId;
                _context.Entry(asset).State = EntityState.Modified;
            }

            if (location != null)
            {
                location!.State = RoomState.Operational;
                location.EditedAt = now.Value;
                location.EditorId = deleterId;
                _context.Entry(location).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public Room? GetWareHouse(string roomName)
    {
        var wareHouse = _context.Rooms.FirstOrDefault(x => x.RoomName!.Trim().Equals(roomName.Trim()));
        return wareHouse;
    }

    public async Task<bool> InsertMaintenance(Maintenance entity, List<Report> mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = now.Value;
            entity.EditedAt = now.Value;
            entity.CreatorId = creatorId;
            entity.Status = RequestStatus.NotStart;
            entity.RequestDate = now.Value;
            await _context.Maintenances.AddAsync(entity);

            var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == entity.AssetId)
                        .FirstOrDefaultAsync();
            if (asset != null)
            {
                asset.RequestStatus = RequestType.Maintenance;
                _context.Entry(asset).State = EntityState.Modified;
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = entity.Description ?? "Yêu cầu bảo trì",
                Title = RequestType.Maintenance.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = entity.Id,
                UserId = entity.AssignedTo
            };
            await _context.Notifications.AddAsync(notification);

            foreach (var mediaFile in mediaFiles)
            {
                var newMediaFile = new Report
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = now.Value,
                    CreatorId = creatorId,
                    EditedAt = now.Value,
                    EditorId = creatorId,
                    FileName = mediaFile.FileName,
                    Uri = mediaFile.Uri,
                    FileType = FileType.File,
                    ItemId = entity.Id
                };
                _context.MediaFiles.Add(newMediaFile);
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }

    public async Task<bool> UpdateMaintenance(Maintenance maintenance, List<Report?> additionMediaFiles, List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            maintenance.EditorId = editorId;
            maintenance.EditedAt = now.Value;
            _context.Entry(maintenance).State = EntityState.Modified;

            var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                .Where(x => x.ItemId == maintenance.Id && !x.DeletedAt.HasValue)
                                                .ToList();

            if (additionMediaFiles.Count > 0)
            {
                foreach (var mediaFile in additionMediaFiles)
                {
                    if (mediaFile != null)
                    {
                        _context.MediaFiles.Add(mediaFile);
                    }

                }
            }

            if (removalMediaFiles.Count > 0)
            {
                foreach (var mediaFile in removalMediaFiles)
                {
                    if (mediaFile != null)
                    {
                        _context.MediaFiles.Remove(mediaFile);
                    }
                }
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }
}