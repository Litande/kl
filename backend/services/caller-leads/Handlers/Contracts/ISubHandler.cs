namespace Plat4Me.DialLeadCaller.Application.Handlers.Contracts;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message, CancellationToken ct = default);
}
