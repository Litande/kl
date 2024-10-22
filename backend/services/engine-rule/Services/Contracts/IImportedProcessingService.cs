using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Services.Contracts;

public interface IImportedProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
