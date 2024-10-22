namespace KL.Agent.API.Application.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset? FromUnixTimeSeconds(long? value)
    {
        return value.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(value.Value)
            : null;
    }
}
