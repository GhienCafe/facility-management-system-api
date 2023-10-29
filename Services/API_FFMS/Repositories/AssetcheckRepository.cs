using AppCore.Extensions;
using DocumentFormat.OpenXml.Vml.Office;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IAssetcheckRepository
    {
        Task<bool> InsertAssetCheck(AssetCheck assetCheck, Guid? creatorId, DateTime? now = null);
        Task<bool> InsertAssetChecks(List<AssetCheck> assetChecks, Guid? creatorId, DateTime? now = null);
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
                assetCheck.IsVerified = false;
                assetCheck.RequestDate = now.Value;
                await _context.AssetChecks.AddAsync(assetCheck);

                var asset = await _context.Assets.FindAsync(assetCheck.AssetId);
                asset!.Status = AssetStatus.NeedInspection;
                asset.EditedAt = now.Value;
                asset.EditorId = creatorId;
                _context.Entry(asset).State = EntityState.Modified;

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

        public async Task<bool> InsertAssetChecks(List<AssetCheck> entities, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            var requestCodes = GetRequestCodes();
            List<int> numbers = requestCodes.Where(x => x.StartsWith("AC"))
                                            .Select(x => int.TryParse(x[2..], out var lastNumber) ? lastNumber : 0)
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
                    entity.RequestCode = "AC" + GenerateRequestCode(ref numbers);
                    await _context.AssetChecks.AddAsync(entity);

                    var asset = await _context.Assets.FindAsync(entity.AssetId);
                    asset!.Status = AssetStatus.NeedInspection;
                    asset.EditedAt = now.Value;
                    asset.EditorId = creatorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    if (entity.IsInternal)
                    {
                        var roomAsset = await _context.RoomAssets
                        .FirstOrDefaultAsync(x => x.AssetId == entity.AssetId && x.ToDate == null);

                        if (roomAsset != null)
                        {
                            roomAsset!.Status = AssetStatus.NeedInspection;
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
                            Title = RequestType.Repairation.GetDisplayName(),
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
            var requests = _context.AssetChecks.Where(x => x.RequestCode.StartsWith("AC"))
                                                .Select(x => x.RequestCode)
                                                .ToList();
            return requests;
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
                    assetCheck.CompletionDate = now.Value;
                    _context.Entry(assetCheck).State = EntityState.Modified;

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
