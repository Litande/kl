namespace KL.Agent.API.Application.Models.Requests;

public record AgentChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);
