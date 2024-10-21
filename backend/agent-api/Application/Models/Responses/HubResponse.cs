using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Responses;

public record HubResponse(HubError? Error = null)
{
    public bool IsSuccess => Error is null;

    public static HubResponse CreateError(string message, HubErrorCode code) => new(new HubError(message, code));
    public static HubResponse CreateSuccess() => new();
}