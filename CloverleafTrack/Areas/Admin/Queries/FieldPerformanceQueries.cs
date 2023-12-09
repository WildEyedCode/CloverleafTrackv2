namespace CloverleafTrack.Areas.Admin.Queries;

public static class FieldPerformanceQueries
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
            athlete.DateUpdated,
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
            AND meet.SeasonId = season.Id;
        """;

    public const string CreatePerformanceSql =
        """
        INSERT INTO FieldPerformance
        (
            Id,
            Distance,
            SchoolRecord,
            SeasonBest,
            PersonalBest,
            FieldEventId,
            MeetId,
            AthleteId,
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
            @AthleteId,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted,
            @Feet,
            @Inches
        );
        """;

    public const string UpdatePerformanceSql =
        """
        UPDATE FieldPerformance
        SET
            Distance = @Distance,
            SchoolRecord = @SchoolRecord,
            SeasonBest = @SeasonBest,
            PersonalBest = @PersonalBest,
            FieldEventId = @EventId,
            MeetId = @MeetId,
            AthleteId = @AthleteId,
            Feet = @Feet,
            Inches = @Inches,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string DeletePerformanceSql =
        """
        UPDATE FieldPerformance
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}