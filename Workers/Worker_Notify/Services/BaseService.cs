using AppCore.Extensions;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.AspNetCore.Http;

namespace Worker_Notify.Services;

public interface IBaseService
{
}

public class BaseService : IBaseService
{
    internal DateTime CurrentDate = DatetimeExtension.UtcNow();
    internal readonly MainUnitOfWork MainUnitOfWork;
    internal readonly IHttpContextAccessor HttpContextAccessor;
    internal readonly IMapperRepository _mapperRepository;
    public BaseService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository)
    {
        MainUnitOfWork = mainUnitOfWork;
        HttpContextAccessor = httpContextAccessor;
        _mapperRepository = mapperRepository;
    }
}
