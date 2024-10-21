using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Services;

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
        var agentIds = agents.Select(x => x.UserId);

        await _leadRepository.ResetAssignation(agentIds, ct);
    }
}
