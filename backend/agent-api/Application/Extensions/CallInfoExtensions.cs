using KL.Agent.API.Application.Models;
using KL.Agent.API.Application.Models.Messages;
using KL.Agent.API.Persistent.Entities.Cache;

namespace KL.Agent.API.Application.Extensions;

public static class CallInfoExtensions
{
    public static CallInfo ToCallInfo(this CallInfoCache cache) =>
        new CallInfo(
            cache.ClientId,
            cache.SessionId,
            cache.CallType,
            cache.LeadId,
            cache.LeadPhone,
            cache.LeadScore,
            cache.QueueId,
            cache.CallOriginatedAt,
            cache.LeadAnsweredAt,
            cache.AgentAnsweredAt,
            cache.CallFinishedAt,
            cache.CallFinishReason,
            cache.ManagerRtcUrl,
            cache.AgentRtcUrl,
            cache.CallAgainCount
            );

    public static CallInfo ToCallInfo(this CallFinishedMessage message, long leadScore, long? callAgainCount) =>
        new CallInfo(
            message.ClientId,
            message.SessionId,
            message.CallType,
            message.LeadId,
            message.LeadPhone,
            leadScore,
            message.QueueId,
            message.CallOriginatedAt.ToUnixTimeSeconds(),
            message.LeadAnswerAt?.ToUnixTimeSeconds(),
            message.AgentAnswerAt?.ToUnixTimeSeconds(),
            message.CallHangupAt.ToUnixTimeSeconds(),
            message.ReasonCode,
            null,
            null,
            callAgainCount
            );

    public static CallInfo ToCallInfo(this CalleeAnsweredMessage message, long leadScore, long? callAgainCount) =>
        new CallInfo(
            message.ClientId,
            message.SessionId,
            message.CallType,
            message.LeadId,
            message.LeadPhone,
            leadScore,
            message.QueueId,
            message.CallOriginatedAt.ToUnixTimeSeconds(),
            message.LeadAnswerAt?.ToUnixTimeSeconds(),
            message.AgentAnswerAt?.ToUnixTimeSeconds(),
            null,
            null,
            message.ManagerRtcUrl,
            message.AgentRtcUrl,
            callAgainCount
            );

      public static CallInfo ToCallInfo(this InviteAgentMessage message) =>
        new CallInfo(
            message.ClientId,
            message.SessionId,
            message.CallType,
            message.LeadId,
            message.LeadPhone,
            0,//leadScore,
            null,// message.QueueId,
            message.CallOriginatedAt.ToUnixTimeSeconds(),
            null, //message.LeadAnswerAt?.ToUnixTimeSeconds(),
            null, //message.AgentAnswerAt?.ToUnixTimeSeconds(),
            null,
            null,
            null, //message.ManagerRtcUrl,
            message.AgentRtcUrl,
            0//callAgainCount
            );
}
