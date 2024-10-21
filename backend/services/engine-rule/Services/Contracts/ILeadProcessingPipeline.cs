namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface ILeadProcessingPipeline
{
    Task Process(CancellationToken ct = default);
}