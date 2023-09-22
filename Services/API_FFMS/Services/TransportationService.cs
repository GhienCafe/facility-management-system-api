using API_FFMS.Dtos;
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
        public Task<ApiResponse> Create(TransportCreateDto createDto);
        public Task<ApiResponse> UpdateTransport(Guid transportId, TransportUpdateDto updateDto);
        public Task<ApiResponse> UpdateTransportDetail(Guid transportId, List<TransportDetailUpdateDto> updateDto);
        Task<ApiResponse<TransportationDto>> TransportTracking(Guid transportId);
    }
    public class TransportationService : BaseService, ITransportationService
    {
        public TransportationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Create(TransportCreateDto createDto)
        {
            var transportation = new Transportation
            {
                ScheduledDate = DateTime.UtcNow,
                ActualDate = createDto.ActualDate,
                Description = createDto.Description,
                AssignedTo = createDto.AssignedTo,
                Status = TransportationStatus.NotStarted
            };

            var transportationDetails = new List<TransportationDetail>();

            foreach (var assetInfo in createDto.Assets)
            {
                var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(assetInfo.AssetId);
                var sourceRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(assetInfo.SourceLocation);
                var destinationRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(assetInfo.DestinationLocation);
                if (asset == null || sourceRoom == null || destinationRoom == null)
                {
                    throw new ApiException("Room or Asset does not exist", StatusCode.BAD_REQUEST);
                }

                var transportationDetail = new TransportationDetail
                {
                    Transportation = transportation,
                    Asset = asset,
                    SourceLocation = sourceRoom.Id,
                    DestinationLocation = destinationRoom.Id,
                    IsDone = createDto.IsDone,
                    Description = assetInfo.Description ?? $"Transport {asset.AssetName} from {sourceRoom.RoomName} to {destinationRoom.RoomName}"
                };

                transportationDetails.Add(transportationDetail);
            }

            transportation.TransportationDetails = transportationDetails;

            if (!await MainUnitOfWork.TransportationRepository.InsertAsync(transportation, AccountId, CurrentDate))
            {
                throw new ApiException("Create failed", StatusCode.BAD_REQUEST);
            }

            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse<TransportationDto>> TransportTracking(Guid transportId)
        {
            var transportation = await MainUnitOfWork.TransportationRepository.FindOneAsync(transportId);

            if (transportation == null)
            {
                throw new ApiException("Transportation not found", StatusCode.NOT_FOUND);
            }

            var transportationDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery()
                                        .Where(td => td.TransportationId == transportId).ToList();

            var includedAssets = new List<AssetTransportDto>();

            foreach (var assetDetail in transportationDetails)
            {
                var asset = await MainUnitOfWork.AssetRepository.FindOneAsync((Guid)assetDetail.AssetId);
                if (asset != null)
                {
                    var assetDto = new AssetTransportDto
                    {
                        AssetCode = asset.AssetCode,
                        AssetName = asset.AssetName
                    };

                    includedAssets.Add(assetDto);
                }
            }
            var transportationDto = new TransportationDto
            {
                ScheduledDate = transportation.ScheduledDate,
                ActualDate = transportation.ActualDate,
                Description = transportation.Description,
                IncludedAssets = includedAssets
            };

            return ApiResponse<TransportationDto>.Success(transportationDto);
        }

        public async Task<ApiResponse> UpdateTransport(Guid transportId, TransportUpdateDto updateDto)
        {
            var transportation = await MainUnitOfWork.TransportationRepository.FindOneAsync(transportId);
            if (transportation == null)
            {
                throw new ApiException("Not found this transportation", StatusCode.NOT_FOUND);
            }

            if (transportation.Status == TransportationStatus.NotStarted)
            {
                transportation.ScheduledDate = updateDto.ScheduledDate ?? transportation.ScheduledDate;
                transportation.ActualDate = updateDto.ActualDate ?? transportation.ActualDate;
                transportation.Status = updateDto.Status ?? transportation.Status;
                transportation.Description = updateDto.Description ?? transportation.Description;
                transportation.AssignedTo = updateDto.AssignedTo ?? transportation.AssignedTo;
            }

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(transportation, AccountId, CurrentDate))
            {
                throw new ApiException("Update failed", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> UpdateTransportDetail(Guid transportId, List<TransportDetailUpdateDto> updateDtos)
        {

            var transportDetails = await MainUnitOfWork.TransportationDetailRepository.GetQuery()
                                        .Where(x => !x!.DeletedAt.HasValue && x.TransportationId == transportId)
                                        .ToListAsync();


            var assets = new List<TransportationDetail>();

            foreach(var item in updateDtos)
            {
                var assetTransport = transportDetails.FirstOrDefault(td => td.AssetId == item.AssetId);
                if (assetTransport != null)
                {
                    assetTransport.AssetId = item.AssetId ?? assetTransport.AssetId;
                    assetTransport.SourceLocation = item.SourceLocation ?? assetTransport.SourceLocation;
                    assetTransport.DestinationLocation = item.DestinationLocation ?? assetTransport.DestinationLocation;
                    assetTransport.Description = item.Description ?? assetTransport.Description;

                    assets.Add(assetTransport);
                }
            }

            if(!await MainUnitOfWork.TransportationDetailRepository.UpdateAsync(assets, AccountId, CurrentDate))
            {
                throw new ApiException("Update fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}
