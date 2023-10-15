using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IRepairationRepository
    {
        Task<bool> InsertRepairation(Repairation repairation, Guid? creatorId, DateTime? now = null);
    }
    public class RepairationRepository : IRepairationRepository
    {
        private readonly DatabaseContext _context;

        public RepairationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertRepairation(Repairation repairation, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                repairation.Id = Guid.NewGuid();
                repairation.CreatedAt = now.Value;
                repairation.EditedAt = now.Value;
                repairation.CreatorId = creatorId;
                repairation.Status = RequestStatus.InProgress;
                await _context.Repairations.AddAsync(repairation);

                var asset = await _context.Assets.FindAsync(repairation.AssetId);
                asset!.Status = AssetStatus.Repair;
                asset.EditedAt = now.Value;
                _context.Entry(asset).State = EntityState.Modified;

                if (repairation.IsInternal)
                {
                    var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == repairation.AssetId && x.ToDate == null);
                    if(roomAsset != null)
                    {
                        roomAsset!.Status = AssetStatus.Repair;
                        roomAsset.EditedAt = now.Value;
                        _context.Entry(roomAsset).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = repairation.Description,
                        Title = RequestType.Repairation.GetDisplayName(),
                        Type = NotificationType.Task,
                        CreatorId = creatorId,
                        IsRead = false,
                        ItemId = repairation.Id,
                        UserId = repairation.AssignedTo
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
    }
}
