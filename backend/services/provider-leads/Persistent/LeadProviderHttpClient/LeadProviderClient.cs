using System.Text.Json;
using System.Text.Json.Nodes;
using KL.Provider.Leads.Persistent.Entities;
using Microsoft.AspNetCore.WebUtilities;

namespace KL.Provider.Leads.Persistent.LeadProviderHttpClient;

public class LeadProviderClient : ILeadProviderClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeadProviderClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<JsonObject> GetLeads(DataSource dataSource, int page, CancellationToken ct = default)
    {
        using var client = _httpClientFactory.CreateWithAuthorizationData(dataSource);
        if (string.IsNullOrEmpty(dataSource.QueryParams))
            throw new ArgumentNullException(nameof(dataSource),
                $"The {nameof(dataSource)} query_params cannot be null or empty.");

        var dataSourceMinUpdateTime = dataSource.MinUpdateDate ?? DateTimeOffset.UtcNow.AddMonths(-2);
        var queryValues = JsonSerializer.Deserialize<Dictionary<string, string>>(dataSource.QueryParams);

        var queryParamValues = queryValues?
            .ToDictionary(x => x.Key,
                x => x.Value switch
                {
                    "page" => $"{page}",
                    "dataSourceMinUpdateTime" => dataSourceMinUpdateTime.ToString("O"),
                    _ => x.Value
                });

        var leads = await client.GetStringAsync(QueryHelpers.AddQueryString("", queryParamValues), ct);
        if (string.IsNullOrEmpty(leads))
            throw new ArgumentNullException(nameof(LeadProviderClient),
                $"The {nameof(LeadProviderClient)} lead response cannot be null or empty.");

        var jsonNodeResponse = JsonNode.Parse(leads);
        return jsonNodeResponse!.AsObject();
    }
}