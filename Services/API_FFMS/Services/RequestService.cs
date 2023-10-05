// using System.Linq.Expressions;
// using API_FFMS.Dtos;
// using API_FFMS.Repositories;
// using AppCore.Extensions;
// using AppCore.Models;
// using MainData;
// using MainData.Entities;
// using MainData.Repositories;
// using Microsoft.EntityFrameworkCore;
//
// namespace API_FFMS.Services;
//
// public interface IRequestService : IBaseService
// {
//     Task<ApiResponse> CreateRequest(ActionRequestCreateDto createDto);
//     Task<ApiResponses<ActionRequestDto>> GetRequests(ActionRequestQuery queryDto);
//     Task<ApiResponse<ActionRequestDetailDto>> GetRequest(Guid id);
//     Task<ApiResponse> DeleteRequest(Guid id);
//     Task<ApiResponse> UpdateRequest(Guid id, ActionRequestUpdateDto updateDto);
// }
//
// public class RequestService : BaseService, IRequestService
// {
//     private readonly IRequestRepository _requestRepository;
//     public RequestService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository, IRequestRepository requestRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
//     {
//         _requestRepository = requestRepository;
//     }
//
//     public async Task<ApiResponse> CreateRequest(ActionRequestCreateDto createDto)
//     {
//         
//         if (!await _requestRepository.InsertRequest(createDto, AccountId, CurrentDate))
//         {
//             throw new ApiException("Thêm thất bại");
//         }
//
//         return ApiResponse.Success("Thêm thành công");
//     }
//
//     public async Task<ApiResponses<ActionRequestDto>> GetRequests(ActionRequestQuery queryDto)
//     {
//         var keyword = queryDto.Keyword?.Trim().ToLower();
//         var requestQueryable = MainUnitOfWork.RequestRepository.GetQuery()
//             .Where(x => !x!.DeletedAt.HasValue);
//
//         if (queryDto.IsInternal != null)
//         {
//             requestQueryable = requestQueryable.Where(x => x!.IsInternal == queryDto.IsInternal);
//         }
//
//         if (keyword != null)
//         {
//             requestQueryable = requestQueryable.Where(x => x!.RequestCode.ToLower().Contains(keyword)
//                 || x.Description!.ToLower().Contains(keyword)
//                 || x.Notes!.ToLower().Contains(keyword));
//         }
//
//         if (queryDto.AssignedTo != null)
//         {
//             requestQueryable = requestQueryable.Where(x => x!.AssignedTo == queryDto.AssignedTo);
//         }
//
//         if (queryDto.RequestStatus != null)
//         {
//             requestQueryable = requestQueryable.Where(x => x!.RequestStatus == queryDto.RequestStatus);
//         }
//
//         if (queryDto.RequestType != null)
//         {
//             requestQueryable = requestQueryable.Where(x => x!.RequestType == queryDto.RequestType);
//         }
//
//         var joinTables = from request in requestQueryable
//                          join user in MainUnitOfWork.UserRepository.GetQuery() on request.AssignedTo equals user.Id into userGroup
//                          from user in userGroup.DefaultIfEmpty()
//                          select new
//                          {
//                              Request = request,
//                              User = user
//                          };
//
//         var totalCount = await joinTables.CountAsync();
//
//         joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);
//
//         var requests = await joinTables.Select(x => new ActionRequestDto
//         {
//             Id = x.Request.Id,
//             RequestCode = x.Request.RequestCode,
//             RequestDate = x.Request.RequestDate,
//             CompletionDate = x.Request.CompletionDate,
//             Description = x.Request.Description,
//             Notes = x.Request.Notes,
//             RequestType = x.Request.RequestType,
//             RequestTypeObj = x.Request.RequestType!.GetValue(),
//             RequestStatus = x.Request.RequestStatus,
//             RequestStatusObj = x.Request.RequestStatus!.GetValue(),
//             AssignedTo = x.Request.AssignedTo,
//             IsInternal = x.Request.IsInternal,
//             CreatedAt = x.Request.CreatedAt,
//             EditedAt = x.Request.EditedAt,
//             CreatorId = x.Request.CreatorId ?? Guid.Empty,
//             EditorId = x.Request.EditorId ?? Guid.Empty,
//             AssignedPerson = new UserDto
//             {
//                 Id = x.User.Id,
//                 Fullname = x.User.Fullname,
//                 Address = x.User.Address,
//                 Status = x.User.Status.GetValue(),
//                 Avatar = x.User.Avatar,
//                 Dob = x.User.Dob,
//                 Email = x.User.Email,
//                 Gender = x.User.Gender,
//                 Role = x.User.Role.GetValue(),
//                 PhoneNumber = x.User.PhoneNumber,
//                 UserCode = x.User.UserCode,
//                 PersonalIdentifyNumber = x.User.PersonalIdentifyNumber,
//                 FirstLoginAt = x.User.FirstLoginAt,
//                 LastLoginAt = x.User.LastLoginAt,
//                 CreatedAt = x.User.CreatedAt,
//                 EditedAt = x.User.EditedAt,
//                 CreatorId = x.User.CreatorId ?? Guid.Empty,
//                 EditorId = x.User.EditorId ?? Guid.Empty,
//             }
//         }).ToListAsync();
//
//         requests = await _mapperRepository.MapCreator(requests);
//
//         return ApiResponses<ActionRequestDto>.Success(
//             requests,
//             totalCount,
//             queryDto.PageSize,
//             queryDto.Page,
//             (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
//     }
//
//     public async Task<ApiResponse<ActionRequestDetailDto>> GetRequest(Guid id)
//     {
//         var requestQuery = MainUnitOfWork.RequestRepository.GetQuery()
//             .Where(x => !x!.DeletedAt.HasValue && x.Id == id);
//
//         if (!requestQuery.Any())
//         {
//             throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
//         }
//
//         var req = await (from request in requestQuery
//             join user in MainUnitOfWork.UserRepository.GetQuery() on request.AssignedTo equals user.Id
//             select new ActionRequestDetailDto
//             {
//                 Id = request.Id,
//                 RequestCode = request.RequestCode,
//                 RequestDate = request.RequestDate,
//                 CompletionDate = request.CompletionDate,
//                 Description = request.Description,
//                 Notes = request.Notes,
//                 RequestType = request.RequestType,
//                 RequestTypeObj = request.RequestType!.GetValue(),
//                 RequestStatus = request.RequestStatus,
//                 RequestStatusObj = request.RequestStatus!.GetValue(),
//                 AssignedTo = request.AssignedTo,
//                 IsInternal = request.IsInternal,
//                 CreatedAt = request.CreatedAt,
//                 EditedAt = request.EditedAt,
//                 CreatorId = request.CreatorId ?? Guid.Empty,
//                 EditorId = request.EditorId ?? Guid.Empty,
//                 AssignedPerson = new UserDto
//                 {
//                     Id = user.Id,
//                     Fullname = user.Fullname,
//                     Address = user.Address,
//                     Status = user.Status.GetValue(),
//                     Avatar = user.Avatar,
//                     Dob = user.Dob,
//                     Email = user.Email,
//                     Gender = user.Gender,
//                     Role = user.Role.GetValue(),
//                     PhoneNumber = user.PhoneNumber,
//                     UserCode = user.UserCode,
//                     PersonalIdentifyNumber = user.PersonalIdentifyNumber,
//                     FirstLoginAt = user.FirstLoginAt,
//                     LastLoginAt = user.LastLoginAt,
//                     CreatedAt = user.CreatedAt,
//                     EditedAt = user.EditedAt,
//                     CreatorId = user.CreatorId ?? Guid.Empty,
//                     EditorId = user.EditorId ?? Guid.Empty,
//                 }
//             }).FirstOrDefaultAsync();
//
//         req = await _mapperRepository.MapCreator(req);
//
//         return ApiResponse<ActionRequestDetailDto>.Success(req);
//     }
//
//     public async Task<ApiResponse> DeleteRequest(Guid id)
//     {
//         var request = await MainUnitOfWork.RequestRepository.FindOneAsync(id);
//
//         if (request == null)
//         {
//             throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
//         }
//         
//         if (request.RequestStatus != RequestStatus.NotStarted && request.RequestStatus != RequestStatus.InProgress)
//         {
//             throw new ApiException($"Không thể xóa yêu cầu có trạng thái {request.RequestStatus?.GetDisplayName()}", StatusCode.BAD_REQUEST);
//         }
//         
//         if (!await MainUnitOfWork.RequestRepository.DeleteAsync(request, AccountId, CurrentDate))
//             throw new ApiException("Xóa yêu cầu thấy bại", StatusCode.SERVER_ERROR);
//
//         return ApiResponse.Success("Xóa yêu cầu thành công");
//     }
//
//     public async Task<ApiResponse> UpdateRequest(Guid id, ActionRequestUpdateDto updateDto)
//     {
//         var request = await MainUnitOfWork.RequestRepository.FindOneAsync(id);
//
//         if (request == null)
//         {
//             throw new ApiException("Không tìm thấy yêu cầu", StatusCode.NOT_FOUND);
//         }
//
//         request.RequestCode = updateDto.RequestCode ?? request.RequestCode;
//         request.Description = updateDto.Description ?? request.Description;
//         request.RequestStatus = updateDto.RequestStatus ?? request.RequestStatus;
//         request.RequestDate = updateDto.RequestDate ?? request.RequestDate;
//         request.Notes = updateDto.Notes ?? request.Notes;
//         request.AssignedTo = updateDto.AssignedTo ?? request.AssignedTo;
//         request.IsInternal = updateDto.IsInternal ?? request.IsInternal;
//         request.CompletionDate = updateDto.CompletionDate ?? request.CompletionDate;
//
//         if (!await MainUnitOfWork.RequestRepository.UpdateAsync(request, AccountId, CurrentDate))
//             throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
//         
//         return ApiResponse.Success("Cập nhật yêu cầu thành công");
//     }
// }