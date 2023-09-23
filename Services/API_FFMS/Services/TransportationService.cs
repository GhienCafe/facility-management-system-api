using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_FFMS.Services
{
    public interface ITransportationService : IBaseService
    {
        Task<ApiResponses<TransportDto>> GetTransports(TransportQueryDto queryDto);
        public Task<ApiResponse> Create(TransportCreateDto createDto);
        public Task<ApiResponse> Update(Guid id, TransportUpdateDto updateDto);
        Task<ApiResponse<TransportDetailDto>> GetTransport(Guid id);
        Task<ApiResponse> Delete(Guid id);
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

            if(existingAsset.Status != AssetStatus.Operational)
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

            if (!existingAssignee.TeamId.Equals(existingAsset.Type.Category.TeamId))
            {
                throw new ApiException("Assigne have wrong major for this asset", StatusCode.BAD_REQUEST);
            }

            var transport = createDto.ProjectTo<TransportCreateDto, Transportation>();

            transport.Status = TransportationStatus.NotStarted;
            existingAsset.Status = AssetStatus.Pending;

            if (!await MainUnitOfWork.TransportationRepository.InsertAsync(transport, AccountId, CurrentDate))
            {
                throw new ApiException("Create failed", StatusCode.BAD_REQUEST);
            }

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
            existingTransport.Asset.Status = AssetStatus.Operational;

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

            if (queryDto.ScheduledDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.ScheduledDate == queryDto.ScheduledDate);
            }

            if (queryDto.ActualDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.ActualDate == queryDto.ActualDate);
            }

            if (queryDto.Status != null)
            {
                transportQuery = transportQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.AssignedTo != null)
            {
                var existingUser = MainUnitOfWork.UserRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue && x.Id == queryDto.AssignedTo);

                if (existingUser == null)
                {
                    throw new ApiException("Not found this user", StatusCode.NOT_FOUND);
                }

                transportQuery = transportQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                var existingAsset = MainUnitOfWork.AssetRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue && x.Id == queryDto.AssetId);

                if (existingAsset == null)
                {
                    throw new ApiException("Not found this asset", StatusCode.NOT_FOUND);
                }

                transportQuery = transportQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            if (queryDto.ToRoomId != null)
            {
                var existingRoom = MainUnitOfWork.RoomRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue && x.Id == queryDto.ToRoomId);

                if (existingRoom == null)
                {
                    throw new ApiException("Not found this room", StatusCode.NOT_FOUND);
                }

                transportQuery = transportQuery.Where(x => x!.ToRoomId == queryDto.ToRoomId);
            }

            var totalCount = transportQuery.Count();

            transportQuery = transportQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var transports = (await transportQuery.ToListAsync())!.ProjectTo<Transportation, TransportDto>();

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

            existingTransport.ScheduledDate = updateDto.ScheduledDate ?? existingTransport.ScheduledDate;
            existingTransport.ActualDate = updateDto.ActualDate ?? existingTransport.ActualDate;
            //transportation.Status = updateDto.Status ?? transportation.Status;
            existingTransport.AssetId = updateDto.AssetId ?? existingTransport.AssetId;
            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
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

            if (!existingAssignee.TeamId.Equals(existingAsset.Type.Category.TeamId))
            {
                throw new ApiException("Assigne have wrong major for this asset", StatusCode.BAD_REQUEST);
            }

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
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
