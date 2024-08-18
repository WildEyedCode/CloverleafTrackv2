namespace CloverleafTrack.Areas.Admin.Queries;

public static class RunningRelayPerformanceQueries
{
    public const string AllPerformancesSql =
        """
        SELECT
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
            AND performance.MeetId = meet.Id AND meet.SeasonId = season.Id;
        """;
    
    public const string AthletesForRelayPerformanceSql = 
        """
        SELECT
            athlete.Id,
            athlete.FirstName,
            athlete.LastName,
            athlete.Gender,
            athlete.GraduationYear,
            athlete.DateCreated,
            athlete.DateUpdated,
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
            athlete.FirstName;
        """;

    public const string CreatePerformanceSql =
        """
        INSERT INTO RunningRelayPerformance
        (
            Id,
            Time,
            SchoolRecord,
            SeasonBest,
            PersonalBest,
            RunningRelayEventId,
            MeetId,
            AthleteIds,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Deleted,
            Minutes,
            Seconds
        )
        VALUES 
        (
            @Id,
            @Time,
            @SchoolRecord,
            @SeasonBest,
            @PersonalBest,
            @EventId,
            @MeetId,
            null,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted,
            @Minutes,
            @Seconds
        );
        """;

    public const string CreateAthleteForRelayPerformanceSql =
        """
        INSERT INTO RunningRelayPerformanceAthletes
        (
            Id,
            RunningRelayPerformanceId,
            AthleteId,
            DateCreated,
            DateUpdated
        )
        VALUES
        (
            @Id,
            @PerformanceId,
            @AthleteId,
            @DateCreated,
            @DateUpdated
        );
        """;

    public const string UpdatePerformanceSql =
        """
        UPDATE RunningRelayPerformance
        SET
            Time = @Time,
            SchoolRecord = @SchoolRecord,
            SeasonBest = @SeasonBest,
            PersonalBest = @PersonalBest,
            RunningRelayEventId = @EventId,
            MeetId = @MeetId,
            Minutes = @Minutes,
            Seconds = @Seconds,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string RemoveAllAthletesForRelayPerformanceSql =
        """
        DELETE FROM
            RunningRelayPerformanceAthletes
        WHERE
            RunningRelayPerformanceId = @PerformanceId;
        """;

    public const string DeletePerformanceSql =
        """
        UPDATE RunningRelayPerformance
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;

    public const string DeleteAllAthletesForRelayPerformanceSql =
        """
        UPDATE RunningRelayPerformanceAthletes
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            RunningRelayPerformanceId = @PerformanceId;
        """;
    
    public const string ClearAllRecordsSql =
        """
        UPDATE RunningRelayPerformance
        SET
            PersonalBest = 0,
            SeasonBest = 0,
            SchoolRecord = 0
        WHERE
            PersonalBest = 1
            OR SeasonBest = 1
            OR SchoolRecord = 1;
        """;
}