using Plat4Me.DialAgentApi.Application.Common;
using System.Text.Json;
using Plat4Me.DialAgentApi.Persistent.Entities.Settings;

namespace Plat4Me.DialAgentApi.Application.Extensions;

public static class StringExtensions
{
    public static FeedbackSettings? FeedbackSettingsDeserialize(this string settingsJson)
    {
        var settings = JsonSerializer.Deserialize<FeedbackSettings>(settingsJson, JsonSettingsExtensions.Default);

        return settings;
    }
}
