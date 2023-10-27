﻿using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface ITransportationRepository
    {
        Task<bool> InsertTransportations(Transportation transportation, List<Asset?> assets, Guid? creatorId, DateTime? now = null);
        Task<bool> UpdateStatus(Transportation transportation, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    }
    public class TransportationRepository : ITransportationRepository
    {
        private readonly DatabaseContext _context;

        public TransportationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertTransportations(Transportation transportation, List<Asset?> assets, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                transportation.Id = Guid.NewGuid();
                transportation.CreatedAt = now.Value;
                transportation.EditedAt = now.Value;
                transportation.CreatorId = creatorId;
                transportation.Status = RequestStatus.NotStart;
                transportation.RequestDate = now.Value;
                await _context.Transportations.AddAsync(transportation);

                if (transportation.IsInternal)
                {
                    foreach (var asset in assets)
                    {
                        var transpsortDetail = new TransportationDetail
                        {
                            Id = Guid.NewGuid(),
                            AssetId = asset!.Id,
                            TransportationId = transportation.Id,
                            RequestDate = now.Value,
                            Quantity = (int?)asset.Quantity,
                            CreatorId = creatorId,
                            CreatedAt = now.Value,
                            EditedAt = now.Value
                        };
                        await _context.TransportationDetails.AddAsync(transpsortDetail);
                    }

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        EditedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = transportation.Description,
                        Title = RequestType.Maintenance.GetDisplayName(),
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

                var assetIds = transportation.TransportationDetails?.Select(td => td.AssetId).ToList();
                var assets = await _context.Assets
                            .Include(a => a.Type)
                            .Where(asset => assetIds!.Contains(asset.Id))
                            .ToListAsync();

                var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);
                foreach (var asset in assets)
                {
                    var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id && x.ToDate == null);
                    var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset.Id);
                    if (statusUpdate == RequestStatus.Done)
                    {
                        asset.Status = AssetStatus.Operational;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        if (roomAsset != null)
                        {
                            roomAsset.Status = AssetStatus.Operational;
                            roomAsset.EditedAt = now.Value;
                            roomAsset.ToDate = now.Value;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }

                        //var totalAssetInFromRoom = fromRoom!.RoomAssets!.Sum(a => a.Quantity);
                        fromRoom!.State = RoomState.Operational;
                        fromRoom.EditedAt = now.Value;
                        _context.Entry(fromRoom).State = EntityState.Modified;


                        //var totalAssetInToRoom = toRoom!.RoomAssets!.Sum(a => a.Quantity);
                        toRoom!.State = RoomState.Operational;
                        toRoom.EditedAt = now.Value;
                        _context.Entry(toRoom).State = EntityState.Modified;

                        //if (asset.Type!.IsIdentified == true)
                        //{
                        //    var addRoomAsset = new RoomAsset
                        //    {
                        //        AssetId = asset.Id,
                        //        RoomId = toRoom?.Id ?? Guid.Empty,
                        //        Status = AssetStatus.Operational,
                        //        FromDate = now.Value,
                        //        Quantity = 1,
                        //        ToDate = null,
                        //    };
                        //    _context.RoomAssets.Add(addRoomAsset);
                        //}
                        //else
                        //{
                        //    var addRoomAsset = new RoomAsset
                        //    {
                        //        AssetId = asset.Id,
                        //        RoomId = toRoom?.Id ?? Guid.Empty,
                        //        Status = AssetStatus.Operational,
                        //        FromDate = now.Value,
                        //        Quantity = asset.Quantity,
                        //        ToDate = null,
                        //    };
                        //    _context.RoomAssets.Add(addRoomAsset);
                        //}
                    }
                    else if (statusUpdate == RequestStatus.Cancelled)
                    {
                        asset.Status = AssetStatus.Operational;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        fromRoom!.State = RoomState.Operational;
                        fromRoom.EditedAt = now.Value;
                        _context.Entry(fromRoom).State = EntityState.Modified;

                        toRoom!.State = RoomState.Operational;
                        toRoom.EditedAt = now.Value;
                        _context.Entry(toRoom).State = EntityState.Modified;

                        if (roomAsset != null)
                        {
                            roomAsset.Status = AssetStatus.Operational;
                            roomAsset.EditedAt = now.Value;
                            _context.Entry(roomAsset).State = EntityState.Modified;
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
}
