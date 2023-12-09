namespace CloverleafTrack.Queries;

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
            season.Deleted,
            meet.Id AS MeetId,
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
            Seasons season,
            Meets meet
        WHERE
            season.Id = meet.SeasonId
            AND (season.Deleted = 0 OR season.Deleted IS NULL)
            AND (meet.Deleted = 0 OR meet.Deleted IS NULL);
        """;
}
