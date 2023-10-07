using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories
{
    public interface IReplacementRepository
    {
        Task<bool> InsertReplacement(Replacement replacement, Guid? creatorId, DateTime? now = null);
    }
    public class ReplacementRepository : IReplacementRepository
    {
        private readonly DatabaseContext _context;

        public ReplacementRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<bool> InsertReplacement(Replacement replacement, Guid? creatorId, DateTime? now = null)
        {
            await _context.Database.BeginTransactionAsync();
            now ??= DateTime.UtcNow;
            try
            {
                replacement.Id = Guid.NewGuid();
                replacement.CreatedAt = now.Value;
                replacement.EditedAt = now.Value;
                replacement.CreatorId = creatorId;
                replacement.Status = RequestStatus.NotStarted;
                await _context.Replacements.AddAsync(replacement);

                var asset = await _context.Assets.FindAsync(replacement.AssetId);
                asset!.Status = AssetStatus.Replacement;
                asset.EditedAt = now.Value;
                _context.Entry(asset).State = EntityState.Modified;

                var newAsset = await _context.Assets.FindAsync(replacement.NewAssetId);
                newAsset!.Status = AssetStatus.Replacement;
                newAsset.EditedAt = now.Value;
                _context.Entry(newAsset).State = EntityState.Modified;

                if (replacement.IsInternal)
                {

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
