namespace CloverleafTrack.Areas.Admin.Queries;

public static class RunningPerformanceQueries
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
            athlete.DateUpdated,
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
            AND meet.SeasonId = season.Id;
        """;

    public const string CreatePerformanceSql =
        """
        INSERT INTO RunningPerformance
        (
            Id,
            Time,
            SchoolRecord,
            SeasonBest,
            PersonalBest,
            RunningEventId,
            MeetId,
            AthleteId,
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
            @AthleteId,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted,
            @Minutes,
            @Seconds
        );
        """;

    public const string UpdatePerformanceSql =
        """
        UPDATE RunningPerformance
        SET
            Time = @Time,
            SchoolRecord = @SchoolRecord,
            SeasonBest = @SeasonBest,
            PersonalBest = @PersonalBest,
            RunningEventId = @EventId,
            MeetId = @MeetId,
            AthleteId = @AthleteId,
            Minutes = @Minutes,
            Seconds = @Seconds,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string DeletePerformanceSql =
        """
        UPDATE RunningPerformance
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
    
    public const string ClearAllRecordsSql =
        """
        UPDATE RunningPerformance
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