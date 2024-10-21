// using Plat4Me.DialRuleEngine.Application.Enums;
// using Plat4Me.DialRuleEngine.Application.Extensions;
// using Plat4Me.DialRuleEngine.Application.Models;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

// namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

// public class UpdateRemoteAdapterLeadDataAction : RuleActionBase
// {
//     public UpdateRemoteAdapterLeadDataAction() : base(
//         new RuleGroupTypes[]
//         {
//                 RuleGroupTypes.Behavior
//         },
//         new RuleActionDescription(
//             RulesAction.UpdateRemoteAdapterLeadData.ToString(),
//             RulesAction.UpdateRemoteAdapterLeadData.ToDescription(),
//             null, //ActionOperationList(new[] { ActionOperation.Set}),
//             new RuleFieldDescription[]
//             {
//                 new(1, RuleValueType.String.ToDescription(),"", Enumerable.Empty<LabelValue>(), true)
//             }
//         )
//     )
//     { }
//
//     public override Task<IRuleActionExecutor> Create(RuleActionData data)
//     {
//         throw new NotImplementedException();
//     }
//
//     protected class UpdateRemoteAdapterLeadDataExecutor : IRuleActionExecutor
//     {
//         public UpdateRemoteAdapterLeadDataExecutor()
//         {
//         }
//
//         public Task Process(object target)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
