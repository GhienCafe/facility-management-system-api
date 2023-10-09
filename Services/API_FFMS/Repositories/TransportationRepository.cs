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
        Task<bool> UpdateStatus(Transportation transportation, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    }
    public class TransportationRepository : ITransportationRepository
    {
        const string IT_ASSET = "IT"; // IT Device
        const string ELEC_ASSET = "ED"; //Electronice Device
        const string FUR_ASSET = "FD"; //Furniture Device
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

                var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);
                //toRoom!.Status =                                                

                if (transportation.IsInternal)
                {
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

        public async Task<bool> UpdateStatus(Transportation transportation, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                transportation.EditedAt = now.Value;
                transportation.EditorId = editorId;
                transportation.Status = statusUpdate;
                _context.Entry(transportation).State = EntityState.Modified;

                var asset = await _context.Assets.FindAsync(transportation.AssetId);

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);

                if (statusUpdate == RequestStatus.Completed)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.ToDate = now.Value;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    var addRoomAsset = new RoomAsset
                    {
                        AssetId = transportation.AssetId,
                        RoomId = toRoom!.Id,
                        Status = AssetStatus.Operational,
                        FromDate = now.Value,
                        Quantity = 1,
                        ToDate = null,
                    };
                    _context.RoomAssets.Add(addRoomAsset);
                }
                else if (statusUpdate == RequestStatus.Cancelled || statusUpdate == RequestStatus.CantDo)
                {
                    asset!.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    _context.Entry(asset).State = EntityState.Modified;

                    roomAsset!.Status = AssetStatus.Operational;
                    roomAsset.EditedAt = now.Value;
                    _context.Entry(roomAsset).State = EntityState.Modified;
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
