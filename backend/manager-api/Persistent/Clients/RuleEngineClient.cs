using Microsoft.Extensions.Options;
using Plat4Me.DialClientApi.Application.Models;
using Plat4Me.DialClientApi.Persistent.Configurations;
using System.Text;
using System.Text.Json;
using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Clients;

public class RuleEngineClient : IRuleEngineClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RuleEngineClientOptions _ruleEngineClientOptions;
    private readonly ILogger<RuleEngineClient> _logger;

    public RuleEngineClient(
        IHttpClientFactory httpClientFactory,
        IOptions<RuleEngineClientOptions> ruleEngineClientOptions,
        ILogger<RuleEngineClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _ruleEngineClientOptions = ruleEngineClientOptions.Value;
    }

    public async Task<OperationResponse?> ValidateRules(long clientId, RuleGroupTypes ruleType, string rule, CancellationToken ct = default)
    {
        return await PostJsonAsync<OperationResponse>(
            clientId,
            string.Format(_ruleEngineClientOptions.ValidateRules, ruleType),
            rule,
            ct);
    }

    public async Task<Dictionary<RuleGroupTypes, string>> GetConditions(long clientId, CancellationToken ct = default)
    {
        var client = CreateClient(clientId);
        var result = new Dictionary<RuleGroupTypes, string>();
        foreach (RuleGroupTypes ruleType in Enum.GetValues(typeof(RuleGroupTypes)))
        {
            result[ruleType] = (await GetRawAsync(clientId, string.Format(_ruleEngineClientOptions.Conditions, ruleType), client, ct)) ?? "";
        }
        return result;
    }

    public async Task<Dictionary<RuleGroupTypes, string>> GetActions(long clientId, CancellationToken ct = default)
    {
        var client = CreateClient(clientId);
        var result = new Dictionary<RuleGroupTypes, string>();
        foreach (RuleGroupTypes ruleType in Enum.GetValues(typeof(RuleGroupTypes)))
        {
            result[ruleType] = (await GetRawAsync(clientId, string.Format(_ruleEngineClientOptions.Actions, ruleType), client, ct)) ?? "";
        }
        return result;
    }

    private async Task<TResult?> GetJsonAsync<TResult>(
        long clientId,
        string endpoint,
        CancellationToken ct = default)
        where TResult : class?
    {
        var client = CreateClient(clientId);
        var response = await client.GetAsync(endpoint, ct);
        _logger.LogDebug("HTTP client response status code: {0}", response.StatusCode);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var streamLength = stream.Length;
        _logger.LogDebug("Rule engine stream length is: {0}", streamLength);
        if (streamLength is 0) return null;

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var result = await JsonSerializer.DeserializeAsync<TResult>(stream, serializerOptions, ct);
        return result;
    }

    private async Task<TResult?> PostJsonAsync<TResult>(
        long currentClientId,
        string endpoint,
        string data,
        CancellationToken ct = default)
    where TResult : class?
    {
        var client = CreateClient(currentClientId);
        var response = await client.PostAsync(endpoint, new StringContent(data, Encoding.UTF8, "application/json"), ct);
        _logger.LogDebug("HTTP client response status code: {0}", response.StatusCode);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var streamLength = stream.Length;
        _logger.LogDebug("Rule engine stream length is: {0}", streamLength);
        if (streamLength is 0) return null;

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var result = await JsonSerializer.DeserializeAsync<TResult>(stream, serializerOptions, ct);
        return result;
    }

    private async Task<string?> GetRawAsync(
        long clientId,
        string endpoint,
        HttpClient client,
        CancellationToken ct = default)
    {
        var response = await client.GetAsync(endpoint, ct);
        _logger.LogDebug("HTTP client response status code: {0}", response.StatusCode);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var streamLength = stream.Length;
        _logger.LogDebug("Rule engine stream length is: {0}", streamLength);
        if (streamLength is 0) return null;

        return await (new StreamReader(stream)).ReadToEndAsync();
    }

    private HttpClient CreateClient(long clientId)
    {
        var client = _httpClientFactory.CreateClient(nameof(RuleEngineClient));
        var baseAddress = new Uri(_ruleEngineClientOptions.BaseUrl);
        client.BaseAddress = baseAddress;

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("current_client_id", clientId.ToString());

        return client;
    }
}
