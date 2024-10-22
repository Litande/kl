namespace KL.Statistics.Application.Common.Enums;

public enum CallFinishReasons
{
    Unknown = 0,
    CallFinishedByLead = 100,
    CallFinishedByAgent = 101,
    CallFinishedByManager = 102,

    SessionTimeout = 201,
    NoAvailableAgents = 202,
    LeadLineBusy = 203,
    LeadInvalidPhone = 204,
    AgentNotAnswerLeadHangUp = 205,
    AgentReconnectTimeout = 206,
    LeadNotAnswer = 207,
    AgentTimeout = 208,
    ExceededMaxCallDuration = 209,

    SIPTransportError = 300,

    RTCTransportTimeout = 400,

    BridgeFailed = 500
}
