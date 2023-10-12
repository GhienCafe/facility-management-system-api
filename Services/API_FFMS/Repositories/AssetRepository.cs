using MainData;
using MainData.Entities;

namespace API_FFMS.Repositories
{
    public interface IAssetRepository
    {
        Task<bool> InsertAsset(Asset asset, Guid? creatorId, DateTime? now = null);
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
                asset.LastCheckedDate = now.Value;
                asset.LastMaintenanceTime = now.Value;
                asset.Status = AssetStatus.Operational;

                await _context.Assets.AddAsync(asset);

                var roomAsset = new RoomAsset
                {
                    FromDate = now.Value,
                    AssetId = asset.Id,
                    RoomId = GetWareHouse("Kho")!.Id,
                    Status = AssetStatus.Operational,
                    ToDate = null,
                    EditedAt = now.Value,
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

        public Room? GetWareHouse(string roomName)
        {
            var wareHouse = _context.Rooms.FirstOrDefault(x => x.RoomName!.Trim().Equals(roomName.Trim()));
            return wareHouse;
        }
    }

}
