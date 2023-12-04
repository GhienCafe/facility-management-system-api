using API_FFMS.Dtos;
using AppCore.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IReplacementRepository
    {
        Task<bool> InsertReplacement(Replacement replacement, List<Report> mediaFiles, Guid? creatorId, DateTime? now = null);
        Task<bool> ConfirmOrReject(Replacement replacement, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null);
        Task<bool> DeleteReplacement(Replacement replacement, Guid? deleterId, DateTime? now = null);
        Task<bool> DeleteMulti(List<Replacement?> replacements, Guid? deleterId, DateTime? now = null);
        Task<bool> UpdateReplacement(Replacement replacement, List<Report?> additionMediaFiles, List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null);
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
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = deleterId;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                if (newAsset != null)
                {
                    newAsset.RequestStatus = RequestType.Operational;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = deleterId;
                    _context.Entry(newAsset).State = EntityState.Modified;
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

        public async Task<bool> DeleteMulti(List<Replacement?> replacements, Guid? deleterId, DateTime? now = null)
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
                        asset.RequestStatus = RequestType.Operational;
                        asset.EditedAt = now.Value;
                        asset.EditorId = deleterId;
                        _context.Entry(asset).State = EntityState.Modified;
                    }

                    if (newAsset != null)
                    {
                        newAsset.RequestStatus = RequestType.Operational;
                        newAsset.EditedAt = now.Value;
                        newAsset.EditorId = deleterId;
                        _context.Entry(newAsset).State = EntityState.Modified;
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

        public async Task<bool> InsertReplacement(Replacement replacement, List<Report> mediaFiles, Guid? creatorId, DateTime? now = null)
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

                //ASSET
                var asset = await _context.Assets.FindAsync(replacement.AssetId);
                if (asset != null)
                {
                    asset.RequestStatus = RequestType.Replacement;
                    _context.Entry(asset).State = EntityState.Modified;
                }

                var newAsset = await _context.Assets.FindAsync(replacement.NewAssetId);
                if(newAsset != null)
                {
                    newAsset.RequestStatus = RequestType.Replacement;
                    _context.Entry(newAsset).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    EditedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = replacement.Description ?? "Yêu cầu thay thế",
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

        public async Task<bool> UpdateReplacement(Replacement replacement, List<Report?> additionMediaFiles, List<Report?> removalMediaFiles, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.EditorId = editorId;
                replacement.EditedAt = now.Value;
                _context.Entry(replacement).State = EntityState.Modified;

                var mediaFiles = _context.MediaFiles.AsNoTracking()
                                                    .Where(x => x.ItemId == replacement.Id && !x.DeletedAt.HasValue)
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

        public async Task<bool> ConfirmOrReject(Replacement replacement, BaseUpdateStatusDto? confirmOrRejectDto, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.EditedAt = now.Value;
                replacement.EditorId = editorId;
                //replacement.Status = statusUpdate;
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

                var reports = await _context.MediaFiles.FirstOrDefaultAsync(x => x.ItemId == replacement.Id && !x.IsReject && x.IsReported);

                if (confirmOrRejectDto?.Status == RequestStatus.Done)
                {
                    replacement.Status = RequestStatus.Done;
                    replacement.CompletionDate = now.Value;
                    _context.Entry(replacement).State = EntityState.Modified;

                    asset!.Status = roomAsset!.Status;
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = roomAssetNew!.Status;
                    newAsset.RequestStatus = RequestType.Operational;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = editorId;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    roomAsset.ToDate = now.Value;
                    roomAsset.EditorId = editorId;
                    roomAsset.EditedAt = now.Value;
                    _context.Entry(roomAsset).State = EntityState.Modified;

                    roomAssetNew.ToDate = now.Value;
                    roomAssetNew.EditorId = editorId;
                    roomAssetNew.EditedAt = now.Value;
                    _context.Entry(roomAssetNew).State = EntityState.Modified;

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

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = "Đã xác nhận",
                        Title = "Đã xác nhận",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
                        IsRead = false,
                        ItemId = replacement.Id,
                        UserId = replacement.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.NeedAdditional)
                {
                    replacement.Status = RequestStatus.InProgress;
                    _context.Entry(replacement).State = EntityState.Modified;

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
                        ItemId = replacement.Id,
                        UserId = replacement.AssignedTo
                    };
                    await _context.Notifications.AddAsync(notification);
                }
                else if (confirmOrRejectDto?.Status == RequestStatus.Cancelled)
                {
                    replacement.Status = RequestStatus.Cancelled;
                    _context.Entry(replacement).State = EntityState.Modified;

                    if (reports != null)
                    {
                        reports.IsReject = true;
                        reports.RejectReason = confirmOrRejectDto.Reason;
                        _context.Entry(reports).State = EntityState.Modified;
                    }

                    asset!.Status = roomAsset!.Status;
                    asset.RequestStatus = RequestType.Operational;
                    asset.EditedAt = now.Value;
                    asset.EditorId = editorId;
                    _context.Entry(asset).State = EntityState.Modified;

                    newAsset!.Status = roomAssetNew!.Status;
                    newAsset.RequestStatus = RequestType.Operational;
                    newAsset.EditedAt = now.Value;
                    newAsset.EditorId = editorId;
                    _context.Entry(newAsset).State = EntityState.Modified;

                    assetLocation!.State = RoomState.Operational;
                    assetLocation.EditedAt = now.Value;
                    assetLocation.EditorId = editorId;
                    _context.Entry(assetLocation).State = EntityState.Modified;

                    newAssetLocation!.State = RoomState.Operational;
                    newAssetLocation.EditedAt = now.Value;
                    newAssetLocation.EditorId = editorId;
                    _context.Entry(newAssetLocation).State = EntityState.Modified;

                    var notification = new Notification
                    {
                        CreatedAt = now.Value,
                        Status = NotificationStatus.Waiting,
                        Content = confirmOrRejectDto.Reason ?? "Hủy yêu cầu",
                        Title = "Hủy yêu cầu",
                        Type = NotificationType.Task,
                        CreatorId = editorId,
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
    }
}