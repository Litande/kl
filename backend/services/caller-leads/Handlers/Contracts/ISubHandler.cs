namespace KL.Caller.Leads.Handlers.Contracts;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message, CancellationToken ct = default);
}
