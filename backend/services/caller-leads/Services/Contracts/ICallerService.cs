using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Services.Contracts;

public interface ICallerService
{
    Task TryToCallPredictive(CancellationToken ct = default);
    Task TryToCallManual(ManualCallMessage message, CancellationToken ct = default);
    Task<long?> GetFreeAgentId(long clientId, long queueId, long agentId, long? leadId, CancellationToken ct = default);
    Task TryRecall(long clientId, long agentId, long leadQueueId, long leadId, long providerId, string leadPhone, bool isFixedAssigned, bool isTest, CancellationToken ct = default);
}
