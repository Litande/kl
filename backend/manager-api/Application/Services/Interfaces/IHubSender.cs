using System.Collections.Concurrent;
using KL.Manager.API.Application.Models;
using KL.Manager.API.Application.Models.Responses.AgentTrackings;
using KL.Manager.API.Application.Models.Responses.LeadGroups;
using KL.Manager.API.Application.Models.Responses.Leads;

namespace KL.Manager.API.Application.Services.Interfaces;

public interface IHubSender
{
    ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers { get; }
    Task SendLeadGroups(long clientId, IEnumerable<LeadGroup> leadGroups, CancellationToken ct = default);
    Task SendAgentsList(long clientId, IEnumerable<AgentTrackingResponse> agentsTracking, CancellationToken ct = default);
    Task SendLeadsQueue(long clientId, IEnumerable<LeadQueueItem> leadQueues, CancellationToken ct = default);
}
