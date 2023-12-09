namespace CloverleafTrack.Queries;

public static class EventQueries
{
    public const string AllFieldEventsSql = 
        """
        SELECT
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted
        FROM
            FieldEvents event
        WHERE
            (event.Deleted = 0 OR event.Deleted IS NULL);
        """;
    
    public const string AllFieldRelayEventsSql = 
        """
        SELECT
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted
        FROM
            FieldRelayEvents event
        WHERE
            (event.Deleted = 0 OR event.Deleted IS NULL);
        """;
    
    public const string AllRunningEventsSql = 
        """
        SELECT
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted
        FROM
            RunningEvents event
        WHERE
            (event.Deleted = 0 OR event.Deleted IS NULL);
        """;
    
    public const string AllRunningRelayEventsSql = 
        """
        SELECT
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted
        FROM
            RunningRelayEvents event
        WHERE
            (event.Deleted = 0 OR event.Deleted IS NULL);
        """;
}
