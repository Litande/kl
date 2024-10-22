namespace KL.Manager.API.Application.Models.Requests.Agents;

public record AgentChangePasswordRequest(
    string NewPassword,
    string ConfirmPassword);
