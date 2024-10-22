using System.Text.Json.Nodes;
using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Persistent.LeadProviderHttpClient;

public interface ILeadProviderClient
{
    Task<JsonObject> GetLeads(DataSource dataSource, int page, CancellationToken ct = default);
}