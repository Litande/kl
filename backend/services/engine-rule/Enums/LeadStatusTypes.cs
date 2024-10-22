using System.ComponentModel;

namespace KL.Engine.Rule.Enums;

public enum LeadStatusTypes
{
    [Description("Busy")]
    Busy = 1,
    [Description("Call Again Personal")]
    CallAgainPersonal = 2,
    [Description("Call Again General")]
    CallAgainGeneral = 3,
    [Description("Cannot Talk")]
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
    [Description("Excessive Failed To Connect")]
    ExcessiveFailedToConnect = 11,
    [Description("Feedback Default")]
    FeedbackDefault = 12, // Status (Choose Status)
    [Description("In The Money")]
    InTheMoney = 13,
    [Description("Language Barrier")]
    LanguageBarrier = 14, // (no Valid agent)
    [Description("Max Call")]
    MaxCall = 15,
    [Description("Move To Old Brands")]
    MoveToOldBrands = 16,
    [Description("NA")]
    NA = 17,
    [Description("Never Registered")]
    NeverRegistered = 18,
    [Description("New Lead")]
    NewLead = 19,
    [Description("No Money")]
    NoMoney = 20,
    [Description("Not Eligible")]
    NotEligible = 21,
    [Description("Not Interested")]
    NotInterested = 22,
    [Description("Small Barrier")]
    SmallBarrier = 23,
    [Description("System Answer")]
    SystemAnswer = 24, // (Dropped)
    [Description("System Failed To Connect")]
    SystemFailedToConnect = 25,
    [Description("System VM")]
    SystemVM = 26,
    [Description("Test Lead")]
    TestLead = 27,
    [Description("Under 18")]
    Under18 = 28,
    [Description("Wrong Number")]
    WrongNumber = 29,
}
