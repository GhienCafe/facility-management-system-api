using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW Tasks AS
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
                            t3.Status
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
                                t.Status
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
                                m.Status
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
                                a.Status
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
                                r.Status
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
                                r0.Status
                            FROM Repairs r0
                        ) AS t3");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view Tasks;
                ");
        }
    }
}
