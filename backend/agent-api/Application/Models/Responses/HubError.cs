using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Responses;

public record HubError(string Message, HubErrorCode Code);