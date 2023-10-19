using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IMaintenanceRepository
{
    Task<bool> InsertMaintenance(Maintenance maintenance, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateStatus(Maintenance maintenance, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null);
}

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly DatabaseContext _context;

    public MaintenanceRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertMaintenance(Maintenance entity, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            // Add maintenance
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = now.Value;
            entity.EditedAt = now.Value;
            entity.CreatorId = creatorId;
            entity.Status = RequestStatus.NotStart;
            await _context.Maintenances.AddAsync(entity);
           // await _context.SaveChangesAsync();
            
            // Update asset status
            var asset = await _context.Assets.FindAsync(entity.AssetId);
            asset!.Status = AssetStatus.Maintenance;
            asset.EditedAt = now.Value;
            _context.Entry(asset).State = EntityState.Modified;

            // Update room & send notification
            if (entity.IsInternal)
            {
                var roomAsset = await _context.RoomAssets
                    .FirstOrDefaultAsync(x => x.AssetId == entity.AssetId && x.ToDate == null);

                if (roomAsset != null)
                {
                    roomAsset!.Status = AssetStatus.Maintenance;
                    roomAsset.EditedAt = now.Value;
                    _context.Entry(roomAsset).State = EntityState.Modified;
                }

                var notification = new Notification
                {
                    CreatedAt = now.Value,
                    EditedAt = now.Value,
                    Status = NotificationStatus.Waiting,
                    Content = entity.Description,
                    Title = RequestType.Maintenance.GetDisplayName(),
                    Type = NotificationType.Task,
                    CreatorId = creatorId,
                    IsRead = false,
                    ItemId = entity.Id,
                    UserId = entity.AssignedTo
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

    public async Task<bool> UpdateStatus(Maintenance maintenance, RequestStatus? statusUpdate, Guid? editorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
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
            if (statusUpdate == RequestStatus.Done)
            {
                asset!.Status = AssetStatus.Operational;
                asset.EditedAt = now.Value;
                _context.Entry(asset).State = EntityState.Modified;

                roomAsset!.Status = AssetStatus.Operational;
                roomAsset.EditedAt = now.Value;
                roomAsset.ToDate = now.Value;
                _context.Entry(roomAsset).State = EntityState.Modified;

                location!.State = RoomState.Operational;
                _context.Entry(location).State = EntityState.Modified;
            }
            else if (statusUpdate == RequestStatus.Cancelled)
            {
                asset!.Status = AssetStatus.Operational;
                asset.EditedAt = now.Value;
                _context.Entry(asset).State = EntityState.Modified;

                roomAsset!.Status = AssetStatus.Operational;
                roomAsset.EditedAt = now.Value;
                roomAsset.ToDate = now.Value;
                _context.Entry(roomAsset).State = EntityState.Modified;

                location!.State = RoomState.Operational;
                _context.Entry(location).State = EntityState.Modified;
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