using AppCore.Data;
using MainData.Entities;

namespace MainData;

public class MainUnitOfWork : IDisposable
{
    private readonly DatabaseContext _context;

    public MainUnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public BaseRepository<User> UserRepository => new(_context);
    public BaseRepository<Token> TokenRepository => new(_context);
    public BaseRepository<Campus> CampusRepository => new(_context);
    public BaseRepository<Buildings> BuildingsRepository => new(_context);
    public BaseRepository<Floors> FloorsRepository => new(_context);
    public BaseRepository<Rooms> RoomsRepository => new(_context);

    
    public void Dispose()
    {
    }
}