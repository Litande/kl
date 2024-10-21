using Plat4Me.DialLeadProvider.Application.Models;

namespace Plat4Me.DialLeadProvider.Persistent.LeadCallbackHttpClient;

public interface ILeadCallbackClient
{
    Task<HttpResponseMessage> SendLeadCallback(CallbackSettings callbackSettings, string leadId, string? statusId, string comment);
}