﻿using AppCore.Data;
using MainData.Entities;

namespace MainData;

public class MainUnitOfWork : IDisposable
{
    private readonly DatabaseContext _context;

    public MainUnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

   // public DatabaseContext Context => new(_context);
    public BaseRepository<User> UserRepository => new(_context);
    public BaseRepository<Token> TokenRepository => new(_context);
    public BaseRepository<Campus> CampusRepository => new(_context);
    public BaseRepository<Building> BuildingRepository => new(_context);
    public BaseRepository<Floor> FloorRepository => new(_context);
    public BaseRepository<Room> RoomRepository => new(_context);

    public BaseRepository<RoomStatus> RoomStatusRepository => new(_context);
    public BaseRepository<RoomType> RoomTypeRepository => new(_context);

    public BaseRepository<AssetType> AssetTypeRepository => new(_context);
    public BaseRepository<Asset> AssetRepository => new(_context);
    public BaseRepository<RoomAsset> RoomAssetRepository => new(_context);
    public BaseRepository<Transportation> TransportationRepository => new(_context);
    public BaseRepository<Team> TeamRepository => new(_context);
    public BaseRepository<Category> CategoryRepository => new(_context);
    public BaseRepository<Notification> NotificationRepository => new(_context);
    public BaseRepository<Replacement> ReplacementRepository => new(_context);
    public BaseRepository<Maintenance> MaintenanceRepository => new(_context);
    public BaseRepository<MaintenanceScheduleConfig> MaintenanceScheduleRepository => new(_context);
    public BaseRepository<Repairation> RepairationRepository => new(_context);
    public BaseRepository<Model> ModelRepository => new(_context);
    public BaseRepository<ActionRequest> RequestRepository => new(_context);
    public BaseRepository<AssetCheck> AssetCheckRepository => new(_context);
    public BaseRepository<TeamMember> TeamMemberRepository => new(_context);

    public void Dispose()
    {
    }
}