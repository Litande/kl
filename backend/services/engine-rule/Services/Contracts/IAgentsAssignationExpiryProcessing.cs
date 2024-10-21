namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface IAgentsAssignationExpiryProcessing
{
    Task Process(int agentsAssignationExpiresDays, CancellationToken ct = default);
}
