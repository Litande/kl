namespace KL.Engine.Rule.Models.Messages;

public record LeadsImportedMessage(
    IEnumerable<long> ClientIds,
    string Initiator);
