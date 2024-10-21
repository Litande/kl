using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.LeadProviderHttpClient;

public static class LeadProviderAuthorizationHelper
{
    public static HttpClient CreateWithAuthorizationData(this IHttpClientFactory httpClientFactory, DataSource config)
    {
        var client = httpClientFactory.CreateClient(nameof(LeadProviderClient));
        var endpoint = new Uri(config.Endpoint);
        client.BaseAddress = endpoint;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("authorization", config.ApiKey);

        return client;
    }
}