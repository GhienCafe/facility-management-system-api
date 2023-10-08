using System.Linq.Expressions;
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
    Task<ApiResponse<TeamMemberDetailDto>> GetTeamMember(Guid id);
    Task<ApiResponse> DeleteTeamMember(Guid id);
    Task<ApiResponse> CreateTeamMember(TeamMemberCreateDto createDto);
    Task<ApiResponse> UpdateTeamMember(Guid id, TeamMemberUpdateDto updateDto);
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
            join user in MainUnitOfWork.UserRepository.GetQuery() on teamMember.MemberId equals user.Id into userGroup
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
            Team = x.Team.ProjectTo<Team, TeamDto>(),
            Member = x.User.ProjectTo<User, UserBaseDto>()
        }).ToListAsync();
        
        items.ForEach(x =>
        {
            if (x.Member != null)
            {
                x.Member.StatusObj = x.Member.Status?.GetValue();
                x.Member.RoleObj = x.Member.Role?.GetValue();
            }
        });

        items = await _mapperRepository.MapCreator(items);
        
        return ApiResponses<TeamMemberDto>.Success(
            items,
            totalCount,
            queryDto.PageSize,
            queryDto.Page,
            (int)Math.Ceiling(totalCount / (double)queryDto.PageSize));
    }

    public async Task<ApiResponse<TeamMemberDetailDto>> GetTeamMember(Guid id)
    {
        var teamMember = await MainUnitOfWork.TeamMemberRepository.FindOneAsync(id);

        if (teamMember == null)
            throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);
        
        var teamMemberDto = teamMember.ProjectTo<TeamMember, TeamMemberDetailDto>();
        
        teamMemberDto.Member = (await MainUnitOfWork.UserRepository.FindOneAsync(teamMemberDto.MemberId))?
            .ProjectTo<User, UserBaseDto>();
        
        teamMemberDto.Team = (await MainUnitOfWork.TeamRepository.FindOneAsync(teamMemberDto.TeamId))?
            .ProjectTo<Team, TeamDto>();

        if (teamMemberDto.Member != null)
        {
            teamMemberDto.Member.StatusObj = teamMemberDto.Member.Status?.GetValue();
            teamMemberDto.Member.RoleObj = teamMemberDto.Member.Role?.GetValue();
        }

        return ApiResponse<TeamMemberDetailDto>.Success(teamMemberDto);
    }

    public async Task<ApiResponse> DeleteTeamMember(Guid id)
    {
        var teamMember = await MainUnitOfWork.TeamMemberRepository.FindOneAsync(id);

        if (teamMember == null)
            throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);
        
        if (!await MainUnitOfWork.TeamMemberRepository.DeleteAsync(teamMember, AccountId, CurrentDate))
            throw new ApiException("Xóa thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Xóa thành viên thành công");
    }

    public async Task<ApiResponse> CreateTeamMember(TeamMemberCreateDto createDto)
    {
        var teamMember = createDto.ProjectTo<TeamMemberCreateDto, TeamMember>();

        var checkExist = await MainUnitOfWork.TeamMemberRepository.FindAsync(new Expression<Func<TeamMember, bool>>[]
        {
            x => !x.DeletedAt.HasValue,
            x => x.TeamId == createDto.TeamId,
            x => x.MemberId == createDto.MemberId,
        }, null);

        if (checkExist.Any())
            throw new ApiException("Thành viên đã tồn tại trong đội nhóm", StatusCode.ALREADY_EXISTS);

        if (!await MainUnitOfWork.TeamMemberRepository.InsertAsync(teamMember, AccountId, CurrentDate))
            throw new ApiException("Thêm thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Created("Thêm thành viên thành công");
    }

    public async Task<ApiResponse> UpdateTeamMember(Guid id, TeamMemberUpdateDto updateDto)
    {
        var teamMember = await MainUnitOfWork.TeamMemberRepository.FindOneAsync(id);

        if (teamMember == null)
            throw new ApiException("Không tìm thấy thông tin", StatusCode.NOT_FOUND);

        teamMember.TeamId = updateDto.TeamId ?? teamMember.TeamId;
        teamMember.MemberId = updateDto.MemberId ?? teamMember.MemberId;
        teamMember.IsLead = updateDto.IsLead ?? teamMember.IsLead;

        if (!await MainUnitOfWork.TeamMemberRepository.UpdateAsync(teamMember, AccountId, CurrentDate))
            throw new ApiException("Cập nhật thông tin thất bại", StatusCode.SERVER_ERROR);
        
        return ApiResponse.Success("Cập nhật thông tin thành công");
    }
}