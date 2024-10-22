using KL.Caller.Leads.App;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Models.Requests;
using KL.Caller.Leads.Repositories;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Handlers;

public class AgentFilledCallInfoHandler : IAgentFilledCallInfoHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;
    private readonly ILogger<AgentFilledCallInfoHandler> _logger;
    private readonly ILeadRepository _leadRepository;
    private readonly ICDRRepository _cdrRepository;

    public AgentFilledCallInfoHandler(
        IOptions<PubSubjects> pubSubjects,
        INatsPublisher natsPublisher,
        ILogger<AgentFilledCallInfoHandler> logger,
        ILeadRepository leadRepository,
        ICDRRepository cdrRepository)
    {
        _natsPublisher = natsPublisher;
        _logger = logger;
        _leadRepository = leadRepository;
        _pubSubjects = pubSubjects.Value;
        _cdrRepository = cdrRepository;
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

        if (cdr is null || cdr.LastUserId != agentId || cdr.LeadId is null)
            throw new ArgumentNullException(nameof(request.LeadStatus), "Missing value");

        var lead =  await _leadRepository.SaveFeedbackAndGet(
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
            nameof(KL.Caller),
            request.Comment);

        await _natsPublisher.PublishAsync(_pubSubjects.LeadFeedbackFilled, message);
        _logger.LogInformation("Publish lead update message: {LeadId}", cdr.LeadId);
    }
}
