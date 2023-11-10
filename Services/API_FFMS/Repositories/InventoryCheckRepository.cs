using AppCore.Extensions;
using MainData;
using MainData.Entities;

namespace API_FFMS.Repositories;

public interface IInventoryCheckRepository
{
    Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Asset?> assets, Guid? creatorId, DateTime? now = null);
    Task<bool> InsertInventoryCheckV2(InventoryCheck inventoryCheck, List<Room?> rooms, Guid? creatorId, DateTime? now = null);

    //Task<bool> UpdateMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? oldData, IEnumerable<Asset>? newData, Guid? editorId, DateTime? now = null);
}

public class InventoryCheckRepository : IInventoryCheckRepository
{
    private readonly DatabaseContext _context;

    public InventoryCheckRepository(DatabaseContext context)
    {
        _context = context;
    }

    private List<string> GetCodes()
    {
        var requests = _context.InventoryChecks.Where(x => x.RequestCode.StartsWith("SCH"))
            .Select(x => x.RequestCode)
            .ToList();
        return requests;
    }

    private int GenerateRequestCode(ref List<int> numbers)
    {
        int newRequestNumber = numbers.Any() ? numbers.Max() + 1 : 1;
        numbers.Add(newRequestNumber); // Add the new number to the list
        return newRequestNumber;
    }

    public async Task<bool> InsertInventoryCheck(InventoryCheck inventoryCheck, List<Asset?> assets, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.Id = Guid.NewGuid();
            inventoryCheck.CreatedAt = now.Value;
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.CreatorId = creatorId;
            inventoryCheck.Status = RequestStatus.NotStart;
            inventoryCheck.RequestDate = now.Value;
            await _context.InventoryChecks.AddAsync(inventoryCheck);

            foreach (var asset in assets)
            {
                var inventoryCheckDetail = new InventoryCheckDetail
                {
                    Id = Guid.NewGuid(),
                    AssetId = asset!.Id,
                    InventoryCheckId = inventoryCheck.Id,
                    CreatorId = creatorId,
                    CreatedAt = now.Value,
                    Status = asset.Status
                };
                await _context.InventoryCheckDetails.AddAsync(inventoryCheckDetail);
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = inventoryCheck.Description,
                Title = RequestType.Maintenance.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = inventoryCheck.Id,
                UserId = inventoryCheck.AssignedTo
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

    public async Task<bool> InsertInventoryCheckV2(InventoryCheck inventoryCheck, List<Room?> rooms, Guid? creatorId, DateTime? now = null)
    {
        await _context.Database.BeginTransactionAsync();
        now ??= DateTime.UtcNow;
        try
        {
            inventoryCheck.Id = Guid.NewGuid();
            inventoryCheck.CreatedAt = now.Value;
            inventoryCheck.EditedAt = now.Value;
            inventoryCheck.CreatorId = creatorId;
            inventoryCheck.Status = RequestStatus.NotStart;
            inventoryCheck.RequestDate = now.Value;
            await _context.InventoryChecks.AddAsync(inventoryCheck);

            foreach (var room in rooms)
            {
                var roomAssets = _context.RoomAssets
                                         .Where(ra => ra.RoomId == room!.Id && ra.ToDate == null)
                                         .Select(ra => ra.Asset)
                                         .ToList();

                foreach (var asset in roomAssets)
                {
                    var inventoryCheckDetail = new InventoryCheckDetail
                    {
                        Id = Guid.NewGuid(),
                        AssetId = asset!.Id,
                        InventoryCheckId = inventoryCheck.Id,
                        CreatorId = creatorId,
                        CreatedAt = now.Value,
                        RoomId = room!.Id,
                        Status = asset.Status
                    };

                    await _context.InventoryCheckDetails.AddAsync(inventoryCheckDetail);
                }
            }

            var notification = new Notification
            {
                CreatedAt = now.Value,
                EditedAt = now.Value,
                Status = NotificationStatus.Waiting,
                Content = inventoryCheck.Description,
                Title = RequestType.Maintenance.GetDisplayName(),
                Type = NotificationType.Task,
                CreatorId = creatorId,
                IsRead = false,
                ItemId = inventoryCheck.Id,
                UserId = inventoryCheck.AssignedTo
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
}