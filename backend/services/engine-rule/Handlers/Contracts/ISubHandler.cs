namespace KL.Engine.Rule.Handlers.Contracts;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message);
}