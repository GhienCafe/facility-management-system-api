using AppCore.Extensions;
using DocumentFormat.OpenXml.Vml.Office;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IRepairRepository
{
    Task<bool> InsertRepair(Repair repair, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> InsertRepairs(List<Repair> repairs, List<MediaFile>? mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateStatus(Repair repair, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    Task<bool> DeleteRepair(Repair repair, Guid? deleterId, DateTime? now = null);
    Task<bool> DeleteRepairs(List<Repair?> repairs, Guid? deleterId, DateTime? now = null);
    Task<bool> UpdateRepair(Repair repair, List<MediaFile?> additionMediaFiles, List<MediaFile?> removalMediaFiles, Guid? creatorId, DateTime? now = null);
}
public class RepairRepository : IRepairRepository
{
    private readonly DatabaseContext _context;

    public RepairRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertRepairs(List<Repair> entities, List<MediaFile>? mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        var requestCodes = GetRequestCodes();
        List<int> numbers = requestCodes.Where(x => x.StartsWith("REP"))
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
                entity.RequestCode = "REP" + GenerateRequestCode(ref numbers);
                await _context.Repairs.AddAsync(entity);

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = entity.Description,
                    Title = RequestType.Repairation.GetDisplayName(),
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
        var requests = _context.Repairs.Where(x => x.RequestCode.StartsWith("REP"))
                                            .Select(x => x.RequestCode)
                                            .ToList();
        return requests;
    }

    public async Task<bool> UpdateStatus(Repair repair, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            repair.EditedAt = now.Value;
            repair.EditorId = editorId;
            repair.Status = statusUpdate;
            _context.Entry(repair).State = EntityState.Modified;

            var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == repair.AssetId)
                        .FirstOrDefaultAsync();

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
            if (repair.IsInternal == true)
            {
                if (statusUpdate == RequestStatus.Done)
                {
                    repair.CompletionDate = now.Value;
                    _context.Entry(repair).State = EntityState.Modified;

                    asset!.Status = AssetStatus.Operational;
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
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
                }
                else if (statusUpdate == RequestStatus.Cancelled)
                {
                    asset!.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    location!.State = RoomState.Operational;
                    location.EditedAt = now.Value;
                    location.EditorId = editorId;
                    _context.Entry(location).State = EntityState.Modified;
                }
            }
            else if (repair.IsInternal == false)
            {
                if (statusUpdate == RequestStatus.Done)
                {
                    repair.CompletionDate = now.Value;
                    _context.Entry(repair).State = EntityState.Modified;

                    asset!.Status = AssetStatus.Operational;
                    asset.RequestStatus = RequestType.Operational;
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
                }
                else if (statusUpdate == RequestStatus.Cancelled)
                {
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

    public async Task<bool> DeleteRepair(Repair repair, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            repair.DeletedAt = now.Value;
            repair.DeleterId = deleterId;
            _context.Entry(repair).State = EntityState.Modified;

            var asset = await _context.Assets
                        .Include(a => a.Type)
                        .Where(a => a.Id == repair.AssetId)
                        .FirstOrDefaultAsync();

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == repair.Id);
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
                location.State = RoomState.Operational;
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

    public async Task<bool> DeleteRepairs(List<Repair?> repairs, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            foreach (var repair in repairs)
            {
                if (repair != null)
                {
                    repair.DeletedAt = now.Value;
                    repair.DeleterId = deleterId;
                    _context.Entry(repair).State = EntityState.Modified;

                    var asset = await _context.Assets
                                .Include(a => a.Type)
                                .Where(a => a.Id == repair.AssetId)
                                .FirstOrDefaultAsync();

                    var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                    var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                    var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == repair.Id);
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
                        location.State = RoomState.Operational;
                        location.EditedAt = now.Value;
                        location.EditorId = deleterId;
                        _context.Entry(location).State = EntityState.Modified;
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

    public Room? GetWareHouse(string roomName)
    {
        var wareHouse = _context.Rooms.FirstOrDefault(x => x.RoomName!.Trim().Equals(roomName.Trim()));
        return wareHouse;
    }

    public async Task<bool> InsertRepair(Repair repair, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            repair.Id = Guid.NewGuid();
            repair.CreatedAt = now.Value;
            repair.EditedAt = now.Value;
            repair.CreatorId = creatorId;
            repair.Status = RequestStatus.NotStart;
            repair.RequestDate = now.Value;
            await _context.Repairs.AddAsync(repair);

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = repair.Description,
                Title = RequestType.Repairation.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = repair.Id,
                UserId = repair.AssignedTo
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
                    FileType = FileType.File,
                    ItemId = repair.Id
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

    public async Task<bool> UpdateRepair(Repair repair, List<MediaFile?> additionMediaFiles, List<MediaFile?> removalMediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            repair.EditorId = creatorId;
            repair.EditedAt = now.Value;
            _context.Entry(repair).State = EntityState.Modified;

            var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                .Where(x => x.ItemId == repair.Id && !x.DeletedAt.HasValue)
                                                .ToList();

            if(additionMediaFiles.Count > 0 ) 
            {
                foreach(var mediaFile in additionMediaFiles)
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
                        FileType = FileType.File,
                        ItemId = repair.Id
                    };
                    _context.MediaFiles.Add(newMediaFile);
                }
            }

            if (removalMediaFiles.Count > 0)
            {
                foreach (var mediaFile in removalMediaFiles)
                {
                    mediaFile!.DeletedAt = now.Value;
                    _context.Entry(mediaFile).State = EntityState.Modified;
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