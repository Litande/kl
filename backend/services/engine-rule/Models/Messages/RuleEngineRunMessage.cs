namespace Plat4Me.DialRuleEngine.Application.Models.Messages;

public record RuleEngineRunMessage
{
    public string Initiator => nameof(DialRuleEngine);
}
