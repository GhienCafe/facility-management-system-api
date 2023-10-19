using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IAssetcheckRepository
    {
        Task<bool> InsertAssetCheck(AssetCheck assetCheck, Guid? creatorId, DateTime? now = null);
        Task<bool> UpdateStatus(AssetCheck assetCheck, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    }
    public class AssetcheckRepository : IAssetcheckRepository
    {
        private readonly DatabaseContext _context;
        public AssetcheckRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertAssetCheck(AssetCheck assetCheck, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                assetCheck.Id = Guid.NewGuid();
                assetCheck.CreatorId = creatorId;
                assetCheck.CreatedAt = now.Value;
                assetCheck.EditedAt = now.Value;
                assetCheck.Status = RequestStatus.NotStart;
                await _context.AssetChecks.AddAsync(assetCheck);

                if (assetCheck.IsInternal)
                {
                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = assetCheck.Description,
                        Title = RequestType.Repairation.GetDisplayName(),
                        Type = NotificationType.Task,
                        CreatorId = creatorId,
                        IsRead = false,
                        ItemId = assetCheck.Id,
                        UserId = assetCheck.AssignedTo
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

        public async Task<bool> UpdateStatus(AssetCheck assetCheck, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                assetCheck.EditedAt = now.Value;
                assetCheck.EditorId = editorId;
                assetCheck.Status = statusUpdate;
                _context.Entry(assetCheck).State = EntityState.Modified;

                var asset = await _context.Assets.FindAsync(assetCheck.AssetId);
                var roomAsset = await _context.RoomAssets
                                    .FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var assetLocation = await _context.Rooms
                                .FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                if (assetCheck.Status == RequestStatus.Done)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    _context.Entry(assetLocation).State = EntityState.Modified;
                }
                else if (assetCheck.Status == RequestStatus.Cancelled)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    _context.Entry(assetLocation).State = EntityState.Modified;
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
