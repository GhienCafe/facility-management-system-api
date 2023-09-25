using API_FFMS.Dtos;
using AppCore.Models;
using MainData;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface ITaskService : IBaseService
{
    Task<ApiResponses<TaskDto>> GetTasks();
}

public class TaskService : BaseService, ITaskService
{
    public TaskService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public Task<ApiResponses<TaskDto>> GetTasks()
    {
        var maintenance = MainUnitOfWork.MaintenanceRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);
        // var replacement = MainUnitOfWork.Re.GetQuery()
        //     .Where(x => !x!.DeletedAt.HasValue);
        // var maintenance = MainUnitOfWork.MaintenanceRepository.GetQuery()
        //     .Where(x => !x!.DeletedAt.HasValue);
        // var maintenance = MainUnitOfWork.MaintenanceRepository.GetQuery()
        //     .Where(x => !x!.DeletedAt.HasValue);

        maintenance =  maintenance.Where(x => x!.AssignedTo == AccountId);
        
        
        throw new ApiException();
    }
}