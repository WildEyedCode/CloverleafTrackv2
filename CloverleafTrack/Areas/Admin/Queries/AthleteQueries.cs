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
            Athletes athlete;
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
}