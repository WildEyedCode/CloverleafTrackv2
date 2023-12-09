namespace CloverleafTrack.Areas.Admin.Queries;

public static class MeetQueries
{
    public const string AllMeetsSql =
        """
        SELECT
            meet.Id,
            meet.Date,
            meet.Name, 
            meet.SeasonId,
            meet.AllResultsIn,
            meet.Outdoor,
            meet.Location,
            meet.HandTimed,
            meet.Environment,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted
        FROM
            Meets meet
        ORDER BY
            meet.Date;
        """;

    public const string CreateMeetSql =
        """
        INSERT INTO Meets
        (
            Id,
            Date,
            Name,
            SeasonId,
            AllResultsIn,
            Outdoor,
            Location,
            HandTimed,
            Environment,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Deleted
        )
        VALUES 
        (
            @Id,
            @Date,
            @Name,
            @SeasonId,
            @AllResultsIn,
            @Outdoor,
            @Location,
            @HandTimed,
            @Environment,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted
        );
        """;

    public const string UpdateMeetSql =
        """
        UPDATE Meets
        SET
            Date = @Date,
            Name = @Name,
            SeasonId = @SeasonId,
            AllResultsIn = @AllResultsIn,
            Outdoor = @Outdoor,
            Location = @Location,
            HandTimed = @HandTimed,
            Environment = @Environment,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string DeleteMeetSql =
        """
        UPDATE Meets
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}