using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Handlers.Contracts;

public interface IPublishForCallHandler
{
    Task Process(IEnumerable<CallToRequest> requests, CancellationToken ct = default);
    Task Process(CallToRequest request, CancellationToken ct = default);
}
