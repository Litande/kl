using System.Text.Json;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Entities.Settings;

namespace KL.Caller.Leads.Seed;

public static class SeedSettings
{
    public static void Seed(KlDbContext context, long clientId)
    {
        CallFinishedReason(context, clientId);
        CallNaSettings(context, clientId);
    }

    private static void CallFinishedReason(KlDbContext context, long clientId)
    {
        var entity = context.Settings.FirstOrDefault(r => r.Type == SettingTypes.CallFinishedReason && r.ClientId == clientId);
        if (entity is not null) return;

        var value = new CallFinishedSettings
        {
            Default = LeadStatusTypes.NA,
            LeadInvalidPhone = LeadStatusTypes.WrongNumber,
            LeadNotAnswered = LeadStatusTypes.SystemAnswer,
            NoAvailableAgents = LeadStatusTypes.SystemFailedToConnect,
            SystemIssues = LeadStatusTypes.SystemFailedToConnect,
        };

        var valueJson = JsonSerializer.Serialize(value, JsonSettingsExtensions.Default);

        var settings = new SettingsEntity
        {
            Type = SettingTypes.CallFinishedReason,
            ClientId = clientId,
            Value = valueJson,
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }

    private static void CallNaSettings(KlDbContext context, long clientId)
    {
        var entity = context.Settings.FirstOrDefault(r => r.Type == SettingTypes.CallNa && r.ClientId == clientId);
        if (entity is not null) return;

        var value = new CallNaSettings(LeadStatusTypes.Busy, "Default Agent Comment");
        var valueJson = JsonSerializer.Serialize(value, JsonSettingsExtensions.Default);

        var settings = new SettingsEntity
        {
            Type = SettingTypes.CallNa,
            ClientId = clientId,
            Value = valueJson,
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }

}
