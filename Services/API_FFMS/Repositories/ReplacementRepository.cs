using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IReplacementRepository
    {
        Task<bool> InsertReplacement(Replacement replacement, Guid? creatorId, DateTime? now = null);
        Task<bool> UpdateStatus(Replacement replacement, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    }
    public class ReplacementRepository : IReplacementRepository
    {
        private readonly DatabaseContext _context;

        public ReplacementRepository(DatabaseContext context)
        {
            _context = context;
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
                await _context.Replacements.AddAsync(replacement);

                //var asset = await _context.Assets.FindAsync(replacement.AssetId);
                //asset!.Status = AssetStatus.Replacement;
                //asset.EditedAt = now.Value;
                //_context.Entry(asset).State = EntityState.Modified;

                //var newAsset = await _context.Assets.FindAsync(replacement.NewAssetId);
                //newAsset!.Status = AssetStatus.Replacement;
                //newAsset.EditedAt = now.Value;
                //_context.Entry(newAsset).State = EntityState.Modified;

                if (replacement.IsInternal)
                {
                    //var roomAsset = await _context.RoomAssets
                    //                .FirstOrDefaultAsync(x => x.AssetId == replacement.AssetId && x.ToDate == null);
                    //var roomAssetNew = await _context.RoomAssets
                    //                .FirstOrDefaultAsync(x => x.AssetId == replacement.NewAssetId && x.ToDate == null);
                    //if (roomAsset != null)
                    //{
                    //    roomAsset.Status = AssetStatus.Replacement;
                    //    roomAsset.EditedAt = now.Value;
                    //    _context.Entry(roomAsset).State = EntityState.Modified;
                    //}

                    //if (roomAssetNew != null)
                    //{
                    //    roomAssetNew.Status = AssetStatus.Replacement;
                    //    roomAssetNew.EditedAt = now.Value;
                    //    _context.Entry(roomAssetNew).State = EntityState.Modified;
                    //}
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
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = AssetStatus.Operational;
                    newAsset.EditedAt = now.Value;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    var addRoomAssetNew = new RoomAsset
                    {
                        AssetId = replacement.NewAssetId,
                        RoomId = assetLocation!.Id,
                        Status = AssetStatus.Operational,
                        FromDate = now.Value,
                        Quantity = asset.Quantity,
                        ToDate = null,
                    };
                    _context.RoomAssets.Add(addRoomAssetNew);

                    var addRoomAsset = new RoomAsset
                    {
                        AssetId = replacement.AssetId,
                        RoomId = newAssetLocation!.Id,
                        Status = AssetStatus.Operational,
                        FromDate = now.Value,
                        Quantity = newAsset.Quantity,
                        ToDate = null,
                    };
                    _context.RoomAssets.Add(addRoomAsset);
                }
                else if (replacement.Status == RequestStatus.Cancelled)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = AssetStatus.Operational;
                    newAsset.EditedAt = now.Value;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    newAssetLocation!.State = RoomState.Operational;
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
