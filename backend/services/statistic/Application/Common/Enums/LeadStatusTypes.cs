using System.ComponentModel;

namespace Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

public enum LeadStatusTypes
{
    [Description("Busy")]
    Busy = 1,

    [Description("Call Again Personal")]
    CallAgainPersonal = 2,

    [Description("Call Again General")]
    CallAgainGeneral = 3,

    [Description("Can’t Talk")]
    CannotTalk = 4,

    [Description("Check Number")]
    CheckNumber = 5,

    [Description("Decline CAP")]
    DeclineCAP = 6,

    [Description("Decline Not CAP")]
    DeclineNotCAP = 7,

    [Description("DNC")]
    DNC = 8,

    [Description("DNC Country")]
    DNCCountry = 9,

    [Description("Duplicate")]
    Duplicate = 10,

    [Description("Excessive Failed to Connect")]
    ExcessiveFailedToConnect = 11,

    [Description("Feedback Default status (Choose Status)")]
    FeedbackDefault = 12,

    [Description("In the Money")]
    InTheMoney = 13,

    [Description("Language Barrier (no Valid agent)")]
    LanguageBarrier = 14,

    [Description("Max Call")]
    MaxCall = 15,

    [Description("Move to Old Brands")]
    MoveToOldBrands = 16,

    [Description("NA")]
    NA = 17,

    [Description("Never Registred")]
    NeverRegistered = 18,

    [Description("New Lead")]
    NewLead = 19,

    [Description("No Money")]
    NoMoney = 20,

    [Description("Not Elgible")]
    NotEligible = 21,

    [Description("Not Interested")]
    NotInterested = 22,

    [Description("Small Barrier")]
    SmallBarrier = 23,

    [Description("System - Answer (Dropped)")]
    SystemAnswer = 24,

    [Description("System - Failed To Connect")]
    SystemFailedToConnect = 25,

    [Description("System - VM")]
    SystemVM = 26,

    [Description("Test Lead")]
    TestLead = 27,

    [Description("Under 18")]
    Under18 = 28,

    [Description("Wrong Number")]
    WrongNumber = 29,
}
