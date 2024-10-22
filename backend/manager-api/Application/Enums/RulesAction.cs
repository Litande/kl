using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

public enum RulesAction
{
    [Description("Freeze lead for X")]
    FreezeLeadForX = 1,
    [Description("Change status")]
    ChangeStatus = 2,
    [Description("Delete lead")]
    DeleteLead = 3,
    [Description("Lock lead on ratio1")]
    LockLeadOnRatio1 = 4,
    [Description("Change lead fields value")]
    ChangeLeadFieldsValue = 5,
    [Description("Campaign lead permanent agent assignment")]
    CampaignLeadPermanentAgentAssignment = 6,
    [Description("Set lead weight by X")]
    SetLeadWeightByX = 7,
    [Description("Update remote adapter lead data")]
    UpdateRemoteAdapterLeadData = 8,
    [Description("Update status on remote adapter with comment")]
    UpdateStatusOnRemoteAdapterWithComment = 9,
    [Description("Move lead to queue")]
    MoveLeadToQueue = 10,
}
