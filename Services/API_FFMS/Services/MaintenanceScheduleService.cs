using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface IMaintenanceScheduleService : IBaseService
    {
        Task<ApiResponse> CreateMaintenancePeriod();
        Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetMaintenanceScheduleConfig(Guid id);
    }
    public class MaintenanceScheduleService : BaseService, IMaintenanceScheduleService
    {
        public const int PERIOD = 9;
        public MaintenanceScheduleService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> CreateMaintenancePeriod()
        {
            //if (!await MainUnitOfWork.MaintenanceScheduleRepository.InsertAsync(maintenanceSchedule, AccountId, CurrentDate))
            //    throw new ApiException("Create failed", StatusCode.SERVER_ERROR);
            var maintenanceSchedules = new List<MaintenanceConfig>();
            var now = DateTime.UtcNow;

            var assets = await MainUnitOfWork.AssetRepository.FindAsync<Asset>(new Expression<Func<Asset, bool>>[]
            {
                x => !x!.DeletedAt.HasValue
            }, null);
            //foreach (var asset in assets)
            //{
            //    // check in each asset
            //    // if asset.LastMaintenanceTime + 6 == now (as result) then add this asset to maintenanceSchedules
            //    if(asset.Type != null && asset.Type.TypeName.Equals())
            //    {

            //    }
            //}

            return ApiResponse.Created("Created successfully");
        }

        public Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetMaintenanceScheduleConfig(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
