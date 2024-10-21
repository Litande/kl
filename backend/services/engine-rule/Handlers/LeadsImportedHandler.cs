using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models.Messages;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

public class LeadsImportedHandler : ILeadsImportedHandler
{
    private readonly ILeadProcessingImported _leadProcessingImported;

    public LeadsImportedHandler(ILeadProcessingImported leadProcessingImported)
    {
        _leadProcessingImported = leadProcessingImported;
    }

    public async Task Process(LeadsImportedMessage message)
    {
        foreach (var clientId in message.ClientIds)
            await _leadProcessingImported.Process(clientId);
    }
}
