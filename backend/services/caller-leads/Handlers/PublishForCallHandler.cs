using KL.Caller.Leads.App;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Handlers;

public class PublishForCallHandler : IPublishForCallHandler
{
    private readonly LeadOptions _leadOptions;
    private readonly ICallPublishService _callPublishService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ICDRService _cdrService;

    public PublishForCallHandler(
        IOptions<LeadOptions> leadOptions,
        ICallPublishService callPublishService,
        IHostEnvironment hostEnvironment,
        ICDRService cdrService)
    {
        _leadOptions = leadOptions.Value;
        _callPublishService = callPublishService;
        _hostEnvironment = hostEnvironment;
        _cdrService = cdrService;
    }

    public async Task Process(
        IEnumerable<CallToRequest> requests,
        CancellationToken ct = default)
    {
        var overridePhone = !_hostEnvironment.IsProduction()
                            && !string.IsNullOrWhiteSpace(_leadOptions.ReplacePhoneWith);

        foreach (var item in requests)
        {
            var request = overridePhone
                ? item with { Phone = _leadOptions.ReplacePhoneWith! }
                : item;

            await Process(request, ct);
        }
    }

    public async Task Process(
        CallToRequest request,
        CancellationToken ct = default)
    {
        var sessionId = CreateSessionId();
        var cdr = await _cdrService.Add(request, request.Phone, sessionId, ct);
        var command = new CallToLeadMessage(
            request.ClintId,
            request.BridgeId,
            cdr.SessionId!,
            request.CallType,
            request.LeadQueueId,
            request.LeadId,
            request.IsFixedAssigned,
            request.Phone,
            request.AgentId,
            request.RingingTimeout,
            request.MaxCallDuration,
            request.IceServers,
            request.IsTest,
            request.SipProvider);

        await _callPublishService.Process(command, ct);
    }

    public string CreateSessionId()
    {
        // TODO ??? check is sessionId unique
        var sessionId = Guid.NewGuid().ToString();

        return sessionId;
    }
}
