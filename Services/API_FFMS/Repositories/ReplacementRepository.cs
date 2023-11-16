using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IReplacementRepository
    {
        Task<bool> InsertReplacement(Replacement replacement, Guid? creatorId, DateTime? now = null);
        Task<bool> InsertReplacementV2(Replacement replacement, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null);
        Task<bool> UpdateStatus(Replacement replacement, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
        Task<bool> DeleteReplacement(Replacement replacement, Guid? deleterId, DateTime? now = null);
        Task<bool> DeleteReplacements(List<Replacement?> replacements, Guid? deleterId, DateTime? now = null);
    }
    public class ReplacementRepository : IReplacementRepository
    {
        private readonly DatabaseContext _context;

        public ReplacementRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteReplacement(Replacement replacement, Guid? deleterId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.DeletedAt = now.Value;
                replacement.DeleterId = deleterId;
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
                                .FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                var newAssetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == roomAssetNew!.RoomId && roomAssetNew.AssetId == newAsset!.Id);

                var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == replacement.Id);
                if (notification != null)
                {
                    notification.DeletedAt = now.Value;
                    notification.DeleterId = deleterId;
                    _context.Entry(notification).State = EntityState.Modified;
                }

                if (asset != null)
                {
                    asset.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                if (newAsset != null)
                {
                    newAsset.Status = AssetStatus.Operational;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = deleterId;
                    _context.Entry(newAsset).State = EntityState.Modified;
                }

                if (roomAsset != null)
                {
                    roomAsset.Status = AssetStatus.Operational;
                    roomAsset.EditorId = deleterId;
                    roomAsset.EditedAt = now.Value;
                    _context.Entry(roomAsset).State = EntityState.Modified;
                }

                if (roomAssetNew != null)
                {
                    roomAssetNew.Status = AssetStatus.Operational;
                    roomAssetNew.EditorId = deleterId;
                    roomAssetNew.EditedAt = now.Value;
                    _context.Entry(roomAssetNew).State = EntityState.Modified;
                }

                if (assetLocation != null)
                {
                    assetLocation.State = RoomState.Operational;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = deleterId;
                    _context.Entry(assetLocation).State = EntityState.Modified;
                }

                if (newAssetLocation != null)
                {
                    newAssetLocation.State = RoomState.Operational;
                    newAssetLocation.EditedAt = now.Value;
                    newAssetLocation.EditorId = deleterId;
                    _context.Entry(newAssetLocation).State = EntityState.Modified;
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

        public async Task<bool> DeleteReplacements(List<Replacement?> replacements, Guid? deleterId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                foreach (var replacement in replacements)
                {
                    replacement!.DeletedAt = now.Value;
                    replacement.DeleterId = deleterId;
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
                                    .FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                    var newAssetLocation = await _context.Rooms
                                    .FirstOrDefaultAsync(x => x.Id == roomAssetNew!.RoomId && roomAssetNew.AssetId == newAsset!.Id);

                    var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == replacement.Id);
                    if (notification != null)
                    {
                        notification.DeletedAt = now.Value;
                        notification.DeleterId = deleterId;
                        _context.Entry(notification).State = EntityState.Modified;
                    }

                    if (asset != null)
                    {
                        asset.Status = AssetStatus.Operational;
                        asset.EditedAt = now.Value;
                        asset.EditorId = deleterId;
                        _context.Entry(asset).State = EntityState.Modified;
                    }

                    if (newAsset != null)
                    {
                        newAsset.Status = AssetStatus.Operational;
                        newAsset.EditedAt = now.Value;
                        newAsset.EditorId = deleterId;
                        _context.Entry(newAsset).State = EntityState.Modified;
                    }

                    if (roomAsset != null)
                    {
                        roomAsset.Status = AssetStatus.Operational;
                        roomAsset.EditorId = deleterId;
                        roomAsset.EditedAt = now.Value;
                        _context.Entry(roomAsset).State = EntityState.Modified;
                    }

                    if (roomAssetNew != null)
                    {
                        roomAssetNew.Status = AssetStatus.Operational;
                        roomAssetNew.EditorId = deleterId;
                        roomAssetNew.EditedAt = now.Value;
                        _context.Entry(roomAssetNew).State = EntityState.Modified;
                    }

                    if (assetLocation != null)
                    {
                        assetLocation.State = RoomState.Operational;
                        assetLocation.EditedAt = now.Value;
                        assetLocation.EditorId = deleterId;
                        _context.Entry(assetLocation).State = EntityState.Modified;
                    }

                    if (newAssetLocation != null)
                    {
                        newAssetLocation.State = RoomState.Operational;
                        newAssetLocation.EditedAt = now.Value;
                        newAssetLocation.EditorId = deleterId;
                        _context.Entry(newAssetLocation).State = EntityState.Modified;
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

        public async Task<bool> InsertReplacement(Replacement replacement, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.Id = Guid.NewGuid();
                replacement.CreatedAt = now.Value;
                replacement.EditedAt = now.Value;
                replacement.CreatorId = creatorId;
                replacement.Status = RequestStatus.NotStart;
                replacement.RequestDate = now.Value;
                await _context.Replacements.AddAsync(replacement);

                if (replacement.IsInternal)
                {
                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = replacement.Description,
                        Title = RequestType.Replacement.GetDisplayName(),
                        Type = NotificationType.Task,
                        CreatorId = creatorId,
                        IsRead = false,
                        ItemId = replacement.Id,
                        UserId = replacement.AssignedTo
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

        public async Task<bool> InsertReplacementV2(Replacement replacement, List<MediaFile> mediaFiles, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.Id = Guid.NewGuid();
                replacement.CreatedAt = now.Value;
                replacement.EditedAt = now.Value;
                replacement.CreatorId = creatorId;
                replacement.Status = RequestStatus.NotStart;
                replacement.RequestDate = now.Value;
                await _context.Replacements.AddAsync(replacement);

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    EditedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = replacement.Description,
                    Title = RequestType.Replacement.GetDisplayName(),
                    Type = NotificationType.Task,
                    CreatorId = creatorId,
                    IsRead = false,
                    ItemId = replacement.Id,
                    UserId = replacement.AssignedTo
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
                        ItemId = replacement.Id
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

        public async Task<bool> UpdateStatus(Replacement replacement, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
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
                                .FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);

                var newAssetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == roomAssetNew!.RoomId && roomAssetNew.AssetId == newAsset!.Id);
                if (replacement.Status == RequestStatus.Done)
                {
                    replacement.CompletionDate = now.Value;
                    _context.Entry(replacement).State = EntityState.Modified;

                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = AssetStatus.Operational;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = editorId;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    //roomAsset!.Status = AssetStatus.Operational;
                    //roomAsset.ToDate = now.Value;
                    //roomAsset.EditorId = editorId;
                    //roomAsset.EditedAt = now.Value;
                    //_context.Entry(roomAsset).State = EntityState.Modified;

                    //roomAssetNew!.Status = AssetStatus.Operational;
                    //roomAssetNew.ToDate = now.Value;
                    //roomAssetNew.EditorId = editorId;
                    //roomAssetNew.EditedAt = now.Value;
                    //_context.Entry(roomAssetNew).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = editorId;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    newAssetLocation!.State = RoomState.Operational;
                    newAssetLocation.EditedAt = now.Value;
                    newAssetLocation.EditorId = editorId;
                    _context.Entry(newAssetLocation).State = EntityState.Modified;

                    var addRoomAssetNew = new RoomAsset
                    {
                        Id = Guid.NewGuid(),
                        AssetId = replacement.NewAssetId,
                        RoomId = assetLocation!.Id,
                        Status = roomAsset!.Status,
                        FromDate = now.Value,
                        Quantity = roomAsset.Quantity,
                        ToDate = null,
                        CreatorId = editorId,
                        CreatedAt = now.Value,
                    };
                    _context.RoomAssets.Add(addRoomAssetNew);

                    var addRoomAsset = new RoomAsset
                    {
                        Id = Guid.NewGuid(),
                        AssetId = replacement.AssetId,
                        RoomId = newAssetLocation!.Id,
                        Status = roomAssetNew!.Status,
                        FromDate = now.Value,
                        Quantity = roomAssetNew.Quantity,
                        ToDate = null,
                        CreatorId = editorId,
                        CreatedAt = now.Value,
                    };
                    _context.RoomAssets.Add(addRoomAsset);
                }
                else if (replacement.Status == RequestStatus.Cancelled)
                {
                    asset!.Status = roomAsset!.Status;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = roomAssetNew!.Status;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = editorId;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    //roomAsset!.Status = AssetStatus.Operational;
                    //roomAsset.EditorId = editorId;
                    //roomAsset.EditedAt = now.Value;
                    //_context.Entry(roomAsset).State = EntityState.Modified;

                    //roomAssetNew!.Status = AssetStatus.Operational;
                    //roomAssetNew.EditorId = editorId;
                    //roomAssetNew.EditedAt = now.Value;
                    //_context.Entry(roomAssetNew).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = editorId;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    newAssetLocation!.State = RoomState.Operational;
                    newAssetLocation.EditedAt = now.Value;
                    newAssetLocation.EditorId = editorId;
                    _context.Entry(newAssetLocation).State = EntityState.Modified;
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
}