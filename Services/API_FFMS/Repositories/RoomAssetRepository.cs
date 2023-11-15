using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IRoomAssetRepository
{
    Task<bool> AddAssetToRoom(RoomAsset roomAsset, Guid? creatorId, DateTime? now = null);
    Task<bool> AddAssetToRooms(List<RoomAsset> roomAssets, Guid? creatorId, DateTime? now = null);
}

public class RoomAssetRepository : IRoomAssetRepository
{
    private readonly DatabaseContext _context;

    public RoomAssetRepository(DatabaseContext context)
    {
        _context = context;
    }
    public async Task<bool> AddAssetToRoom(RoomAsset roomAsset, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            var asset = await _context.Assets.Include(x => x.Type)
                                             .Where(x => x.Id == roomAsset.AssetId)
                                             .FirstOrDefaultAsync();
            var room = await _context.Rooms.FindAsync(roomAsset.RoomId);
            var currentLocation = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);
            

            if(asset!.Type!.Unit == Unit.Individual)
            {
                if (currentLocation != null)
                {
                    currentLocation.ToDate = now.Value;
                    currentLocation.EditedAt = now.Value;
                    currentLocation.EditorId = creatorId;
                    _context.Entry(currentLocation).State = EntityState.Modified;
                }

                roomAsset.Id = Guid.NewGuid();
                roomAsset.CreatedAt = now.Value;
                roomAsset.EditedAt = now.Value;
                roomAsset.CreatorId = creatorId;
                roomAsset.Status = AssetStatus.Operational;
                roomAsset.FromDate = now.Value;
                roomAsset.Quantity = 1;
                roomAsset.ToDate = null;
                roomAsset.AssetId = asset!.Id;
                roomAsset.RoomId = room!.Id;
                await _context.RoomAssets.AddAsync(roomAsset);
            }
            else if(asset!.Type!.Unit == Unit.Quantity)
            {
                if (currentLocation != null)
                {
                    currentLocation.Quantity = currentLocation.Quantity - roomAsset.Quantity;
                    currentLocation.EditedAt = now.Value;
                    currentLocation.EditorId = creatorId;
                    _context.Entry(currentLocation).State = EntityState.Modified;
                }

                roomAsset.Id = Guid.NewGuid();
                roomAsset.CreatedAt = now.Value;
                roomAsset.EditedAt = now.Value;
                roomAsset.CreatorId = creatorId;
                roomAsset.Status = AssetStatus.Operational;
                roomAsset.FromDate = now.Value;
                roomAsset.Quantity = roomAsset.Quantity;
                roomAsset.ToDate = null;
                roomAsset.AssetId = asset!.Id;
                roomAsset.RoomId = room!.Id;
                await _context.RoomAssets.AddAsync(roomAsset);
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

    public async Task<bool> AddAssetToRooms(List<RoomAsset> roomAssets, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            foreach(var roomAsset in roomAssets)
            {
                var asset = await _context.Assets.Include(x => x.Type)
                                             .Where(x => x.Id == roomAsset.AssetId)
                                             .FirstOrDefaultAsync();
                var room = await _context.Rooms.FindAsync(roomAsset.RoomId);
                var currentLocation = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset!.Id && x.ToDate == null);


                if (asset!.Type!.Unit == Unit.Individual)
                {
                    if (currentLocation != null)
                    {
                        currentLocation.ToDate = now.Value;
                        currentLocation.EditedAt = now.Value;
                        currentLocation.EditorId = creatorId;
                        _context.Entry(currentLocation).State = EntityState.Modified;
                    }

                    roomAsset.Id = Guid.NewGuid();
                    roomAsset.CreatedAt = now.Value;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.CreatorId = creatorId;
                    roomAsset.Status = AssetStatus.Operational;
                    roomAsset.FromDate = now.Value;
                    roomAsset.Quantity = 1;
                    roomAsset.ToDate = null;
                    roomAsset.AssetId = asset!.Id;
                    roomAsset.RoomId = room!.Id;
                    await _context.RoomAssets.AddAsync(roomAsset);
                }
                else if (asset!.Type!.Unit == Unit.Quantity)
                {
                    if (currentLocation != null)
                    {
                        currentLocation.Quantity = currentLocation.Quantity - roomAsset.Quantity;
                        currentLocation.EditedAt = now.Value;
                        currentLocation.EditorId = creatorId;
                        _context.Entry(currentLocation).State = EntityState.Modified;
                    }

                    roomAsset.Id = Guid.NewGuid();
                    roomAsset.CreatedAt = now.Value;
                    roomAsset.EditedAt = now.Value;
                    roomAsset.CreatorId = creatorId;
                    roomAsset.Status = AssetStatus.Operational;
                    roomAsset.FromDate = now.Value;
                    roomAsset.Quantity = roomAsset.Quantity;
                    roomAsset.ToDate = null;
                    roomAsset.AssetId = asset!.Id;
                    roomAsset.RoomId = room!.Id;
                    await _context.RoomAssets.AddAsync(roomAsset);
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
