using KL.Agent.API.Application.Configurations;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Messages;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Agent.API.Application.Handlers;

public class AgentFilledCallInfoHandler : IAgentFilledCallInfoHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly ILogger<AgentFilledCallInfoHandler> _logger;
    private readonly ILeadRepository _leadRepository;
    private readonly ICDRRepository _cdrRepository;

    public AgentFilledCallInfoHandler(
        ILogger<AgentFilledCallInfoHandler> logger,
        ILeadRepository leadRepository,
        ICDRRepository cdrRepository,
        IAgentCurrentStatusHandler agentCurrentStatusHandler,
        INatsPublisher natsPublisher,
        IOptions<NatsPubSubOptions> natsPubSubOptions
    )
    {
        _logger = logger;
        _leadRepository = leadRepository;
        _cdrRepository = cdrRepository;
        _natsPublisher = natsPublisher;
        _natsPubSubOptions = natsPubSubOptions.Value;
    }

    public async Task Handle(
        long clientId,
        long agentId,
        AgentFilledCallRequest request,
        bool isGenerated = false,
        CancellationToken ct = default)
    {
        if (!Enum.IsDefined(request.LeadStatus))
            throw new ArgumentNullException(nameof(request.LeadStatus), "Missing value");

        var cdr = await _cdrRepository.GetBySessionId(request.SessionId, ct);

        if (cdr is null || cdr.UserId != agentId || cdr.LeadId is null)
            throw new ArgumentNullException(nameof(request.LeadStatus), "Missing value");

        var lead = await _leadRepository.SaveFeedbackAndGet(
            clientId,
            agentId,
            isGenerated,
            cdr.LeadId.Value,
            LeadSystemStatusTypes.PostProcessing,
            request.LeadStatus,
            request.RemindOn,
            ct);

        if (lead is null)
            throw new KeyNotFoundException($"Cannot find lead with id: {cdr.LeadId}");

        var message = new LeadFeedbackFilledMessage(
            clientId,
            agentId,
            QueueId: null,
            request.SessionId,
            lead.Id,
            request.LeadStatus,
            request.RemindOn,
            request.Comment);

        await _natsPublisher.PublishAsync(_natsPubSubOptions.LeadFeedbackFilled, message);
        _logger.LogInformation("Publish lead update message: {LeadId}", cdr.LeadId);
    }
}
