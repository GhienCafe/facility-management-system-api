using MainData;
using MainData.Entities;

namespace API_FFMS.Repositories;

public interface IMaintenanceScheduleRepository
{
    Task<bool> InsertMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? assets, Guid? creatorId, DateTime? now = null);
}

public class MaintenanceScheduleRepository : IMaintenanceScheduleRepository
{
    private readonly DatabaseContext _databaseContext;

    public MaintenanceScheduleRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<bool> InsertMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? assets,
        Guid? creatorId, DateTime? now = null)
    {
        _databaseContext.Database.BeginTransaction();
    
        now ??= DateTime.UtcNow;
        try
        {
            maintenanceScheduleConfig.Id = Guid.NewGuid();
            maintenanceScheduleConfig.CreatedAt = now.Value;
            maintenanceScheduleConfig.EditedAt = now.Value;
            maintenanceScheduleConfig.CreatorId = creatorId;
            await _databaseContext.MaintenanceScheduleConfigs.AddAsync(maintenanceScheduleConfig);

            if (assets != null && assets.Any())
            {
                foreach (var asset in assets)
                {
                    asset.MaintenanceConfigId = maintenanceScheduleConfig.Id;
                }
            
                // Corrected code to use UpdateRange
                _databaseContext.Assets.UpdateRange(assets);
            }
        
            await _databaseContext.SaveChangesAsync();
            _databaseContext.Database.CommitTransaction();

            return true;
        }
        catch (Exception ex)
        {
            _databaseContext.Database.RollbackTransaction();
            return false;
        }
    }
}