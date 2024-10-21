namespace Plat4Me.DialAgentApi.Application.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset? FromUnixTimeSeconds(long? value)
    {
        return value.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(value.Value)
            : null;
    }
}
