using AppCore.Extensions;
using MainData;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IBaseService
{
}

public class BaseService : IBaseService
{
    internal DateTime CurrentDate = DatetimeExtension.UtcNow();
    internal readonly MainUnitOfWork MainUnitOfWork;
    internal readonly IHttpContextAccessor HttpContextAccessor;
    internal readonly IMapperRepository _mapperRepository;
    internal Guid? AccountId;
    public BaseService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository)
    {
        MainUnitOfWork = mainUnitOfWork;
        HttpContextAccessor = httpContextAccessor;
        _mapperRepository = mapperRepository;
        AccountId = httpContextAccessor.HttpContext?.User.GetAccountId();
    }
}