using KL.Caller.Leads.App;
using KL.Caller.Leads.Clients;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Services;

public class BridgeService : IBridgeService
{
    private List<BridgeInfo> _bridges = new();
    private readonly ILogger<BridgeService> _logger;
    private readonly IServiceProvider _services;
    private readonly INatsPublisher _natsPublisher;
    private readonly SubSubjects _subSubjects;
    private static object _bridgeLock = new();

    public BridgeService(
        ILogger<BridgeService> logger,
        IServiceProvider services,
        INatsPublisher natsPublisher,
        IOptions<SubSubjects> subSubjects
    )
    {
        _logger = logger;
        _services = services;
        _natsPublisher = natsPublisher;
        _subSubjects = subSubjects.Value;
    }

    public bool AnyBridgeRegistered()
    {
        lock (_bridgeLock)
        {
            return _bridges.Any();
        }
    }

    public void RegisterBridge(string bridgeId, string bridgeAddr)
    {
        _logger.LogInformation("Register bridge {bridgeId} on {bridgeAddr}", bridgeId, bridgeAddr);
        var bridgeInfo = new BridgeInfo
        {
            BridgeId = bridgeId,
            BridgeAddr = bridgeAddr
        };

        lock (_bridgeLock)
        {
            if (!_bridges.Any(x => string.Equals(x.BridgeId, bridgeId)))
                _bridges.Add(bridgeInfo);
        }
    }

    public string? GetBridge()
    {
        lock (_bridgeLock)
        {
            var bridge = _bridges.FirstOrDefault();
            if (bridge == null)
                return null;

            if (_bridges.Count > 1)
            {
                _bridges.RemoveAt(0);
                _bridges.Add(bridge);
            }

            return bridge.BridgeId;
        }
    }

    public async Task PingBridges()
    {
        BridgeInfo[] bridges;
        lock (_bridgeLock)
        {
            bridges = _bridges.ToArray();
        }
        await using var scope = _services.CreateAsyncScope();

        var failedBridges = await PingBridgesAndGetFailed(scope, bridges);
        lock (_bridgeLock)
        {
            _bridges = _bridges.Where(x => !failedBridges.Contains(x.BridgeId)).ToList();
        }
        var failedCalls = await GetFailedCalls(scope, failedBridges);
        await GenerateCallFailedForFailedCalls(failedCalls);
    }

    private async Task<HashSet<string>> PingBridgesAndGetFailed(AsyncServiceScope scope, IEnumerable<BridgeInfo> bridges)
    {
        var requests = bridges.Select(async x =>
            {
                try
                {
                    var client = scope.ServiceProvider.GetRequiredService<IBridgeClient>();
                    var bridgeId = await client.Ping(x.BridgeAddr);
                    if (string.Equals(bridgeId, x.BridgeId))
                    {
                        return null;
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bridge ping failed");
                }
                return x.BridgeId;
            });
        return (await Task.WhenAll(requests)).Where(x => x is not null).ToHashSet()!;
    }

    private async Task<IReadOnlyCollection<CallDetailRecord>> GetFailedCalls(AsyncServiceScope scope, IReadOnlyCollection<string> failedBridges)
    {
        var cdrRepository = scope.ServiceProvider.GetRequiredService<ICDRRepository>();
        return await cdrRepository.GetIncompleteByBridgeIds(failedBridges);
    }

    private async Task GenerateCallFailedForFailedCalls(IEnumerable<CallDetailRecord> failedCalls)
    {
        foreach (var callInfo in failedCalls)
        {
            try
            {
                var message = new CallFinishedMessage(
                    ClientId: callInfo.ClientId,
                    BridgeId: callInfo.BridgeId!,
                    SessionId: callInfo.SessionId,
                    CallType: callInfo.CallType,
                    AgentId: callInfo.LastUserId!.Value,
                    QueueId: callInfo.LeadQueueId,
                    LeadId: callInfo.LeadId,
                    LeadPhone: callInfo.LeadPhone,
                    ReasonCode: CallFinishReasons.BridgeFailed,
                    ReasonDetails: null,
                    SipProviderId: callInfo.SipProviderId,
                    SipErrorCode: null,
                    CallOriginatedAt: callInfo.OriginatedAt,
                    LeadAnswerAt: callInfo.LeadAnswerAt ?? DateTimeOffset.UtcNow,
                    AgentAnswerAt: callInfo.UserAnswerAt ?? DateTimeOffset.UtcNow,
                    CallHangupAt: DateTimeOffset.UtcNow);

                await _natsPublisher.PublishAsync(_subSubjects.CallFailed, message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed call processing failed for agent {agentId}", callInfo.LastUserId);
            }
        }
    }
}
