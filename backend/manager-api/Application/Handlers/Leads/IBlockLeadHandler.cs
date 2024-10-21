namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

public interface IBlockLeadHandler
{
    Task Handle(long clientId, long userId, long leadId, CancellationToken ct = default);
}