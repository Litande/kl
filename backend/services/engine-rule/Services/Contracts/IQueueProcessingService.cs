using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Services.Contracts;

public interface IQueueProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
