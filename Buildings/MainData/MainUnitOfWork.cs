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
    public BaseRepository<Department> DepartmentRepository => new(_context);
    public BaseRepository<Building> BuildingsRepository => new(_context);
    public BaseRepository<Floor> FloorsRepository => new(_context);
    public BaseRepository<Room> RoomRepository => new(_context);

    public BaseRepository<RoomStatus> RoomStatusRepository => new(_context);

    public BaseRepository<AssetCategory> AssetCategoryRepository => new(_context);
    public BaseRepository<Asset> AssetRepository => new(_context);

    public void Dispose()
    {
    }
}