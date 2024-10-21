using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Handlers.Contracts;

public interface IPublishForCallHandler
{
    Task Process(IEnumerable<CallToRequest> requests, CancellationToken ct = default);
    Task Process(CallToRequest request, CancellationToken ct = default);
}
