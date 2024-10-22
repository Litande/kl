using KL.Statistics.Application.Models.Messages;

namespace KL.Statistics.Application.Handlers;

public interface ICdrUpdatedService
{
    void AddToQueueUpdatedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default);
    void AddToQueueInsertedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default);
    Task Process(CancellationToken ct = default);
}