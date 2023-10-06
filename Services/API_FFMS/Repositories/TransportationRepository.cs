using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface ITransportationRepository
    {
        //Task<bool> InsertTransportation(Transportation transportation, List<Guid> assetIds, Guid? creatorId, DateTime? now = null);
        Task<bool> InsertTransportation(Transportation transportation, Guid? creatorId, DateTime? now = null);
    }
    public class TransportationRepository : ITransportationRepository
    {
        private readonly DatabaseContext _context;

        public TransportationRepository(DatabaseContext context)
        {
            _context = context;
        }

        //public async Task<bool> InsertTransportation(Transportation transportation, List<Guid> assetIds, Guid? creatorId, DateTime? now = null)
        //{
        //    await _context.Database.BeginTransactionAsync();
        //    now ??= DateTime.UtcNow;
        //    try
        //    {
        //        foreach(var assetId in assetIds)
        //        {
        //            transportation.Id = Guid.NewGuid();
        //            transportation.AssetId = assetId;
        //            transportation.CreatedAt = now.Value;
        //            transportation.EditedAt = now.Value;
        //            transportation.CreatorId = creatorId;
        //            transportation.Status = RequestStatus.NotStarted;
        //            await _context.Transportations.AddAsync(transportation);

        //            var asset = await _context.Assets.FindAsync(transportation.AssetId);
        //            asset!.Status = AssetStatus.Transportation;
        //            asset.EditedAt = now.Value;
        //            _context.Entry(asset).State = EntityState.Modified;
        //        }

        //        if (transportation.IsInternal)
        //        {
        //            var destinationRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);
        //            var roomAsset = await _context.RoomAssets
        //            .FirstOrDefaultAsync(x => x.AssetId == transportation.AssetId && x.ToDate == null);
        //            if (roomAsset != null)
        //            {
        //                roomAsset!.Status = AssetStatus.Repair;
        //                roomAsset.EditedAt = now.Value;
        //                _context.Entry(roomAsset).State = EntityState.Modified;
        //            }

        //            var notification = new Notification
        //            {
        //                CreatedAt = now.Value,
        //                EditedAt = now.Value,
        //                Status = NotificationStatus.Waiting,
        //                Content = transportation.Description,
        //                Title = RequestType.Repairation.GetDisplayName(),
        //                Type = NotificationType.Task,
        //                CreatorId = creatorId,
        //                IsRead = false,
        //                ItemId = transportation.Id,
        //                UserId = transportation.AssignedTo
        //            };
        //            await _context.Notifications.AddAsync(notification);
        //        }

        //        await _context.SaveChangesAsync();
        //        await _context.Database.CommitTransactionAsync();
        //        return true;
        //    }
        //    catch
        //    {
        //        await _context.Database.RollbackTransactionAsync();
        //        return false;
        //    }
        //}
        public async Task<bool> InsertTransportation(Transportation transportation, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                    transportation.Id = Guid.NewGuid();
                    transportation.CreatedAt = now.Value;
                    transportation.EditedAt = now.Value;
                    transportation.CreatorId = creatorId;
                    transportation.Status = RequestStatus.NotStarted;
                    await _context.Transportations.AddAsync(transportation);

                    var asset = await _context.Assets.FindAsync(transportation.AssetId);
                    asset!.Status = AssetStatus.Transportation;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                if (transportation.IsInternal)
                {
                    //var destinationRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);
                    var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == transportation.AssetId && x.ToDate == null);
                    if (roomAsset != null)
                    {
                        roomAsset!.Status = AssetStatus.Transportation;
                        roomAsset.EditedAt = now.Value;
                        _context.Entry(roomAsset).State = EntityState.Modified;
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = transportation.Description,
                        Title = RequestType.Repairation.GetDisplayName(),
                        Type = NotificationType.Task,
                        CreatorId = creatorId,
                        IsRead = false,
                        ItemId = transportation.Id,
                        UserId = transportation.AssignedTo
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
