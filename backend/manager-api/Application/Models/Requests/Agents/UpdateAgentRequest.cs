namespace KL.Manager.API.Application.Models.Requests.Agents;

public record UpdateAgentRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<long> TeamIds,
    IReadOnlyCollection<long>? TagIds,
    IReadOnlyCollection<long>? LeadQueueIds);