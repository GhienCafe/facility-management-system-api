// using MainData;
// using MainData.Entities;
// using Microsoft.EntityFrameworkCore;
//
// namespace API_FFMS.Repositories;
//
// public interface IMaintenanceScheduleRepository
// {
//     Task<bool> InsertMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? assets, Guid? creatorId, DateTime? now = null);
//     
//     Task<bool> UpdateMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? oldData, IEnumerable<Asset>? newData, Guid? editorId, DateTime? now = null);
// }
//
// public class MaintenanceScheduleRepository : IMaintenanceScheduleRepository
// {
//     private readonly DatabaseContext _databaseContext;
//
//     public MaintenanceScheduleRepository(DatabaseContext databaseContext)
//     {
//         _databaseContext = databaseContext;
//     }
//
//     public async Task<bool> InsertMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? assets,
//         Guid? creatorId, DateTime? now = null)
//     {
//         var requestCodes = GetCodes();
//         List<int> numbers = requestCodes.Where(x => x.StartsWith("SCH"))
//             .Select(x => int.TryParse(x[3..], out var lastNumber) ? lastNumber : 0)
//             .ToList();
//         
//         _databaseContext.Database.BeginTransaction();
//     
//         now ??= DateTime.UtcNow;
//         try
//         {
//             maintenanceScheduleConfig.Id = Guid.NewGuid();
//             maintenanceScheduleConfig.CreatedAt = now.Value;
//             maintenanceScheduleConfig.EditedAt = now.Value;
//             maintenanceScheduleConfig.CreatorId = creatorId;
//             maintenanceScheduleConfig.Code = "SCH" + GenerateRequestCode(ref numbers);
//             await _databaseContext.MaintenanceScheduleConfigs.AddAsync(maintenanceScheduleConfig);
//
//             if (assets != null && assets.Any())
//             {
//                 foreach (var asset in assets)
//                 {
//                     asset.MaintenanceConfigId = maintenanceScheduleConfig.Id;
//                 }
//             
//                 // Corrected code to use UpdateRange
//                 _databaseContext.Assets.UpdateRange(assets);
//             }
//         
//             await _databaseContext.SaveChangesAsync();
//             _databaseContext.Database.CommitTransaction();
//
//             return true;
//         }
//         catch (Exception ex)
//         {
//             _databaseContext.Database.RollbackTransaction();
//             return false;
//         }
//     }
//
//     public async Task<bool> UpdateMaintenanceScheduleConfig(MaintenanceScheduleConfig maintenanceScheduleConfig, IEnumerable<Asset>? oldData,
//         IEnumerable<Asset>? newData, Guid? editorId, DateTime? now = null)
//     {
//         _databaseContext.Database.BeginTransaction();
//     
//         now ??= DateTime.UtcNow;
//
//         try
//         {
//             maintenanceScheduleConfig.EditedAt = now.Value;
//             maintenanceScheduleConfig.EditorId = editorId;
//             
//             // Clear old config
//             if (oldData != null && oldData.Any())
//             {
//                 foreach (var asset in oldData)
//                 {
//                     asset.MaintenanceConfigId = null;
//                 }
//             
//                 // Corrected code to use UpdateRange
//                 _databaseContext.Assets.UpdateRange(oldData);
//             }
//             
//             if (newData != null && newData.Any())
//             {
//                 foreach (var asset in newData)
//                 {
//                     asset.MaintenanceConfigId = maintenanceScheduleConfig.Id;
//                 }
//             
//                 // Corrected code to use UpdateRange
//                 _databaseContext.Assets.UpdateRange(newData);
//             }
//             
//             _databaseContext.Entry(maintenanceScheduleConfig).State = EntityState.Modified;
//
//             await _databaseContext.SaveChangesAsync();
//             _databaseContext.Database.CommitTransaction();
//
//             return true;
//         }
//         catch
//         {
//             _databaseContext.Database.RollbackTransaction();
//             return false;
//         }
//     }
//
//     private List<string> GetCodes()
//     {
//         var requests = _databaseContext.MaintenanceScheduleConfigs.Where(x => x.Code.StartsWith("SCH"))
//             .Select(x => x.Code)
//             .ToList();
//         return requests;
//     }
//     
//     private int GenerateRequestCode(ref List<int> numbers)
//     {
//         int newRequestNumber = numbers.Any() ? numbers.Max() + 1 : 1;
//         numbers.Add(newRequestNumber); // Add the new number to the list
//         return newRequestNumber;
//     }
// }