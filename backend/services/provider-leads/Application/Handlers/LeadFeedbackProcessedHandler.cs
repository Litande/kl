using System.Text.Json;
using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Application.Handlers.Interfaces;
using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Application.Models.Messages;
using KL.Provider.Leads.Persistent.LeadCallbackHttpClient;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Application.Handlers;

public class LeadFeedbackProcessedHandler : ILeadFeedbackProcessedHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly IStatusDataSourceMapRepository _statusDataSourceMapRepository;
    private readonly ILeadCallbackClient _leadCallbackClient;
    private readonly ILogger<LeadFeedbackProcessedHandler> _logger;

    public LeadFeedbackProcessedHandler(
        ILeadRepository leadRepository,
        IStatusDataSourceMapRepository statusDataSourceMapRepository,
        ILeadCallbackClient leadCallbackClient,
        ILogger<LeadFeedbackProcessedHandler> logger)
    {
        _leadRepository = leadRepository;
        _statusDataSourceMapRepository = statusDataSourceMapRepository;
        _leadCallbackClient = leadCallbackClient;
        _logger = logger;
    }

    public async Task Process(LeadFeedbackProcessedMessage message, CancellationToken ct = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var lead = await _leadRepository.GetLeadWithDataSourceById(message.LeadId, ct);
        if (lead is null)
            throw new KeyNotFoundException($"Cannot find lead with id: {message.LeadId}");

        if (lead.DataSource.CallbackEndpoints is null)
            return;

        var callbackSettings =
            JsonSerializer.Deserialize<IReadOnlyCollection<CallbackSettings>>(lead.DataSource.CallbackEndpoints);

        if (callbackSettings is null)
            return;

        var statusDataSourceMap = await _statusDataSourceMapRepository.GetByStatus(message.Status, ct);
        foreach (var callback in callbackSettings)
        {
            if (callback.CallbackType == CallbackTypes.LeadStatus.ToString() &&
                statusDataSourceMap is null)
                continue;

            var response = await _leadCallbackClient.SendLeadCallback(callback, lead.ExternalId,
                statusDataSourceMap?.ExternalStatusId, message.Comment);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP {Callback} return response: {0}", callback.CallbackType, response);
            }
        }
    }
}