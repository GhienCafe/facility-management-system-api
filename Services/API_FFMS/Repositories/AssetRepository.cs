using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IAssetRepository
{
    Task<bool> InsertAsset(Asset asset, Guid? creatorId, DateTime? now = null);
    Task<bool> InsertAssets(List<Asset> assets, Guid? creatorId, DateTime? now = null);
    Task<bool> DeleteAssets(List<Asset?> assets, Guid? deleterId, DateTime? now = null);
    Task<bool> DeleteAsset(Asset asset, Guid? deleterId, DateTime? now = null);
}
public class AssetRepository : IAssetRepository
{
    private readonly DatabaseContext _context;

    public AssetRepository(DatabaseContext context)
    {
        _context = context;
    }
    public async Task<bool> InsertAsset(Asset asset, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            asset.Id = Guid.NewGuid();
            asset.CreatedAt = now.Value;
            asset.EditedAt = now.Value;
            asset.CreatorId = creatorId;
            asset.StartDateOfUse = now.Value;
            asset.Status = AssetStatus.Operational;
            asset.RequestStatus = RequestType.Operational;
            await _context.Assets.AddAsync(asset);

            var roomAsset = new RoomAsset
            {
                FromDate = now.Value,
                AssetId = asset.Id,
                RoomId = GetWareHouse("207")!.Id,
                Status = AssetStatus.Operational,
                ToDate = null,
                Quantity = asset.Quantity,
                CreatedAt = now.Value,
                CreatorId = creatorId,
            };
            await _context.RoomAssets.AddAsync(roomAsset);

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

    public async Task<bool> InsertAssets(List<Asset> assets, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;

        var assetQuery = _context.Assets.Include(a => a.Type)
                                        .Include(a => a.Model)
                                        .ToList();

        var typeQuery = _context.AssetTypes.Where(x => x.Unit == Unit.Quantity).Select(x => x.Id).ToList();
        var modelQuery = _context.Models.Select(x => x.Id).ToList();

        try
        {
            foreach (var asset in assets)
            {
                if (modelQuery.Any(x => x == asset.ModelId) && typeQuery.Any(x => x == asset.TypeId))
                {
                    var existAsset = _context.Assets.FirstOrDefault(x => x.TypeId == asset.TypeId && x.ModelId == asset.ModelId);
                    if (existAsset != null)
                    {
                        existAsset.Quantity += asset.Quantity;
                        _context.Entry(existAsset).State = EntityState.Modified;
                    }
                }
                else
                {
                    asset.CreatedAt = now.Value;
                    asset.EditedAt = now.Value;
                    asset.CreatorId = creatorId;
                    asset.Status = AssetStatus.Operational;
                    asset.RequestStatus = RequestType.Operational;
                    _context.Assets.Add(asset);
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

    public Room? GetWareHouse(string roomName)
    {
        var wareHouse = _context.Rooms.FirstOrDefault(x => x.RoomCode!.Trim().Equals(roomName.Trim()));
        return wareHouse;
    }

    public async Task<bool> DeleteAssets(List<Asset?> assets, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            foreach (var asset in assets)
            {
                asset!.DeletedAt = now.Value;
                asset.DeleterId = deleterId;
                _context.Entry(asset).State = EntityState.Modified;

                var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id && x.ToDate == null);
                roomAsset!.EditedAt = now.Value;
                roomAsset.EditorId = deleterId;
                roomAsset.ToDate = now.Value;
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

    public async Task<bool> DeleteAsset(Asset asset, Guid? deleterId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            asset.DeletedAt = now.Value;
            asset.DeleterId = deleterId;
            _context.Entry(asset).State = EntityState.Modified;

            var roomAsset = await _context.RoomAssets.FirstOrDefaultAsync(x => x.AssetId == asset.Id && x.ToDate == null);
            roomAsset!.EditedAt = now.Value;
            roomAsset.EditorId = deleterId;
            roomAsset.ToDate = now.Value;
            _context.Entry(roomAsset).State = EntityState.Modified;

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