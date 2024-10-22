using System.Text.Json.Nodes;
using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Application.Services.Interfaces;

public interface ILeadDataMapperService
{
    Lead? MapToLead(long clientId, long dataSourceId, LeadImportDefaultStatusSettings settings, JsonNode? lead);
}