using MainData;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IExportService : IBaseService
{
    
}
public class ExportService : BaseService, IExportService
{
    public ExportService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }
}