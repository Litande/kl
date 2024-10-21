using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface ILeadRepository
{
    Task MarkForSave(IEnumerable<Lead> leads, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
    Task<IReadOnlyCollection<Lead>> GetLeads(IEnumerable<string?>? externalIds, CancellationToken ct = default);
    Task<ICollection<Lead>> GetLeadsByPhone(long clientId, IEnumerable<string> phones, CancellationToken ct = default);
    void UpdateLead(Lead updatedLead, long existLeadId);
    Task<Lead?> GetLeadWithDataSourceById(long leadId, CancellationToken ct = default);
}