using AppCore.Extensions;
using DocumentFormat.OpenXml.Vml.Office;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IMaintenanceRepository
{
    Task<bool> InsertMaintenance(Maintenance maintenance, Guid? creatorId, DateTime? now = null);
    Task<bool> InsertMaintenanceV2(Maintenance maintenance, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateStatus(Maintenance maintenance, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    Task<bool> InsertMaintenances(List<Maintenance> maintenances, Guid? creatorId, DateTime? now = null);
    Task<bool> DeleteMaintenances(List<Maintenance?> maintenances, Guid? deleterId, DateTime? now = null);
    Task<bool> DeleteMaintenance(Maintenance maintenance, Guid? deleterId, DateTime? now = null);
}

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly DatabaseContext _context;

    public MaintenanceRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertMaintenance(Maintenance entity, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            // Add maintenance
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = now.Value;
            entity.EditedAt = now.Value;
            entity.CreatorId = creatorId;
            entity.Status = RequestStatus.NotStart;
            entity.RequestDate = now.Value;
            await _context.Maintenances.AddAsync(entity);

            // Update asset status
            var asset = await _context.Assets.FindAsync(entity.AssetId);
            asset!.Status = AssetStatus.Maintenance;
            asset.EditedAt = now.Value;
            _context.Entry(asset).State = EntityState.Modified;

            // Update room & send notification
            if (entity.IsInternal)
            {
                var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == entity.AssetId && x.ToDate == null);

                if (roomAsset != null)
                {
                    roomAsset!.Status = AssetStatus.Maintenance;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.EditorId = creatorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    EditedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = entity.Description,
                    Title = RequestType.Maintenance.GetDisplayName(),
                    Type = NotificationType.Task,
                    CreatorId = creatorId,
                    IsRead = false,
                    ItemId = entity.Id,
                    UserId = entity.AssignedTo
                };

                await _context.Notifications.AddAsync(notification);
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

    public async Task<bool> InsertMaintenances(List<Maintenance> entities, Guid? creatorId, DateTime? now = null)
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
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = now.Value;
                entity.EditedAt = now.Value;
                entity.CreatorId = creatorId;
                entity.Status = RequestStatus.NotStart;
                entity.RequestDate = now.Value;
                entity.RequestCode = "MTN" + GenerateRequestCode(ref numbers);
                await _context.Maintenances.AddAsync(entity);

                var asset = await _context.Assets.FindAsync(entity.AssetId);
                asset!.Status = AssetStatus.Maintenance;
                asset.EditedAt = now.Value;
                asset.EditorId = creatorId;
                _context.Entry(asset).State = EntityState.Modified;

                if (entity.IsInternal)
                {
                    var roomAsset = await _context.RoomAssets
                        .FirstOrDefaultAsync(x => x.AssetId == entity.AssetId && x.ToDate == null);

                    if (roomAsset != null)
                    {
                        roomAsset!.Status = AssetStatus.Maintenance;
                        roomAsset.EditedAt = now.Value;
                        roomAsset.EditorId = creatorId;
                        _context.Entry(roomAsset).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = entity.Description,
                        Title = RequestType.Maintenance.GetDisplayName(),
                        Type = NotificationType.Task,
                        CreatorId = creatorId,
                        IsRead = false,
                        ItemId = entity.Id,
                        UserId = entity.AssignedTo
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

    public async Task<bool> UpdateStatus(Maintenance maintenance, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            maintenance.EditedAt = now.Value;
            maintenance.EditorId = editorId;
            maintenance.Status = statusUpdate;
            maintenance.CompletionDate = now.Value;
            _context.Entry(maintenance).State = EntityState.Modified;

            var asset = await _context.Assets
                            .Include(a => a.Type)
                            .Where(a => a.Id == maintenance.AssetId)
                            .FirstOrDefaultAsync();

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
            if (maintenance.IsInternal == true)
            {
                if (statusUpdate == RequestStatus.Done)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.ToDate = now.Value;
                    roomAsset.EditorId = editorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    location!.State = RoomState.Operational;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Cancelled)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.ToDate = now.Value;
                    roomAsset.EditorId = editorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    location!.State = RoomState.Operational;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;
                }
            }
            else if (maintenance.IsInternal == false)
            {
                if (statusUpdate == RequestStatus.Done)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

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
                }
                else if (statusUpdate == RequestStatus.Cancelled)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.ToDate = now.Value;
                    roomAsset.EditorId = editorId;
                    _context.Entry(roomAsset).State = EntityState.Modified;
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

    public async Task<bool> DeleteMaintenances(List<Maintenance?> maintenances, Guid? deleterId, DateTime? now = null)
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
                asset!.Status = AssetStatus.Maintenance;
                asset.EditedAt = now.Value;
                asset.EditorId = deleterId;
                _context.Entry(asset).State = EntityState.Modified;

                var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == maintenance.AssetId && x.ToDate == null);
                var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                if (asset != null)
                {
                    asset.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                if (roomAsset != null)
                {
                    roomAsset.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.EditorId = deleterId;
                    _context.Entry(roomAsset).State = EntityState.Modified;
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

            if (asset != null)
            {
                asset.Status = AssetStatus.Operational;
                asset.EditedAt = now.Value;
                asset.EditorId = deleterId;
                _context.Entry(asset).State = EntityState.Modified;
            }

            if (roomAsset != null)
            {
                roomAsset.Status = AssetStatus.Operational;
                roomAsset.EditedAt = now.Value;
                roomAsset.EditorId = deleterId;
                _context.Entry(roomAsset).State = EntityState.Modified;
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

    public async Task<bool> InsertMaintenanceV2(Maintenance entity, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            // Add maintenance
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = now.Value;
            entity.EditedAt = now.Value;
            entity.CreatorId = creatorId;
            entity.Status = RequestStatus.NotStart;
            entity.RequestDate = now.Value;
            await _context.Maintenances.AddAsync(entity);

            // Update asset status
            var asset = await _context.Assets.FindAsync(entity.AssetId);
            asset!.Status = AssetStatus.Maintenance;
            asset.EditedAt = now.Value;
            _context.Entry(asset).State = EntityState.Modified;

            // Update room & send notification
            var roomAsset = await _context.RoomAssets
                .FirstOrDefaultAsync(x => x.AssetId == entity.AssetId && x.ToDate == null);

            if (roomAsset != null)
            {
                roomAsset!.Status = AssetStatus.Maintenance;
                roomAsset.EditedAt = now.Value;
                roomAsset.EditorId = creatorId;
                _context.Entry(roomAsset).State = EntityState.Modified;
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = entity.Description,
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
                var newMediaFile = new MediaFile
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = now.Value,
                    CreatorId = creatorId,
                    EditedAt = now.Value,
                    EditorId = creatorId,
                    FileName = mediaFile.FileName,
                    Uri = mediaFile.Uri,
                    FileType = mediaFile.FileType,
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
}