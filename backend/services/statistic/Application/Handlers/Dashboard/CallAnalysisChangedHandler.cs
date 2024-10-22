using System.Text.Json;
using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;
using KL.Statistics.Application.Services;
using KL.Statistics.Application.SignalR;
using KL.Statistics.DAL.Repositories;

namespace KL.Statistics.Application.Handlers.Dashboard;

public class CallAnalysisChangedHandler : ICallAnalysisChangedHandler
{
    private readonly IClientRepository _clientRepository;
    private readonly IDashboardService _dashboardService;
    private readonly IHubSender _hubSender;
    private readonly ILogger<CallAnalysisChangedHandler> _logger;

    public CallAnalysisChangedHandler(
        IDashboardService dashboardService,
        ILogger<CallAnalysisChangedHandler> logger,
        IClientRepository clientRepository,
        IHubSender hubSender)
    {
        _dashboardService = dashboardService;
        _logger = logger;
        _clientRepository = clientRepository;
        _hubSender = hubSender;
    }

    public async Task Handle(CancellationToken ct = default)
    {
        var clients = await _clientRepository.GetAll(ct);
        foreach (var client in clients)
        {
            _logger.LogInformation(
                "{CallAnalysisChangedHandler} requested for client Id: {clientId}",
                nameof(CallAnalysisChangedHandler), client.Id);

            var callAnalysis = new List<CallAnalysisResponse>();
            foreach (var type in Enum.GetValues(typeof(PeriodTypes)).Cast<PeriodTypes>())
            {
                var analysis = await _dashboardService.CalculateCallAnalysis(client.Id, type, ct);
                callAnalysis.Add(analysis);
            }

            await _hubSender.SendCallAnalysis(client.Id, callAnalysis, ct);

            _logger.LogInformation("The client Id {clientId} sent call analysis statistics: {callAnalysis}",
                client.Id, JsonSerializer.Serialize(callAnalysis));
        }
    }
}