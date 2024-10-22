using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Services.Contracts;

public interface IBehaviourProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
