using MainData.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainData;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Campus> Campuses { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomAsset> RoomAssets { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<MaintenanceDetail> MaintenanceDetails { get; set; }
    public DbSet<MaintenanceScheduleConfig> MaintenanceScheduleConfigs { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    
    public DbSet<MediaFile> MediaFiles { get; set; }
    
    public DbSet<Replacement> Replacements { get; set; }
    public DbSet<ReplacementDetail> ReplacementDetails { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Transportation> Transportations { get; set; }
    public DbSet<TransportationDetail> TransportationDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new TokenConfig());
        modelBuilder.ApplyConfiguration(new CampusConfig());
        modelBuilder.ApplyConfiguration(new BuildingConfig());
        modelBuilder.ApplyConfiguration(new FloorConfig());
        modelBuilder.ApplyConfiguration(new RoomConfig());
        modelBuilder.ApplyConfiguration(new AssetConfig());
        modelBuilder.ApplyConfiguration(new RoomAssetConfig());
        modelBuilder.ApplyConfiguration(new AssetCategoryConfig());
        
        modelBuilder.ApplyConfiguration(new MaintenanceConfig());
        modelBuilder.ApplyConfiguration(new MaintenanceDetailConfig());
        
        modelBuilder.ApplyConfiguration(new RoomStatusConfig());
        modelBuilder.ApplyConfiguration(new MediaFileConfig());
        
        modelBuilder.ApplyConfiguration(new MaintenanceScheduleConfigConfig());
        
        modelBuilder.ApplyConfiguration(new ReplacementConfig());
        modelBuilder.ApplyConfiguration(new ReplacementDetailConfig());
        modelBuilder.ApplyConfiguration(new NotificationConfig());
        modelBuilder.ApplyConfiguration(new TransportationConfig());
        modelBuilder.ApplyConfiguration(new TransportationDetailConfig());
    }
}