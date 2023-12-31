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
    //public BaseRepository<MaintenanceScheduleConfig> MaintenanceScheduleRepository => new(_context);
    public BaseRepository<Repair> RepairRepository => new(_context);
    public BaseRepository<Model> ModelRepository => new(_context);
    public BaseRepository<AssetCheck> AssetCheckRepository => new(_context);
    public BaseRepository<TeamMember> TeamMemberRepository => new(_context);
    public BaseRepository<TransportationDetail> TransportationDetailRepository => new(_context);
    public BaseRepository<Report> MediaFileRepository => new(_context);
    public BaseRepository<Brand> BrandRepository => new(_context);
    public BaseRepository<InventoryCheck> InventoryCheckRepository => new(_context);
    public BaseRepository<InventoryCheckDetail> InventoryCheckDetailRepository => new(_context);
    public BaseRepository<InventoryCheckConfig> InventoryCheckConfigRepository => new(_context);
    public BaseRepository<InventoryDetailConfig> InventoryDetailConfigRepository => new(_context);
    
    //******* VIEWS *******//
    public BaseRepository<TaskView> TaskRepository => new(_context);
    public void Dispose()
    {
    }
}