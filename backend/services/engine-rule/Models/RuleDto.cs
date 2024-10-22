namespace KL.Engine.Rule.Models;

public record RuleDto(
    long? QueueId,
    //bool DefaultQueue,
    string Name,
    string Rules,
    long Ordinal
    );
