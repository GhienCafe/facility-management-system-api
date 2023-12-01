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
        Task<ApiResponse> DeleteTransports(DeleteMutilDto deleteDto);
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
            var assets = MainUnitOfWork.AssetRepository.GetQuery()
                                                       .Include(x => x!.Type)
                                                       .Where(x => createDto.Assets.Select(dto => dto.AssetId)
                                                                                   .Contains(x!.Id))
                                                       .ToList();

            var assetQuery = MainUnitOfWork.AssetRepository.GetQuery()
                                                           .Where(x => createDto.Assets!.Select(dto => dto.AssetId)
                                                                                        .Contains(x!.Id));
            var typeQuery = MainUnitOfWork.AssetTypeRepository.GetQuery();
            var joinTable = from asset in assetQuery
                            join type in typeQuery on asset.TypeId equals type.Id into typeGroup
                            from type in typeGroup.DefaultIfEmpty()
                            select new
                            {
                                Asset = asset,
                                AssetType = type
                            };

            var assetList = await joinTable.ToListAsync();

            foreach (var a in assetList)
            {
                if (a.AssetType.Unit == Unit.Individual && a.Asset.RequestStatus != RequestType.Operational)
                {
                    throw new ApiException($"Thiết bị {a.Asset.AssetCode} đang trong một yêu cầu khác", StatusCode.SERVER_ERROR);
                }
                var correspondingDto = createDto.Assets!.FirstOrDefault(dto => dto.AssetId == a.Asset.Id);
                if (correspondingDto != null)
                {
                    a.Asset.Quantity = correspondingDto.Quantity ?? 0;
                }
            }
            var totalQuantity = createDto.Assets?.Sum(assetDto => assetDto.Quantity);

            var toRoom = await MainUnitOfWork.RoomRepository.FindOneAsync(createDto.ToRoomId);
            if (toRoom != null)
            {
                var roomAssets = await MainUnitOfWork.RoomAssetRepository.FindAsync(
                    new Expression<Func<RoomAsset, bool>>[]
                    {
                        x => !x.DeletedAt.HasValue,
                        x => x.RoomId == toRoom.Id,
                        x => x.ToDate == null
                    }, null);

                var currentQuantityAssetInRoom = roomAssets.Sum(x => x!.Quantity);


                var checkCapacity = currentQuantityAssetInRoom + totalQuantity;
                if (checkCapacity > toRoom.Capacity)
                {
                    throw new ApiException("Số lượng trang thiết bị vượt quá dung tích phòng", StatusCode.UNPROCESSABLE_ENTITY);
                }
            }

            var transportation = new Transportation
            {
                Id = Guid.NewGuid(),
                RequestCode = GenerateRequestCode(),
                Description = createDto.Description,
                Notes = createDto.Notes,
                Priority = createDto.Priority,
                IsInternal = createDto.IsInternal,
                Quantity = (int?)totalQuantity,
                AssignedTo = createDto.AssignedTo,
                ToRoomId = createDto.ToRoomId
            };

            var mediaFiles = createDto.RelatedFiles?.Select(file => new Report
            {
                FileName = file.FileName ?? "",
                Uri = file.Uri ?? ""
            }).ToList();

            var transportationDetails = new List<TransportationDetail>();
            foreach (var asset in assets)
            {
                if (asset != null)
                {
                    var roomAsset = await MainUnitOfWork.RoomAssetRepository.FindOneAsync(
                            new Expression<Func<RoomAsset, bool>>[]
                            {
                                x => !x.DeletedAt.HasValue,
                                x => x.RoomId == createDto.FromRoomId,
                                x => x.AssetId == asset.Id,
                                x => x.ToDate == null
                            });
                    if (roomAsset != null)
                    {
                        if (asset.Type?.IsIdentified == true || asset.Type?.Unit == Unit.Individual)
                        {

                            if (createDto.FromRoomId == toRoom!.Id)
                            {
                                throw new ApiException($"Thiết bị {asset.AssetCode} đã có trong phòng này", StatusCode.UNPROCESSABLE_ENTITY);
                            }

                            var transpsortDetail = new TransportationDetail
                            {
                                Id = Guid.NewGuid(),
                                AssetId = asset.Id,
                                TransportationId = transportation.Id,
                                FromRoomId = createDto.FromRoomId,
                                RequestDate = CurrentDate,
                                Quantity = 1,
                                CreatorId = AccountId,
                                CreatedAt = CurrentDate
                            };
                            transportationDetails.Add(transpsortDetail);
                        }
                        else if (asset.Type?.IsIdentified == false || asset.Type?.Unit == Unit.Quantity)
                        {
                            var assetTransportDto = createDto.Assets!.FirstOrDefault(dto => dto.AssetId == asset.Id);
                            if(assetTransportDto != null && assetTransportDto.Quantity > roomAsset.Quantity)
                            {
                                throw new ApiException($"Phòng lấy thiết bị chỉ có: {roomAsset.Quantity} {asset.AssetName}", StatusCode.UNPROCESSABLE_ENTITY);
                            }
                            var transpsortDetail = new TransportationDetail
                            {
                                Id = Guid.NewGuid(),
                                AssetId = asset.Id,
                                TransportationId = transportation.Id,
                                RequestDate = CurrentDate,
                                Quantity = (int?)assetTransportDto!.Quantity,
                                FromRoomId = createDto.FromRoomId,
                                CreatorId = AccountId,
                                CreatedAt = CurrentDate
                            };
                            transportationDetails.Add(transpsortDetail);
                        }
                    }
                }
            }

            if (!await _transportationRepository.InsertTransportation(transportation, transportationDetails, mediaFiles, AccountId, CurrentDate))
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

            if (existingTransport.Status != RequestStatus.Done &&
               existingTransport.Status != RequestStatus.NotStart &&
               existingTransport.Status != RequestStatus.Cancelled)
            {
                throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {existingTransport.Status?.GetDisplayName()}", StatusCode.BAD_REQUEST);
            }

            if (!await MainUnitOfWork.TransportationRepository.DeleteAsync(existingTransport, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteTransports(DeleteMutilDto deleteDto)
        {
            var transportDeleteds = await MainUnitOfWork.TransportationRepository.FindAsync(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                x => deleteDto.ListId!.Contains(x.Id)
                }, null);

            var transportations = transportDeleteds.Where(t => t != null).ToList();

            foreach (var transportation in transportations)
            {
                if (transportation!.Status != RequestStatus.Done &&
                    transportation.Status != RequestStatus.NotStart &&
                    transportation.Status != RequestStatus.Cancelled)
                {
                    throw new ApiException($"Không thể xóa yêu cầu đang có trạng thái: {transportation.Status?.GetDisplayName()}" +
                                           $"kiểm tra yêu cầu: {transportation.RequestCode}", StatusCode.BAD_REQUEST);
                }
            }

            if (!await MainUnitOfWork.TransportationRepository.DeleteAsync(transportations, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TransportDto>> GetTransportation(Guid id)
        {
            var existingTransport = await MainUnitOfWork.TransportationRepository.FindOneAsync<TransportDto>(
                new Expression<Func<Transportation, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });

            if (existingTransport == null)
            {
                throw new ApiException("Không tìm thấy yêu cầu vận chuyển", StatusCode.NOT_FOUND);
            }

            var relatedMediaFileQuery = MainUnitOfWork.MediaFileRepository.GetQuery().Where(m => m!.ItemId == id && !m.IsReported);
            var relatedMediaFile = relatedMediaFileQuery.Select(x => new MediaFileDetailDto
            {
                FileName = x!.FileName,
                Uri = x.Uri,
            }).ToList();

            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();
            var toRoom = await roomDataset
                            .Where(r => r!.Id == existingTransport.ToRoomId)
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
            var assignTo = await staffs.Where(x => x!.Id == existingTransport.AssignedTo)
                            .Select(x => new UserBaseDto
                            {
                                Id = x!.Id,
                                UserCode = x.UserCode,
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

            var roomQuery = MainUnitOfWork.RoomRepository.GetQuery();
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
                                        Quantity = (double)td!.Quantity!,
                                        IsMovable = asset.IsMovable,
                                        IsRented = asset.IsRented,
                                        ManufacturingYear = asset.ManufacturingYear,
                                        RequestStatusObj = asset.RequestStatus.GetValue(),
                                        RequestStatus = asset.RequestStatus,
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
                                        Id = (Guid)td.FromRoomId!,
                                        RoomCode = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.RoomCode,
                                        RoomName = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.RoomName,
                                        StatusId = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.StatusId,
                                        FloorId = roomQuery.FirstOrDefault(x => x!.Id == td.FromRoomId)!.FloorId
                                    }
                                }).ToListAsync();

            var reports = await MainUnitOfWork.MediaFileRepository.GetQuery()
                .Where(m => m!.ItemId == id && m.IsReported).ToListAsync();

            //TODO: orderby
            var listReport = new List<MediaFileDto>();
            foreach (var report in reports)
            {
                // Deserialize the URI string back into a List<string>
                var uriList = JsonConvert.DeserializeObject<List<string>>(report.Uri);
            
                listReport.Add(new MediaFileDto
                {
                    ItemId = report.ItemId,
                    Uri = uriList,
                    FileType = report.FileType,
                    Content = report.Content,
                    IsReject = report.IsReject,
                    RejectReason = report.RejectReason
                });
            }
            
            var tranportation = new TransportDto
            {
                Id = existingTransport.Id,
                RequestCode = existingTransport.RequestCode,
                Description = existingTransport.Description,
                Notes = existingTransport.Notes,
                Status = existingTransport.Status,
                IsInternal = existingTransport.IsInternal,
                StatusObj = existingTransport.Status!.GetValue(),
                RequestDate = existingTransport.RequestDate,
                Quantity = existingTransport.Quantity,
                Priority = existingTransport.Priority,
                PriorityObj = existingTransport.Priority.GetValue(),
                Checkin = existingTransport.Checkin,
                Result = existingTransport.Result,
                Checkout = existingTransport.Checkout,
                CreatedAt = existingTransport.CreatedAt,
                EditedAt = existingTransport.EditedAt,
                CreatorId = existingTransport.CreatorId,
                EditorId = existingTransport.EditorId,
                Assets = assets,
                ToRoom = toRoom,
                RelatedFiles = relatedMediaFile,
                Reports = listReport,
                AssignedTo = existingTransport.AssignedTo,
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

            if (queryDto.Priority != null)
            {
                transportQuery = transportQuery.Where(x => x!.Priority == queryDto.Priority);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                transportQuery = transportQuery.Where(x => x!.Description!.ToLower().Contains(keyword)
                                                                   || x.Notes!.ToLower().Contains(keyword) ||
                                                                   x.RequestCode.ToLower().Contains(keyword));
            }

            transportQuery = transportQuery.OrderByDescending(x => x!.CreatedAt);

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
                Result = t.Result,
                Priority = t.Priority,
                PriorityObj = t.Priority.GetValue(),
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
                        Quantity = (double)td!.Quantity!,
                        Status = td.Asset.Status,
                        StatusObj = td.Asset.Status.GetValue(),
                        RequestStatus = td.Asset.RequestStatus,
                        RequestStatusObj = td.Asset.RequestStatus.GetValue(),
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
                },
                AssignTo = new UserBaseDto
                {
                    UserCode = t.User!.UserCode,
                    Fullname = t.User.Fullname,
                    RoleObj = t.User.Role.GetValue(),
                    Avatar = t.User.Avatar,
                    StatusObj = t.User.Status.GetValue(),
                    Email = t.User.Email,
                    PhoneNumber = t.User.PhoneNumber,
                    Address = t.User.Address,
                    Gender = t.User.Gender,
                    Dob = t.User.Dob
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

            if (existingTransport.Status != RequestStatus.NotStart)
            {
                throw new ApiException("Chỉ được cập nhật yêu cầu đang có trạng thái chưa bắt đầu", StatusCode.BAD_REQUEST);
            }

            existingTransport.Description = updateDto.Description ?? existingTransport.Description;
            existingTransport.Notes = updateDto.Notes ?? existingTransport.Notes;
            existingTransport.Priority = updateDto.Priority ?? existingTransport.Priority;
            existingTransport.AssignedTo = updateDto.AssignedTo ?? existingTransport.AssignedTo;
            existingTransport.IsInternal = updateDto.IsInternal ?? existingTransport.IsInternal;

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

            if (!await _transportationRepository.UpdateTransportation(existingTransport, additionMediaFiles, removalMediaFiles, AccountId, CurrentDate))
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

            var roomDataset = MainUnitOfWork.RoomRepository.GetQuery();
            var toRoom = await roomDataset
                            .Where(r => r!.Id == existingTransport.ToRoomId)
                            .FirstOrDefaultAsync();

            var roomAssets = await MainUnitOfWork.RoomAssetRepository.FindAsync(
                new Expression<Func<RoomAsset, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.RoomId == existingTransport.ToRoomId,
                    x => x.ToDate == null
                }, null);
            var currentQuantityAssetInRoom = roomAssets.Sum(x => x!.Quantity);

            var transportDetails = MainUnitOfWork.TransportationDetailRepository.GetQuery();
            var assetQuantity = await transportDetails
                        .Where(td => td!.TransportationId == id)
                        .Join(MainUnitOfWork.AssetRepository.GetQuery(),
                                td => td!.AssetId,
                                asset => asset!.Id, (td, asset) => td!.Quantity).ToListAsync();

            var totalQuantity = assetQuantity.Sum(a => a!.Value);
            var checkCapacity = currentQuantityAssetInRoom + totalQuantity;
            if (checkCapacity > toRoom!.Capacity)
            {
                throw new ApiException("Số lượng trang thiết bị vượt quá dung tích phòng", StatusCode.UNPROCESSABLE_ENTITY);
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