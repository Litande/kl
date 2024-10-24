﻿using KL.Caller.Leads.App;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Clients;

public class BridgeClient : IBridgeClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<BridgeClientOptions> _options; 

    public BridgeClient(
        IHttpClientFactory httpClientFactory,
        IOptions<BridgeClientOptions> options
    )
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<string?> Ping(string bridgeAddr)
    {
        var client = CreateClient(bridgeAddr);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.Value.PingRequestTimeout));
        var response = await client.GetAsync(_options.Value.PingEndpoint, cts.Token);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsStringAsync(cts.Token);
    }

    private HttpClient CreateClient(string bridgeAddr)
    {
        var client = _httpClientFactory.CreateClient(nameof(BridgeClient));
        var baseAddress = new Uri("http://"+bridgeAddr);
        client.BaseAddress = baseAddress;
        return client;
    }
}
