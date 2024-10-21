using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Plat4Me.DialClientApi.Application.Models;
using Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;
using Plat4Me.DialClientApi.Application.Models.Responses.LeadGroups;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Configurations;

namespace Plat4Me.DialClientApi.SignalR;

public class HubSender : IHubSender
{
    private readonly IHubContext<TrackingHub> _trackingHub;
    private readonly SignalROptions _options;

    public ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers =>
        TrackingHub.PerformanceSubscribers;

    public HubSender(
        IHubContext<TrackingHub> trackingHub,
        IOptions<SignalROptions> options)
    {
        _trackingHub = trackingHub;
        _options = options.Value;
    }

    public async Task SendLeadGroups(
        long clientId,
        IEnumerable<LeadGroup> leadGroups,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.LeadGroups, leadGroups, cancellationToken: ct);
    }

    public async Task SendAgentsList(
        long clientId,
        IEnumerable<AgentTrackingResponse> agentsTracking,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.AgentsList, agentsTracking, cancellationToken: ct);
    }

    public async Task SendLeadsQueue(
        long clientId,
        IEnumerable<LeadQueueItem> leadQueues,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.LeadsQueue, leadQueues, cancellationToken: ct);
    }
}