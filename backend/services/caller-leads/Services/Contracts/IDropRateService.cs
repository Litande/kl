namespace Plat4Me.DialLeadCaller.Application.Services.Contracts;

public interface IDropRateService
{
    Task Process(CancellationToken ct = default);
}
