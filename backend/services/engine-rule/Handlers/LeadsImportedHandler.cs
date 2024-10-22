using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models.Messages;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Handlers;

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
