namespace KL.Caller.Leads.Services.Contracts;

public interface IDropRateService
{
    Task Process(CancellationToken ct = default);
}
