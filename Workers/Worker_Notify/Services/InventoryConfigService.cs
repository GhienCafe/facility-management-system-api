using MainData;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Worker_Notify.Services;

public interface IInventoryConfigService : IBaseService
{
    Task CreateSystemNotification();
}

public class InventoryConfigService : BaseService, IInventoryConfigService
{
    public InventoryConfigService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public Task CreateSystemNotification()
    {
        throw new NotImplementedException();
    }
}
