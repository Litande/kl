namespace KL.Manager.API.Application.Handlers.Leads;

public interface IBlockLeadHandler
{
    Task Handle(long clientId, long userId, long leadId, CancellationToken ct = default);
}