using System.Text.Json;
using KL.Agent.API.Application.Common;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Settings;

namespace KL.Agent.API.Persistent.Seed;

public static class SeedSettings
{
    public static void Seed(DialDbContext context, long clientId)
    {
        FeedbackSettings(context, clientId);
    }

    private static void FeedbackSettings(DialDbContext context, long clientId)
    {
        var entity = context.Settings.FirstOrDefault(r => r.Type == SettingTypes.Feedback && r.ClientId == clientId);
        if (entity is not null) return;

        var value = new FeedbackSettings
        (
            PageTimeout: 300,
            DefaultStatus: LeadStatusTypes.NewLead,
            RedialsLimit: 0
        );

        var valueJson = JsonSerializer.Serialize(value, JsonSettingsExtensions.Default);

        var settings = new SettingsEntity
        {
            Type = SettingTypes.Feedback,
            ClientId = clientId,
            Value = valueJson,
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }
}
