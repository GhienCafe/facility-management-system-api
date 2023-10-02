using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services
{
    public interface IMaintenanceScheduleService : IBaseService
    {
        Task<ApiResponse> CreateMaintenancePeriod(MaintenancePeriodCreateDto createDto);
        Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetMaintenanceScheduleConfig(Guid id);
    }
    public class MaintenanceScheduleService : BaseService, IMaintenanceScheduleService
    {
        public MaintenanceScheduleService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> CreateMaintenancePeriod(MaintenancePeriodCreateDto createDto)
        {
            var maintenanceSchedule = createDto.ProjectTo<MaintenancePeriodCreateDto, MaintenanceScheduleConfig>();
            if (!await MainUnitOfWork.MaintenanceScheduleRepository.InsertAsync(maintenanceSchedule, AccountId, CurrentDate))
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Created successfully");
        }

        public Task<ApiResponse<MaintenanceScheduleConfigDetailDto>> GetMaintenanceScheduleConfig(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
