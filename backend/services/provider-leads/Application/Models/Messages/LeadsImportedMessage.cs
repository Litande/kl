namespace KL.Provider.Leads.Application.Models.Messages;

public record LeadsImportedMessage(
    IEnumerable<long> ClientIds)
{
    public string Initiator => nameof(KL.Provider.Leads);
}