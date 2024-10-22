namespace KL.Agent.API.Application.Models.Responses;

public record MeResponse(
    long UserId,
    string FirstName,
    string LastName,
    string[]? IceServers = null
);
