using System.Text.Json;

namespace KL.Caller.Leads.Extensions;

public static class JsonHelper
{
    public static T? Deserialize<T>(string? value, long clientId, ILogger logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                logger.LogWarning("The {Type} JSON value for client " +
                                  "{ClientId} is empty or null", nameof(T), clientId);
                return default;
            }

            var settings = JsonSerializer.Deserialize<T?>(value, JsonSettingsExtensions.Default);
            if (settings is null)
                logger.LogError("The {Type} JSON value parsing" +
                                " failed for client {ClientId}", nameof(T), clientId);
            return settings;
        }
        catch (Exception e)
        {
            logger.LogError(e, "The {Type} JSON value parsing" +
                               " failed for client {ClientId}", nameof(T), clientId);
            return default;
        }
    }
}