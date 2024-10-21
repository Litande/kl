using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface ICallPublishService
{
    Task Process(CallToLeadMessage message, CancellationToken ct = default);
}
