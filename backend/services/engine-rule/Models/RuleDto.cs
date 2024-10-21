namespace Plat4Me.DialRuleEngine.Application.Models;

public record RuleDto(
    long? QueueId,
    //bool DefaultQueue,
    string Name,
    string Rules,
    long Ordinal
    );
