namespace CloverleafTrack.Queries;

public static class MeetQueries
{
    public const string AllMeetsSql = 
        """
        SELECT
            meet.Id,
            meet.Date,
            meet.Name,
            meet.Location,
            meet.Outdoor,
            meet.AllResultsIn,
            meet.HandTimed,
            meet.Environment,
            meet.SeasonId,
            meet.DateCreated,
            meet.DateUpdated,
            meet.DateDeleted,
            meet.Deleted,
            season.Id,
            season.Name,
            season.DateCreated,
            season.DateUpdated,
            season.DateDeleted,
            season.Deleted
        FROM
            Meets meet,
            Seasons season
        WHERE
            meet.SeasonId = season.Id;
        """;
}
