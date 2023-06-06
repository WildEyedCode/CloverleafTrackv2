namespace CloverleafTrack.Queries;

public static class AthleteQueries
{
    public const string SelectAllAthletesSql = 
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
            Athletes athlete;";
}
