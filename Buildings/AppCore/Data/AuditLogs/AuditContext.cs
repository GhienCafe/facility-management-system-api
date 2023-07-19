using Microsoft.EntityFrameworkCore;

namespace AppCore.Data.AuditLogs
{
    public class AuditContext : DbContext
    {
        public AuditContext(DbContextOptions<AuditContext> options) : base(options)
        {
        }


        public DbSet<AuditLog> AuditLogs { set; get; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuditLogConfig());
        }
    }
}