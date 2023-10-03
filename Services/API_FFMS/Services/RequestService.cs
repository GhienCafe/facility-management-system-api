using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API_FFMS.Services;

public interface IRequestService : IBaseService
{
    Task<ApiResponse> CreateRequest(ActionRequestCreateDto createDto);
    Task<ApiResponses<ActionRequestDto>> GetRequests(ActionRequestQuery queryDto);
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
        if (!await _requestRepository.InsertRequest(createDto, AccountId, CurrentDate))
        {
            throw new ApiException("Thêm thất bại");
        }

        return ApiResponse.Success("Thêm thành công");
    }

    public async Task<ApiResponses<ActionRequestDto>> GetRequests(ActionRequestQuery queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var requestQueryable = MainUnitOfWork.RequestRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.IsInternal != null)
        {
            requestQueryable = requestQueryable.Where(x => x!.IsInternal == queryDto.IsInternal);
        }

        if (keyword != null)
        {
            requestQueryable = requestQueryable.Where(x => x!.RequestCode.ToLower().Contains(keyword)
                || x.Description!.ToLower().Contains(keyword)
                || x.Notes!.ToLower().Contains(keyword));
        }

        if (queryDto.AssignedTo != null)
        {
            requestQueryable = requestQueryable.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.RequestStatus != null)
        {
            requestQueryable = requestQueryable.Where(x => x!.RequestStatus == queryDto.RequestStatus);
        }

        if (queryDto.RequestType != null)
        {
            requestQueryable = requestQueryable.Where(x => x!.RequestType == queryDto.RequestType);
        }

        var joinTables = from request in requestQueryable
                         join user in MainUnitOfWork.UserRepository.GetQuery() on request.AssignedTo equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         select new
                         {
                             Request = request,
                             User = user
                         };

        var totalCount = await joinTables.CountAsync();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var requests = await joinTables.Select(x => new ActionRequestDto
        {
            Id = x.Request.Id,
            RequestCode = x.Request.RequestCode,
            RequestDate = x.Request.RequestDate,
            CompletionDate = x.Request.CompletionDate,
            Description = x.Request.Description,
            Notes = x.Request.Notes,
            RequestType = x.Request.RequestType,
            RequestTypeObj = x.Request.RequestType!.GetValue(),
            RequestStatus = x.Request.RequestStatus,
            RequestStatusObj = x.Request.RequestStatus!.GetValue(),
            AssignedTo = x.Request.AssignedTo,
            IsInternal = x.Request.IsInternal,
            CreatedAt = x.Request.CreatedAt,
            EditedAt = x.Request.EditedAt,
            CreatorId = x.Request.CreatorId ?? Guid.Empty,
            EditorId = x.Request.EditorId ?? Guid.Empty,
            AssignedPerson = new UserDto
            {
                Id = x.User.Id,
                Fullname = x.User.Fullname,
                Address = x.User.Address,
                Status = x.User.Status.GetValue(),
                Avatar = x.User.Avatar,
                Dob = x.User.Dob,
                Email = x.User.Email,
                Gender = x.User.Gender,
                Role = x.User.Role.GetValue(),
                PhoneNumber = x.User.PhoneNumber,
                UserCode = x.User.UserCode,
                PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
                FirstLoginAt = x.User.FirstLoginAt,
                LastLoginAt = x.User.LastLoginAt,
                CreatedAt = x.User.CreatedAt,
                EditedAt = x.User.EditedAt,
                CreatorId = x.User.CreatorId ?? Guid.Empty,
                EditorId = x.User.EditorId ?? Guid.Empty,
            }
        }).ToListAsync();

        requests = await _mapperRepository.MapCreator(requests);

        return ApiResponses<ActionRequestDto>.Success(
            requests,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}