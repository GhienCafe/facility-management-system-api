using API_FFMS.Dtos;
using API_FFMS.Repositories;
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
        Task<ApiResponses<TransportDto>> GetTransports(TransportationQueryDto queryDto);
        Task<ApiResponse> Create(TransportCreateDto createDto);
        Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto);
        Task<ApiResponse<TransportDto>> GetTransportation(Guid id);
        //Task<ApiResponses<TransportDto>> GetTransportOfStaff(TransportOfStaffQueryDto queryDto);
        Task<ApiResponse> Delete(Guid id);
        public Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto updateStatusDto);
    }
    public class TransportationService : BaseService, ITransportationService
    {
        private readonly ITransportationRepository _transportationRepository;
        public TransportationService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor,
                                    IMapperRepository mapperRepository, ITransportationRepository transportationRepository)
                                    : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
            _transportationRepository = transportationRepository;
        }

        public async Task<ApiResponse> Create(TransportCreateDto createDto)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(createDto.AssetId);
            if (asset == null)
            {
                throw new ApiException("Không tìm thấy trang thiết bị", StatusCode.NOT_FOUND);
            }

            if (asset.Status != AssetStatus.Operational)
            {
                throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
            }

            //var teams = MainUnitOfWork.TeamMemberRepository.GetQuery();
            //var assignee = await MainUnitOfWork.TeamMemberRepository.FindOneAsync(
            //    new Expression<Func<TeamMember, bool>>[]
            //    {
            //        x => !x.DeletedAt.HasValue,
            //        x => x.MemberId == createDto.AssignedTo,
            //        x => x.TeamId == createDto.AssignedTo
            //    });
            //if (assignee == null)
            //{
            //    throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.BAD_REQUEST);
            //}

            var transportation = createDto.ProjectTo<TransportCreateDto, Transportation>();

            if (!await _transportationRepository.InsertTransportation(transportation, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Gửi yêu cầu thành công");
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (existingTransport == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển này", StatusCode.NOT_FOUND);
            }

            existingTransport.Status = RequestStatus.Cancelled;

            if (!await MainUnitOfWork.TransportationRepository.DeleteAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }
        public async Task<ApiResponse<TransportDto>> GetTransportation(Guid id)
        {
            var transportation = await MainUnitOfWork.TransportationRepository.FindOneAsync<TransportDto>(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (transportation == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển", StatusCode.NOT_FOUND);
            }

            transportation.Asset = await MainUnitOfWork.AssetRepository.FindOneAsync<AssetBaseDto>(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == transportation.AssetId
                });

            if (transportation.Asset != null)
            {
                transportation.Asset!.StatusObj = transportation.Asset.Status?.GetValue();
                var roomAsset = await MainUnitOfWork.RoomAssetRepository.FindOneAsync<RoomAsset>(
                            new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.AssetId == transportation.Asset.Id,
                    x => x.ToDate == null
                });

                if (roomAsset != null)
                {
                    transportation.FromRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                        new Expression<Func<Room, bool>>[]
                        {
                            x => !x.DeletedAt.HasValue,
                            x => x.Id == roomAsset.RoomId
                        });
                }
            }

            transportation.ToRoom = await MainUnitOfWork.RoomRepository.FindOneAsync<RoomBaseDto>(
                new Expression<Func<Room, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == transportation.ToRoomId
                });

            transportation = await _mapperRepository.MapCreator(transportation);

            return ApiResponse<TransportDto>.Success(transportation);
        }

        public async Task<ApiResponses<TransportDto>> GetTransports(TransportationQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue);

            if (keyword != null)
            {
                transportQuery = transportQuery.Where(x => x!.RequestCode.ToLower().Contains(keyword));
            }

            if (queryDto.AssignedTo != null)
            {
                transportQuery = transportQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.AssetId != null)
            {
                transportQuery = transportQuery.Where(x => x!.AssetId == queryDto.AssetId);
            }

            if (queryDto.Status != null)
            {
                transportQuery = transportQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (queryDto.RequestDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.RequestDate == queryDto.RequestDate);
            }

            if (queryDto.CompletionDate != null)
            {
                transportQuery = transportQuery.Where(x => x!.CompletionDate == queryDto.CompletionDate);
            }

            var roomAssets = MainUnitOfWork.RoomAssetRepository.GetQuery();
            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();

            var joinTables = from transport in transportQuery
                             join user in MainUnitOfWork.UserRepository.GetQuery() on transport.AssignedTo equals user.Id
                             join asset in MainUnitOfWork.AssetRepository.GetQuery() on transport.AssetId equals asset.Id

                             join roomAsset in roomAssets on transport.AssetId equals roomAsset.AssetId
                             join fromRoom in roomDataset on roomAsset.RoomId equals fromRoom.Id

                             join toRoom in roomDataset on transport.ToRoomId equals toRoom.Id
                             select new
                             {
                                 Transportation = transport,
                                 Asset = asset,
                                 User = user,
                                 ToRoom = toRoom,
                                 FromRoom = fromRoom
                             };

            var totalCount = await joinTables.CountAsync();
            joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var transportations = await joinTables.Select(x => new TransportDto
            {
                Id = x.Transportation.Id,
                RequestCode = x.Transportation.RequestCode,
                RequestDate = x.Transportation.RequestDate,
                CompletionDate = x.Transportation.CompletionDate,
                Status = x.Transportation.Status,
                Description = x.Transportation.Description,
                Notes = x.Transportation.Notes,
                IsInternal = x.Transportation.IsInternal,
                AssignedTo = x.Transportation.AssignedTo,
                AssetId = x.Transportation.AssetId,
                CreatedAt = x.Transportation.CreatedAt,
                EditedAt = x.Transportation.EditedAt,
                CreatorId = x.Transportation.CreatorId ?? Guid.Empty,
                EditorId = x.Transportation.EditorId ?? Guid.Empty,
                Asset = new AssetBaseDto
                {
                    Id = x.Asset.Id,
                    AssetName = x.Asset.AssetName,
                    AssetCode = x.Asset.AssetCode,
                    IsMovable = x.Asset.IsMovable,
                    Status = x.Asset.Status,
                    StatusObj = x.Asset.Status.GetValue(),
                    ManufacturingYear = x.Asset.ManufacturingYear,
                    SerialNumber = x.Asset.SerialNumber,
                    Quantity = x.Asset.Quantity,
                    Description = x.Asset.Description,
                    LastCheckedDate = x.Asset.LastCheckedDate,
                    LastMaintenanceTime = x.Asset.LastMaintenanceTime,
                    TypeId = x.Asset.TypeId,
                    ModelId = x.Asset.ModelId,
                    IsRented = x.Asset.IsRented,
                    StartDateOfUse = x.Asset.StartDateOfUse
                },
                ToRoom = new RoomBaseDto
                {
                    Id = x.ToRoom.Id,
                    RoomCode = x.ToRoom.RoomCode,
                    RoomName = x.ToRoom.RoomName,
                    StatusId = x.ToRoom.StatusId,
                    FloorId = x.ToRoom.FloorId,
                    CreatedAt = x.ToRoom.CreatedAt,
                    EditedAt = x.ToRoom.EditedAt
                },
                FromRoom = new RoomBaseDto
                {
                    Id = x.FromRoom.Id,
                    RoomCode = x.FromRoom.RoomCode,
                    RoomName = x.FromRoom.RoomName,
                    StatusId = x.FromRoom.StatusId,
                    FloorId = x.FromRoom.FloorId,
                    CreatedAt = x.FromRoom.CreatedAt,
                    EditedAt = x.FromRoom.EditedAt
                }
            }).ToListAsync();

            transportations = await _mapperRepository.MapCreator(transportations);

            return ApiResponses<TransportDto>.Success(
                transportations,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Update(Guid id, BaseRequestUpdateDto updateDto)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(id);
            if (existingTransport == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển này", StatusCode.NOT_FOUND);
            }

            if(existingTransport.Status != RequestStatus.NotStarted)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            existingTransport.RequestCode = updateDto.RequestCode ?? existingTransport.RequestCode;
            existingTransport.RequestDate = updateDto.RequestDate ?? existingTransport.RequestDate;
            existingTransport.CompletionDate = updateDto.CompletionDate ?? existingTransport.CompletionDate;
            existingTransport.Status = updateDto.Status ?? existingTransport.Status;
            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
            existingTransport.Notes = updateDto.Notes ?? existingTransport.Notes;
            existingTransport.IsInternal = updateDto.IsInternal;

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, TransportUpdateStatusDto requestStatus)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (existingTransport == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển này", StatusCode.NOT_FOUND);
            }

            existingTransport.Status = requestStatus.Status ?? existingTransport.Status;

            if (!await _transportationRepository.UpdateStatus(existingTransport, requestStatus.Status, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật trạng thái yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }
    }
}
