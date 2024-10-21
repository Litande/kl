using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Plat4Me.DialSipBridge.Application.Configurations;
using Plat4Me.DialSipBridge.Application.Enums;
using Plat4Me.DialSipBridge.Application.Session;
using SIPSorcery.SIP;

namespace Plat4Me.DialSipBridge.Application.Services;

public class CallService : ICallService, IDisposable
{
    private readonly ConcurrentDictionary<string, ICallSession> _sessions = new();
    private readonly ILogger _logger;
    private readonly SIPOptions _sipOptions;
    private readonly SIPTransport _sipTransport;

    private readonly IServiceProvider _serviceProvider;

    public CallService(
        ILogger<CallService> logger,
        IServiceProvider serviceProvider,
        IOptions<SIPOptions> sipOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _sipOptions = sipOptions.Value;
        _sipTransport = new SIPTransport();
        InitSipTransport();
    }

    public ICallSession? GetSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
            return session;

        return null;
    }

    public async Task<string?> CreateSession(string sessionId, InitCallData callData)
    {
        try
        {
            var builder = _serviceProvider.GetRequiredService<CallSessionBuilder>();

            var callSession = builder
                .Build(sessionId, callData, _sipTransport);

            callSession.OnClosed += s =>
            {
                _sessions.Remove(s.Id, out _);
                _logger.LogInformation("Session closed {sessionId}", s.Id);
            };

            _sessions.TryAdd(callSession.Id, callSession);
            await callSession.Start(callData);
            if (callData.CallType == CallType.Predictive)
                _logger.LogInformation("Create predictive session {id} for {leadId}/{leadPhone} and agentId {agentId} (MaxDur={duration},RingTO={ringingTimout})",
                    callSession.Id, callData.LeadId, callData.LeadPhone, callData.AgentId, callData.MaxCallDuration, callData.RingingTimeout);
            else
                _logger.LogInformation("Create manual session {id} for {leadPhone} and agentId {agentId}, (MaxDur={duration},RingTO={ringingTimout})",
                    callSession.Id, callData.LeadPhone, callData.AgentId,  callData.MaxCallDuration, callData.RingingTimeout);
            return callSession.Id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during session creation");
        }
        return null;
    }

    public void CloseSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.Close(new CloseCommand(CallFinishReasons.Unknown));
        }
    }

    public long SessionCount()
    {
        return _sessions.Count;
    }

    public void Dispose()
    {
        _sipTransport.Dispose();
    }

    private void InitSipTransport()
    {
        _sipTransport.AddSIPChannel(new SIPUDPChannel(new IPEndPoint(IPAddress.Any, _sipOptions.UDPTransportPort)));

        _sipTransport.SIPRequestInTraceEvent += (localEP, remoteEP, req) =>
        {
            _logger.LogTrace($"Request received: {localEP}<-{remoteEP}\n{req.ToString()}");
        };

        _sipTransport.SIPRequestOutTraceEvent += (localEP, remoteEP, req) =>
        {
            _logger.LogTrace($"Request sent: {localEP}->{remoteEP}\n{req.ToString()}");
        };

        _sipTransport.SIPResponseInTraceEvent += (localEP, remoteEP, resp) =>
        {
            _logger.LogTrace($"Response received: {localEP}<-{remoteEP}\n{resp.ToString()}");
        };

        _sipTransport.SIPResponseOutTraceEvent += (localEP, remoteEP, resp) =>
        {
            _logger.LogTrace($"Response sent: {localEP}->{remoteEP}\n{resp.ToString()}");
        };

        _sipTransport.SIPRequestRetransmitTraceEvent += (tx, req, count) =>
        {
            _logger.LogTrace($"Request retransmit {count} for request {req.StatusLine}, initial transmit {DateTime.Now.Subtract(tx.InitialTransmit).TotalSeconds.ToString("0.###")}s ago.\n{req.ToString()}");
        };

        _sipTransport.SIPResponseRetransmitTraceEvent += (tx, resp, count) =>
        {
            _logger.LogTrace($"Response retransmit {count} for response {resp.ShortDescription}, initial transmit {DateTime.Now.Subtract(tx.InitialTransmit).TotalSeconds.ToString("0.###")}s ago.\n{resp.ToString()}");
        };
    }
}
