namespace CloverleafTrack.Queries;

public static class EventQueries
{
    public const string SelectAllFieldEventsSql = 
        @"SELECT
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
            FieldEvents event;";
    
    public const string SelectAllFieldRelayEventsSql = 
        @"SELECT
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
            FieldRelayEvents event;";
    
    public const string SelectAllRunningEventsSql = 
        @"SELECT
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
            RunningEvents event;";
    
    public const string SelectAllRunningRelayEventsSql = 
        @"SELECT
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
            RunningRelayEvents event;";
}
