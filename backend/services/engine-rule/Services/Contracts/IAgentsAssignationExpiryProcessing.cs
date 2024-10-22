namespace KL.Engine.Rule.Services.Contracts;

public interface IAgentsAssignationExpiryProcessing
{
    Task Process(int agentsAssignationExpiresDays, CancellationToken ct = default);
}
