using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Responses;

public record HubResponse(HubError? Error = null)
{
    public bool IsSuccess => Error is null;

    public static HubResponse CreateError(string message, HubErrorCode code) => new(new HubError(message, code));
    public static HubResponse CreateSuccess() => new();
}