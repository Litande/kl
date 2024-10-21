using Microsoft.Extensions.Logging;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using System.Text.RegularExpressions;
using System.Text.Json;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models;
using Plat4Me.DialAgentApi.Application.Extensions;
using Plat4Me.DialAgentApi.Application.Services;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class CallInfoHandler : ICallInfoHandler
{
    private readonly IHubSender _hubSender;
    private readonly ILeadRepository _leadRepository;
    private readonly IRuleRepository _ruleRepository;
    private readonly ILogger<CallInfoHandler> _logger;

    public CallInfoHandler(
        IHubSender hubSender,
        ILeadRepository leadRepository,
        IRuleRepository ruleRepository,
        ILogger<CallInfoHandler> logger
        )
    {
        _hubSender = hubSender;
        _leadRepository = leadRepository;
        _ruleRepository = ruleRepository;
        _logger = logger;
    }

    public async Task Handle(long agentId, CallInfo callInfo, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("CallInfoHandler(cache): agentId: {AgentId}, leadId: {LeadId}, LeadPhone: {LeadPhone} AgentRtcUrl: {AgentRtcUrl}, SessionId: {SessionId}",
                 agentId, callInfo.LeadId, callInfo.LeadPhone, callInfo.AgentRtcUrl, callInfo.SessionId);

            var lead = await TryGetById(callInfo.ClientId, callInfo.LeadId, ct);

            var callInfoData = lead.ToCallInfo(
                leadPhone: callInfo.LeadPhone!,
                callType: callInfo.CallType,
                sessionId: callInfo.SessionId!,
                agentRtcUrl: callInfo.AgentRtcUrl,
                callOriginatedAt: DateTimeOffset.FromUnixTimeSeconds(callInfo.CallOriginatedAt!.Value),
                leadAnsweredAt: DateTimeOffsetExtensions.FromUnixTimeSeconds(callInfo.LeadAnsweredAt),
                agentAnsweredAt: DateTimeOffsetExtensions.FromUnixTimeSeconds(callInfo.AgentAnsweredAt),
                callFinishAt: DateTimeOffsetExtensions.FromUnixTimeSeconds(callInfo.CallFinishedAt),
                callAgainCount: callInfo.CallAgainCount ?? 0,
                iframeUrl: PrepareIframeUrl(lead),
                availableStatuses: await TryGetAvailableStatuses(callInfo.ClientId, lead, ct)
                );

            await _hubSender.SendCallInfo(agentId.ToString(), callInfoData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(CallInfoHandler));
            throw;
        }
    }

    private async Task<Lead?> TryGetById(
        long clientId,
        long? leadId,
        CancellationToken ct = default)
    {
        if (leadId is null) return null;

        var lead = await _leadRepository.GetWithDataSource(clientId, leadId.Value, ct);
        if (lead is null)
            _logger.LogWarning("Lead {LeadId} was not found", leadId.Value);

        return lead;
    }

    private async Task<IReadOnlyCollection<LeadStatusTypes>?> TryGetAvailableStatuses(
        long clientId,
        Lead? lead,
        CancellationToken ct = default)
    {
        if (lead is null) return null;

        return await _ruleRepository.GetAvailableStatuses(clientId, lead.Status, ct);
    }

    private string? PrepareIframeUrl(Lead? lead)
    {
        if (lead is null
            || string.IsNullOrEmpty(lead.Metadata)
            || string.IsNullOrEmpty(lead.DataSource.IframeTemplate))
            return null;
        try
        {
            var paramsExp = new Regex("{[^{}]+}");
            var matches = paramsExp
                .Matches(lead.DataSource.IframeTemplate)
                .Select(x => x.Value)
                .Distinct()
                .ToList();
            if (!matches.Any())
                return lead.DataSource.IframeTemplate;

            var metaValues = JsonSerializer.Deserialize<Dictionary<string, object?>>(lead.Metadata)?
                .ToDictionary(x => $"{{{x.Key}}}", x => x.Value);

            var values = metaValues?
                    .Where(x => x.Value is not null && matches.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value!.ToString());

            var result = values?
                        .Aggregate(lead.DataSource.IframeTemplate, (t, v) => t.Replace(v.Key, v.Value))
                        .Trim()
                    ?? lead.DataSource.IframeTemplate;

            if (!Uri.TryCreate(result, UriKind.RelativeOrAbsolute, out _))
            {
                _logger.LogWarning("IframeUrl is invalid URI: {Uri}", result);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during {Service} executing", nameof(CallInfoHandler));
            throw;
        }
    }
}
