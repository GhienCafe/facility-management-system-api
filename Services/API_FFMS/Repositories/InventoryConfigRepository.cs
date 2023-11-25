using MainData;
using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Repositories;

public interface IInventoryConfigRepository
{
    Task<bool> InsertInventoryConfig(InventoryCheckConfig inventoryConfig, List<InventoryDetailConfig>? checkDates, Guid? creatorId, DateTime? now = null);
    Task<bool> UpdateInsertInventoryConfig(InventoryCheckConfig inventoryConfig, List<InventoryDetailConfig> checkDates, Guid? creatorId, DateTime? now = null);
}

public class InventoryConfigRepository : IInventoryConfigRepository
{
    private readonly DatabaseContext _context;

    public InventoryConfigRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> InsertInventoryConfig(InventoryCheckConfig inventoryConfig, List<InventoryDetailConfig>? checkDates, Guid? creatorId, DateTime? now = null)
    {
        await using var transaction =  await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;

        try
        {
            inventoryConfig.Id = Guid.NewGuid();
            inventoryConfig.CreatedAt = now.Value;
            inventoryConfig.CreatorId = creatorId;
            _context.InventoryCheckConfigs.Add(inventoryConfig);
            await _context.SaveChangesAsync();

            if (checkDates != null)
            {
                foreach (var checkDate in checkDates)
                {
                    checkDate.InventoryConfigId = inventoryConfig.Id;
                    _context.InventoryDetailConfigs.Add(checkDate);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> UpdateInsertInventoryConfig(InventoryCheckConfig inventoryConfig, List<InventoryDetailConfig> checkDates, Guid? creatorId, DateTime? now = null)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            now ??= DateTime.UtcNow;

            var existingConfig = await _context.InventoryCheckConfigs.FindAsync(inventoryConfig.Id);
            if (existingConfig == null)
            {
                inventoryConfig.CreatedAt = now.Value;
                inventoryConfig.CreatorId = creatorId;
                _context.InventoryCheckConfigs.Add(inventoryConfig);
            }
            else
            {
                existingConfig.Content = inventoryConfig.Content; 
                existingConfig.NotificationDays = inventoryConfig.NotificationDays; 
                existingConfig.IsActive = inventoryConfig.IsActive;

                _context.InventoryCheckConfigs.Update(existingConfig);
            }

            await _context.SaveChangesAsync();


            var existingCheckDates = await _context.InventoryDetailConfigs
                .Where(cd => cd.InventoryConfigId == inventoryConfig.Id)
                .ToListAsync();

            _context.InventoryDetailConfigs.RemoveRange(existingCheckDates);
            
            foreach (var checkDate in checkDates)
            {
                checkDate.InventoryConfigId = inventoryConfig.Id;
                _context.InventoryDetailConfigs.Add(checkDate);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

}