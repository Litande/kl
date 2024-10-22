namespace KL.Engine.Rule.Models.Messages;

public record RuleEngineRunMessage
{
    public string Initiator => nameof(KL.Engine.Rule);
}
