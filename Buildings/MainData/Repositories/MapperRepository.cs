using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace MainData.Repositories;

public interface IMapperRepository
{
    Task<List<TDto>> MapCreator<TDto>(List<TDto> dtos) where TDto : BaseDto;
    Task<TDto> MapCreator<TDto>(TDto dto) where TDto : BaseDto;
}

public class MapperRepository : IMapperRepository
{
    private readonly DatabaseContext _context;

    public MapperRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<TDto>> MapCreator<TDto>(List<TDto> dtos) where TDto : BaseDto
    {
        var accountIds = dtos.SelectMany(x => new[] { x.CreatorId, x.EditorId })
            .GroupBy(x => x)
            .Select(x => x.Key);

        var accountCreators = (from accountId in accountIds
            join account in _context.Users
                on accountId equals account.Id
            join user in _context.Users
                on account.Id equals user.Id
            select new AccountCreator
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar
            }).ToList();
        dtos = dtos.Select(x =>
        {
            x.Creator = accountCreators.FirstOrDefault(z => z.Id == x.CreatorId);
            x.Editor = accountCreators.FirstOrDefault(z => z.Id == x.EditorId);
            return x;
        }).ToList();
        await Task.Delay(1);
        return dtos;
    }

    public async Task<TDto> MapCreator<TDto>(TDto dto) where TDto : BaseDto
    {
        var accountCreators = await (from account in _context.Users
            join user in _context.Users
                on account.Id equals user.Id
            where account.Id == dto.CreatorId ||
                  account.Id == dto.EditorId
            select new AccountCreator
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar
            }).ToListAsync();

        dto.Creator = accountCreators.FirstOrDefault(x => x.Id == dto.CreatorId);
        dto.Editor = accountCreators.FirstOrDefault(x => x.Id == dto.EditorId);
        return dto;
    }
}
