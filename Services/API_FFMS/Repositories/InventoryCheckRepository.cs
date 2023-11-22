using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IInventoryCheckRepository
{
    Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Room?> rooms, List<MediaFile>? mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateInventoryCheckStatus(InventoryCheck inventoryCheck, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    Task<bool> UpdateInventoryCheck(InventoryCheck inventoryCheck, List<MediaFile?> additionMediaFiles, List<MediaFile?> removalMediaFiles, Guid? editorId, DateTime? now = null);
}

public class InventoryCheckRepository : IInventoryCheckRepository
{
    private readonly DatabaseContext _context;

    public InventoryCheckRepository(DatabaseContext context)
    {
        _context = context;
    }

    private List<string> GetCodes()
    {
        var requests = _context.InventoryChecks.Where(x => x.RequestCode.StartsWith("SCH"))
            .Select(x => x.RequestCode)
            .ToList();
        return requests;
    }

    private int GenerateRequestCode(ref List<int> numbers)
    {
        int newRequestNumber = numbers.Any() ? numbers.Max() + 1 : 1;
        numbers.Add(newRequestNumber); // Add the new number to the list
        return newRequestNumber;
    }

    public async Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Room?> rooms, List<MediaFile>? mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.Id = Guid.NewGuid();
            inventoryCheck.CreatedAt = now.Value;
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.CreatorId = creatorId;
            inventoryCheck.Status = RequestStatus.NotStart;
            inventoryCheck.RequestDate = now.Value;
            await _context.InventoryChecks.AddAsync(inventoryCheck);

            foreach (var room in rooms)
            {
                if (room != null)
                {
                    var assets = _context.RoomAssets
                                             .Where(ra => ra.RoomId == room.Id && ra.ToDate == null)
                                             .Select(ra => ra.Asset)
                                             .ToList();

                    foreach (var asset in assets)
                    {
                        if (asset != null)
                        {
                            var roomAssets = _context.RoomAssets
                                                .Where(ra => ra.RoomId == room.Id && ra.AssetId == asset.Id && ra.ToDate == null)
                                                .FirstOrDefault();
                            var inventoryCheckDetail = new InventoryCheckDetail
                            {
                                Id = Guid.NewGuid(),
                                AssetId = asset.Id,
                                InventoryCheckId = inventoryCheck.Id,
                                CreatorId = creatorId,
                                CreatedAt = now.Value,
                                RoomId = room.Id,
                                StatusBefore = asset.Status,
                                QuantityBefore = roomAssets!.Quantity
                            };

                            await _context.InventoryCheckDetails.AddAsync(inventoryCheckDetail);
                        }
                    }
                }
            }

            if (mediaFiles != null)
            {
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
                        ItemId = inventoryCheck.Id
                    };
                    _context.MediaFiles.Add(newMediaFile);
                }
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = inventoryCheck.Description ?? "Yêu cầu đi kiểm kê",
                Title = RequestType.Maintenance.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = inventoryCheck.Id,
                UserId = inventoryCheck.AssignedTo
            };
            await _context.Notifications.AddAsync(notification);

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

    public async Task<bool> UpdateInventoryCheckStatus(InventoryCheck inventoryCheck, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.EditorId = editorId;
            inventoryCheck.Status = statusUpdate;
            _context.Entry(inventoryCheck).State = EntityState.Modified;

            var inventoryCheckDetails = _context.InventoryCheckDetails
                                            .Include(x => x.Asset)
                                            .Where(x => x.InventoryCheckId == inventoryCheck.Id)
                                            .ToList();

            foreach (var detail in inventoryCheckDetails)
            {
                if (statusUpdate == RequestStatus.Done)
                {
                    var roomAsset = _context.RoomAssets
                            .FirstOrDefault(x => x.AssetId == detail.AssetId && x.RoomId == detail.RoomId);
                    var asset = _context.Assets
                            .FirstOrDefault(x => x.Id == detail.AssetId);

                    if (asset != null)
                    {
                        asset.Status = detail.StatusReported;
                        _context.Entry(asset).State = EntityState.Modified;
                    }

                    if (roomAsset != null)
                    {
                        roomAsset.Quantity = detail.QuantityReported;
                        roomAsset.Status = detail.StatusReported;
                        _context.Entry(roomAsset).State = EntityState.Modified;
                    }

                    inventoryCheck.CompletionDate = now.Value;
                    _context.Entry(inventoryCheck).State = EntityState.Modified;
                }
                else if (statusUpdate == RequestStatus.Cancelled)
                {
                    inventoryCheck.Status = RequestStatus.Cancelled;
                    _context.Entry(inventoryCheck).State = EntityState.Modified;
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

    public async Task<bool> UpdateInventoryCheck(InventoryCheck inventoryCheck, List<MediaFile?> additionMediaFiles, List<MediaFile?> removalMediaFiles, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.EditorId = editorId;
            inventoryCheck.EditedAt = now.Value;
            _context.Entry(inventoryCheck).State = EntityState.Modified;

            var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                .Where(x => x.ItemId == inventoryCheck.Id && !x.DeletedAt.HasValue)
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