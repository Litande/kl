using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Services;

public class AgentsAssignationExpiryProcessing : IAgentsAssignationExpiryProcessing
{
    private readonly IUserRepository _userRepository;
    private readonly ILeadRepository _leadRepository;

    public AgentsAssignationExpiryProcessing(
        IUserRepository userRepository,
        ILeadRepository leadRepository)
    {
        _userRepository = userRepository;
        _leadRepository = leadRepository;
    }

    public async Task Process(int agentsAssignationExpiresDays, CancellationToken ct = default)
    {
        var fromDate = DateTimeOffset.UtcNow.AddDays(-agentsAssignationExpiresDays);

        var agents = await _userRepository.GetOfflineAgentsSince(fromDate, ct);
        var agentIds = agents.Select(x => x.Id);

        await _leadRepository.ResetAssignation(agentIds, ct);
    }
}
