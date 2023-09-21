    using AppCore.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace MainData.Entities;

    public class Maintenance : BaseEntity
    {
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public MaintenanceStatus Status { get; set; }
        public MaintenanceType Type { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? AssetId { get; set; }
        
        //
        public virtual User? PersonInCharge { get; set; }
        public virtual Asset? Asset { get; set; }
        
    }

    public enum MaintenanceType
    {
        Permanent = 1,
        Unexpected  = 2
    }

    public enum MaintenanceStatus
    {
        NotStarted = 1,
        InProgress = 1,
        Completed = 3,
        Cancelled = 4,
    }

    public class MaintenanceConfig : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.ToTable("Maintenances");
            builder.Property(x => x.RequestedDate).IsRequired();
            builder.Property(x => x.CompletionDate).IsRequired(false);
            builder.Property(x => x.Description).IsRequired(false);
            builder.Property(x => x.Note).IsRequired(false);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.AssignedTo).IsRequired(false);
       
            //Relationship
            
            // builder.HasOne(x => x.Creator)
            //     .WithMany(x => x.Maintenances)
            //     .HasForeignKey(x => x.CreatorId);
            
            // builder.HasOne(x => x.PersonInCharge)
            //     .WithMany(x => x.Maintenances)
            //     .HasForeignKey(x => x.AssignedTo);
            
            builder.HasOne(x => x.Asset)
                .WithMany(x => x.Maintenances)
                .HasForeignKey(x => x.AssetId);

        }
    }
