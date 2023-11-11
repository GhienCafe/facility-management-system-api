using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class TaskView : BaseEntity
{
    public Guid? AssetId { get; set; }
    public string? RequestCode { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public RequestStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? Result { get; set; }
    public bool? IsInternal { get; set; }
    public DateTime? Checkin { get; set; }
    public DateTime? Checkout { get; set; }
    public Guid? AssignedTo { get; set; }    
    public Guid? AssetTypeId { get; set; }
    public Guid? CategoryId { get; set; }
    public Priority? Priority { get; set; }
    public RequestType Type { get; set; }
    public int? Quantity { get; set; }
    public Guid? ToRoomId { get; set; }
}

public class TaskViewConfig : IEntityTypeConfiguration<TaskView>
{
    public void Configure(EntityTypeBuilder<TaskView> builder)
    {
        builder.ToView("Tasks")
            .HasKey(x => x.Id);
    }
}   