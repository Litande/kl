using KL.Provider.Leads.Application.Models;

namespace KL.Provider.Leads.Persistent.LeadCallbackHttpClient;

public interface ILeadCallbackClient
{
    Task<HttpResponseMessage> SendLeadCallback(CallbackSettings callbackSettings, string leadId, string? statusId, string comment);
}