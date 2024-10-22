namespace KL.Manager.API.Application.Models.Responses;

public record MeResponse(
    long UserId,
    string FirstName,
    string LastName,
    string[]? IceServers = null
);