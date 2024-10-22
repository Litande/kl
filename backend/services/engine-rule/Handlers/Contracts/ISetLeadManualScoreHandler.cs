namespace KL.Engine.Rule.Handlers.Contracts;

public interface ISetLeadManualScoreHandler
{
    Task Process(long clientId, long queueId, long leadId, long score, CancellationToken ct = default);
}
