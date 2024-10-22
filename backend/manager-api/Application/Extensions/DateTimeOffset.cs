namespace KL.Manager.API.Application.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset? FromUnixTimeSeconds(this long? value)
    {
        return value.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(value.Value)
            : null;
    }
}
