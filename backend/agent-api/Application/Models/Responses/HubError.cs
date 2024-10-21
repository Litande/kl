using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Responses;

public record HubError(string Message, HubErrorCode Code);