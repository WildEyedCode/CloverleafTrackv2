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
            FieldEvents event;
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
            FieldRelayEvents event;
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
            RunningEvents event;
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
            RunningRelayEvents event;
        """;
}
