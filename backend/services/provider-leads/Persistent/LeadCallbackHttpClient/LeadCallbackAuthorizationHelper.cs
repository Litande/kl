namespace Plat4Me.DialLeadProvider.Persistent.LeadCallbackHttpClient;

public static class LeadCallbackAuthorizationHelper
{
    public static HttpClient CreateWithAuthorizationData(this IHttpClientFactory httpClientFactory, string endpointUrl, Dictionary<string, string>? headers)
    {
        var client = httpClientFactory.CreateClient(nameof(LeadCallbackClient));
        var endpoint = new Uri(endpointUrl);
        client.BaseAddress = endpoint;
        client.DefaultRequestHeaders.Clear();
        if (headers != null)
        {
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        return client;
    }
}