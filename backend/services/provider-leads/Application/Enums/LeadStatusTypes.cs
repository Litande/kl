namespace Plat4Me.DialLeadProvider.Application.Enums;

public enum LeadStatusTypes
{
    Busy = 1,
    CallAgainPersonal = 2,
    CallAgainGeneral = 3,
    CannotTalk = 4,
    CheckNumber = 5,
    DeclineCAP = 6,
    DeclineNotCAP = 7,
    DNC = 8,
    DNCCountry = 9,
    Duplicate = 10,
    ExcessiveFailedToConnect = 11,
    FeedbackDefault = 12, // Status (Choose Status)
    InTheMoney = 13,
    LanguageBarrier = 14, // (no Valid agent)
    MaxCall = 15,
    MoveToOldBrands = 16,
    NA = 17,
    NeverRegistered = 18,
    NewLead = 19,
    NoMoney = 20,
    NotEligible = 21,
    NotInterested = 22,
    SmallBarrier = 23,
    SystemAnswer = 24, // (Dropped)
    SystemFailedToConnect = 25,
    SystemVM = 26,
    TestLead = 27,
    Under18 = 28,
    WrongNumber = 29,
    Processing = 51,
    InTheCall = 52,
}