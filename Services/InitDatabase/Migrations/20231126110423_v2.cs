using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.Sql(@"
               	CREATE VIEW Tasks AS 
                SELECT 
                t3.Type,
                t3.Id,
                t3.CreatorId,
                t3.EditorId,
                t3.CreatedAt,
                t3.EditedAt,
                t3.ToRoomId,
                t3.Quantity,
                t3.AssignedTo,
                t3.CompletionDate,
                t3.Description,
                t3.Notes,
                t3.IsInternal,
                t3.RequestCode,
                t3.RequestDate,
                t3.Status,
                t3.AssetId,
                t3.Result,
                t3.Checkin,
                t3.Checkout,
                t3.AssetTypeId,
                t3.CategoryId,
                t3.Priority,
                t3.DeletedAt,
                t3.DeleterId
                FROM (
                SELECT 
                    5 AS Type, 
                    t.Id, 
                    COALESCE(t.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(t.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    t.CreatedAt, 
                    t.EditedAt, 
                    t.ToRoomId, 
                    t.Quantity, 
                    t.AssignedTo, 
                    t.CompletionDate, 
                    t.Description, 
                    t.IsInternal, 
                    t.Notes, 
                    t.RequestCode, 
                    t.RequestDate, 
                    t.Status,
                    Null as AssetId,
                    t.Result,
                    t.Checkin,
                    t.Checkout,
                    NULL as AssetTypeId,
                    NULL as CategoryId,
                    t.Priority,
                    t.DeletedAt,
	                t.DeleterId
                FROM Transportations t
                UNION
                SELECT 
                    2 AS Type, 
                    m.Id, 
                    COALESCE(m.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(m.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    m.CreatedAt, 
                    m.EditedAt, 
                    NULL AS ToRoomId, 
                    NULL AS Quantity, 
                    m.AssignedTo, 
                    m.CompletionDate, 
                    m.Description, 
                    m.IsInternal, 
                    m.Notes, 
                    m.RequestCode, 
                    m.RequestDate, 
                    m.Status,
                    m.AssetId,
                    m.Result,
                    m.Checkin,
                    m.Checkout,
                    m.AssetTypeId as AssetTypeId,
                    m.CategoryId as CategoryId,
                    m.Priority,
                    m.DeletedAt,
	                m.DeleterId
                FROM Maintenances m
                UNION
                SELECT 
                    1 AS Type, 
                    a.Id, 
                    COALESCE(a.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(a.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    a.CreatedAt, 
                    a.EditedAt, 
                    NULL AS ToRoomId, 
                    NULL AS Quantity, 
                    a.AssignedTo, 
                    a.CompletionDate, 
                    a.Description, 
                    a.IsInternal, 
                    a.Notes, 
                    a.RequestCode, 
                    a.RequestDate, 
                    a.Status,
                    a.AssetId,
                    a.Result,
                    a.Checkin,
                    a.Checkout,
                    NULL as AssetTypeId,
                    NULL as CategoryId,
                    NULL as Priority,
                    a.DeletedAt,
	                a.DeleterId
                FROM AssetChecks a
                UNION
                SELECT 
                    4 AS Type, 
                    r.Id, 
                    COALESCE(r.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(r.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    r.CreatedAt, 
                    r.EditedAt, 
                    NULL AS ToRoomId, 
                    NULL AS Quantity, 
                    r.AssignedTo, 
                    r.CompletionDate, 
                    r.Description, 
                    r.IsInternal, 
                    r.Notes, 
                    r.RequestCode, 
                    r.RequestDate, 
                    r.Status,
                    r.AssetId,
                    r.Result,
                    r.Checkin,
                    r.Checkout,
                    r.AssetTypeId,
                    r.CategoryId,
                    r.Priority,
                    r.DeletedAt,
	                r.DeleterId
                FROM Replacements r
                UNION
                SELECT 
                    3 AS Type, 
                    r0.Id, 
                    COALESCE(r0.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(r0.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    r0.CreatedAt, 
                    r0.EditedAt, 
                    NULL AS ToRoomId, 
                    NULL AS Quantity, 
                    r0.AssignedTo, 
                    r0.CompletionDate, 
                    r0.Description, 
                    r0.IsInternal, 
                    r0.Notes, 
                    r0.RequestCode, 
                    r0.RequestDate, 
                    r0.Status,
                    r0.AssetId,
                    r0.Result,
                    r0.Checkin,
                    r0.Checkout,
                    r0.AssetTypeId as AssetTypeId,
                    r0.CategoryId as CategoryId,
                    r0.Priority,
                    r0.DeletedAt,
	                r0.DeleterId
                FROM Repairs r0
                UNION
                SELECT 
                    6 AS Type, 
                    iven.Id, 
                    COALESCE(iven.CreatorId, '00000000-0000-0000-0000-000000000000') AS CreatorId,
                    COALESCE(iven.EditorId, '00000000-0000-0000-0000-000000000000') AS EditorId,
                    iven.CreatedAt, 
                    iven.EditedAt, 
                    NULL AS ToRoomId, 
                    NULL AS Quantity, 
                    iven.AssignedTo, 
                    iven.CompletionDate, 
                    iven.Description, 
                    iven.IsInternal, 
                    iven.Notes, 
                    iven.RequestCode, 
                    iven.RequestDate, 
                    iven.Status,
                    null as AssetId,
                    iven.Result,
                    iven.Checkin,
                    iven.Checkout,
                    null as AssetTypeId,
                    null as CategoryId,
                    iven.Priority,
                    iven.DeletedAt,
	                iven.DeleterId
                FROM InventoryChecks iven) AS t3
                WHERE t3.DeletedAt is null");
        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
