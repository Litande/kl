using System.Text.Json.Nodes;
using Plat4Me.DialLeadProvider.Application.Models;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Application.Services.Interfaces;

public interface ILeadDataMapperService
{
    Lead? MapToLead(long clientId, long dataSourceId, LeadImportDefaultStatusSettings settings, JsonNode? lead);
}