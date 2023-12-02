using API_FFMS.Dtos;
using API_FFMS.Repositories;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace API_FFMS.Services;
public interface IInventoryCheckService : IBaseService
{
    Task<ApiResponse> Create(InventoryCheckCreateDto createDto);
    Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id);
    Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto);
    Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
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
        try
        {
            var rooms = await MainUnitOfWork.RoomRepository.FindAsync(
                new Expression<Func<Room, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => createDto.Rooms!.Select(x => x.RoomId).Contains(x.Id)
                }, null);

            var roomAssets = await MainUnitOfWork.RoomAssetRepository.FindAsync(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => rooms.Select(r => r!.Id).Contains(x.RoomId),
                    x => x.ToDate == null
                }, null);

            var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => roomAssets.Select(ra => ra!.AssetId).Contains(x.Id)
                }, null);

            var mediaFiles = createDto.RelatedFiles?.Select(file => new Report
            {
                FileName = file.FileName ?? "",
                Uri = file.Uri ?? ""
            }).ToList();

            var inventoryCheck = new InventoryCheck
            {
                RequestCode = GenerateRequestCode(),
                Description = createDto.Description,
                Notes = createDto.Notes,
                Priority = createDto.Priority,
                IsInternal = createDto.IsInternal,
                AssignedTo = createDto.AssignedTo
            };

            if (!await _repository.InsertInventoryCheck(inventoryCheck, rooms, mediaFiles, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id)
    {
        try
        {
            var inventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync<InventoryCheckDto>(
                    new Expression<Func<InventoryCheck, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.Id == id
                    });
            if (inventoryCheck == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
            }
            inventoryCheck.PriorityObj = inventoryCheck.Priority.GetValue();
            inventoryCheck.StatusObj = inventoryCheck.Status.GetValue();

            //Related file
            var relatedMediaFiles = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && !m.IsReported).FirstOrDefaultAsync();

            inventoryCheck.RelatedFiles = JsonConvert.DeserializeObject<List<MediaFileDetailDto>>(relatedMediaFiles.Uri);

            var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && m.IsReported).ToListAsync();
            
            //TODO: orderby
            inventoryCheck.Reports = new List<MediaFileDto>();
            foreach (var report in reports)
            {
                // Deserialize the URI string back into a List<string>
                var uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);
            
                inventoryCheck.Reports.Add(new MediaFileDto
                {
                    ItemId = report.ItemId,
                    Uri = uriList,
                    FileType = report.FileType,
                    Content = report.Content,
                    IsReject = report.IsReject,
                    RejectReason = report.RejectReason
                });
            }

            var userQuery = MainUnitOfWork.UserRepository.GetQuery().Where(x => x!.Id == inventoryCheck.AssignedTo);
            inventoryCheck.Staff = await userQuery.Select(x => new AssignedInventoryCheckDto
            {
                Id = x!.Id,
                UserCode = x.UserCode,
                Fullname = x.Fullname,
                RoleObj = x.Role.GetValue(),
                Avatar = x.Avatar,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address
            }).FirstOrDefaultAsync();

            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
            var roomAssetQuery = MainUnitOfWork.RoomAssetRepository.GetQuery();
            var roomStatusQuery = MainUnitOfWork.RoomStatusRepository.GetQuery();

            var inventoryCheckDetails = await MainUnitOfWork.InventoryCheckDetailRepository.GetQuery()
                                              .Include(x => x!.Asset)
                                              .Where(x => x!.InventoryCheckId == inventoryCheck.Id)
                                              .ToListAsync();

            var distinctRoomIds = inventoryCheckDetails.Select(detail => detail!.RoomId).Distinct();

            var rooms = await MainUnitOfWork.RoomRepository.GetQuery()
                            .Where(room => distinctRoomIds.Contains(room!.Id))
                            .ToListAsync();

            inventoryCheck.Rooms = distinctRoomIds.Select(roomId =>
            {
                var room = rooms.FirstOrDefault(r => r!.Id == roomId);
                if (room != null)
                {
                    return new RoomInventoryCheckDto
                    {
                        Id = room.Id,
                        RoomName = room.RoomName,
                        Area = room.Area,
                        RoomCode = room.RoomCode,
                        FloorId = room.FloorId,
                        StatusId = room.StatusId,
                        Status = roomStatusQuery.Where(x => x!.Id == room.StatusId).Select(x => new RoomStatusInvenDto
                        {
                            StatusName = x!.StatusName,
                            Description = x.Description,
                            Color = x.Color
                        }).FirstOrDefault(),
                        Assets = inventoryCheckDetails
                            .Where(detail => detail!.RoomId == roomId)
                            .Select(detail => new AssetInventoryCheckDto
                            {
                                Id = detail!.AssetId,
                                AssetName = detail.Asset!.AssetName,
                                AssetCode = detail.Asset.AssetCode,
                                QuantityBefore = detail.QuantityBefore,
                                StatusBefore = detail.StatusBefore,
                                StatusBeforeObj = detail.StatusBefore.GetValue(),
                                QuantityReported = detail.QuantityReported,
                                StatusReported = detail.StatusReported,
                                StatusReportedObj = detail.StatusReported.GetValue(),
                            }).ToList()
                    };
                }
                throw new ApiException("Lấy thông tin yêu cầu thất bại", StatusCode.NOT_FOUND);
            }).ToList();

            inventoryCheck = await _mapperRepository.MapCreator(inventoryCheck);
            return ApiResponse<InventoryCheckDto>.Success(inventoryCheck);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
    {
        var existinginventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(id);
        if (existinginventoryCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
        }

        if (existinginventoryCheck.Status != RequestStatus.NotStart)
        {
            throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.NOT_FOUND);
        }

        existinginventoryCheck.Description = updateDto.Description ?? existinginventoryCheck.Description;
        existinginventoryCheck.Notes = updateDto.Notes ?? existinginventoryCheck.Notes;
        existinginventoryCheck.Priority = updateDto.Priority ?? existinginventoryCheck.Priority;
        existinginventoryCheck.AssignedTo = updateDto.AssignedTo ?? existinginventoryCheck.AssignedTo;
        existinginventoryCheck.IsInternal = updateDto.IsInternal ?? existinginventoryCheck.IsInternal;

        var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(x => x!.ItemId == id).ToList();

        var newMediaFile = updateDto.RelatedFiles.Select(dto => new Report
        {
            FileName = dto.FileName,
            Uri = dto.Uri,
            CreatedAt = CurrentDate,
            CreatorId = AccountId,
            ItemId = id,
            FileType = FileType.File
        }).ToList() ?? new List<Report>();

        var additionMediaFiles = newMediaFile.Except(mediaFileQuery).ToList();
        var removalMediaFiles = mediaFileQuery.Except(newMediaFile).ToList();

        if (!await _repository.UpdateInventoryCheck(existinginventoryCheck, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
        {
            throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Cập nhật yêu cầu thông tin thành công");
    }

    public async Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto)
    {
        try
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

            if (queryDto.Priority != null)
            {
                inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.Priority == queryDto.Priority);
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

            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
            var roomAssetQuery = MainUnitOfWork.RoomAssetRepository.GetQuery();
            var roomStatusQuery = MainUnitOfWork.RoomStatusRepository.GetQuery();

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
                Rooms = i.InventoryCheckDetails!
                                    .GroupBy(x => x.RoomId)
                                    .Select(group => new RoomInventoryCheckDto
                                    {
                                        Id = group.Key,
                                        RoomName = roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.RoomName,
                                        Area = roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.Area,
                                        RoomCode = roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.RoomCode,
                                        FloorId = roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.FloorId,
                                        StatusId = roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.StatusId,
                                        Status = roomStatusQuery.Where(x => x!.Id == roomQuery.FirstOrDefault(r => r!.Id == group.Key)!.StatusId).Select(x => new RoomStatusInvenDto
                                        {
                                            StatusName = x!.StatusName,
                                            Description = x.Description,
                                            Color = x.Color
                                        }).FirstOrDefault(),
                                        Assets = group.Select(x => new AssetInventoryCheckDto
                                        {
                                            Id = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.AssetId,
                                            AssetCode = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Asset!.AssetCode,
                                            AssetName = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Asset!.AssetName,
                                            QuantityBefore = x.QuantityBefore,
                                            StatusBefore = x.StatusBefore,
                                            StatusBeforeObj = x.StatusBefore.GetValue(),
                                            QuantityReported = x.QuantityReported,
                                            StatusReported = x.StatusReported,
                                            StatusReportedObj = x.StatusReported.GetValue()
                                        }).ToList()
                                    }).ToList(),
                Staff = new AssignedInventoryCheckDto
                {
                    Id = i.User!.Id,
                    UserCode = i.User.UserCode,
                    Fullname = i.User.Fullname,
                    RoleObj = i.User.Role.GetValue(),
                    Avatar = i.User.Avatar,
                    Email = i.User.Email,
                    PhoneNumber = i.User.PhoneNumber,
                    Address = i.User.Address
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> Delete(Guid id)
    {
        var existingInventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync(
                new Expression<Func<InventoryCheck, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
        if (existingInventoryCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
        }

        if (existingInventoryCheck.Status != RequestStatus.Done &&
            existingInventoryCheck.Status != RequestStatus.NotStart &&
            existingInventoryCheck.Status != RequestStatus.Cancelled)
        {
            throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingInventoryCheck.Status?.GetDisplayName()}", StatusCode.NOT_FOUND);
        }

        if (!await MainUnitOfWork.InventoryCheckRepository.DeleteAsync(existingInventoryCheck, AccountId, CurrentDate))
        {
            throw new ApiException("Xóa yêu cầu thất bại", StatusCode.SERVER_ERROR);
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

    public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto)
    {
        var existingInventCheck = MainUnitOfWork.InventoryCheckRepository.GetQuery()
                                    .Where(x => x.Id == id)
                                    .FirstOrDefault();
        if (existingInventCheck == null)
        {
            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
        }

        existingInventCheck.Status = updateStatusDto.Status ?? existingInventCheck.Status;

        if (!await _repository.UpdateInventoryCheckStatus(existingInventCheck, updateStatusDto.Status, AccountId, CurrentDate))
        {
            throw new ApiException("Xác nhận yêu cầu thất bại", StatusCode.SERVER_ERROR);
        }

        return ApiResponse.Success("Xác nhận yêu cầu thành công");
    }
}