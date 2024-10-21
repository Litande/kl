using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers;

public interface ICdrUpdatedService
{
    void AddToQueueUpdatedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default);
    void AddToQueueInsertedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default);
    Task Process(CancellationToken ct = default);
}