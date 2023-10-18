using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface ITaskRepository
    {
        Task<bool> UpdateStatus(List<MediaFile> mediaFiles, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
    }
    public class TaskRepository : ITaskRepository
    {
        private readonly DatabaseContext _context;

        public TaskRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<bool> UpdateStatus(List<MediaFile> mediaFiles, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                //ASSET CHECK
                var assetCheck = await _context.AssetChecks
                                 .Include(x => x.Asset)
                                 .FirstOrDefaultAsync(x => x.Id == mediaFiles.First().ItemId);
                if (assetCheck != null)
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

                    if (assetCheck.Status == RequestStatus.InProgress)
                    {
                        asset!.Status = AssetStatus.NeedInspection;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        assetLocation!.State = RoomState.NeedInspection;
                        _context.Entry(assetLocation).State = EntityState.Modified;
                    }
                    else if (assetCheck.Status == RequestStatus.Reported)
                    {
                        foreach (var mediaFile in mediaFiles)
                        {
                            var newMediaFile = new MediaFile
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = now.Value,
                                CreatorId = editorId,
                                EditedAt = now.Value,
                                EditorId = editorId,
                                FileName = mediaFile.FileName,
                                Key = mediaFile.Key,
                                RawUri = mediaFile.RawUri,
                                Uri = mediaFile.Uri,
                                Extensions = mediaFile.Extensions,
                                FileType = mediaFile.FileType,
                                Content = mediaFile.Content,
                                ItemId = assetCheck.Id
                            };
                            _context.MediaFiles.Add(newMediaFile);
                        }
                        assetCheck.Result = mediaFiles.First().Content;
                        _context.Entry(assetCheck).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }

                //TRANSPORTATION
                var transportation = await _context.Transportations
                                    .Include(x => x.Asset)
                                    .Include(x => x.TransportationDetails)
                                    .FirstOrDefaultAsync(x => x.Id == mediaFiles.First().ItemId);
                if (transportation != null)
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
                        if (statusUpdate == RequestStatus.InProgress)
                        {
                            if (roomAsset != null)
                            {
                                roomAsset!.Status = AssetStatus.Transportation;
                                roomAsset.EditedAt = now.Value;
                                _context.Entry(roomAsset).State = EntityState.Modified;
                            }

                            asset.Status = AssetStatus.Transportation;
                            asset.EditedAt = now.Value;
                            _context.Entry(asset).State = EntityState.Modified;

                            toRoom!.State = RoomState.Transportation;
                            _context.Entry(toRoom).State = EntityState.Modified;

                            fromRoom!.State = RoomState.Transportation;
                            _context.Entry(fromRoom).State = EntityState.Modified;

                            //var roomAsset = _context.RoomAssets.FirstOrDefault(x => x.Id == asset.Id && x.ToDate == null);
                        }
                        else if (statusUpdate == RequestStatus.Reported)
                        {
                            foreach (var mediaFile in mediaFiles)
                            {
                                var newMediaFile = new MediaFile
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedAt = now.Value,
                                    CreatorId = editorId,
                                    EditedAt = now.Value,
                                    EditorId = editorId,
                                    FileName = mediaFile.FileName,
                                    Key = mediaFile.Key,
                                    RawUri = mediaFile.RawUri,
                                    Uri = mediaFile.Uri,
                                    Extensions = mediaFile.Extensions,
                                    FileType = mediaFile.FileType,
                                    Content = mediaFile.Content,
                                    ItemId = transportation.Id
                                };
                                _context.MediaFiles.Add(newMediaFile);
                            }
                            transportation.Result = mediaFiles.First().Content;
                            _context.Entry(transportation).State = EntityState.Modified;
                        }
                    }
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }

                //REPAIRATION
                var repairation = await _context.Repairations
                                .Include(x => x.Asset)
                                .FirstOrDefaultAsync(x => x.Id == mediaFiles.First().ItemId);
                if (repairation != null)
                {
                    repairation.EditedAt = now.Value;
                    repairation.EditorId = editorId;
                    repairation.Status = statusUpdate;
                    _context.Entry(repairation).State = EntityState.Modified;

                    var asset = await _context.Assets
                            .Include(a => a.Type)
                            .Where(a => a.Id == repairation.AssetId)
                            .FirstOrDefaultAsync();

                    var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                    var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                    if (statusUpdate == RequestStatus.InProgress)
                    {
                        if (roomAsset != null)
                        {
                            roomAsset!.Status = AssetStatus.Repair;
                            roomAsset.EditedAt = now.Value;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }

                        asset!.Status = AssetStatus.Repair;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        location!.State = RoomState.Repair;
                        _context.Entry(location).State = EntityState.Modified;
                    }
                    else if (statusUpdate == RequestStatus.Reported)
                    {
                        foreach (var mediaFile in mediaFiles)
                        {
                            var newMediaFile = new MediaFile
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = now.Value,
                                CreatorId = editorId,
                                EditedAt = now.Value,
                                EditorId = editorId,
                                FileName = mediaFile.FileName,
                                Key = mediaFile.Key,
                                RawUri = mediaFile.RawUri,
                                Uri = mediaFile.Uri,
                                Extensions = mediaFile.Extensions,
                                FileType = mediaFile.FileType,
                                Content = mediaFile.Content,
                                ItemId = repairation.Id
                            };
                            _context.MediaFiles.Add(newMediaFile);
                        }
                        repairation.Result = mediaFiles.First().Content;
                        _context.Entry(repairation).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }

                //REPLACEMENT
                var replacement = await _context.Replacements
                                  .Include(x => x.Asset)
                                  .FirstOrDefaultAsync(x => x.Id == mediaFiles.First().ItemId);
                if (replacement != null)
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

                    if (replacement.Status == RequestStatus.InProgress)
                    {
                        asset!.Status = AssetStatus.Replacement;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        newAsset!.Status = AssetStatus.Replacement;
                        newAsset.EditedAt = now.Value;
                        _context.Entry(newAsset).State = EntityState.Modified;

                        assetLocation!.State = RoomState.Replacement;
                        _context.Entry(assetLocation).State = EntityState.Modified;

                        newAssetLocation!.State = RoomState.Replacement;
                        _context.Entry(newAssetLocation).State = EntityState.Modified;
                    }
                    else if (replacement.Status == RequestStatus.Reported)
                    {
                        foreach (var mediaFile in mediaFiles)
                        {
                            var newMediaFile = new MediaFile
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = now.Value,
                                CreatorId = editorId,
                                EditedAt = now.Value,
                                EditorId = editorId,
                                FileName = mediaFile.FileName,
                                Key = mediaFile.Key,
                                RawUri = mediaFile.RawUri,
                                Uri = mediaFile.Uri,
                                Extensions = mediaFile.Extensions,
                                FileType = mediaFile.FileType,
                                Content = mediaFile.Content,
                                ItemId = replacement.Id
                            };
                            _context.MediaFiles.Add(newMediaFile);
                        }
                        replacement.Result = mediaFiles.First().Content;
                        _context.Entry(replacement).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }

                //MAINTENANCE
                var maintenance = await _context.Maintenances
                                  .Include(x => x.Asset)
                                  .FirstOrDefaultAsync(x => x.Id == mediaFiles.First().ItemId);
                if (maintenance != null)
                {
                    maintenance.EditedAt = now.Value;
                    maintenance.EditorId = editorId;
                    maintenance.Status = statusUpdate;
                    _context.Entry(maintenance).State = EntityState.Modified;

                    var asset = await _context.Assets
                            .Include(a => a.Type)
                            .Where(a => a.Id == maintenance.AssetId)
                            .FirstOrDefaultAsync();

                    var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
                    var location = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == roomAsset!.RoomId && roomAsset.AssetId == asset!.Id);
                    if (statusUpdate == RequestStatus.InProgress)
                    {
                        if (roomAsset != null)
                        {
                            roomAsset!.Status = AssetStatus.Maintenance;
                            roomAsset.EditedAt = now.Value;
                            _context.Entry(roomAsset).State = EntityState.Modified;
                        }

                        asset!.Status = AssetStatus.Maintenance;
                        asset.EditedAt = now.Value;
                        _context.Entry(asset).State = EntityState.Modified;

                        location!.State = RoomState.Maintenance;
                        _context.Entry(location).State = EntityState.Modified;
                    }
                    else if (statusUpdate == RequestStatus.Reported)
                    {
                        foreach (var mediaFile in mediaFiles)
                        {
                            var newMediaFile = new MediaFile
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = now.Value,
                                CreatorId = editorId,
                                EditedAt = now.Value,
                                EditorId = editorId,
                                FileName = mediaFile.FileName,
                                Key = mediaFile.Key,
                                RawUri = mediaFile.RawUri,
                                Uri = mediaFile.Uri,
                                Extensions = mediaFile.Extensions,
                                FileType = mediaFile.FileType,
                                Content = mediaFile.Content,
                                ItemId = maintenance.Id
                            };
                            _context.MediaFiles.Add(newMediaFile);
                        }
                        maintenance.Result = mediaFiles.First().Content;
                        _context.Entry(maintenance).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }

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
