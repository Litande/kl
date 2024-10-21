using System.Collections.Concurrent;
using Plat4Me.DialClientApi.Application.Models;
using Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;
using Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;
using Plat4Me.DialClientApi.Application.Models.Responses.LeadGroups;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Application.Services.Interfaces;

public interface IHubSender
{
    ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers { get; }
    Task SendLeadGroups(long clientId, IEnumerable<LeadGroup> leadGroups, CancellationToken ct = default);
    Task SendAgentsList(long clientId, IEnumerable<AgentTrackingResponse> agentsTracking, CancellationToken ct = default);
    Task SendLeadsQueue(long clientId, IEnumerable<LeadQueueItem> leadQueues, CancellationToken ct = default);
}
