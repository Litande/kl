using System.Text.Json;
using KL.Agent.API.Application.Common;
using KL.Agent.API.Persistent.Entities.Settings;

namespace KL.Agent.API.Application.Extensions;

public static class StringExtensions
{
    public static FeedbackSettings? FeedbackSettingsDeserialize(this string settingsJson)
    {
        var settings = JsonSerializer.Deserialize<FeedbackSettings>(settingsJson, JsonSettingsExtensions.Default);

        return settings;
    }
}
