namespace KL.Provider.Leads.Application.Models.Messages;

public record LeadsImportedMessage(
    IEnumerable<long> ClientIds)
{
    public string Initiator => nameof(DialLeadProvider);
}