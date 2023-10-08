using API_FFMS.Dtos;
using AppCore.Extensions;
using AppCore.Models;
using MainData;
using MainData.Entities;
using MainData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_FFMS.Services;

public interface ITeamMemberService : IBaseService
{
    Task<ApiResponses<TeamMemberDto>> GetTeamsMember(TeamMemberQueryDto queryDto);
}

public class TeamMemberService : BaseService, ITeamMemberService
{
    public TeamMemberService(MainUnitOfWork mainUnitOfWork, IHttpContextAccessor httpContextAccessor, IMapperRepository mapperRepository) : base(mainUnitOfWork, httpContextAccessor, mapperRepository)
    {
    }

    public async Task<ApiResponses<TeamMemberDto>> GetTeamsMember(TeamMemberQueryDto queryDto)
    {
        var teamMemberQueryable = MainUnitOfWork.TeamMemberRepository.GetQuery()
            .Where(x => !x!.DeletedAt.HasValue);

        if (queryDto.IsLead != null)
        {
            teamMemberQueryable = teamMemberQueryable.Where(x => x!.IsLead == queryDto.IsLead);
        }
        
        if (queryDto.TeamId != null)
        {
            teamMemberQueryable = teamMemberQueryable.Where(x => x!.TeamId == queryDto.TeamId);
        }
        
        if (queryDto.MemberId != null)
        {
            teamMemberQueryable = teamMemberQueryable.Where(x => x!.MemberId == queryDto.MemberId);
        }

        var joinTables = from teamMember in teamMemberQueryable
            join team in MainUnitOfWork.TeamRepository.GetQuery() on teamMember.TeamId equals team.Id into teamGroup
            from team in teamGroup.DefaultIfEmpty()
            join user in MainUnitOfWork.UserRepository.GetQuery() on teamMember.TeamId equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty()
            select new
            {
                TeamMember = teamMember,
                Team = team,
                User = user
            };

        var totalCount = await joinTables.CountAsync();

        joinTables = joinTables.Skip(queryDto.Skip()).Take(queryDto.PageSize);

        var items = await joinTables.Select(x => new TeamMemberDto
        {
            Id = x.TeamMember.Id,
            TeamId = x.TeamMember.TeamId,
            MemberId = x.TeamMember.MemberId,
            IsLead = x.TeamMember.IsLead,
            CreatedAt = x.TeamMember.CreatedAt,
            EditedAt = x.TeamMember.EditedAt,
            CreatorId = x.TeamMember.CreatorId ?? Guid.Empty,
            EditorId = x.TeamMember.EditorId ?? Guid.Empty,
            TeamDto = x.Team.ProjectTo<Team, TeamDto>(),
            Member = x.User.ProjectTo<User, UserBaseDto>()
        }).ToListAsync();
        
        items.ForEach(x =>
        {
            x.Member!.StatusObj = x.Member.Status?.GetValue();
            x.Member!.RoleObj = x.Member.Role?.GetValue();
        });

        items = await _mapperRepository.MapCreator(items);
        
        return ApiResponses<TeamMemberDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }
}