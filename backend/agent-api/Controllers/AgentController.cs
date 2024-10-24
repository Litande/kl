﻿using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Handlers;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace KL.Agent.API.Controllers;

[Route("agents")]
public class AgentController : ApiAuthorizeBase
{
    private readonly IAgentFilledCallInfoHandler _agentFilledCallInfoHandler;
    private readonly IAsyncPolicy _asyncPolicy;
    private readonly IAgentTimeoutService _agentTimeoutService;

    public AgentController(
        IAgentFilledCallInfoHandler agentFilledCallInfoHandler,
        IAgentTimeoutService agentTimeoutService,
        IAsyncPolicy asyncPolicy
    )
    {
        _agentFilledCallInfoHandler = agentFilledCallInfoHandler;
        // _agentVoiceMailHandler = agentVoiceMailHandler;
        _agentTimeoutService = agentTimeoutService;
        _asyncPolicy = asyncPolicy;
    }

    [HttpPost("filledcall")]
    public async Task<IActionResult> FilledCall(
        [FromBody] AgentFilledCallRequest request,
        CancellationToken ct)
    {
        if (!Enum.IsDefined(request.LeadStatus))
            return BadRequest("Invalid lead status");

        _agentTimeoutService.TryCancelTimeout(AgentTimeoutTypes.FeedbackTimeout, request.SessionId);

        var sessionId = request.SessionId;

        await _asyncPolicy.ExecuteAsync(
            async _ => await _agentFilledCallInfoHandler.Handle(CurrentClientId, CurrentUserId, request, ct: ct),
            new Context(sessionId));

        return Ok();
    }

}
