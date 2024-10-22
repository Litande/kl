namespace KL.Caller.Leads.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset? FromUnixTimeSeconds(this long? unixTimeSeconds)
    {
        if (unixTimeSeconds is null) return null;

        return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds.Value);
    }
}