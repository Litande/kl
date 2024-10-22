using KL.Engine.Rule.App;
using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models.Entities;
using KL.Engine.Rule.Models.Messages;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Engine.Rule.Handlers;

public class LeadFeedbackFilledHandler : ILeadFeedbackFilledHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly IBehaviourProcessingService _behaviourProcessingService;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadsQueueUpdateNotificationHandler _updateNotificationHandler;
    private readonly ILeadCommentRepository _leadCommentRepository;
    private readonly ILogger<LeadFeedbackFilledHandler> _logger;
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;

    public LeadFeedbackFilledHandler(
        ILeadRepository leadRepository,
        IBehaviourProcessingService behaviourProcessingService,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ILeadsQueueUpdateNotificationHandler updateNotificationHandler,
        ILogger<LeadFeedbackFilledHandler> logger,
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> pubSubjects,
        ILeadCommentRepository leadCommentRepository)
    {
        _leadRepository = leadRepository;
        _behaviourProcessingService = behaviourProcessingService;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _updateNotificationHandler = updateNotificationHandler;
        _logger = logger;
        _natsPublisher = natsPublisher;
        _leadCommentRepository = leadCommentRepository;
        _pubSubjects = pubSubjects.Value;
    }

    public async Task Process(LeadFeedbackFilledMessage message)
    {
        var leadIds = new[] { message.LeadId };
        var leads = await _leadRepository.GetForPostProcessing(message.ClientId, leadIds);
        await _behaviourProcessingService.Process(message.ClientId, leads);

        await ProcessAgentComment(message);

        // We changes tracking values and then update System status
        await _leadRepository.UpdateLeads(leads);
        await _leadRepository.UpdateSystemStatus(message.LeadId, systemStatus: null);

        var lastLeadQueueId = message.QueueId
                              ?? await _queueLeadsCacheRepository.GetQueueId(message.ClientId, message.LeadId);
        if (lastLeadQueueId is null)
        {
            _logger.LogWarning("Queue not found where lead Id {LeadId} for client Id {ClientId}",
                message.LeadId, message.ClientId);
            return;
        }

        await _queueLeadsCacheRepository.Remove(message.ClientId, lastLeadQueueId.Value, message.LeadId);
        await _updateNotificationHandler.Process(message.ClientId, lastLeadQueueId.Value);
    }

    private async Task ProcessAgentComment(LeadFeedbackFilledMessage message)
    {
        if (string.IsNullOrEmpty(message.AgentComment))
            return;

        var leadComment = new LeadComment
        {
            Comment = message.AgentComment,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedById = message.AgentId,
            LeadId = message.LeadId,
        };

        await _leadCommentRepository.AddComment(leadComment);

        var leadFeedbackProcessedMessage = new LeadFeedbackProcessedMessage(
            message.ClientId, message.LeadId,
            message.LeadStatus, message.AgentId,
            message.AgentComment!);

        await _natsPublisher.PublishAsync(_pubSubjects.LeadFeedbackProcessed, leadFeedbackProcessedMessage);
    }
}
