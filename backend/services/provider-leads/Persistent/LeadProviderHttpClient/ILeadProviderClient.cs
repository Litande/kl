using System.Text.Json.Nodes;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.LeadProviderHttpClient;

public interface ILeadProviderClient
{
    Task<JsonObject> GetLeads(DataSource dataSource, int page, CancellationToken ct = default);
}