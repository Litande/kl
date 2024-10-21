namespace Plat4Me.DialRuleEngine.Application.Models.Messages;

public record LeadsImportedMessage(
    IEnumerable<long> ClientIds,
    string Initiator);
