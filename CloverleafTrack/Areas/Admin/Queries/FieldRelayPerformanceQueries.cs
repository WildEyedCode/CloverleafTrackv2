namespace CloverleafTrack.Areas.Admin.Queries;

public static class FieldRelayPerformanceQueries
{
    public const string AllPerformancesSql =
        """
        SELECT
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
            AND meet.SeasonId = season.Id;
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
            FieldRelayPerformanceAthletes lookup
        WHERE
            athlete.Id = lookup.AthleteId
            AND lookup.FieldRelayPerformanceId = @PerformanceId
        ORDER BY
            athlete.LastName,
            athlete.FirstName;
        """;

    public const string CreatePerformanceSql =
        """
        INSERT INTO FieldRelayPerformance
        (
            Id,
            Distance,
            SchoolRecord,
            SeasonBest,
            PersonalBest,
            FieldRelayEventId,
            MeetId,
            AthleteIds,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Deleted,
            Feet,
            Inches
        )
        VALUES 
        (
            @Id,
            @Distance,
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
            @Feet,
            @Inches
        );
        """;

    public const string CreateAthleteForRelayPerformanceSql =
        """
        INSERT INTO FieldRelayPerformanceAthletes
        (
            Id,
            FieldRelayPerformanceId,
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
        UPDATE FieldRelayPerformance
        SET
            Distance = @Distance,
            SchoolRecord = @SchoolRecord,
            SeasonBest = @SeasonBest,
            PersonalBest = @PersonalBest,
            FieldRelayEventId = @EventId,
            MeetId = @MeetId,
            Feet = @Feet,
            Inches = @Inches,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string RemoveAllAthletesForRelayPerformanceSql =
        """
        DELETE FROM
            FieldRelayPerformanceAthletes
        WHERE
            FieldRelayPerformanceId = @PerformanceId;
        """;

    public const string DeletePerformanceSql =
        """
        UPDATE FieldRelayPerformance
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;

    public const string DeleteAllAthletesForRelayPerformanceSql =
        """
        UPDATE FieldRelayPerformanceAthletes
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            FieldRelayPerformanceId = @PerformanceId;
        """;
}