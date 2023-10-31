using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services
{
    public interface ITeamService : IBaseService
    {
        Task<ApiResponses<TeamDto>> GetTeams(TeamQueryDto queryDto);
        Task<ApiResponse<TeamDetailDto>> GetTeam(Guid id);
        public Task<ApiResponse> Insert(TeamCreateDto createDto);
        Task<ApiResponse> Delete(Guid id);
        Task<ApiResponse> DeleteTeams(List<Guid> ids);
        Task<ApiResponse> Update(Guid id, TeamUpdateDto updateDto);
    }
    public class TeamService : BaseService, ITeamService
    {
        public TeamService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
        {
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var existingTeam = await MainUnitOfWork.TeamRepository.FindOneAsync(id);
            if (existingTeam == null)
            {
                throw new ApiException("Không tìm thấy đội nhóm", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.TeamRepository.DeleteAsync(existingTeam, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success("Xóa thành công");
        }

        public async Task<ApiResponse> DeleteTeams(List<Guid> ids)
        {
            var teamDeleteds = await MainUnitOfWork.TeamRepository.FindAsync(
                new Expression<Func<Team, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => ids.Contains(x.Id)
                }, null);

            if (!await MainUnitOfWork.TeamRepository.DeleteAsync(teamDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TeamDetailDto>> GetTeam(Guid id)
        {
            var team = await MainUnitOfWork.TeamRepository.FindOneAsync<TeamDetailDto>(
                new Expression<Func<Team, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
            if (team == null)
            {
                throw new ApiException("Không tìm thấy đội nhóm", StatusCode.NOT_FOUND);
            }

            team.TotalMember = MainUnitOfWork.TeamMemberRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
                && x.TeamId == team.Id);

            team = await _mapperRepository.MapCreator(team);

            return ApiResponse<TeamDetailDto>.Success(team);
        }

        public async Task<ApiResponses<TeamDto>> GetTeams(TeamQueryDto queryDto)
        {
            var keyword = queryDto.Keyword?.Trim().ToLower();
            var teamQueryable = MainUnitOfWork.TeamRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);

            var teamMemberQueryable = MainUnitOfWork.TeamMemberRepository.GetQuery()
                .Where(x => !x!.DeletedAt.HasValue);

            var joinTables = from t in teamQueryable
                             join tm in teamMemberQueryable on t.Id equals tm.TeamId into teamMemberGroup
                             from tm in teamMemberGroup.DefaultIfEmpty()
                             group new { t, tm } by new
                             {
                                 t.Id,
                                 t.TeamName,
                                 t.Description,
                                 t.CreatedAt,
                                 t.EditedAt,
                                 t.CreatorId,
                                 t.EditorId
                             }
                into groupedData
                             select new TeamDto
                             {
                                 Id = groupedData.Key.Id,
                                 TeamName = groupedData.Key.TeamName,
                                 Description = groupedData.Key.Description,
                                 EditedAt = groupedData.Key.EditedAt,
                                 CreatedAt = groupedData.Key.CreatedAt,
                                 EditorId = groupedData.Key.EditorId ?? Guid.Empty,
                                 CreatorId = groupedData.Key.CreatorId ?? Guid.Empty,
                                 TotalMember = groupedData.Count(item => item.tm.Id != null)
                             };

            var totalCount = await joinTables.CountAsync();

            joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

            var items = await joinTables.Select(x => new TeamDto
            {
                Id = x.Id,
                TeamName = x.TeamName,
                Description = x.Description,
                EditedAt = x.EditedAt,
                CreatedAt = x.CreatedAt,
                EditorId = x.EditorId,
                CreatorId = x.CreatorId,
                TotalMember = x.TotalMember
            }).ToListAsync();

            items = await _mapperRepository.MapCreator(items);

            return ApiResponses<TeamDto>.Success(
                items,
                totalCount,
                queryDto.PageSize,
                queryDto.Page,
                (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Insert(TeamCreateDto createDto)
        {
            var team = createDto.ProjectTo<TeamCreateDto, Team>();

            if (!await MainUnitOfWork.TeamRepository.InsertAsync(team, AccountId, CurrentDate))
            {
                throw new ApiException("Tạo mới thất bại", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Tạo mới đội nhóm thành công");
        }

        public async Task<ApiResponse> Update(Guid id, TeamUpdateDto updateDto)
        {
            var team = await MainUnitOfWork.TeamRepository.FindOneAsync(id);

            if (team == null)
            {
                throw new ApiException("Không tìm thấy đội nhóm", StatusCode.NOT_FOUND);
            }

            team.TeamName = updateDto.TeamName ?? team.TeamName;
            team.Description = updateDto.Description ?? team.Description;

            if (!await MainUnitOfWork.TeamRepository.UpdateAsync(team, AccountId, CurrentDate))
            {
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }
    }
}
