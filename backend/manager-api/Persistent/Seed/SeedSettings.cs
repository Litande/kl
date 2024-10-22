using System.Text.Json;
using KL.Manager.API.Application.Common;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Seed;

public static class SeedSettings
{
    public static void Seed(KlDbContext context, long clientId)
    {
        AddLeadImportDefaultStatusIfNeed(context, clientId);
        AddLeadStatisticSettingIfNeed(context, clientId);
    }

    private static void AddLeadImportDefaultStatusIfNeed(KlDbContext context, long clientId)
    {
        var entity = context.Settings.FirstOrDefault(r =>
            r.Type == SettingTypes.LeadImportDefaultStatus && r.ClientId == clientId);
        if (entity is not null) return;

        var value = new
        {
            LeadImportDefaultStatus = LeadStatusTypes.NewLead
        };

        var valueJson = JsonSerializer.Serialize(value, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var settings = new SettingsEntity
        {
            Type = SettingTypes.LeadImportDefaultStatus,
            ClientId = clientId,
            Value = valueJson,
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }

    private static void AddLeadStatisticSettingIfNeed(KlDbContext context, long clientId)
    {
        var entity =
            context.Settings.FirstOrDefault(r => r.Type == SettingTypes.LeadStatistic && r.ClientId == clientId);
        if (entity is not null) return;

        var value = new
        {
            LeadStatuses = new[]
            {
                LeadStatusTypes.NewLead,
                LeadStatusTypes.Busy,
                LeadStatusTypes.DNC,
                LeadStatusTypes.NA,
            }
        };

        var valueJson = JsonSerializer.Serialize(value, JsonSettingsExtensions.Default);

        var settings = new SettingsEntity
        {
            Type = SettingTypes.LeadStatistic,
            ClientId = clientId,
            Value = valueJson,
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }
}
