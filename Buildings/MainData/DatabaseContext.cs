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
    public DbSet<AssetCategory> AssetCategories { get; set; }
    public DbSet<AssetRequest> AssetRequests { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<RequestParticipant> RequestParticipants { get; set; }
    public DbSet<RequestDetail> RequestDetails { get; set; }
    
    public DbSet<AssetHandover> AssetHandovers { get; set; }
    public DbSet<HandoverDetail> HandoverDetails { get; set; }
    public DbSet<HandoverParticipant> HandoverParticipants { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTeam> InventoryTeams { get; set; }
    public DbSet<InventoryTeamMember> InventoryTeamMembers { get; set; }
    public DbSet<InventoryDetail> InventoryDetails { get; set; }
    
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<MaintenanceParticipant> MaintenanceParticipants { get; set; }
    public DbSet<MaintenanceDetail> MaintenanceDetails { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    
    public DbSet<MediaFile> MediaFiles { get; set; }
    
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
        modelBuilder.ApplyConfiguration(new AssetRequestConfig());
        modelBuilder.ApplyConfiguration(new DepartmentConfig());
        modelBuilder.ApplyConfiguration(new RequestDetailConfig());
        modelBuilder.ApplyConfiguration(new RequestParticipantConfig());
        
        modelBuilder.ApplyConfiguration(new AssetHandoverConfig());
        modelBuilder.ApplyConfiguration(new HandoverDetailConfig());
        modelBuilder.ApplyConfiguration(new HandoverParticipantConfig());
        modelBuilder.ApplyConfiguration(new InventoryConfig());
        modelBuilder.ApplyConfiguration(new InventoryDetailConfig());
        modelBuilder.ApplyConfiguration(new InventoryTeamConfig());
        modelBuilder.ApplyConfiguration(new InventoryTeamMemberConfig());
        
        modelBuilder.ApplyConfiguration(new MaintenanceConfig());
        modelBuilder.ApplyConfiguration(new MaintenanceParticipantConfig());
        modelBuilder.ApplyConfiguration(new MaintenanceDetailConfig());
        
        modelBuilder.ApplyConfiguration(new RoomStatusConfig());
        modelBuilder.ApplyConfiguration(new MediaFileConfig());
    }
}