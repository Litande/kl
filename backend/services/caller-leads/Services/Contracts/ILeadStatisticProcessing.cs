namespace KL.Caller.Leads.Services.Contracts;

public interface ILeadStatisticProcessing
{
    Task ProcessAll(CancellationToken ct = default);
    Task Process(long clientId, CancellationToken ct = default);
}