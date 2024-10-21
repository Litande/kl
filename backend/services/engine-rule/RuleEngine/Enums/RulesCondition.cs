using System.ComponentModel;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

public enum RulesCondition
{
    [Description("Previous status")]
    PreviousStatus = 1,
    [Description("New status")]
    NewStatus = 2,
    [Description("Campaign lead weight is")]
    CampaignLeadWeightIs = 3,
    [Description("New status was total X times")]
    NewStatusWasTotalXTimes = 4,
    [Description("New status was X consecutive times")]
    NewStatusWasXConsecutiveTimes = 5,
    [Description("Campaign lead assigned to agent")]
    CampaignLeadAssignedToAgent = 6,
    [Description("Lead had total of X calls")]
    LeadHadTotalOfXCalls = 7,
    [Description("New campaign lead")]
    NewCampaignLead = 8,
    [Description("Call duration")]
    CallDuration = 9,
    [Description("Country")]
    Country = 10,
    [Description("Status for time-based only")]
    StatusForTimeBasedOnly = 11,
    [Description("Lead field X total Y")]
    LeadFieldXTotalY = 12,
    [Description("Lead is in the system")]
    LeadIsInSystem = 13,
    [Description("Is imported")]
    IsImported = 14,
    [Description("Is test lead")]
    IsTestLead = 15,
    [Description("Is fixed assigned feedback")]
    IsFixedAssignedFeedback = 16,
    [Description("Is future call")]
    IsFutureCall = 17,
    [Description("New status was total X times for Y period")]
    NewStatusWasTotalXTimesYPeriod = 18,
    [Description("New status was X consecutive times for Y period")]
    NewStatusWasXConsecutiveTimesYPeriod = 19,
    [Description("Lead had total of X calls for Y period")]
    LeadHadTotalOfXCallsYPeriod = 20,
    [Description("Current status")]
    CurrentStatus = 21,
    [Description("Lead Field Generic")]
    LeadField = 22,
}
