// using Plat4Me.DialRuleEngine.Application.Enums;
// using Plat4Me.DialRuleEngine.Application.Extensions;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
// using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
// using RulesEngine.Models;
//
// namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;
//
// public class StatusForTimeBasedOnlyCondition : RuleConditionBase, IRuleCondition
// {
//     public StatusForTimeBasedOnlyCondition() : base(
//         new RuleGroupTypes[]
//         {
//             RuleGroupTypes.Behavior,
//             RuleGroupTypes.LeadScoring
//         },
//         new RuleConditionDescription(
//             RulesCondition.StatusForTimeBasedOnly.ToString(),
//             RulesCondition.StatusForTimeBasedOnly.ToDescription(),
//             null,
//             null
//         )
//     )
//     { }
//
//     public override Task<Rule> Prepare(RuleGroupData data)
//     {
//         throw new NotImplementedException();
//     }
// }
