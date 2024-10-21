// using Plat4Me.DialRuleEngine.Application.Enums;
// using Plat4Me.DialRuleEngine.Application.Extensions;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
//
// namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

// public class CampaignLeadPermanentAgentAssignmentAction : RuleActionBase
// {
//     public CampaignLeadPermanentAgentAssignmentAction() : base(
//         new RuleGroupTypes[]
//         {
//             RuleGroupTypes.Behavior
//         },
//         new RuleActionDescription(
//             RulesAction.CampaignLeadPermanentAgentAssignment.ToString(),
//             RulesAction.CampaignLeadPermanentAgentAssignment.ToDescription(),
//             ActionOperationList(new[] { ActionOperation.Is }),
//             new RuleFieldDescription[]
//             {
//                 new(1, RuleValueType.String.ToDescription(), "assign", Toggles, true)
//             }
//         ))
//     {
//     }
//
//     public override Task<IRuleActionExecutor> Create(RuleActionData data)
//     {
//         throw new NotImplementedException();
//     }
//
//     protected class CampaignLeadPermanentAgentAssignmentExecutor : IRuleActionExecutor
//     {
//         public CampaignLeadPermanentAgentAssignmentExecutor()
//         {
//         }
//
//         public Task Process(object target)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
