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
    //public DbSet<MaintenanceScheduleConfig> MaintenanceScheduleConfigs { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    
    public DbSet<MediaFile> MediaFiles { get; set; }
    
    public DbSet<Replacement> Replacements { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Transportation> Transportations { get; set; }
    public DbSet<Repair> Repairs { get; set; }
    public DbSet<AssetCheck> AssetChecks { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Model> Models { get; set; }
    //public DbSet<ActionRequest> ActionRequests { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<TransportationDetail> TransportationDetails { get; set; }
    public DbSet<Brand> Brands { get; set; }
    
    public DbSet<InventoryCheck> InventoryChecks { get; set; }
    public DbSet<InventoryCheckDetail> InventoryCheckDetails { get; set; }
    public DbSet<InventoryCheckConfig> InventoryCheckConfigs { get; set; }
    public DbSet<InventoryDetailConfig> InventoryDetailConfigs { get; set; }

    
    // ***** VIEW ***** //
    public DbSet<TaskView> Tasks { get; set; }
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
        
        modelBuilder.ApplyConfiguration(new RoomStatusConfig());
        modelBuilder.ApplyConfiguration(new MediaFileConfig());
        
        //modelBuilder.ApplyConfiguration(new MaintenanceScheduleConfigConfig());
        
        modelBuilder.ApplyConfiguration(new ReplacementConfig());
        modelBuilder.ApplyConfiguration(new NotificationConfig());
        modelBuilder.ApplyConfiguration(new TransportationConfig());
        modelBuilder.ApplyConfiguration(new RepairConfig());
        modelBuilder.ApplyConfiguration(new AssetCheckConfig());
        
        modelBuilder.ApplyConfiguration(new TeamConfig());
        modelBuilder.ApplyConfiguration(new ModelConfig());
        modelBuilder.ApplyConfiguration(new CategoryConfig());
        modelBuilder.ApplyConfiguration(new RoomTypeConfig());
        //modelBuilder.ApplyConfiguration(new RequestConfig());
        modelBuilder.ApplyConfiguration(new TeamMemberConfig());
        modelBuilder.ApplyConfiguration(new TransportationDetailConfig());
        modelBuilder.ApplyConfiguration(new BrandConfig());
        
        modelBuilder.ApplyConfiguration(new InventoryCheckDbConfig());
        modelBuilder.ApplyConfiguration(new InventoryCheckConfigConfig());
        modelBuilder.ApplyConfiguration(new InventoryCheckDetailConfig());
        modelBuilder.ApplyConfiguration(new TaskViewConfig());
    }
}