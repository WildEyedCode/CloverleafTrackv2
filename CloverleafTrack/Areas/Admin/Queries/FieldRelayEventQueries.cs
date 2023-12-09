namespace CloverleafTrack.Areas.Admin.Queries;

public static class FieldRelayEventQueries
{
    public const string AllEventsSql =
        """
        SELECT
            events.Id,
            events.Name,
            events.Gender,
            events.SortOrder,
            events.DateCreated,
            events.DateUpdated,
            events.DateDeleted,
            events.Environment,
            events.Deleted
        FROM
            FieldRelayEvents events
        ORDER BY
            events.SortOrder;
        """;
    
    public const string CreateEventSql =
        """
        INSERT INTO FieldRelayEvents
        (
            Id,
            Name,
            Gender,
            SortOrder,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Environment,
            Deleted
        )
        VALUES
        (
            @Id,
            @Name,
            @Gender,
            @SortOrder,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Environment,
            @Deleted
        );
        """;
    
    public const string UpdateEventSql =
        """
        UPDATE FieldRelayEvents
        SET
            Name = @Name,
            Gender = @Gender,
            Environment = @Environment,
            SortOrder = @SortOrder,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;
    
    public const string DeleteEventSql =
        """
        UPDATE FieldRelayEvents
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}