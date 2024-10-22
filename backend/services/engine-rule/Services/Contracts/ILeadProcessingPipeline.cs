namespace KL.Engine.Rule.Services.Contracts;

public interface ILeadProcessingPipeline
{
    Task Process(CancellationToken ct = default);
}