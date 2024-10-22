using System.Collections.Concurrent;
using KL.Manager.API.Application.Models;
using KL.Manager.API.Application.Models.Responses.AgentTrackings;
using KL.Manager.API.Application.Models.Responses.LeadGroups;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Application.Services.Interfaces;
using KL.Manager.API.Configurations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.SignalR;

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