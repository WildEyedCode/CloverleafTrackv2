namespace CloverleafTrack.Queries;

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
}
