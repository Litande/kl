using System.Text;
using System.Text.Json;
using KL.Provider.Leads.Application.Models;

namespace KL.Provider.Leads.Persistent.LeadCallbackHttpClient;

public class LeadCallbackClient : ILeadCallbackClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeadCallbackClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> SendLeadCallback(CallbackSettings callbackSettings, string leadId,
        string? statusId, string comment)
    {
        using var client =
            _httpClientFactory.CreateWithAuthorizationData(callbackSettings.Url, callbackSettings.Headers);

        var request = new HttpRequestMessage(CreateHttpMethod(callbackSettings.Method), "")
        {
            Content = GetHttpContent(callbackSettings, leadId, statusId, comment)
        };

        return await client.SendAsync(request);
    }

    private HttpContent GetHttpContent(CallbackSettings callbackSettings, string leadId, string? statusId,
        string comment)
    {
        return callbackSettings.Method.ToUpper() switch
        {
            "POST" => GetContent(callbackSettings.Body, leadId, statusId, comment),
            "GET" => GetContent(callbackSettings.Query, leadId, statusId, comment),
            _ => throw new NotImplementedException()
        };
    }

    private HttpContent GetContent(Dictionary<string, string>? settingsContent, string leadId, string? statusId,
        string comment)
    {
        var bodyValues = settingsContent?
            .ToDictionary(x => x.Key,
                x => x.Value switch
                {
                    "{{lead_id}}" => $"{leadId}",
                    "{{status_id}}" => $"{statusId}",
                    "{{comment}}" => $"{comment}",
                    _ => x.Value
                });
        var data = JsonSerializer.Serialize(bodyValues);
        var content = new StringContent(data, Encoding.UTF8, "application/json");

        return content;
    }

    private HttpMethod CreateHttpMethod(string method)
    {
        return method.ToUpper() switch
        {
            "POST" => HttpMethod.Post,
            "GET" => HttpMethod.Get,
            _ => throw new NotImplementedException()
        };
    }
}