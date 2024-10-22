using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace KL.Provider.Leads.Persistent.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly DialDbContext _context;

    public LeadRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task MarkForSave(
        IEnumerable<Lead> leads,
        CancellationToken ct = default)
    {
        await _context.Leads.AddRangeAsync(leads, ct); //TODO: Implement bulk insert. Created task: kl-67.
    }

    public async Task SaveChanges(
        CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<ICollection<Lead>> GetLeadsByPhone(
        long clientId,
        IEnumerable<string> phones,
        CancellationToken ct = default)
    {
        return await _context.Leads
            .Where(i => !string.IsNullOrWhiteSpace(i.Phone)
                        && i.ClientId == clientId
                        && phones.Contains(i.Phone))
            .ToListAsync(cancellationToken: ct);
    }

    public async Task<IReadOnlyCollection<Lead>> GetLeads(
        IEnumerable<string?>? externalIds,
        CancellationToken ct = default)
    {
        var existLeads = await _context.Leads
            .Where(i => externalIds != null
                        && !string.IsNullOrWhiteSpace(i.ExternalId)
                        && externalIds.Contains(i.ExternalId))
            .ToArrayAsync(ct);

        return existLeads;
    }

    public async Task<Lead?> GetLeadWithDataSourceById(long leadId, CancellationToken ct = default)
    {
        var lead = await _context.Leads
            .Include(d => d.DataSource)
            .FirstOrDefaultAsync(x => x.Id == leadId, ct);
        return lead;
    }

    public void UpdateLead(Lead updatedLead, long existLeadId)
    {
        var lastUpdateTime = new MySqlParameter("@lastUpdateTime", DateTimeOffset.UtcNow);
        var firstName = new MySqlParameter("@firstName", updatedLead.FirstName);
        var lastName = new MySqlParameter("@lastName", updatedLead.LastName);
        var languageCode = new MySqlParameter("@languageCode", updatedLead.LanguageCode);
        var countryCode = new MySqlParameter("@countryCode", updatedLead.CountryCode);
        var lastTimeOnline = new MySqlParameter("@lastTimeOnline", updatedLead.LastTimeOnline);
        var registrationTime = new MySqlParameter("@registrationTime", updatedLead.RegistrationTime);
        var dataSourceId = new MySqlParameter("@dataSourceId", updatedLead.DataSourceId);
        var clientId = new MySqlParameter("@clientId", updatedLead.ClientId);
        var leadId = new MySqlParameter("@leadId", existLeadId);
        var leadStatus = new MySqlParameter("@leadStatus", updatedLead.Status);
        var email = new MySqlParameter("@email", updatedLead.Email);
        var timeZone = new MySqlParameter("@timeZone", updatedLead.Timezone);

        var response = _context.Database.ExecuteSqlRaw(@$"Update dial.lead set
                                      last_update_time=@lastUpdateTime,
                                      first_name=@firstName,
                                      last_name=@lastName,
                                      language_code=@languageCode,
                                      country_code=@countryCode,
                                      last_time_online=@lastTimeOnline,
                                      registration_time=@registrationTime,
                                      data_source_id=@dataSourceId,
                                      status=@leadStatus,
                                      email=@email,
                                      timeZone=@timeZone,
                                      client_id=@clientId where id=@leadId",
            parameters: new object[]
            {
                lastUpdateTime, firstName, lastName, languageCode, countryCode, lastTimeOnline, registrationTime,
                dataSourceId, leadStatus, clientId, leadId, email, timeZone
            });
    }
}