using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Services;

public class LeadProcessingPipeline : ILeadProcessingPipeline
{
    private readonly ILogger<LeadProcessingPipeline> _logger;
    private readonly ILeadRepository _leadRepository;
    private readonly IScoreProcessingService _scoreProcessingService;
    private readonly IQueueProcessingService _queueProcessingService;
    private readonly ILeadQueueStoreUpdateHandler _leadQueueStoreUpdateHandler;

    public LeadProcessingPipeline(
        ILogger<LeadProcessingPipeline> logger,
        ILeadRepository leadRepository,
        IScoreProcessingService scoreProcessingService,
        IQueueProcessingService queueProcessingService,
        ILeadQueueStoreUpdateHandler leadQueueStoreUpdateHandler)
    {
        _logger = logger;
        _leadRepository = leadRepository;
        _scoreProcessingService = scoreProcessingService;
        _queueProcessingService = queueProcessingService;
        _leadQueueStoreUpdateHandler = leadQueueStoreUpdateHandler;
    }

    public async Task Process(CancellationToken ct = default)
    {
        var clients = await _leadRepository.GetAllClients(ct);
        
        foreach (var client in clients)
        {
            try
            {
                await ProcessClient(client.Id, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {name} executing for client Id {clientId}",
                    nameof(ProcessClient), client.Id);
            }
        }
    }

    private async Task ProcessClient(long clientId, CancellationToken ct = default)
    {
        var leads = await _leadRepository.GetForPreProcessing(clientId, ct);
        
        _logger.LogInformation("Start {name} with lead Ids {leadIds} for client Id {clientId}",
            nameof(LeadProcessingPipeline), string.Join(", ", leads.Select(r => r.LeadId)), clientId);

        await _scoreProcessingService.Process(clientId, leads, ct);
        await _queueProcessingService.Process(clientId, leads, ct);

        await _leadRepository.UpdateLeads(leads, ct);
        await _leadQueueStoreUpdateHandler.Process(clientId, leads, ct);
    }
}
