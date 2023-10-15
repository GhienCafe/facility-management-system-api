using AppCore.Extensions;
using MainData;
using MainData.Entities;

namespace API_FFMS.Repositories
{
    public interface IAssetcheckRepository
    {
        Task<bool> InsertAssetCheck(AssetCheck repairation, Guid? creatorId, DateTime? now = null);
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
                assetCheck.Status = RequestStatus.InProgress;
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
    }
}
