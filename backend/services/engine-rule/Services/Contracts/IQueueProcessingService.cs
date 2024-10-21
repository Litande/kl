using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface IQueueProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
