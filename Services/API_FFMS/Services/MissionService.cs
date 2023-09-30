using System.Linq.Expressions;
using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;



namespace API_FFMS.Services
{
    public interface IMissionService : IBaseService
    {
        Task<ApiResponses<MissionDto>> GetListTask(QueryMissionDto queryDto);
    }

    public class MissionService : BaseService, IMissionService
    {

        public MissionService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponses<MissionDto>> GetListTask(QueryMissionDto queryDto)
        {
            var maintenanceEntities = await MainUnitOfWork.MaintenanceRepository.GetQuery()
                .Where(entity => entity!.AssignedTo == AccountId)
                .ToListAsync();

            var replacementEntities = await MainUnitOfWork.ReplacementRepository.GetQuery()
                .Where(entity => entity!.AssignedTo == AccountId)
                .ToListAsync();

            var repairEntities = await MainUnitOfWork.RepairationRepository.GetQuery()
                .Where(entity => entity!.AssignedTo == AccountId)
                .ToListAsync();

            var transportEntities = await MainUnitOfWork.TransportationRepository.GetQuery()
                .Where(entity => entity!.AssignedTo == AccountId)
                .ToListAsync();

            var allEntities = new List<object>();
            allEntities.AddRange(maintenanceEntities.Cast<object>());
            allEntities.AddRange(replacementEntities.Cast<object>());
            allEntities.AddRange(repairEntities.Cast<object>());
            allEntities.AddRange(transportEntities.Cast<object>());

            var missionDtos = new List<MissionDto>();

            foreach (var entity in allEntities)
            {
                var missionDto = new MissionDto();

                if (entity is Maintenance maintenance)
                {
                    missionDto.MaintenanceMission = new MaintenanceDto
                    {
                        AssetId = maintenance.AssetId,
                        Id = maintenance.Id,
                        AssignedTo = maintenance.AssignedTo,
                        CompletionDate = maintenance.CompletionDate,
                        Description = maintenance.Description,
                        Type = maintenance.Type.GetValue(),
                        Note = maintenance.Note,
                        RequestedDate = maintenance.RequestedDate,
                        Status = maintenance.Status.GetValue(),
                        CreatedAt = maintenance.CreatedAt,
                        EditedAt = maintenance.EditedAt,
                        CreatorId = maintenance.CreatorId ?? Guid.Empty,
                        EditorId = maintenance.EditorId ?? Guid.Empty,
                        Asset = await MapAssetFromAssetId((Guid)maintenance.AssetId!),
                        PersonInCharge = maintenance.PersonInCharge!.ProjectTo<User, UserDto>()
                    };
                    missionDto.MaintenanceMission =
                        await _mapperRepository.MapCreator(missionDto.MaintenanceMission);
                }
                else if (entity is Replacement replacement)
                {
                    Console.WriteLine(replacement.Description);
                    Console.WriteLine(entity);
                    missionDto.ReplacementMission = new ReplacementDto
                    {
                        Description = replacement.Description,
                        CompletionDate = replacement.CompletionDate,
                        RequestedDate = replacement.RequestedDate,
                        Note = replacement.Note,
                        Reason = replacement.Reason,
                        Status = replacement.Status.GetValue(),
                        AssignedTo = replacement.AssignedTo,
                        AssetId = replacement.AssetId,
                        NewAssetId = replacement.NewAssetId,
                        CreatorId = replacement.CreatorId ?? Guid.Empty,
                        EditorId = replacement.EditorId ?? Guid.Empty,
                        PersonInCharge = replacement.PersonInCharge!.ProjectTo<User, UserDto>(),
                        Asset = await MapAssetFromAssetId((Guid)replacement.AssetId!),
                    };
                    missionDto.ReplacementMission =
                        await _mapperRepository.MapCreator(missionDto.ReplacementMission);
                }
                else if (entity is Repairation repair)
                {
                    missionDto.RepairMission = new RepairationDto
                    {
                        AssignedTo = repair.AssignedTo,
                        Status = repair.Status.GetValue(),
                        Id = repair.Id,
                        CompletionDate = repair.CompletionDate,
                        CreatedAt = repair.CreatedAt,
                        EditedAt = repair.EditedAt,
                        CreatorId = repair.CreatorId ?? Guid.Empty,
                        EditorId = repair.EditorId ?? Guid.Empty,
                        AssetId = repair.AssetId,
                        RequestedDate = repair.RequestedDate,
                        Note = repair.Note,
                        Asset = await MapAssetFromAssetId((Guid)repair.AssetId!),
                        PersonInCharge = repair.PersonInCharge!.ProjectTo<User, UserDto>(),
                    };
                    missionDto.RepairMission = await _mapperRepository.MapCreator(missionDto.RepairMission);
                }
                else if (entity is Transportation transport)
                {
                    missionDto.TransportMission = new TransportDto
                    {
                        AssignedTo = transport.AssignedTo,
                        ToRoomId = transport.ToRoomId,
                        Status = transport.Status.GetValue(),
                        Id = transport.Id,
                        CompletionDate = transport.CompletionDate,
                        CreatedAt = transport.CreatedAt,
                        EditedAt = transport.EditedAt,
                        CreatorId = transport.CreatorId ?? Guid.Empty,
                        EditorId = transport.EditorId ?? Guid.Empty,
                        AssetId = transport.AssetId,
                        RequestedDate = transport.RequestedDate,
                        Description = transport.Description,
                        Note = transport.Note,
                        Quantity = transport.Quantity,
                        Asset = await MapAssetFromAssetId((Guid)transport.AssetId!),
                        PersonInCharge = transport.PersonInCharge!.ProjectTo<User, UserDto>(),
                        ToRoom = transport.ToRoom!.ProjectTo<Room, RoomDto>()
                    };
                    missionDto.TransportMission = await _mapperRepository.MapCreator(missionDto.TransportMission);
                }

                missionDtos.Add(missionDto);
            }

            return ApiResponses<MissionDto>.Success(
                missionDtos,
                missionDtos.Count(),
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(missionDtos.Count() / (double)queryDto.PageSize)
            );
        }
        private async Task<AssetDto> MapAssetFromAssetId(Guid assetId)
        {
            var asset = await MainUnitOfWork.AssetRepository.FindOneAsync(new Expression<Func<Asset, bool>>[]
            {
                x => !x.DeletedAt.HasValue && x.Id == assetId
            });

            if (asset != null)
            {
                return asset.ProjectTo<Asset, AssetDto>();
            }

            return null;
        }
    }
    
}
