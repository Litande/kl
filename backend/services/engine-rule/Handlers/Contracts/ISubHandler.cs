namespace Plat4Me.DialRuleEngine.Application.Handlers.Contracts;

public interface ISubHandler<in TMessage>
{
    Task Process(TMessage message);
}