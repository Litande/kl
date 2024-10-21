using System.Text.Json;
using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Models.Responses;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

public class GetNextFromLeadQueueHandler : IGetNextFromLeadQueueHandler
{
    private readonly ILogger<GetNextFromLeadQueueHandler> _logger;
    private readonly ILeadsQueueStore _leadsQueueStore;
    private readonly ILeadsQueueUpdateNotificationHandler _leadsQueueUpdateNotificationHandler;
    private readonly ILeadRepository _leadRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;

    public GetNextFromLeadQueueHandler(
        ILogger<GetNextFromLeadQueueHandler> logger,
        ILeadsQueueStore leadsQueueStore,
        ILeadsQueueUpdateNotificationHandler leadsQueueUpdateNotificationHandler,
        ILeadRepository leadRepository,
        ISettingsRepository settingsRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository)
    {
        _logger = logger;
        _leadsQueueStore = leadsQueueStore;
        _leadsQueueUpdateNotificationHandler = leadsQueueUpdateNotificationHandler;
        _leadRepository = leadRepository;
        _settingsRepository = settingsRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
    }

    public async Task<GetNextLeadResponse?> Process(
        long clientId,
        long queueId,
        IReadOnlyCollection<long>? agentIds,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Get next lead from leadQueue Id {leadQueueId} for agent Ids {agentIds}",
            queueId, agentIds is null ? null : string.Join(", ", agentIds));

        var lead = _leadsQueueStore.PopNextLead(clientId, queueId, agentIds);
        if (lead is null) return null;

        if (!lead.LeadQueueId.HasValue)
            throw new ArgumentNullException(nameof(lead.LeadQueueId), "Lead does not assigned to queue");

        if (lead.AssignedAgentId is null && !await IsLeadAvailableForCall(lead, clientId))
            return null;

        await _leadRepository.UpdateSystemStatus(lead.LeadId, LeadSystemStatusTypes.Processing, ct);
        await _queueLeadsCacheRepository.UpdateSystemStatus(clientId, queueId, lead.LeadId,
            LeadSystemStatusTypes.Processing);
        await _leadsQueueUpdateNotificationHandler.Process(queueId);

        return new GetNextLeadResponse(
            lead.LeadQueueId!.Value,
            lead.LeadId,
            lead.LeadPhone,
            lead.AssignedAgentId,
            lead.IsTest
        );
    }

    private async Task<bool> IsLeadAvailableForCall(TrackedLead lead, long clientId)
    {
        try
        {
            if (lead.Timezone is null || lead.CountryCode is null)
                return true;

            var leadCallHours = await GetCallHoursByCountry(clientId, lead.CountryCode);
            if (leadCallHours is null)
                return true;

            return CheckIfTimeInRange(lead.Timezone, leadCallHours);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Method} executing", nameof(IsLeadAvailableForCall));
            return true;
        }
    }

    private async Task<IEnumerable<CallHours>?> GetCallHoursByCountry(long clientId, string countryCode)
    {
        var settingsJson = await _settingsRepository.GetValue(clientId, SettingTypes.CallHours);
        if (settingsJson is null) return null;

        var settings = JsonSerializer.Deserialize<CallHoursSettings>(settingsJson, JsonSettingsExtensions.Default);

        var callHours = settings?.CallHours
            .Where(x => x.Country.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

        return callHours;
    }

    private static bool CheckIfTimeInRange(string leadTimeZoneId, IEnumerable<CallHours> leadCallHours)
    {
        var leadTimeZone = TimeZoneInfo.FindSystemTimeZoneById(leadTimeZoneId);
        var leadTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, leadTimeZone).TimeOfDay;

        var isLeadTimeInCallHoursRange = leadCallHours.Any(x => x.From < leadTimeNow && leadTimeNow < x.Till);

        return isLeadTimeInCallHoursRange;
    }
}
