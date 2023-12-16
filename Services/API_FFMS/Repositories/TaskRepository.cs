using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface ITaskRepository
{
    Task<bool> UpdateStatus(List<Report> reports, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    Task<bool> InventoryCheckReport(List<Report> reports, List<InventoryCheckDetail>? inventoryCheckDetails, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
}
public class TaskRepository : ITaskRepository
{
    private readonly DatabaseContext _context;

    public TaskRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InventoryCheckReport(List<Report> reports, List<InventoryCheckDetail>? inventoryCheckDetails, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            var inventoryCheck = await _context.InventoryChecks
                                    .Include(x => x.InventoryCheckDetails)
                                    .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (inventoryCheck != null)
            {
                inventoryCheck.EditedAt = now.Value;
                inventoryCheck.EditorId = editorId;
                inventoryCheck.Status = statusUpdate;
                _context.Entry(inventoryCheck).State = EntityState.Modified;

                if (statusUpdate == RequestStatus.InProgress)
                {
                    inventoryCheck.Checkin = now.Value;
                    _context.Entry(inventoryCheck).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    if (inventoryCheckDetails != null)
                    {
                        foreach (var detail in inventoryCheckDetails)
                        {
                            var existingDetail = inventoryCheck.InventoryCheckDetails!
                                    .FirstOrDefault(x => x.AssetId == detail.AssetId &&
                                                         x.RoomId == detail.RoomId &&
                                                         x.InventoryCheckId == inventoryCheck.Id);
                            if (existingDetail != null)
                            {
                                existingDetail.EditedAt = now.Value;
                                existingDetail.EditorId = editorId;
                                existingDetail.StatusReported = detail.StatusReported;
                                existingDetail.QuantityReported = detail.QuantityReported;
                                _context.Entry(existingDetail).State = EntityState.Modified;
                            }
                        }
                    }

                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = inventoryCheck.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }

                    inventoryCheck.Result = reports.First().Content;
                    inventoryCheck.Checkout = now.Value;
                    _context.Entry(inventoryCheck).State = EntityState.Modified;
                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = inventoryCheck.Description ?? "Báo cáo kiểm kê",
                        Title = "Báo cáo kiểm kê",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = inventoryCheck.Id,
                        UserId = inventoryCheck.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
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

    public async Task<bool> UpdateStatus(List<Report> reports, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            //ASSET CHECK
            var assetCheck = await _context.AssetChecks
                             .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (assetCheck != null)
            {
                assetCheck.EditedAt = now.Value;
                assetCheck.EditorId = editorId;
                assetCheck.Status = statusUpdate;
                _context.Entry(assetCheck).State = EntityState.Modified;

                var asset = await _context.Assets.Include(x => x.Type).FirstOrDefaultAsync(x => x.Id == assetCheck.AssetId);
                var roomAsset = await _context.RoomAssets
                                .FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var assetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                if (assetCheck.Status == RequestStatus.InProgress)
                {
                    assetLocation!.State = RoomState.NeedInspection;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = editorId;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    assetCheck.Checkin = now.Value;
                    _context.Entry(assetCheck).State = EntityState.Modified;
                }
                else if (assetCheck.Status == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = assetCheck.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    assetCheck.Result = reports.First().Content;
                    assetCheck.Checkout = now.Value;
                    assetCheck.IsVerified = reports.First().IsVerified;
                    assetCheck.CompletionDate = now.Value;
                    _context.Entry(assetCheck).State = EntityState.Modified;

                    if (assetCheck.IsVerified == true)
                    {
                        if (asset != null && asset.Type!.Unit == Unit.Individual)
                        {
                            asset.Status = AssetStatus.Damaged;
                            asset.RequestStatus = RequestType.Operational;
                            asset.EditedAt = now.Value;
                            asset.EditorId = editorId;
                            _context.Entry(asset).State = EntityState.Modified;
                        }

                        if (roomAsset != null)
                        {
                            roomAsset.Status = AssetStatus.Damaged;
                            roomAsset.EditedAt = now.Value;
                            roomAsset.EditorId = editorId;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }
                    }
                    else if (assetCheck.IsVerified == false)
                    {
                        asset!.Status = AssetStatus.Operational;
                        asset.RequestStatus = RequestType.Operational;
                        asset.EditedAt = now.Value;
                        asset.EditorId = editorId;
                        _context.Entry(asset).State = EntityState.Modified;

                        if (roomAsset != null)
                        {
                            roomAsset.Status = AssetStatus.Operational;
                            roomAsset.EditedAt = now.Value;
                            roomAsset.EditorId = editorId;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = assetCheck.Description ?? "Báo cáo kiểm tra",
                        Title = "Báo cáo kiểm tra",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = assetCheck.Id,
                        UserId = assetCheck.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }

            //TRANSPORTATION
            var transportation = await _context.Transportations
                                .Include(x => x.TransportationDetails)
                                .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (transportation != null)
            {
                transportation.EditedAt = now.Value;
                transportation.EditorId = editorId;
                transportation.Status = statusUpdate;
                _context.Entry(transportation).State = EntityState.Modified;

                var assetIds = transportation.TransportationDetails!.Select(td => td.AssetId).ToList();
                var assets = await _context.Assets
                            .Include(a => a.Type)
                            .Where(asset => assetIds!.Contains(asset.Id))
                            .ToListAsync();

                var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);

                if (statusUpdate == RequestStatus.InProgress)
                {
                    transportation.Checkin = now.Value;
                    _context.Entry(transportation).State = EntityState.Modified;

                    foreach (var asset in assets)
                    {
                        var transportDetail = await _context.TransportationDetails
                                                        .FirstOrDefaultAsync(x => x.TransportationId == transportation.Id &&
                                                                                  x.AssetId == asset.Id);
                        var fromRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                           x.RoomId == transportDetail!.FromRoomId &&
                                                                                           x.ToDate == null);

                        var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id && x.ToDate == null);
                        var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == transportDetail!.FromRoomId);

                        if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                        {
                            asset.RequestStatus = RequestType.Transportation;
                            asset.EditedAt = now.Value;
                            asset.EditorId = editorId;
                            _context.Entry(asset).State = EntityState.Modified;
                        }

                        toRoom!.State = RoomState.Transportation;
                        toRoom.EditedAt = now.Value;
                        toRoom.EditorId = editorId;
                        _context.Entry(toRoom).State = EntityState.Modified;

                        fromRoom!.State = RoomState.Transportation;
                        fromRoom.EditedAt = now.Value;
                        fromRoom.EditorId = editorId;
                        _context.Entry(fromRoom).State = EntityState.Modified;
                    }
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = transportation.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    transportation.Result = reports.First().Content;
                    transportation.Checkout = now.Value;
                    _context.Entry(transportation).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = transportation.Description ?? "Báo cáo vận chuyển",
                        Title = "Báo cáo vận chuyển",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = transportation.Id,
                        UserId = transportation.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }

            //REPAIRATION
            var repairation = await _context.Repairs
                            .Include(x => x.Asset)
                            .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (repairation != null && repairation.IsInternal)
            {
                repairation.EditedAt = now.Value;
                repairation.EditorId = editorId;
                repairation.Status = statusUpdate;
                _context.Entry(repairation).State = EntityState.Modified;

                var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == repairation.AssetId)
                        .FirstOrDefaultAsync();

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                if (statusUpdate == RequestStatus.InProgress)
                {
                    if (asset != null)
                    {
                        if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                        {
                            asset.RequestStatus = RequestType.Repairation;
                            asset.EditedAt = now.Value;
                            asset.EditorId = editorId;
                            _context.Entry(asset).State = EntityState.Modified;
                        }

                        if (asset.IsMovable == false)
                        {
                            location!.State = RoomState.Repair;
                            location.EditedAt = now.Value;
                            location.EditorId = editorId;
                            _context.Entry(location).State = EntityState.Modified;
                        }
                        else if (asset.IsMovable == true)
                        {
                            location!.State = RoomState.MissingAsset;
                            location.EditedAt = now.Value;
                            location.EditorId = editorId;
                            _context.Entry(location).State = EntityState.Modified;
                        }
                    }
                    repairation.Checkin = now.Value;
                    _context.Entry(repairation).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = repairation.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    repairation.Result = reports.First().Content;
                    repairation.Checkout = now.Value;
                    _context.Entry(repairation).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = repairation.Description ?? "Báo cáo sửa chữa",
                        Title = "Báo cáo sửa chữa",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = repairation.Id,
                        UserId = repairation.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }
            else if (repairation != null && repairation.IsInternal == false)
            {
                repairation.EditedAt = now.Value;
                repairation.EditorId = editorId;
                repairation.Status = statusUpdate;
                _context.Entry(repairation).State = EntityState.Modified;

                var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == repairation.AssetId)
                        .FirstOrDefaultAsync();

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                if (statusUpdate == RequestStatus.InProgress)
                {
                    asset!.RequestStatus = RequestType.Repairation;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    location!.State = RoomState.MissingAsset;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;

                    repairation.Checkin = now.Value;
                    _context.Entry(repairation).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            ItemId = repairation.Id,
                            IsReported = true,
                            RepairId = repairation.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    repairation.Result = reports.First().Content;
                    repairation.Checkout = now.Value;
                    _context.Entry(repairation).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = repairation.Description ?? "Báo cáo sửa chữa",
                        Title = "Báo cáo sửa chữa",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = repairation.Id,
                        UserId = repairation.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }

            //REPLACEMENT
            var replacement = await _context.Replacements
                              .Include(x => x.Asset)
                              .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (replacement != null)
            {
                replacement.EditedAt = now.Value;
                replacement.EditorId = editorId;
                replacement.Status = statusUpdate;
                _context.Entry(replacement).State = EntityState.Modified;

                //ASSET
                var asset = await _context.Assets.FindAsync(replacement.AssetId);
                var newAsset = await _context.Assets.FindAsync(replacement.NewAssetId);

                //ROOMASSET
                var roomAsset = await _context.RoomAssets
                                .FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var roomAssetNew = await _context.RoomAssets
                                .FirstOrDefaultAsync(x => x.AssetId == newAsset!.Id && x.ToDate == null);

                //LOCATION
                var assetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == replacement.RoomId);

                var newAssetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == replacement.NewRoomId);

                if (replacement.Status == RequestStatus.InProgress)
                {
                    asset!.RequestStatus = RequestType.Replacement;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.RequestStatus = RequestType.Replacement;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = editorId;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Replacement;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = editorId;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    newAssetLocation!.State = RoomState.Replacement;
                    newAssetLocation.EditedAt = now.Value;
                    newAssetLocation.EditorId = editorId;
                    _context.Entry(newAssetLocation).State = EntityState.Modified;

                    replacement.Checkin = now.Value;
                    _context.Entry(replacement).State = EntityState.Modified;
                }
                else if (replacement.Status == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = replacement.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    replacement.Result = reports.First().Content;
                    replacement.Checkout = now.Value;
                    _context.Entry(replacement).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = replacement.Description ?? "Báo cáo thay thế",
                        Title = "Báo cáo thay thế",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = replacement.Id,
                        UserId = replacement.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }

            //MAINTENANCE
            var maintenance = await _context.Maintenances
                              .Include(x => x.Asset)
                              .FirstOrDefaultAsync(x => x.Id == reports.First().ItemId);
            if (maintenance != null && maintenance.IsInternal == true)
            {
                maintenance.EditedAt = now.Value;
                maintenance.EditorId = editorId;
                maintenance.Status = statusUpdate;
                _context.Entry(maintenance).State = EntityState.Modified;

                var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == maintenance.AssetId)
                        .FirstOrDefaultAsync();

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                if (statusUpdate == RequestStatus.InProgress)
                {
                    asset!.RequestStatus = RequestType.Maintenance;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    location!.State = RoomState.Maintenance;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;

                    maintenance.Checkin = now.Value;
                    _context.Entry(maintenance).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = maintenance.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    maintenance.Result = reports.First().Content;
                    maintenance.Checkout = now.Value;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = maintenance.Description ?? "Báo cáo bảo trì",
                        Title = "Báo cáo bảo trì",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }
            else if (maintenance != null && maintenance.IsInternal == true)
            {
                maintenance.EditedAt = now.Value;
                maintenance.EditorId = editorId;
                maintenance.Status = statusUpdate;
                _context.Entry(maintenance).State = EntityState.Modified;

                var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == maintenance.AssetId)
                        .FirstOrDefaultAsync();

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                if (statusUpdate == RequestStatus.InProgress)
                {
                    asset!.RequestStatus = RequestType.Maintenance;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    if (asset.IsMovable == false)
                    {
                        location!.State = RoomState.Maintenance;
                        location.EditedAt = now.Value;
                        location.EditorId = editorId;
                        _context.Entry(location).State = EntityState.Modified;
                    }
                    else if (asset.IsMovable == true)
                    {
                        location!.State = RoomState.MissingAsset;
                        location.EditedAt = now.Value;
                        location.EditorId = editorId;
                        _context.Entry(location).State = EntityState.Modified;
                    }

                    maintenance.Checkin = now.Value;
                    _context.Entry(maintenance).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Reported)
                {
                    foreach (var report in reports)
                    {
                        var newReport = new Report
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = now.Value,
                            CreatorId = editorId,
                            EditedAt = now.Value,
                            EditorId = editorId,
                            FileName = report.FileName,
                            Uri = report.Uri,
                            FileType = report.FileType,
                            Content = report.Content,
                            IsReported = true,
                            ItemId = maintenance.Id,
                            MaintenanceId = maintenance.Id
                        };
                        _context.MediaFiles.Add(newReport);
                    }
                    maintenance.Result = reports.First().Content;
                    maintenance.Checkout = now.Value;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = maintenance.Description ?? "Báo cáo bảo trì",
                        Title = "Báo cáo bảo trì",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = maintenance.Id,
                        UserId = maintenance.CreatorId
                    };
                    await _context.Notifications.AddAsync(notification);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return true;
            }

            return true;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            return false;
        }
    }
}