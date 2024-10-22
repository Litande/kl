using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Services.Contracts;

public interface IScoreProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
