using KL.Caller.Leads.App;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Services;

public class DropRateService : IDropRateService
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly ICDRRepository _crdrRepository;
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;
    private readonly IQueueDropRateCacheRepository _queueDropRateCacheRepository;
    private readonly ILogger<DropRateService> _logger;

    public DropRateService(
        ILeadQueueRepository leadQueueRepository,
        ICDRRepository crdrRepository,
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> pubSubjects,
        IQueueDropRateCacheRepository queueDropRateCacheRepository,
        ILogger<DropRateService> logger)
    {
        _leadQueueRepository = leadQueueRepository;
        _crdrRepository = crdrRepository;
        _natsPublisher = natsPublisher;
        _queueDropRateCacheRepository = queueDropRateCacheRepository;
        _logger = logger;
        _pubSubjects = pubSubjects.Value;
    }

    public async Task Process(CancellationToken ct = default)
    {
        var queues = await _leadQueueRepository.GetAll(ct);
        var clientQueueGroups = queues.GroupBy(r => r.ClientId);

        foreach (var item in clientQueueGroups)
            await ProcessClient(item.Key, item, ct);
    }

    private async Task ProcessClient(
        long clientId,
        IGrouping<long, LeadQueue> queues,
        CancellationToken ct = default)
    {
        var startProcessTime = DateTimeOffset.UtcNow;
        var maxDropRatePeriod = queues.Max(r => r.DropRatePeriod);
        var processTimeFrom = startProcessTime.AddSeconds(-maxDropRatePeriod);
        var cdrList = await _crdrRepository.GetForPeriod(clientId, processTimeFrom, ct);
        var queueRatioUpdates = new Dictionary<long, double>();
        var queueDropRateCaches = await _queueDropRateCacheRepository.GetQueueByClient(clientId, ct);

        foreach (var queue in queues)
        {
            var queueCdrList = cdrList.Where(r => r.LeadQueueId == queue.Id);
            var newDropRate = CalculateDropRate(startProcessTime, queueCdrList, queue.DropRatePeriod);
            queueDropRateCaches.TryGetValue(queue.Id, out var queueCache);

            _logger.LogInformation("Client id {clientId} update queue id {queueId} with drop rate {dropRate} (current drop rate {currentDropRate})",
                clientId, queue.Id, newDropRate, queueCache?.DropRate);

            if (newDropRate is null)// || newDropRate == queueCache?.DropRate)
                continue;

            await _queueDropRateCacheRepository.Update(clientId, queue.Id, newDropRate.Value);

            var isRatioUpdateAvailable = !queue.RatioUpdatedAt.HasValue || startProcessTime > queue.RatioUpdatedAt + TimeSpan.FromSeconds(queue.RatioFreezeTime);
            _logger.LogInformation("isRatioUpdateAvailable {isRatioUpdateAvailable}", isRatioUpdateAvailable);

            if (!isRatioUpdateAvailable) continue;

            if (TryAdjustRatioByThreshold(queue, newDropRate.Value, out var newRatio))
            {
                queueRatioUpdates.Add(queue.Id, newRatio!.Value);
                _logger.LogInformation("TryAdjustRatioByThreshold {newRatio}", newRatio);
            }
        }

        if (queueRatioUpdates.Any())
            await _leadQueueRepository.UpdateRatio(queueRatioUpdates, ct);

        var message = new QueuesUpdatedMessage(clientId);
        await _natsPublisher.PublishAsync(_pubSubjects.LeadsQueueUpdate, message);
    }

    private static double? CalculateDropRate(
        DateTimeOffset startProcessTime,
        IEnumerable<CallDetailRecord> cdrList,
        int dropRatePeriod)
    {
        var calculationPeriodFrom = startProcessTime.AddSeconds(-dropRatePeriod);
        var queueCdrList = cdrList.Where(r => r.OriginatedAt > calculationPeriodFrom).ToArray();
        var totalCallCount = queueCdrList.Length;

        if (totalCallCount == 0)
            return null;

        var droppedCallCount = queueCdrList
            .Count(r => (!r.UserAnswerAt.HasValue && r.CallHangupStatus is CallFinishReasons.AgentTimeout)
                        || r.CallHangupStatus is CallFinishReasons.NoAvailableAgents
                            or CallFinishReasons.AgentNotAnswerLeadHangUp);

        if (droppedCallCount is 0)
            return 0;

        return (double)droppedCallCount / (double)totalCallCount;
    }

    private static bool TryAdjustRatioByThreshold(LeadQueue queue, double dropRate, out double? ratio)
    {
        const int minRatioValue = 1;
        ratio = null;
        if (queue.DropRateUpperThreshold <= dropRate)
        {
            ratio = queue.Ratio - queue.RatioStep;
            if (ratio < minRatioValue)
                ratio = minRatioValue;

            return true;
        }

        if (queue.DropRateLowerThreshold >= dropRate)
        {
            ratio = queue.Ratio + queue.RatioStep;
            return true;
        }

        return false;
    }
}
