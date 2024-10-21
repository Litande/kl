namespace Plat4Me.DialAgentApi.Application.Models.Requests;

public record AgentChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);
