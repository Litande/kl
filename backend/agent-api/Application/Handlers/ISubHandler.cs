namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message, CancellationToken ct = default);
}
