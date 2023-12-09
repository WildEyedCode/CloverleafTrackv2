namespace CloverleafTrack.Areas.Admin.Queries;

public static class SeasonQueries
{
    public const string AllSeasonsSql =
        """
        SELECT
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted
        FROM
            Seasons season
        ORDER BY
            season.Name;
        """;

    public const string CreateSeasonSql =
        """
        INSERT INTO Seasons
        (
            Id,
            Name,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Deleted
        )
        VALUES
        (
            @Id,
            @Name,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted
        );
        """;

    public const string UpdateSeasonSql =
        """
        UPDATE Seasons
        SET
            Name = @Name,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string DeleteSeasonSql =
        """
        UPDATE Seasons
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}