using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Services;

public class LeadProcessingImported : ILeadProcessingImported
{
    private readonly ILogger<LeadProcessingPipeline> _logger;
    private readonly ILeadRepository _leadRepository;
    private readonly IImportedProcessingService _importedProcessingService;

    public LeadProcessingImported(
        ILogger<LeadProcessingPipeline> logger,
        ILeadRepository leadRepository,
        IImportedProcessingService importedProcessingService)
    {
        _logger = logger;
        _leadRepository = leadRepository;
        _importedProcessingService = importedProcessingService;
    }

    public async Task Process(long clientId, CancellationToken ct = default)
    {
        var leads = await _leadRepository.GetForImportedProcessing(clientId, ct: ct);
        var leadIds = leads.Select(r => r.LeadId);

        _logger.LogInformation("Start {name} lead Ids {leadIds} for client Id {clientId}",
            nameof(LeadProcessingImported), string.Join(", ", leadIds), clientId);

        await _importedProcessingService.Process(clientId, leads, ct);

        await _leadRepository.UpdateLeads(leads, ct);
        await _leadRepository.ClearSystemStatuses(leadIds, ct);
    }
}
