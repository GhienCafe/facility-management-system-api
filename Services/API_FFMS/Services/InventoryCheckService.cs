using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services;
public interface IInventoryCheckService : IBaseService
{
    Task<ApiResponse> Create(InventoryCheckCreateDto createDto);
    Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id);
    Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto);
    Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
}


public class InventoryCheckService : BaseService, IInventoryCheckService
{
    private readonly IInventoryCheckRepository _repository;
    public InventoryCheckService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                 IMapperRepository mapperRepository, IInventoryCheckRepository repository)
                                 : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse> Create(InventoryCheckCreateDto createDto)
    {
        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDto.AssetIds.Contains(x.Id)
                }, null);

        var inventoryCheck = new InventoryCheck
        {
            InventoryCheckConfigId = createDto.InventoryCheckConfigId,
            RequestCode = GenerateRequestCode(),
            Description = createDto.Description,
            Notes = createDto.Notes,
            Priority = createDto.Priority,
            IsInternal = createDto.IsInternal,
            AssignedTo = createDto.AssignedTo,
            RoomId = createDto.RoomId
        };

        if (!await _repository.InsertInventoryCheck(inventoryCheck, assets, AccountId, CurrentDate))
        {
            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Created("Gửi yêu cầu thành công");
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingInventoryCheck= await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(
                new Expression<Func<InventoryCheck, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
        if (existingInventoryCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
        }

        if (!await MainUnitOfWork.InventoryCheckRepository.DeleteAsync(existingInventoryCheck, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        }
        return ApiResponse.Success();
    }

    public string GenerateRequestCode()
    {
        var requests = MainUnitOfWork.InventoryCheckRepository.GetQueryAll().ToList();

        var numbers = new List<int>();
        foreach (var t in requests)
        {
            int.TryParse(t!.RequestCode[3..], out int lastNumber);
            numbers.Add(lastNumber);
        }

        string newRequestCode = "IVC1";

        if (requests.Any())
        {
            var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
            if (requests.Any(x => x!.RequestCode.StartsWith("IVC")))
            {
                newRequestCode = $"IVC{lastCode + 1}";
            }
        }
        return newRequestCode;
    }

    public async Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id)
    {
        var existingInventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync<InventoryCheckDto>(
                new Expression<Func<InventoryCheck, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

        if (existingInventoryCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
        }

        var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery()
                                                               .Where(m => m!.ItemId == existingInventoryCheck.Id);
        var mediaFile = new MediaFileDto
        {
            FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
            Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
            Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
        };

        var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();
        var room = await roomDataset
                            .Where(r => r!.Id == existingInventoryCheck.RoomId)
                            .Select(r => new RoomBaseDto
                            {
                                Id = r!.Id,
                                RoomCode = r.RoomCode,
                                RoomName = r.RoomName,
                                StatusId = r.StatusId,
                                FloorId = r.FloorId,
                                CreatedAt = r.CreatedAt,
                                EditedAt = r.EditedAt
                            }).FirstOrDefaultAsync();

        var userQuery = MainUnitOfWork.UserRepository.GetQuery();
        var assignedTo = await userQuery.Where(x => x!.Id == existingInventoryCheck.AssignedToId)
                        .Select(x => new UserBaseDto
                        {
                            UserCode = x!.UserCode,
                            Fullname = x.Fullname,
                            RoleObj = x.Role.GetValue(),
                            Avatar = x.Avatar,
                            StatusObj = x.Status.GetValue(),
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber,
                            Address = x.Address,
                            Gender = x.Gender,
                            Dob = x.Dob
                        }).FirstOrDefaultAsync();

        var inventoryCheckDetails = MainUnitOfWork.InventoryCheckDetailRepository.GetQuery();
        var assetQuery = MainUnitOfWork.AssetRepository.GetQuery();
        var assets = await inventoryCheckDetails
                     .Where(detail => detail!.InventoryCheckId == id)
                     .Join(assetQuery,
                            detail => detail!.AssetId,
                            asset => asset!.Id, (detail, asset) => new AssetInventoryCheck
                            {
                                AssetName = asset!.AssetName,
                                AssetCode = asset.AssetCode,
                                StatusObj = detail!.Status.GetValue(),
                                Status = detail.Status,
                            }).ToListAsync();

        var inventoryCheck = new InventoryCheckDto
        {
            Id = existingInventoryCheck.Id,
            RequestCode = existingInventoryCheck.RequestCode,
            Description = existingInventoryCheck.Description,
            Notes = existingInventoryCheck.Notes,
            IsInternal = existingInventoryCheck.IsInternal,
            Status = existingInventoryCheck.Status,
            StatusObj = existingInventoryCheck.Status.GetValue(),
            RequestDate = existingInventoryCheck.RequestDate,
            CompletionDate = existingInventoryCheck.CompletionDate,
            Priority = existingInventoryCheck.Priority,
            PriorityObj = existingInventoryCheck.Priority.GetValue(),
            Checkin = existingInventoryCheck.Checkin,
            Checkout = existingInventoryCheck.Checkout,
            Result = existingInventoryCheck.Result,
            Assets = assets,
            Room = room,
            MediaFile = mediaFile,
            AssignedTo = assignedTo
        };

        inventoryCheck = await _mapperRepository.MapCreator(inventoryCheck);
        return ApiResponse<InventoryCheckDto>.Success(inventoryCheck);
    }

    public async Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto)
    {
        var keyword = queryDto.Keyword?.Trim().ToLower();
        var inventoryCheckQuery = MainUnitOfWork.InventoryCheckRepository.GetQuery()
                             .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.IsInternal != null)
        {
            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
        }

        if (queryDto.AssignedTo != null)
        {
            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
        }

        if (queryDto.Status != null)
        {
            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.Status == queryDto.Status);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                               || x.Notes!.ToLower().Contains(keyword) ||
                                                               x.RequestCode.ToLower().Contains(keyword));
        }

        inventoryCheckQuery = inventoryCheckQuery.OrderByDescending(x => x!.CreatedAt);

        var totalCount = await inventoryCheckQuery.CountAsync();
        inventoryCheckQuery = inventoryCheckQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var inventoryChecks = await inventoryCheckQuery.Select(i => new InventoryCheckDto
        {
            Id = i!.Id,
            RequestCode = i.RequestCode,
            RequestDate = i.RequestDate,
            CompletionDate = i.CompletionDate,
            Status = i.Status,
            StatusObj = i.Status!.GetValue(),
            Description = i.Description,
            Checkout = i.Checkout,
            Checkin = i.Checkout,
            Result = i.Result,
            Priority = i.Priority,
            PriorityObj = i.Priority.GetValue(),
            Notes = i.Notes,
            IsInternal = i.IsInternal,
            Assets = i.InventoryCheckDetails!.Select(detail => new AssetInventoryCheck
            {
                AssetName = detail.Asset!.AssetName,
                AssetCode = detail.Asset.AssetCode,
                StatusObj = detail!.Status.GetValue(),
                Status = detail.Status,
            }).ToList(),
            Room = new RoomBaseDto
            {
                //Id = i.RoomId,
                //RoomCode = i.Room!.RoomCode,
                //RoomName = i.Room.RoomName,
                //StatusId = i.Room.StatusId,
                //FloorId = i.Room.FloorId,
                //CreatedAt = i.Room.CreatedAt,
                //EditedAt = i.Room.EditedAt
            },
            AssignedTo = new UserBaseDto
            {
                UserCode = i.User!.UserCode,
                Fullname = i.User.Fullname,
                RoleObj = i.User.Role.GetValue(),
                Avatar = i.User.Avatar,
                StatusObj = i.User.Status.GetValue(),
                Email = i.User.Email,
                PhoneNumber = i.User.PhoneNumber,
                Address = i.User.Address,
                Gender = i.User.Gender,
                Dob = i.User.Dob
            }
        }).ToListAsync();

        inventoryChecks = await _mapperRepository.MapCreator(inventoryChecks);

        return ApiResponses<InventoryCheckDto>.Success(
                inventoryChecks,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
    {
        var existinginventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(id);
        if (existinginventoryCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầunày", StatusCode.NOT_FOUND);
        }

        if (existinginventoryCheck.Status != RequestStatus.InProgress)
        {
            throw new ApiException("Chỉ được cập nhật các yêu cầu chưa được tiếp nhận", StatusCode.NOT_FOUND);
        }

        existinginventoryCheck.Description = updateDto.Description ?? existinginventoryCheck.Description;
        existinginventoryCheck.Notes = updateDto.Notes ?? existinginventoryCheck.Notes;
        existinginventoryCheck.Priority = updateDto.Priority ?? existinginventoryCheck.Priority;
        existinginventoryCheck.AssignedTo = updateDto.AssignedTo ?? existinginventoryCheck.AssignedTo;
        existinginventoryCheck.IsInternal = updateDto.IsInternal ?? existinginventoryCheck.IsInternal;

        if (!await MainUnitOfWork.InventoryCheckRepository.UpdateAsync(existinginventoryCheck, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật yêu cầu thành công");
    }
}
