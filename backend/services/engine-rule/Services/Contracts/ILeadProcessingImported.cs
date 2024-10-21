namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface ILeadProcessingImported
{
    Task Process(long clientId, CancellationToken ct = default);
}
