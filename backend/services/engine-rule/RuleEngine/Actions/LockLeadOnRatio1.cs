// using Plat4Me.DialRuleEngine.Application.Enums;
// using Plat4Me.DialRuleEngine.Application.Extensions;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
//
// namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;
//
// public class LockLeadOnRatio1Action : RuleActionBase
// {
//     public LockLeadOnRatio1Action() : base(
//         new RuleGroupTypes[]
//         {
//             RuleGroupTypes.Behavior
//         },
//         new RuleActionDescription(
//             RulesAction.LockLeadOnRatio1.ToString(),
//             RulesAction.LockLeadOnRatio1.ToDescription(),
//             ActionOperationList(new[] { ActionOperation.Is }),
//             new RuleFieldDescription[]
//             {
//                 new(1, RuleValueType.String.ToDescription(), "lock", Toggles, true)
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
//
//     protected class LockLeadOnRatio1Executor : IRuleActionExecutor
//     {
//         public LockLeadOnRatio1Executor()
//         {
//         }
//
//         public Task Process(object target)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
