namespace Plat4Me.DialClientApi.Application.Models.Requests.Agents;

public record AgentChangePasswordRequest(
    string NewPassword,
    string ConfirmPassword);
