namespace Plat4Me.DialClientApi.Application.Models.Requests.Agents;

public record CreateAgentRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    IReadOnlyCollection<long> TeamIds,
    IReadOnlyCollection<long>? TagIds,
    IReadOnlyCollection<long>? LeadQueueIds);