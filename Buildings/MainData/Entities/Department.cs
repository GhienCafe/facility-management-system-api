using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Department : BaseEntity
{
    public Guid CampusId { get; set; }
    public string DepartmentCode { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    
    //
    public virtual Campus? Campus { get; set; }
    public virtual IEnumerable<User>? Users { get; set; }
}

public class DepartmentConfig : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.Property(a => a.CampusId).IsRequired();
        builder.Property(a => a.DepartmentCode).IsRequired();
        builder.Property(a => a.DepartmentName).IsRequired();
        
        // Attributes
        builder.HasIndex(a => a.DepartmentCode).IsUnique();

        //Relationship
        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.CampusId);
        
        builder.HasMany(x => x.Users)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId);
    }
}   