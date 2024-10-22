using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Services.Contracts;

public interface ICallPublishService
{
    Task Process(CallToLeadMessage message, CancellationToken ct = default);
}
