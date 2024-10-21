using Microsoft.Extensions.Options;
using Plat4Me.DialLeadCaller.Application.Clients;
using Plat4Me.DialLeadCaller.Application.Extensions;
using Plat4Me.DialLeadCaller.Application.Models.Requests;
using Plat4Me.DialLeadCaller.Application.Models.Responses;
using Plat4Me.DialLeadCaller.Infrastructure.App;
using System.Net.Http.Json;
using System.Text.Json;

namespace Plat4Me.DialLeadCaller.Infrastructure.Clients;

public class LeadRuleEngineClient : ILeadRuleEngineClient
{
    private const string ClientIdHeaderKey = "current_client_id";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LeadQueueClientOptions _leadQueueClientOptions;

    public LeadRuleEngineClient(
        IHttpClientFactory httpClientFactory,
        IOptions<LeadQueueClientOptions> leadQueueClientOptions)
    {
        _httpClientFactory = httpClientFactory;
        _leadQueueClientOptions = leadQueueClientOptions.Value;
    }

    public async Task<GetNextLeadResponse?> GetNextLead(
        long clientId,
        GetNextLeadRequest request,
        CancellationToken ct = default) =>
        await PutAndReadJsonAsync<GetNextLeadResponse?, IEnumerable<long>>(
            clientId,
            _leadQueueClientOptions.GetNextEndpoint.Replace("{queueId}", request.LeadQueueId.ToString()),
            request.WaitingAgentIds,
            ct);

    private async Task<TResult?> PutAndReadJsonAsync<TResult, TValue>(
        long clientId,
        string endpoint,
        TValue value,
        CancellationToken ct = default)
        where TResult : class?
    {
        var client = CreateClient(clientId);
        var response = await client.PutAsJsonAsync(endpoint, value, ct);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        if (stream.Length is 0) return null;

        var result = await JsonSerializer.DeserializeAsync<TResult>(stream, JsonSettingsExtensions.Default, ct);
        return result;
    }

    private HttpClient CreateClient(long clientId)
    {
        var client = _httpClientFactory.CreateClient(nameof(LeadRuleEngineClient));
        var baseAddress = new Uri(_leadQueueClientOptions.BaseUrl);

        client.BaseAddress = baseAddress;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(ClientIdHeaderKey, clientId.ToString());

        return client;
    }
}
