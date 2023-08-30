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
    public DbSet<Campus> Campus { get; set; }
    public DbSet<Buildings> Buildings { get; set; }
    public DbSet<Floors> Floors { get; set; }
    public DbSet<Rooms> Rooms { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new TokenConfig());
        modelBuilder.ApplyConfiguration(new CampusConfig());
        modelBuilder.ApplyConfiguration(new BuildingsConfig());
        modelBuilder.ApplyConfiguration(new FloorsConfig());
        modelBuilder.ApplyConfiguration(new RoomsConfig());
    }
}