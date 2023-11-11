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
    //Task<ApiResponse> Create(InventoryCheckCreateDto createDto);
    //Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id);
    //Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto);

    Task<ApiResponse> Create(InventoryCheckCreateDto createDto);
    Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id);
    Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto);
    Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
    Task<ApiResponse> Delete(Guid id);
    //Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
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
                    x => createDto.RoomIds.Contains(x.Id)
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

            var inventoryCheck = new InventoryCheck
            {
                InventoryCheckConfigId = createDto.InventoryCheckConfigId,
                RequestCode = GenerateRequestCode(),
                Description = createDto.Description,
                Notes = createDto.Notes,
                Priority = createDto.Priority,
                IsInternal = createDto.IsInternal,
                AssignedTo = createDto.AssignedTo
            };

            if (!await _repository.InsertInventoryCheck(inventoryCheck, rooms, AccountId, CurrentDate))
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

            var inventoryCheckDetails = await MainUnitOfWork.InventoryCheckDetailRepository.GetQuery()
                                              .Include(x => x!.Asset)
                                              .Where(x => x!.InventoryCheckId == inventoryCheck.Id)
                                              .ToListAsync();

            var distinctRoomIds = inventoryCheckDetails.Select(detail => detail!.RoomId).Distinct();

            var rooms = await MainUnitOfWork.RoomRepository.GetQuery()
                             .Where(room => distinctRoomIds.Contains(room!.Id))
                             .ToListAsync();

            inventoryCheck.Rooms = distinctRoomIds.Select(roomId => new RoomInventoryCheckDto
            {
                Id = roomId,
                RoomName = rooms.FirstOrDefault(r => r!.Id == roomId)!.RoomName,
                Area = rooms.FirstOrDefault(r => r!.Id == roomId)!.Area,
                RoomCode = rooms.FirstOrDefault(r => r!.Id == roomId)!.RoomCode,
                FloorId = rooms.FirstOrDefault(r => r!.Id == roomId)!.FloorId,
                StatusId = rooms.FirstOrDefault(r => r!.Id == roomId)!.StatusId,
                Assets = inventoryCheckDetails
                    .Where(detail => detail!.RoomId == roomId)
                    .Select(detail => new AssetInventoryCheckDto
                    {
                        Id = detail!.AssetId,
                        AssetName = detail.Asset!.AssetName,
                        AssetCode = detail.Asset.AssetCode,
                        Status = inventoryCheck.Status != RequestStatus.Done ? detail.Asset.Status : detail.Status,
                        StatusObj = inventoryCheck.Status != RequestStatus.Done ? detail.Asset.Status.GetValue() : detail.Status.GetValue(),
                    }).ToList()

            })
            .ToList();

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
                                        Assets = group.Select(x => new AssetInventoryCheckDto
                                        {
                                            Id = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.AssetId,
                                            AssetCode = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Asset!.AssetCode,
                                            AssetName = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Asset!.AssetName,
                                            Quantity = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Quantity,
                                            Status = i.Status != RequestStatus.Done ? roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Status : x.Status,
                                            StatusObj = i.Status != RequestStatus.Done ? roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)!.Status.GetValue() : x.Status.GetValue()
                                        }).ToList()
                                    })
                                    .ToList(),
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

    //public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto)
    //{
    //    var existingInventoryCheck = MainUnitOfWork.InventoryCheckRepository.GetQuery()
    //                                    .Include(x => x.InventoryCheckDetails)
    //                                    .Where(x => x.Id == id)
    //                                    .FirstOrDefault();
    //    if (existingInventoryCheck == null)
    //    {
    //        throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
    //    }

    //    existingInventoryCheck.Status = updateStatusDto.Status ?? existingInventoryCheck.Status;


    //}

    //public async Task<ApiResponse> Create(InventoryCheckCreateDto createDto)
    //{
    //    try
    //    {
    //        var room = await MainUnitOfWork.RoomRepository.FindOneAsync(
    //                new Expression<Func<Room, bool>>[]
    //                {
    //                    x => !x!.DeletedAt.HasValue,
    //                    x => createDto.RoomId == x.Id
    //                });

    //        var roomAssets = await MainUnitOfWork.RoomAssetRepository.FindAsync(
    //                new Expression<Func<RoomAsset, bool>>[]
    //                {
    //                    x => !x.DeletedAt.HasValue,
    //                    x => x.RoomId == room!.Id,
    //                    x => x.ToDate == null
    //                }, null);

    //        var assets = await MainUnitOfWork.AssetRepository.FindAsync(
    //            new Expression<Func<Asset, bool>>[]
    //            {
    //                x => !x.DeletedAt.HasValue,
    //                x => roomAssets.Select(ra => ra!.AssetId).Contains(x.Id)
    //            }, null);

    //        var inventoryCheck = new InventoryCheck
    //        {
    //            InventoryCheckConfigId = createDto.InventoryCheckConfigId,
    //            RequestCode = GenerateRequestCode(),
    //            Description = createDto.Description,
    //            Notes = createDto.Notes,
    //            Priority = createDto.Priority,
    //            IsInternal = createDto.IsInternal,
    //            AssignedTo = createDto.AssignedTo
    //        };

    //        if (!await _repository.InsertInventoryCheck(inventoryCheck, assets, AccountId, CurrentDate))
    //        {
    //            throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
    //        }

    //        return ApiResponse.Created("Gửi yêu cầu thành công");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}


    //public async Task<ApiResponse<InventoryCheckDto>> GetInventoryCheck(Guid id)
    //{
    //    try
    //    {
    //        var existingInventoryCheck = await MainUnitOfWork.InventoryCheckRepository.FindOneAsync<InventoryCheckDto>(
    //                new Expression<Func<InventoryCheck, bool>>[]
    //                {
    //                    x => !x.DeletedAt.HasValue,
    //                    x => x.Id == id
    //                });

    //        if (existingInventoryCheck == null)
    //        {
    //            throw new ApiException("Không tìm thấy yêu cầu này", StatusCode.NOT_FOUND);
    //        }
    //        existingInventoryCheck.PriorityObj = existingInventoryCheck.Priority.GetValue();
    //        existingInventoryCheck.StatusObj = existingInventoryCheck.Status.GetValue();
    //        //var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery()
    //        //                                                       .Where(m => m!.ItemId == existingInventoryCheck.Id);
    //        //var mediaFile = new MediaFileDto
    //        //{
    //        //    FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
    //        //    Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
    //        //    Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
    //        //};

    //        var userQuery = MainUnitOfWork.UserRepository.GetQuery().Where(x => x!.Id == existingInventoryCheck.AssignedTo);
    //        existingInventoryCheck.Staff = await userQuery.Select(x => new AssignedInventoryCheckDto
    //        {
    //            Id = x!.Id,
    //            UserCode = x.UserCode,
    //            Fullname = x.Fullname,
    //            RoleObj = x.Role.GetValue(),
    //            Avatar = x.Avatar,
    //            Email = x.Email,
    //            PhoneNumber = x.PhoneNumber,
    //            Address = x.Address
    //        }).FirstOrDefaultAsync();

    //        var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
    //        var roomAssetQuery = MainUnitOfWork.RoomAssetRepository.GetQuery();

    //        var inventoryCheckDetails = MainUnitOfWork.InventoryCheckDetailRepository.GetQuery().Include(x => x!.Asset)
    //                                    .Where(x => x!.InventoryCheckId == existingInventoryCheck.Id &&
    //                                                x.RoomId == existingInventoryCheck.RoomId);

    //        existingInventoryCheck.AssetLocations = new AssetInventoryCheck
    //        {
    //            Room = inventoryCheckDetails.Select(x => new RoomInventoryCheckDto
    //            {
    //                Id = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.Id,
    //                RoomName = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.RoomName,
    //                Area = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.Area,
    //                RoomCode = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.RoomCode,
    //                FloorId = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.FloorId,
    //                StatusId = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.StatusId
    //            }).FirstOrDefault(),
    //            Assets = inventoryCheckDetails.Select(x => new AssetInventoryCheckDto
    //            {
    //                Id = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x!.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.Id)!.AssetId,
    //                AssetCode = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x!.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x.RoomId)!.Id)!.Asset!.AssetCode,
    //                AssetName = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x!.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x.RoomId)!.Id)!.Asset!.AssetName,
    //                //Quantity = ra.Quantity,
    //                Status = x!.Status,
    //                StatusObj = x.Status.GetValue()!
    //            }).ToList()
    //        };

    //        existingInventoryCheck = await _mapperRepository.MapCreator(existingInventoryCheck);
    //        return ApiResponse<InventoryCheckDto>.Success(existingInventoryCheck);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}

    //public async Task<ApiResponses<InventoryCheckDto>> GetInventoryChecks(InventoryCheckQueryDto queryDto)
    //{
    //    try
    //    {
    //        var keyword = queryDto.Keyword?.Trim().ToLower();
    //        var inventoryCheckQuery = MainUnitOfWork.InventoryCheckRepository.GetQuery()
    //                             .Where(x => !x!.DeletedAt.HasValue);

    //        if (queryDto.IsInternal != null)
    //        {
    //            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
    //        }

    //        if (queryDto.AssignedTo != null)
    //        {
    //            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
    //        }

    //        if (queryDto.Status != null)
    //        {
    //            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.Status == queryDto.Status);
    //        }

    //        if (!string.IsNullOrEmpty(keyword))
    //        {
    //            inventoryCheckQuery = inventoryCheckQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
    //                                                               || x.Notes!.ToLower().Contains(keyword) ||
    //                                                               x.RequestCode.ToLower().Contains(keyword));
    //        }

    //        inventoryCheckQuery = inventoryCheckQuery.OrderByDescending(x => x!.CreatedAt);

    //        var totalCount = await inventoryCheckQuery.CountAsync();
    //        inventoryCheckQuery = inventoryCheckQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

    //        var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
    //        var roomAssetQuery = MainUnitOfWork.RoomAssetRepository.GetQuery();

    //        var inventoryChecks = await inventoryCheckQuery.Select(i => new InventoryCheckDto
    //        {
    //            Id = i!.Id,
    //            RequestCode = i.RequestCode,
    //            RequestDate = i.RequestDate,
    //            CompletionDate = i.CompletionDate,
    //            Status = i.Status,
    //            StatusObj = i.Status!.GetValue(),
    //            Description = i.Description,
    //            Checkout = i.Checkout,
    //            Checkin = i.Checkout,
    //            Result = i.Result,
    //            Priority = i.Priority,
    //            PriorityObj = i.Priority.GetValue(),
    //            Notes = i.Notes,
    //            IsInternal = i.IsInternal,
    //            AssetLocations = new AssetInventoryCheck
    //            {
    //                Room = i.InventoryCheckDetails!.Select(x => new RoomInventoryCheckDto
    //                {
    //                    Id = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.Id,
    //                    RoomName = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.RoomName,
    //                    Area = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.Area,
    //                    RoomCode = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.RoomCode,
    //                    FloorId = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.FloorId,
    //                    StatusId = roomQuery.FirstOrDefault(r => r!.Id == x!.RoomId)!.StatusId
    //                }).FirstOrDefault(),
    //                Assets = i.InventoryCheckDetails!.Select(x => new AssetInventoryCheckDto
    //                {
    //                    Id = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x.RoomId)!.Id)!.AssetId,
    //                    AssetCode = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x.RoomId)!.Id)!.Asset!.AssetCode,
    //                    AssetName = roomAssetQuery.FirstOrDefault(ra => ra!.AssetId == x.AssetId && ra.RoomId == roomQuery.FirstOrDefault(r => r!.Id == x.RoomId)!.Id)!.Asset!.AssetName,
    //                    //Quantity = ra.Quantity,
    //                    Status = x.Status,
    //                    StatusObj = x.Status.GetValue()
    //                }).ToList()
    //            },
    //            Staff = new AssignedInventoryCheckDto
    //            {
    //                Id = i.User!.Id,
    //                UserCode = i.User.UserCode,
    //                Fullname = i.User.Fullname,
    //                RoleObj = i.User.Role.GetValue(),
    //                Avatar = i.User.Avatar,
    //                Email = i.User.Email,
    //                PhoneNumber = i.User.PhoneNumber,
    //                Address = i.User.Address
    //            }
    //        }).ToListAsync();

    //        inventoryChecks = await _mapperRepository.MapCreator(inventoryChecks);

    //        return ApiResponses<InventoryCheckDto>.Success(
    //                inventoryChecks,
    //                totalCount,
    //                queryDto.PageSize,
    //                queryDto.Page,
    //                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}



    //private RoomInventoryCheckDto MapRoomDto(IQueryable<Room> roomQuery, Guid roomId)
    //{
    //    return roomQuery
    //        .Where(r => r!.Id == roomId)
    //        .Select(r => new RoomInventoryCheckDto
    //        {
    //            Id = r.Id,
    //            RoomName = r.RoomName,
    //            Area = r.Area,
    //            RoomCode = r.RoomCode,
    //            FloorId = r.FloorId,
    //            StatusId = r.StatusId
    //        })
    //        .FirstOrDefault();
    //}

    //private AssetInventoryCheckDto MapAssetDto(IQueryable<RoomAsset>? roomAssetQuery, IQueryable<Room>? roomQuery, InventoryCheckDetail? x)
    //{
    //    return roomAssetQuery
    //        .Where(ra => ra!.AssetId == x.AssetId && ra.RoomId == x.RoomId)
    //        .Select(ra => new AssetInventoryCheckDto
    //        {
    //            Id = ra.AssetId,
    //            AssetCode = ra.Asset!.AssetCode,
    //            AssetName = ra.Asset!.AssetName,
    //            Status = x.Status,
    //            StatusObj = x.Status!.GetValue()
    //        })
    //        .FirstOrDefault();
    //}
}