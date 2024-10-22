namespace KL.Engine.Rule.Services.Contracts;

public interface ILeadProcessingImported
{
    Task Process(long clientId, CancellationToken ct = default);
}
