﻿using Plat4Me.DialLeadCaller.Application.Enums;
namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record EnqueueAgentForCallMessage(
    long ClientId,
    long AgentId,
    CallType callType,
    string Initiator);
