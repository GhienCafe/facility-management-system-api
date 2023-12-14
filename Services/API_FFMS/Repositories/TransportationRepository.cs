using API_FFMS.Dtos;
using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface ITransportationRepository
{
    Task<bool> ConfirmOrReject(Transportation transportation, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null);
    Task<bool> DeleteTransport(Transportation transportation, Guid? deleterId, DateTime? now = null);
    Task<bool> DeleteMulti(List<Transportation?> transportations, Guid? deleterId, DateTime? now = null);
    Task<bool> InsertTransportation(Transportation transportation, List<TransportationDetail> transportationDetails, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateTransportation(Transportation transportation, List<Report?> additionMediaFiles, List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null);
}
public class TransportationRepository : ITransportationRepository
{
    private readonly DatabaseContext _context;

    public TransportationRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteTransport(Transportation transportation, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            transportation.DeletedAt = now.Value;
            transportation.DeleterId = deleterId;
            _context.Entry(transportation).State = EntityState.Modified;

            var assetIds = transportation.TransportationDetails?.Select(td => td.AssetId).ToList();
            var assets = await _context.Assets
                        .Include(a => a.Type)
                        .Where(asset => assetIds!.Contains(asset.Id))
                        .ToListAsync();

            var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);

            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == transportation.Id);
            if (notification != null)
            {
                notification.DeletedAt = now.Value;
                notification.DeleterId = deleterId;
                _context.Entry(notification).State = EntityState.Modified;
            }

            foreach (var asset in assets)
            {
                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id && x.ToDate == null);
                var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset.Id);

                if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                {
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                if (fromRoom != null)
                {
                    fromRoom.State = RoomState.Operational;
                    fromRoom.EditedAt = now.Value;
                    fromRoom.EditorId = deleterId;
                    _context.Entry(fromRoom).State = EntityState.Modified;
                }

                if (toRoom != null)
                {
                    toRoom.State = RoomState.Operational;
                    toRoom.EditedAt = now.Value;
                    toRoom.EditorId = deleterId;
                    _context.Entry(toRoom).State = EntityState.Modified;
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

    public async Task<bool> DeleteMulti(List<Transportation?> transportations, Guid? deleterId, DateTime? now = null)
    {
        now ??= DateTime.UtcNow;
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var transportation in transportations)
            {
                if (transportation == null) continue;

                transportation.DeletedAt = now.Value;
                transportation.DeleterId = deleterId;
                _context.Entry(transportation).State = EntityState.Modified;

                var assetIds = transportation.TransportationDetails?.Select(td => td.AssetId).ToList();
                var assets = await _context.Assets
                    .Include(a => a.Type)
                    .Where(asset => assetIds!.Contains(asset.Id))
                    .ToListAsync();

                var roomAssetQuery = _context.RoomAssets.Where(ra => assetIds!.Contains(ra.AssetId) && ra.ToDate == null);
                var fromRoomIds = await roomAssetQuery.Select(ra => ra.RoomId).ToListAsync();
                var rooms = await _context.Rooms.Where(room => fromRoomIds.Contains(room.Id)).ToListAsync();

                var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.ItemId == transportation.Id);
                if (notification != null)
                {
                    notification.DeletedAt = now.Value;
                    notification.DeleterId = deleterId;
                    _context.Entry(notification).State = EntityState.Modified;
                }

                foreach (var asset in assets)
                {
                    asset.Status = AssetStatus.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                foreach (var room in rooms)
                {
                    room.State = RoomState.Operational;
                    room.EditedAt = now.Value;
                    room.EditorId = deleterId;
                    _context.Entry(room).State = EntityState.Modified;
                }

                await roomAssetQuery.ForEachAsync(ra =>
                {
                    ra.Status = AssetStatus.Operational;
                    ra.EditedAt = now.Value;
                    ra.EditorId = deleterId;
                    _context.Entry(ra).State = EntityState.Modified;
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> InsertTransportation(Transportation transportation, List<TransportationDetail> transportationDetails, List<Report>? mediaFiles, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            transportation.CreatedAt = now.Value;
            transportation.EditedAt = now.Value;
            transportation.CreatorId = creatorId;
            transportation.Status = RequestStatus.NotStart;
            transportation.RequestDate = now.Value;
            await _context.Transportations.AddAsync(transportation);

            var assetIds = transportationDetails.Select(td => td.AssetId).ToList();
            var assets = await _context.Assets
                            .Include(a => a.Type)
                            .Where(asset => assetIds!.Contains(asset.Id))
                            .ToListAsync();

            foreach (var asset in assets)
            {
                if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                {
                    asset.RequestStatus = RequestType.Transportation;
                    _context.Entry(asset).State = EntityState.Modified;
                }
            }

            foreach (var transpsortDetail in transportationDetails)
            {
                if (transpsortDetail != null)
                {
                    _context.TransportationDetails.Add(transpsortDetail);
                }
            }

            if (mediaFiles != null)
            {
                foreach (var mediaFile in mediaFiles)
                {
                    var newMediaFile = new Report
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = now.Value,
                        CreatorId = creatorId,
                        EditedAt = now.Value,
                        EditorId = creatorId,
                        FileName = mediaFile.FileName,
                        Uri = mediaFile.Uri,
                        FileType = FileType.File,
                        ItemId = transportation.Id
                    };
                    _context.MediaFiles.Add(newMediaFile);
                }
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = transportation.Description ?? "Yêu cầu vận chuyển",
                Title = RequestType.Transportation.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = transportation.Id,
                UserId = transportation.AssignedTo
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

    public async Task<bool> ConfirmOrReject(Transportation transportation, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            transportation.EditedAt = now.Value;
            transportation.EditorId = editorId;
            _context.Entry(transportation).State = EntityState.Modified;

            var assetIds = transportation.TransportationDetails?.Select(td => td.AssetId).ToList();
            var assets = await _context.Assets
                            .Include(a => a.Type)
                            .Where(asset => assetIds!.Contains(asset.Id))
                            .ToListAsync();

            var toRoom = await _context.Rooms.FindAsync(transportation.ToRoomId);

            var reports = await _context.MediaFiles.FirstOrDefaultAsync(x => x.ItemId == transportation.Id && !x.IsReject && x.IsReported);



            if (confirmOrRejectDto?.Status == RequestStatus.Done)
            {
                foreach (var asset in assets)
                {
                    if (asset != null)
                    {
                        var transportDetail = await _context.TransportationDetails
                                                        .FirstOrDefaultAsync(x => x.TransportationId == transportation.Id &&
                                                                                  x.AssetId == asset.Id);

                        transportation.CompletionDate = now.Value;
                        transportation.Status = RequestStatus.Done;
                        _context.Entry(transportation).State = EntityState.Modified;

                        if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                        {
                            var fromRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                           x.RoomId == transportDetail!.FromRoomId &&
                                                                                           x.ToDate == null);

                            var toRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                    x.RoomId == toRoom!.Id &&
                                                                                    x.ToDate == null);

                            var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == fromRoomAsset!.RoomId);

                            asset.RequestStatus = RequestType.Operational;
                            _context.Entry(asset).State = EntityState.Modified;

                            if (fromRoomAsset != null)
                            {
                                fromRoomAsset.ToDate = now.Value;
                                _context.Entry(fromRoomAsset).State = EntityState.Modified;
                            }

                            fromRoom!.State = RoomState.Operational;
                            _context.Entry(fromRoom).State = EntityState.Modified;

                            toRoom!.State = RoomState.Operational;
                            _context.Entry(toRoom).State = EntityState.Modified;

                            var addRoomAsset = new RoomAsset
                            {
                                Id = Guid.NewGuid(),
                                AssetId = asset.Id,
                                RoomId = toRoom.Id,
                                Status = asset.Status,
                                FromDate = now.Value,
                                Quantity = 1,
                                ToDate = null,
                                CreatorId = editorId,
                                CreatedAt = now.Value,
                            };
                            await _context.RoomAssets.AddAsync(addRoomAsset);
                        }
                        else if (asset.Type!.Unit == Unit.Quantity || asset.Type.IsIdentified == false)
                        {
                            var fromRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                           x.RoomId == transportDetail!.FromRoomId &&
                                                                                           x.ToDate == null);

                            var toRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                    x.RoomId == toRoom!.Id &&
                                                                                    x.ToDate == null);

                            if (fromRoomAsset != null)
                            {
                                fromRoomAsset.Quantity -= transportDetail!.Quantity;
                                if (fromRoomAsset.Quantity <= 0)
                                {
                                    fromRoomAsset.Quantity = 0;
                                    fromRoomAsset.ToDate = now.Value;
                                    //_context.Entry(fromRoomAsset).State = EntityState.Modified;
                                }
                                _context.Entry(fromRoomAsset).State = EntityState.Modified;
                            }

                            if (toRoomAsset != null)
                            {
                                toRoomAsset.Quantity += transportDetail!.Quantity;
                                _context.Entry(toRoomAsset).State = EntityState.Modified;
                            }
                            else
                            {
                                var addRoomAsset = new RoomAsset
                                {
                                    AssetId = asset.Id,
                                    RoomId = toRoom!.Id,
                                    Status = asset.Status,
                                    FromDate = now.Value,
                                    Quantity = transportDetail!.Quantity,
                                    ToDate = null,
                                    CreatorId = editorId,
                                    CreatedAt = now.Value,
                                };
                                await _context.RoomAssets.AddAsync(addRoomAsset);
                            }
                        }
                    }
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = "Đã xác nhận",
                    Title = "Đã xác nhận",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
                    IsRead = false,
                    ItemId = transportation.Id,
                    UserId = transportation.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);
            }
            else if (confirmOrRejectDto?.Status == RequestStatus.NeedAdditional)
            {
                transportation.Status = RequestStatus.InProgress;
                _context.Entry(transportation).State = EntityState.Modified;

                if (reports != null)
                {
                    reports.IsReject = true;
                    reports.RejectReason = confirmOrRejectDto.Reason;
                    _context.Entry(reports).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = confirmOrRejectDto.Reason ?? "Cần bổ sung",
                    Title = "Báo cáo lại",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
                    IsRead = false,
                    ItemId = transportation.Id,
                    UserId = transportation.AssignedTo
                };
                await _context.Notifications.AddAsync(notification);
            }
            else if (confirmOrRejectDto?.Status == RequestStatus.Cancelled)
            {
                transportation.Status = RequestStatus.Cancelled;
                _context.Entry(transportation).State = EntityState.Modified;

                if (reports != null)
                {
                    reports.IsReject = true;
                    reports.RejectReason = confirmOrRejectDto.Reason;
                    _context.Entry(reports).State = EntityState.Modified;
                }
                foreach (var asset in assets)
                {
                    if (asset != null)
                    {
                        var transportDetails = await _context.TransportationDetails
                                                        .FirstOrDefaultAsync(x => x.TransportationId == transportation.Id &&
                                                                                  x.AssetId == asset.Id);
                        var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                           x.RoomId == transportDetails!.FromRoomId &&
                                                                                           x.ToDate == null);


                        var fromRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId);

                        var toRoomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id &&
                                                                                    x.RoomId == toRoom!.Id &&
                                                                                    x.ToDate == null);
                        if (asset.Type!.Unit == Unit.Individual || asset.Type.IsIdentified == true)
                        {
                            asset.RequestStatus = RequestType.Operational;
                            _context.Entry(asset).State = EntityState.Modified;

                            fromRoom!.State = RoomState.Operational;
                            _context.Entry(fromRoom).State = EntityState.Modified;

                            toRoom!.State = RoomState.Operational;
                            _context.Entry(toRoom).State = EntityState.Modified;
                        }
                    }
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = confirmOrRejectDto.Reason ?? "Hủy yêu cầu",
                    Title = "Hủy yêu cầu",
                    Type = NotificationType.Task,
                    CreatorId = editorId,
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

    public async Task<bool> UpdateTransportation(Transportation transportation, List<Report?> additionMediaFiles,
                                                 List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            transportation.EditorId = editorId;
            transportation.EditedAt = now.Value;
            _context.Entry(transportation).State = EntityState.Modified;

            var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                .Where(x => x.ItemId == transportation.Id && !x.DeletedAt.HasValue)
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