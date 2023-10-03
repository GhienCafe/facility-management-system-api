using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;

namespace API_FFMS.Services;

public interface IRequestService : IBaseService
{
    Task<ApiResponse> CreateRequest(ActionRequestCreateDto createDto);
}

public class RequestService : BaseService, IRequestService
{
    private readonly IRequestRepository _requestRepository;
    public RequestService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IRequestRepository requestRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<ApiResponse> CreateRequest(ActionRequestCreateDto createDto)
    {
        if (! await _requestRepository.InsertRequest(createDto, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm thất bại");
        }

        return ApiResponse.Success("Thêm thành công");
    }
}