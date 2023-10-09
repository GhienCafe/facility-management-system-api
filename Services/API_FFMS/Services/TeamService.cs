using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using System.Linq.Expressions;

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
                throw new ApiException("Not found this team", StatusCode.NOT_FOUND);
            }

            if (!await MainUnitOfWork.TeamRepository.DeleteAsync(existingTeam, AccountId, CurrentDate))
            {
                throw new ApiException("Can't not delete", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DeleteTeams(List<Guid> ids)
        {
            var teamDeleteds = new List<Team>();
            foreach(var id in ids)
            {
                var existingTeam = await MainUnitOfWork.TeamRepository.FindOneAsync(
                new Expression<Func<Team, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => x.Id == id
                });
                if (existingTeam == null)
                {
                    throw new ApiException("Không tìm thấy đội phụ trách này", StatusCode.NOT_FOUND);
                }

                teamDeleteds.Add(existingTeam);
            }

            if (!await MainUnitOfWork.TeamRepository.DeleteAsync(teamDeleteds, AccountId, CurrentDate))
            {
                throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
            }
            return ApiResponse.Success();
        }

        public async Task<ApiResponse<TeamDetailDto>> GetTeam(Guid id)
        {
            // var team = await MainUnitOfWork.TeamRepository.FindOneAsync<TeamDetailDto>(
            //     new Expression<Func<Team, bool>>[]
            //     {
            //         x => !x.DeletedAt.HasValue,
            //         x => x.Id == id
            //     });
            // if (team == null)
            // {
            //     throw new ApiException("Not found this team", StatusCode.NOT_FOUND);
            // }
            //
            // team.TotalMember = MainUnitOfWork.UserRepository.GetQuery().Count(x => !x!.DeletedAt.HasValue
            //     && x.TeamId == team.Id);
            //
            // team.Members = (IEnumerable<TeamIncludeDto>?)MainUnitOfWork.UserRepository.GetQuery()
            //                 .Where(x => x!.TeamId == team.Id).ToList();
            //
            // team = await _mapperRepository.MapCreator(team);
            //
            // return ApiResponse<TeamDetailDto>.Success(team);
            throw new ApiException("");
        }

        public async Task<ApiResponses<TeamDto>> GetTeams(TeamQueryDto queryDto)
        {
            var teams = await MainUnitOfWork.TeamRepository.FindResultAsync<TeamDto>(
                new Expression<Func<Team, bool>>[]
                {
                    x => !x.DeletedAt.HasValue,
                    x => queryDto.TeamName == null || x.TeamName!.ToLower().Contains(queryDto.TeamName.Trim().ToLower())
                }, queryDto.OrderBy, queryDto.Skip(), queryDto.PageSize);

            teams.Items = await _mapperRepository.MapCreator(teams.Items.ToList());

            return ApiResponses<TeamDto>.Success(
            teams.Items,
            teams.TotalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(teams.TotalCount / (double)queryDto.PageSize));
        }

        public async Task<ApiResponse> Insert(TeamCreateDto createDto)
        {
            var team = createDto.ProjectTo<TeamCreateDto, Team>();

            if (!await MainUnitOfWork.TeamRepository.InsertAsync(team, AccountId, CurrentDate))
            {
                throw new ApiException("Insert fail", StatusCode.SERVER_ERROR);
            }

            return ApiResponse.Created("Create successfully");
        }

        public async Task<ApiResponse> Update(Guid id, TeamUpdateDto updateDto)
        {
            var team = await MainUnitOfWork.TeamRepository.FindOneAsync(id);

            if (team == null)
            {
                throw new ApiException("Not found this team", StatusCode.NOT_FOUND);
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
