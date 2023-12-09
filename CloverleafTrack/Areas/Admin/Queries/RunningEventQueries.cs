namespace CloverleafTrack.Areas.Admin.Queries;

public static class RunningEventQueries
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
            RunningEvents events
        ORDER BY
            events.SortOrder;
        """;
    
    public const string CreateEventSql =
        """
        INSERT INTO RunningEvents
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
        UPDATE RunningEvents
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
        UPDATE RunningEvents
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}