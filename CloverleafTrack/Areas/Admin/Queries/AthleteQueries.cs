namespace CloverleafTrack.Areas.Admin.Queries;

public static class AthleteQueries
{
    public const string AllAthletesSql = 
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
            Athletes athlete
        ORDER BY
            athlete.LastName,
            athlete.FirstName;
        """;

    public const string CreateAthleteSql =
        """
        INSERT INTO Athletes
        (
            Id,
            FirstName,
            LastName,
            Gender,
            GraduationYear,
            DateCreated,
            DateUpdated,
            DateDeleted,
            Deleted
        )
        VALUES
        (
            @Id,
            @FirstName,
            @LastName,
            @Gender,
            @GraduationYear,
            @DateCreated,
            @DateUpdated,
            @DateDeleted,
            @Deleted
        );
        """;

    public const string UpdateAthleteSql =
        """
        UPDATE Athletes
        SET
            FirstName = @FirstName,
            LastName = @LastName,
            Gender = @Gender,
            GraduationYear = @GraduationYear,
            DateUpdated = @DateUpdated
        WHERE
            Id = @Id;
        """;

    public const string DeleteAthleteSql =
        """
        UPDATE Athletes
        SET
            Deleted = @Deleted,
            DateDeleted = @DateDeleted
        WHERE
            Id = @Id;
        """;
}