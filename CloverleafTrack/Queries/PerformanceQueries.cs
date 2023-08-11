namespace CloverleafTrack.Queries;

public static class PerformanceQueries
{
    public const string SelectAllFieldPerformancesSql = 
        @"SELECT
            performance.Id,
            performance.SchoolRecord,
            performance.SeasonBest,
            performance.PersonalBest,
            performance.Feet,
            performance.Inches,
            performance.DateCreated,
            performance.DateUpdated,
            performance.DateDeleted,
            performance.Deleted,
            performance.FieldEventId as EventId,
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted,
            performance.MeetId,
            meet.Id,
            meet.Date,
            meet.Name,
            meet.AllResultsIn,
            meet.Outdoor,
            meet.Location,
            meet.HandTimed,
            meet.Environment,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted,
            meet.SeasonId,
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted,
            performance.AthleteId,
            athlete.Id,
            athlete.FirstName,
            athlete.LastName,
            athlete.Gender,
            athlete.GraduationYear,
            athlete.DateCreated,
            athlete.DatedUpdated,
            athlete.DateDeleted,
            athlete.Deleted
        FROM
            FieldPerformance performance,
            FieldEvents event,
            Meets meet,
            Seasons season,
            Athletes athlete
        WHERE
            performance.FieldEventId = event.Id
            AND performance.MeetId = meet.Id
            AND performance.AthleteId = athlete.Id
            AND meet.SeasonId = season.Id;";
    
    public const string SelectAllFieldRelayPerformancesSql = 
        @"SELECT
            performance.Id,
            performance.SchoolRecord,
            performance.SeasonBest,
            performance.PersonalBest,
            performance.Feet,
            performance.Inches,
            performance.DateCreated,
            performance.DateUpdated,
            performance.DateDeleted,
            performance.Deleted,
            performance.FieldRelayEventId as EventId,
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted,
            performance.MeetId,
            meet.Id,
            meet.Date,
            meet.Name,
            meet.AllResultsIn,
            meet.Outdoor,
            meet.Location,
            meet.HandTimed,
            meet.Environment,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted,
            meet.SeasonId,
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted,
            performance.AthleteIds
        FROM
            FieldRelayPerformance performance,
            FieldRelayEvents event,
            Meets meet,
            Seasons season
        WHERE
            performance.FieldRelayEventId = event.Id
            AND performance.MeetId = meet.Id
            AND meet.SeasonId = season.Id;";
    
    public const string SelectAthletesForFieldRelayPerformanceSql = 
        @"SELECT
            athlete.Id,
            athlete.FirstName,
            athlete.LastName,
            athlete.Gender,
            athlete.GraduationYear,
            athlete.DateCreated,
            athlete.DatedUpdated,
            athlete.DateDeleted,
            athlete.Deleted
        FROM
            Athletes athlete,
            FieldRelayPerformanceAthletes lookup
        WHERE
            athlete.Id = lookup.AthleteId
            AND lookup.FieldRelayPerformanceId = @PerformanceId
        ORDER BY
            athlete.LastName,
            athlete.FirstName;";
    
    public const string SelectAllRunningPerformancesSql = 
        @"SELECT
            performance.Id,
            performance.SchoolRecord,
            performance.SeasonBest,
            performance.PersonalBest,
            performance.Minutes,
            performance.Seconds,
            performance.DateCreated,
            performance.DateUpdated,
            performance.DateDeleted,
            performance.Deleted,
            performance.RunningEventId as EventId,
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted,
            performance.MeetId,
            meet.Id,
            meet.Date,
            meet.Name,
            meet.AllResultsIn,
            meet.Outdoor,
            meet.Location,
            meet.HandTimed,
            meet.Environment,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted,
            meet.SeasonId,
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted,
            performance.AthleteId,
            athlete.Id,
            athlete.FirstName,
            athlete.LastName,
            athlete.Gender,
            athlete.GraduationYear,
            athlete.DateCreated,
            athlete.DatedUpdated,
            athlete.DateDeleted,
            athlete.Deleted
        FROM
            RunningPerformance performance,
            RunningEvents event,
            Meets meet,
            Seasons season,
            Athletes athlete
        WHERE
            performance.RunningEventId = event.Id
            AND performance.MeetId = meet.Id
            AND performance.AthleteId = athlete.Id
            AND meet.SeasonId = season.Id;";
    
    public const string SelectAllRunningRelayPerformancesSql = 
        @"SELECT
            performance.Id,
            performance.SchoolRecord,
            performance.SeasonBest,
            performance.PersonalBest,
            performance.Minutes,
            performance.Seconds,
            performance.DateCreated,
            performance.DateUpdated,
            performance.DateDeleted,
            performance.Deleted,
            performance.RunningRelayEventId as EventId,
            event.Id,
            event.Name,
            event.Gender,
            event.SortOrder,
            event.DateCreated,
            event.DateUpdated,
            event.DateDeleted,
            event.Environment,
            event.Deleted,
            performance.MeetId,
            meet.Id,
            meet.Date,
            meet.Name,
            meet.AllResultsIn,
            meet.Outdoor,
            meet.Location,
            meet.HandTimed,
            meet.Environment,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted,
            meet.SeasonId,
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted,
            performance.AthleteIds
        FROM
            RunningRelayPerformance performance,
            RunningRelayEvents event,
            Meets meet,
            Seasons season
        WHERE
            performance.RunningRelayEventId = event.Id
            AND performance.MeetId = meet.Id AND meet.SeasonId = season.Id;";
    
    public const string SelectAthletesForRunningRelayPerformanceSql = 
        @"SELECT
            athlete.Id,
            athlete.FirstName,
            athlete.LastName,
            athlete.Gender,
            athlete.GraduationYear,
            athlete.DateCreated,
            athlete.DatedUpdated,
            athlete.DateDeleted,
            athlete.Deleted
        FROM
            Athletes athlete,
            RunningRelayPerformanceAthletes lookup
        WHERE
            athlete.Id = lookup.AthleteId
            AND lookup.RunningRelayPerformanceId = @PerformanceId
        ORDER BY
            athlete.LastName,
            athlete.FirstName;";
}
