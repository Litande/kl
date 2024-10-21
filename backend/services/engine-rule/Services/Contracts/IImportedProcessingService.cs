using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface IImportedProcessingService
{
    Task Process(long clientId, IReadOnlyCollection<TrackedLead> leads, CancellationToken ct = default);
}
