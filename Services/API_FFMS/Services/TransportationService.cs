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
        Task<ApiResponse> DeleteTransports(List<Guid> ids);
        public Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto updateStatusDto);
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
            var assets = await MainUnitOfWork.AssetRepository.FindAsync(
                new Expression<Func<Asset, bool>>[]
                {
                    x => !x!.DeletedAt.HasValue,
                    x => createDto.Assets!.Select(dto => dto.AssetId).Contains(x.Id)
                }, null);

            foreach (var asset in assets)
            {
                if (asset!.Status != AssetStatus.Operational)
                {
                    throw new ApiException("Trang thiết bị đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
                }

                var correspondingDto = createDto.Assets!.FirstOrDefault(dto => dto.AssetId == asset!.Id);
                if (correspondingDto != null)
                {
                    asset!.Quantity = correspondingDto.Quantity ?? 0;
                }
            }

            //var toRoom = await MainUnitOfWork.RoomRepository.FindOneAsync((Guid)createDto.ToRoomId);
            double? totalQuantity = createDto.Assets?.Sum(assetDto => assetDto.Quantity);

            var transportation = new Transportation
            {
                RequestCode = GenerateRequestCode(),
                RequestDate = CurrentDate,
                CompletionDate = createDto.CompletionDate,
                Description = createDto.Description,
                Notes = createDto.Notes,
                Priority = createDto.Priority,
                IsInternal = createDto.IsInternal,
                Quantity = (int?)totalQuantity,
                AssignedTo = createDto.AssignedTo,
                ToRoomId = createDto.ToRoomId
            };

            if (!await _transportationRepository.InsertTransportations(transportation, assets, AccountId, CurrentDate))
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

        public async Task<ApiResponse> DeleteTransports(List<Guid> ids)
        {
            var transportDeleteds = await MainUnitOfWork.TransportationRepository.FindAsync(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                x => ids.Contains(x.Id)
                }, null);

            if (!await MainUnitOfWork.TransportationRepository.DeleteAsync(transportDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TransportDto>> GetTransportation(Guid id)
        {
            var existingtransport = await MainUnitOfWork.TransportationRepository.FindOneAsync<TransportDto>(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (existingtransport == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển", StatusCode.NOT_FOUND);
            }

            var mediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == existingtransport.Id);
            var mediaFile = new MediaFileDto
            {
                FileType = mediaFileQuery.Select(m => m!.FileType).FirstOrDefault(),
                Uri = mediaFileQuery.Select(m => m!.Uri).ToList(),
                Content = mediaFileQuery.Select(m => m!.Content).FirstOrDefault()
            };

            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();
            var toRoom = await roomDataset
                            .Where(r => r!.Id == existingtransport.ToRoomId)
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

            var staffs = MainUnitOfWork.UserRepository.GetQuery();
            var assignTo = await staffs.Where(x => x!.Id == existingtransport.AssignedTo)
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

            var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery();
            var assets = await transportDetails
                        .Where(td => td!.TransportationId == id)
                        .Join(MainUnitOfWork.AssetRepository.GetQuery(),
                                td => td!.AssetId,
                                asset => asset!.Id, (td, asset) => new FromRoomAssetDto
                                {
                                    Asset = new AssetBaseDto
                                    {
                                        Id = asset!.Id,
                                        Description = asset.Description,
                                        AssetCode = asset.AssetCode,
                                        AssetName = asset.AssetName,
                                        Quantity = asset.Quantity,
                                        IsMovable = asset.IsMovable,
                                        IsRented = asset.IsRented,
                                        ManufacturingYear = asset.ManufacturingYear,
                                        StatusObj = asset.Status.GetValue(),
                                        Status = asset.Status,
                                        StartDateOfUse = asset.StartDateOfUse,
                                        SerialNumber = asset.SerialNumber,
                                        LastCheckedDate = asset.LastCheckedDate,
                                        LastMaintenanceTime = asset.LastMaintenanceTime,
                                        TypeId = asset.TypeId,
                                        ModelId = asset.ModelId,
                                        CreatedAt = asset.CreatedAt,
                                        EditedAt = asset.EditedAt,
                                        CreatorId = asset.CreatorId ?? Guid.Empty,
                                        EditorId = asset.EditorId ?? Guid.Empty
                                    },
                                    FromRoom = new RoomBaseDto
                                    {
                                        Id = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.Id,
                                        RoomCode = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.RoomCode,
                                        RoomName = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.RoomName,
                                        StatusId = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.StatusId,
                                        FloorId = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.FloorId,
                                        CreatedAt = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.CreatedAt,
                                        EditedAt = asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td!.AssetId)!.Room!.EditedAt
                                    }
                                }).ToListAsync();

            var tranportation = new TransportDto
            {
                Id = existingtransport.Id,
                RequestCode = existingtransport.RequestCode,
                Description = existingtransport.Description,
                Notes = existingtransport.Notes,
                Status = existingtransport.Status,
                StatusObj = existingtransport.Status!.GetValue(),
                RequestDate = existingtransport.RequestDate,
                Quantity = existingtransport.Quantity,
                Checkin = existingtransport.Checkin,
                Checkout = existingtransport.Checkout,
                CreatedAt = existingtransport.CreatedAt,
                EditedAt = existingtransport.EditedAt,
                CreatorId = existingtransport.CreatorId,
                EditorId = existingtransport.EditorId,
                Assets = assets,
                ToRoom = toRoom,
                MediaFile = mediaFile,
                AssignTo = assignTo
            };

            tranportation = await _mapperRepository.MapCreator(tranportation);

            return ApiResponse<TransportDto>.Success(tranportation);
        }

        public async Task<ApiResponses<TransportDto>> GetTransports(TransportationQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var transportQuery = MainUnitOfWork.TransportationRepository.GetQuery()
                                 .Where(x => !x!.DeletedAt.HasValue);

            if (queryDto.IsInternal != null)
            {
                transportQuery = transportQuery.Where(x => x!.IsInternal == queryDto.IsInternal);
            }

            if (queryDto.AssignedTo != null)
            {
                transportQuery = transportQuery.Where(x => x!.AssignedTo == queryDto.AssignedTo);
            }

            if (queryDto.Status != null)
            {
                transportQuery = transportQuery.Where(x => x!.Status == queryDto.Status);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                transportQuery = transportQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                                   || x.Notes!.ToLower().Contains(keyword) ||
                                                                   x.RequestCode.ToLower().Contains(keyword));
            }

            var totalCount = await transportQuery.CountAsync();
            transportQuery = transportQuery.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var transportations = await transportQuery.Select(t => new TransportDto
            {
                Id = t!.Id,
                RequestCode = t.RequestCode,
                RequestDate = t.RequestDate,
                CompletionDate = t.CompletionDate,
                Status = t.Status,
                StatusObj = t.Status!.GetValue(),
                Description = t.Description,
                Checkout = t.Checkout,
                Checkin = t.Checkout,
                Notes = t.Notes,
                IsInternal = t.IsInternal,
                Quantity = t.Quantity,
                ToRoomId = t.ToRoomId,
                AssignedTo = t.AssignedTo,
                CreatedAt = t.CreatedAt,
                EditedAt = t.EditedAt,
                CreatorId = t.CreatorId ?? Guid.Empty,
                EditorId = t.EditorId ?? Guid.Empty,
                Assets = t.TransportationDetails!.Select(td => new FromRoomAssetDto
                {
                    Asset = new AssetBaseDto
                    {
                        Id = (Guid)td.AssetId!,
                        AssetCode = td.Asset!.AssetCode,
                        AssetName = td.Asset.AssetName,
                        Quantity = td.Asset.Quantity,
                        Status = td.Asset.Status,
                        StatusObj = td.Asset.Status.GetValue(),
                        IsMovable = td.Asset.IsMovable,
                        Description = td.Asset.Description,
                        IsRented = td.Asset.IsRented,
                        ManufacturingYear = td.Asset.ManufacturingYear,
                        LastCheckedDate = td.Asset.LastCheckedDate,
                        StartDateOfUse = td.Asset.StartDateOfUse
                    },
                    FromRoom = new RoomBaseDto
                    {
                        Id = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.Id,
                        RoomCode = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.RoomCode,
                        RoomName = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.RoomName,
                        StatusId = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.StatusId,
                        FloorId = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.FloorId,
                        CreatedAt = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.CreatedAt,
                        EditedAt = td.Asset.RoomAssets!.FirstOrDefault(ra => ra.AssetId == td.AssetId)!.Room!.EditedAt
                    }
                }).ToList(),
                ToRoom = new RoomBaseDto
                {
                    Id = t.ToRoom!.Id,
                    RoomCode = t.ToRoom.RoomCode,
                    RoomName = t.ToRoom.RoomName,
                    StatusId = t.ToRoom.StatusId,
                    FloorId = t.ToRoom.FloorId,
                    CreatedAt = t.ToRoom.CreatedAt,
                    EditedAt = t.ToRoom.EditedAt
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

            if (existingTransport.Status != RequestStatus.InProgress)
            {
                throw new ApiException("Chỉ được cập nhật các yêu cầu chưa hoàn thành", StatusCode.NOT_FOUND);
            }

            existingTransport.RequestDate = updateDto.RequestDate ?? existingTransport.RequestDate;
            //existingTransport.CompletionDate = updateDto.CompletionDate ?? existingTransport.CompletionDate;
            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
            existingTransport.Notes = updateDto.Notes ?? existingTransport.Notes;
            existingTransport.Piority = updateDto.Piority ?? existingTransport.Piority;

            if (!await MainUnitOfWork.TransportationRepository.UpdateAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Cập nhật thông tin yêu cầu thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Cập nhật yêu cầu thành công");
        }

        public async Task<ApiResponse> UpdateStatus(Guid id, BaseUpdateStatusDto requestStatus)
        {
            var existingTransport = MainUnitOfWork.TransportationRepository.GetQuery()
                                    .Include(t => t!.TransportationDetails)
                                    .Where(t => t!.Id == id)
                                    .FirstOrDefault();
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

        public string GenerateRequestCode()
        {
            var requests = MainUnitOfWork.TransportationRepository.GetQueryAll().ToList();

            var numbers = new List<int>();
            foreach (var t in requests)
            {
                int.TryParse(t!.RequestCode[3..], out int lastNumber);
                numbers.Add(lastNumber);
            }

            string newRequestCode = "TRS1";

            if (requests.Any())
            {
                var lastCode = numbers.AsQueryable().OrderDescending().FirstOrDefault();
                if (requests.Any(x => x!.RequestCode.StartsWith("TRS")))
                {
                    newRequestCode = $"TRS{lastCode + 1}";
                }
            }
            return newRequestCode;
        }
    }
}