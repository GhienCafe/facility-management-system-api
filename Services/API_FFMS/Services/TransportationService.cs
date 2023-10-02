using API_FFMS.Dtos;
using AppCore.Data;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface ITransportationService : IBaseService
    {
        Task<ApiResponses<TransportDto>> GetTransports(TransportQueryDto queryDto);
        public Task<ApiResponse> Create(TransportCreateDto createDto);
        public Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto);
        Task<ApiResponse<TransportRequestDto>> GetTransport(Guid id);
        Task<ApiResponse> Delete(Guid id);
        public Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateStatusDto);
    }
    public class TransportationService : BaseService, ITransportationService
    {
        private readonly ITransportationRepository _transportRepository;
        public TransportationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository, ITransportationRepository transportRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _transportRepository = transportRepository;
        }

        public async Task<ApiResponse> Create(TransportCreateDto createDto)
        {
            var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            {
                 x => !x.DeletedAt.HasValue && x.Id == createDto.AssignedTo
            });
            if (existingAssignee == null)
            {
                throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
            }

            var existingRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(new Expression<Func<Room, bool>>[]
            {
                 x => !x.DeletedAt.HasValue && x.Id == createDto.ToRoomId
            });
            if (existingRoom == null)
            {
                throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
            }

            var request = new ActionRequest
            {
                Id = Guid.NewGuid(),
                RequestCode = createDto.RequestCode,
                RequestDate = createDto.RequestedDate,
                CompletionDate = createDto.CompletionDate,
                RequestType = RequestType.Transportation,
                RequestStatus = RequestStatus.InProgress,
                Description = createDto.Description,
                Notes = createDto.Notes,
                IsInternal = createDto.IsInternal,
                AssignedTo = createDto.AssignedTo,
            };

            var transports = new List<Transportation>();
            foreach (var assetId in createDto.AssetId)
            {
                var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
                {
                     x => !x.DeletedAt.HasValue
                     && x.Id == assetId
                });
                if (existingAsset == null)
                {
                    throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
                }

                if (existingAsset.Status != AssetStatus.Operational)
                {
                    throw new ApiException("Some asset is in another request", StatusCode.NOT_ACTIVE);
                }

                
                var transport = new Transportation
                {
                    Id = Guid.NewGuid(),
                    Quantity = createDto.Quantity,
                    ToRoomId = createDto.ToRoomId,
                    RequestId = request.Id,
                    AssetId = assetId
                };
                transports.Add(transport);
                existingAsset.Status = AssetStatus.Transportation;
            }
            request.Transportations = transports;

            if (!await _transportRepository.InsertTransport(request, AccountId, CurrentDate))
            {
                throw new ApiException("Create failed", StatusCode.BAD_REQUEST);
            }
            // transport.Id = Guid.NewGuid();
            //
            // transport.Status = ActionStatus.NotStarted;
            // existingAsset.Status = AssetStatus.Pending;
            //
            // if (!await MainUnitOfWork.TransportationRepository.InsertAsync(transport, AccountId, CurrentDate))
            // {
            //     throw new ApiException("Create failed", StatusCode.BAD_REQUEST);
            // }
            //
            // var notification = new Notification
            // {
            //     UserId = transport.AssignedTo,
            //     Status = NotificationStatus.Waiting,
            //     Content = "Di dời " + existingAsset.AssetName,
            //     Title = "Di dời trang thiết bị",
            //     Type = NotificationType.Task,
            //     IsRead = false,
            //     ItemId = transport.Id,
            //     ShortContent = "Di dời trang thiết bị"
            // };
            //
            // if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
            //     throw new ApiException("Fail to create notification", StatusCode.SERVER_ERROR);
            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingTransport = await MainUnitOfWork.RequestRepository.FindOneAsync(
                new Expression<Func<ActionRequest, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id,
                    x => x.RequestType == RequestType.Transportation
                });

            if (existingTransport == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            existingTransport.RequestStatus = RequestStatus.Cancelled;
            var assetTransport = existingTransport.Transportations!.Select(x => x.Asset).ToList();
            foreach( var asset in assetTransport)
            {
                asset!.Status = AssetStatus.Operational;
            }

            if (!await MainUnitOfWork.RequestRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TransportRequestDto>> GetTransport(Guid id)
        {
            //var existingTransport = await MainUnitOfWork.RequestRepository.FindOneAsync<TransportDetailDto>(
            //new Expression<Func<ActionRequest, bool>>[]
            //{
            //    x => !x.DeletedAt.HasValue,
            //    x => x.Id == id,
            //    x => x.RequestType == RequestType.Transportation
            //});

            var request = await MainUnitOfWork.RequestRepository.GetQuery()
                            .Where(x => !x!.DeletedAt.HasValue && x.Id == id && x.RequestType == RequestType.Transportation)
                            .Select(x => new TransportRequestDto
                            {
                                Id = x!.Id,
                                RequestCode = x.RequestCode,
                                RequestedDate = x.RequestDate,
                                CompletionDate = x.CompletionDate,
                                Description = x.Description,
                                Notes = x.Notes,
                                RequestType = x.RequestType,
                                RequestStatus = x.RequestStatus,
                                IsInternal = x.IsInternal,
                                AssignedTo = x.AssignedTo,
                                CreatedAt = x.CreatedAt,
                                EditedAt = x.EditedAt,
                                CreatorId = x.CreatorId ?? Guid.Empty,
                                EditorId = x.EditorId ?? Guid.Empty,
                            }).FirstOrDefaultAsync();
            if (request == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển này", StatusCode.NOT_FOUND);
            }

            request.PersonInCharge = await MainUnitOfWork.UserRepository.GetQuery()
                                     .Where(u => u!.Id == request.AssignedTo)
                                     .Select(u => new AssigneeTransportDto
                                     {
                                         Fullname = u.Fullname
                                     }).FirstOrDefaultAsync();

            var transports = await MainUnitOfWork.TransportationRepository.GetQuery()
                            .Where(t => t!.RequestId == id)
                            .ToListAsync();

            var assetIds = transports.Select(t => t!.AssetId).ToList();
            request.TransportDetail!.Assets = await MainUnitOfWork.AssetRepository.GetQuery()
                                                .Where(a => assetIds.Equals(a.Id))
                                                .Select(asset => new AssetTransportDto
                                                {
                                                    AssetName = asset!.AssetName,
                                                    AssetCode = asset.AssetCode,
                                                    Status = asset.Status,
                                                    ManufacturingYear = asset.ManufacturingYear,
                                                    SerialNumber = asset.SerialNumber,
                                                    Quantity = asset.Quantity,
                                                    Description = asset.Description
                                                }).ToListAsync();

            var toRoomId = transports.FirstOrDefault()?.ToRoomId;
            request.TransportDetail.ToRoom = await MainUnitOfWork.RoomRepository.GetQuery()
                                                .Where(r => r!.Id == toRoomId)
                                                .Select(r => new RoomTransportDto
                                                {
                                                    RoomName = r!.RoomName,
                                                    RoomCode = r.RoomCode
                                                }).FirstOrDefaultAsync();

            //existingTransport = await _mapperRepository.MapCreator(existingTransport);
            return ApiResponse<TransportRequestDto>.Success(request);
        }

        public async Task<ApiResponses<TransportDto>> GetTransports(TransportQueryDto queryDto)
        {
            // var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
            //                      .Where(x => !x!.DeletedAt.HasValue);
            //
            // if (queryDto.RequestedDate != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.RequestedDate == queryDto.RequestedDate);
            // }
            //
            // if (queryDto.CompletionDate != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            // }
            //
            // if (queryDto.Status != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.Status == queryDto.Status);
            // }
            //
            // if (queryDto.AssignedTo != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            // }
            //
            // if (queryDto.AssetId != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.AssetId == queryDto.AssetId);
            // }
            //
            // if (queryDto.ToRoomId != null)
            // {
            //     transportQuery = transportQuery.Where(x => x!.ToRoomId == queryDto.ToRoomId);
            // }
            //
            // var joinTables = from transport in transportQuery
            //                  join room in MainUnitOfWork.RoomRepository.GetQuery() on transport.ToRoomId equals room.Id into roomGroup
            //                  from room in roomGroup.DefaultIfEmpty()
            //                  join asset in MainUnitOfWork.AssetRepository.GetQuery() on transport.AssetId equals asset.Id into assetGroup
            //                  from asset in assetGroup.DefaultIfEmpty()
            //                  join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on transport.AssignedTo equals personInCharge.Id into personInChargeGroup
            //                  from personInCharge in personInChargeGroup.DefaultIfEmpty()
            //                  select new
            //                  {
            //                      Transport = transport,
            //                      Room = room,
            //                      Asset = asset,
            //                      PersonInCharge = personInCharge
            //                  };
            //
            // var totalCount = joinTables.Count();
            //
            // joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);
            // //transportQuery = transportQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);
            //
            // //var transports = (await transportQuery.ToListAsync())!.ProjectTo<Transportation, TransportDto>();
            // var transports = await joinTables.Select(
            //     x => new TransportDto
            //     {
            //         AssignedTo = x.Transport.AssignedTo,
            //         ToRoomId = x.Transport.ToRoomId,
            //         Status = x.Transport.Status.GetValue(),
            //         Id = x.Transport.Id,
            //         CompletionDate = x.Transport.CompletionDate,
            //         CreatedAt = x.Transport.CreatedAt,
            //         EditedAt = x.Transport.EditedAt,
            //         CreatorId = x.Transport.CreatorId ?? Guid.Empty,
            //         EditorId = x.Transport.EditorId ?? Guid.Empty,
            //         AssetId = x.Transport.AssetId,
            //         RequestedDate = x.Transport.RequestedDate,
            //         Description = x.Transport.Description,
            //         Note = x.Transport.Note,
            //         Quantity = x.Transport.Quantity,
            //         Asset = x.Asset.ProjectTo<Asset, AssetDto>(),
            //         PersonInCharge = x.PersonInCharge.ProjectTo<User, UserDto>(),
            //         ToRoom = x.Room.ProjectTo<Room, RoomDto>()
            //     }
            // ).ToListAsync();
            //
            //
            // transports = await _mapperRepository.MapCreator(transports);
            //
            // return ApiResponses<TransportDto>.Success(
            //        transports,
            //        totalCount,
            //        queryDto.PageSize,
            //        queryDto.Page,
            //        (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
            throw new ApiException("");
        }

        public async Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto)
        {
            var existingTransport = await MainUnitOfWork.RequestRepository.FindOneAsync(new Expression<Func<ActionRequest, bool>>[]
            {
                 x => !x.DeletedAt.HasValue
                 && x.Id == id
                 && x.RequestStatus == RequestStatus.NotStarted
                 && x.RequestType == RequestType.Transportation
            });
            if (existingTransport == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            existingTransport.RequestDate = updateDto.RequestDate ?? existingTransport.RequestDate;
            existingTransport.CompletionDate = updateDto.CompletionDate ?? existingTransport.CompletionDate;
            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
            existingTransport.Notes = updateDto.Notes ?? existingTransport.Notes;
            existingTransport.AssignedTo = updateDto.AssignedTo ?? existingTransport.AssignedTo;

            var existingAssets = existingTransport.Transportations!.Select(ea => ea.Asset!.Id).ToList();

            var newAssetIds = updateDto.AssetId ?? new List<Guid>();
            var additionAssets = newAssetIds.Except(existingAssets).ToList();
            var removalAssets = existingAssets.Except(newAssetIds).ToList();

            foreach (var assetIdToAdd in additionAssets)
            {
                existingTransport.Transportations.Add(new Transportation
                {
                    RequestId = id,
                    AssetId = assetIdToAdd
                });
            }

            foreach (var assetIdToRemove in removalAssets)
            {
                var assetToRemove = existingTransport.Transportations!.SingleOrDefault(ra => ra.AssetId == assetIdToRemove);
                if (assetToRemove != null)
                {
                    existingTransport.Transportations.Remove(assetToRemove);
                }
            }

            var existingAssignee = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            {
                 x => !x.DeletedAt.HasValue && x.Id == updateDto.AssignedTo
            });
            if (existingAssignee == null)
            {
                throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
            }

            var existingRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(new Expression<Func<Room, bool>>[]
            {
                 x => !x.DeletedAt.HasValue && x.Id == updateDto.ToRoomId
            });
            if (existingRoom == null)
            {
                throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.RequestRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateStatusDto)
        {
            // var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(new Expression<Func<Transportation, bool>>[]
            // {
            //     x => !x.DeletedAt.HasValue
            //     && x.Id == id
            // });
            // if (existingTransport == null)
            // {
            //     throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            // }
            //
            // var userAuthen = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            // {
            //     x => !x.DeletedAt.HasValue && x.Id == AccountId
            // });
            // if (!existingTransport.AssignedTo.Equals(AccountId) || userAuthen!.Role != UserRole.Manager)
            // {
            //     throw new ApiException("This account doesn't have permission for this", StatusCode.UNAUTHORIZED);
            // }
            //
            // if (existingTransport.Status == ActionStatus.Cancelled)
            // {
            //     throw new ApiException("Can not update request was cancelled", StatusCode.FORBIDDEN);
            // }
            //
            // existingTransport.Status = updateStatusDto.Status;
            //
            // var assetInclude = await MainUnitOfWork.AssetRepository.FindOneAsync((Guid)existingTransport.AssetId!);
            //
            // if (updateStatusDto.Status == ActionStatus.Cancelled || updateStatusDto.Status == ActionStatus.Completed)
            // {
            //     assetInclude!.Status = AssetStatus.Operational;
            // }
            // else if (updateStatusDto.Status == ActionStatus.NotStarted || updateStatusDto.Status == ActionStatus.InProgress)
            // {
            //     assetInclude!.Status = AssetStatus.Pending;
            // }
            //
            // if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            // {
            //     throw new ApiException("Update status failed", StatusCode.SERVER_ERROR);
            // }
            //
            // return ApiResponse.Success();
            throw new ApiException("");
        }

        //public async Task<ApiResponse> UpdateTransportDetail(Guid transportId, TransportDetailUpdateDto updateDtos)
        //{
        //    var transportation = await MainUnitOfWork.TransportationRepository.FindOneAsync(transportId);
        //    if (transportation == null)
        //    {
        //        throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
        //    }

        //    if (updateDtos.AddMoreAssetId != null && updateDtos.AddMoreAssetId.Count > 0 && transportation.Status.Equals(TransportationStatus.NotStarted))
        //    {
        //        foreach (var assetId in updateDtos.AddMoreAssetId)
        //        {

        //            var assetToAdd = await MainUnitOfWork.AssetRepository.FindOneAsync(assetId);
        //            if (assetToAdd == null)
        //            {
        //                throw new ApiException($"Asset with ID {assetId} not found", StatusCode.NOT_FOUND);
        //            }

        //            // Add the asset directly to the transportation's assets collection
        //            transportation.Asset.Add(assetToAdd);
        //        }
        //    }

        //    if (updateDtos.RemoveAssetId != null && updateDtos.RemoveAssetId.Count > 0)
        //    {
        //        foreach (var assetId in updateDtos.RemoveAssetId)
        //        {
        //            // Assuming you have a method to retrieve an asset by its ID
        //            var assetToRemove = transportation.Asset.
        //            if (assetToRemove != null)
        //            {
        //                transportation.Assets.Remove(assetToRemove);
        //            }
        //        }
        //    }

        //    //if(!await MainUnitOfWork.TransportationDetailRepository.UpdateAsync(assets, AccountId, CurrentDate))
        //    //{
        //    //    throw new ApiException("Update fail", StatusCode.SERVER_ERROR);
        //    //}

        //    return ApiResponse.Success();
        //}
    }
}
