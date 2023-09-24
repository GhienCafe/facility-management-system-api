using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Notification = MainData.Entities.Notification;

namespace API_FFMS.Services
{
    public interface ITransportationService : IBaseService
    {
        Task<ApiResponses<TransportDto>> GetTransports(TransportQueryDto queryDto);
        public Task<ApiResponse> Create(TransportCreateDto createDto);
        public Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto);
        Task<ApiResponse<TransportDetailDto>> GetTransport(Guid id);
        Task<ApiResponse> Delete(Guid id);
        public Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateStatusDto);
    }
    public class TransportationService : BaseService, ITransportationService
    {
        public TransportationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
            IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(TransportCreateDto createDto)
        {
            var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == createDto.AssetId
            });

            if (existingAsset == null)
            {
                throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
            }

            if (existingAsset.Status != AssetStatus.Operational)
            {
                throw new ApiException("This asset is in another request", StatusCode.NOT_ACTIVE);
            }

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

            // if (!existingAssignee.TeamId.Equals(existingAsset.Type!.Category!.TeamId))
            // {
            //     throw new ApiException("Assign have wrong major for this asset", StatusCode.BAD_REQUEST);
            // }

            var transport = createDto.ProjectTo<TransportCreateDto, Transportation>();
            transport.Id = Guid.NewGuid();

            transport.Status = TransportationStatus.NotStarted;
            existingAsset.Status = AssetStatus.Pending;

            if (!await MainUnitOfWork.TransportationRepository.InsertAsync(transport, AccountId, CurrentDate))
            {
                throw new ApiException("Create failed", StatusCode.BAD_REQUEST);
            }

            var notification = new Notification
            {
                UserId = transport.AssignedTo,
                Status = NotificationStatus.Waiting,
                Content = "Di dời " + existingAsset.AssetName,
                Title = "Di dời trang thiết bị",
                Type = NotificationType.Task,
                IsRead = false,
                ItemId = transport.Id,
                ShortContent = "Di dời trang thiết bị"
            };

            if (!await MainUnitOfWork.NotificationRepository.InsertAsync(notification, AccountId, CurrentDate))
                throw new ApiException("Fail to create notification", StatusCode.SERVER_ERROR);

            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(id);

            if (existingTransport == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            existingTransport.Status = TransportationStatus.Cancelled;
            existingTransport.Asset!.Status = AssetStatus.Operational;

            if (!await MainUnitOfWork.TransportationRepository.DeleteAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Delete fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TransportDetailDto>> GetTransport(Guid id)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync<TransportDetailDto>(
            new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue,
                x => x.Id == id
            });

            if (existingTransport == null)
            {
                throw new ApiException("Transportation not found", StatusCode.NOT_FOUND);
            }

            existingTransport = await _mapperRepository.MapCreator(existingTransport);
            return ApiResponse<TransportDetailDto>.Success(existingTransport);
        }

        public async Task<ApiResponses<TransportDto>> GetTransports(TransportQueryDto queryDto)
        {
            var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue);

            if (queryDto.RequestedDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.RequestedDate == queryDto.RequestedDate);
            }

            if (queryDto.CompletionDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            }

            if (queryDto.Status != null)
            {
                transportQuery = transportQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.AssignedTo != null)
            {
                transportQuery = transportQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                transportQuery = transportQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            if (queryDto.ToRoomId != null)
            {
                transportQuery = transportQuery.Where(x => x!.ToRoomId == queryDto.ToRoomId);
            }

            var joinTables = from transport in transportQuery
                             join room in MainUnitOfWork.RoomRepository.GetQuery() on transport.ToRoomId equals room.Id into roomGroup
                             from room in roomGroup.DefaultIfEmpty()
                             join asset in MainUnitOfWork.AssetRepository.GetQuery() on transport.AssetId equals asset.Id into assetGroup
                             from asset in assetGroup.DefaultIfEmpty()
                             join personInCharge in MainUnitOfWork.UserRepository.GetQuery() on transport.AssignedTo equals personInCharge.Id into personInChargeGroup
                             from personInCharge in personInChargeGroup.DefaultIfEmpty()
                             select new
                             {
                                 Transport = transport,
                                 Room = room,
                                 Asset = asset,
                                 PersonInCharge = personInCharge
                             };

            var totalCount = joinTables.Count();

            joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);
            //transportQuery = transportQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            //var transports = (await transportQuery.ToListAsync())!.ProjectTo<Transportation, TransportDto>();
            var transports = await joinTables.Select(
                x => new TransportDto
                {
                    AssignedTo = x.Transport.AssignedTo,
                    ToRoomId = x.Transport.ToRoomId,
                    Status = x.Transport.Status.GetValue(),
                    Id = x.Transport.Id,
                    CompletionDate = x.Transport.CompletionDate,
                    CreatedAt = x.Transport.CreatedAt,
                    EditedAt = x.Transport.EditedAt,
                    CreatorId = x.Transport.CreatorId ?? Guid.Empty,
                    EditorId = x.Transport.EditorId ?? Guid.Empty,
                    AssetId = x.Transport.AssetId,
                    RequestedDate = x.Transport.RequestedDate,
                    Description = x.Transport.Description,
                    Note = x.Transport.Note,
                    Quantity = x.Transport.Quantity,
                    Asset = x.Asset.ProjectTo<Asset, AssetDto>(),
                    PersonInCharge = x.PersonInCharge.ProjectTo<User, UserDto>(),
                    ToRoom = x.Room.ProjectTo<Room, RoomDto>()
                }
            ).ToListAsync();


            transports = await _mapperRepository.MapCreator(transports);

            return ApiResponses<TransportDto>.Success(
                   transports,
                   totalCount,
                   queryDto.PageSize,
                   queryDto.Skip(),
                   (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == id
                && x.Status == TransportationStatus.NotStarted
            });
            if (existingTransport == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            existingTransport.RequestedDate = updateDto.RequestedDate ?? existingTransport.RequestedDate;
            existingTransport.CompletionDate = updateDto.CompletionDate ?? existingTransport.CompletionDate;
            //transportation.Status = updateDto.Status ?? transportation.Status;
            existingTransport.AssetId = updateDto.AssetId ?? existingTransport.AssetId;
            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
            existingTransport.Note = updateDto.Note ?? existingTransport.Note;
            existingTransport.Quantity = updateDto.Quantity ?? existingTransport.Quantity;
            existingTransport.AssignedTo = updateDto.AssignedTo ?? existingTransport.AssignedTo;

            var existingAsset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == updateDto.AssetId
                && x.Status == AssetStatus.Operational
            });
            if (existingAsset == null)
            {
                throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
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

            if (!existingAssignee.TeamId.Equals(existingAsset.Type!.Category!.TeamId))
            {
                throw new ApiException("Assign have wrong major for this asset", StatusCode.BAD_REQUEST);
            }

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateStatusDto)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(new Expression<Func<Transportation, bool>>[]
            {
                x => !x.DeletedAt.HasValue
                && x.Id == id
            });
            if (existingTransport == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            var userAuthen = await MainUnitOfWork.UserRepository.FindOneAsync(new Expression<Func<User, bool>>[]
            {
                x => !x.DeletedAt.HasValue && x.Id == AccountId
            });
            if (!existingTransport.AssignedTo.Equals(AccountId) || userAuthen!.Role != UserRole.Manager)
            {
                throw new ApiException("This account doesn't have permission for this", StatusCode.UNAUTHORIZED);
            }

            if(existingTransport.Status == TransportationStatus.Cancelled)
            {
                throw new ApiException("Can not update request was cancelled", StatusCode.FORBIDDEN);
            }

            existingTransport.Status = updateStatusDto.Status;

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Update status failed", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
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
