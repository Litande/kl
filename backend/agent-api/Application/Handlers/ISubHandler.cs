namespace KL.Agent.API.Application.Handlers;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message, CancellationToken ct = default);
}
