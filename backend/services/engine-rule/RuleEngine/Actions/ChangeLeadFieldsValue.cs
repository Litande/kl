using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using System.Reflection;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public class ChangeLeadFieldsValueAction : RuleActionBase
{
    public ChangeLeadFieldsValueAction() : base(
        new[]
        {
            RuleGroupTypes.ForwardRules,
            RuleGroupTypes.Behavior,
        },
        new RuleActionDescription(
            RulesAction.ChangeLeadFieldsValue.ToString(),
            RulesAction.ChangeLeadFieldsValue.ToDescription(),
            ActionOperationList(new[] { ActionOperation.Set }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Select.ToDescription(), "field",
                    LeadFields, true),
                new(2, RuleValueType.String.ToDescription(), "value",
                    Enumerable.Empty<LabelValue>(), true)
            }
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        ValidateFields(data, 2);

        var param1 = GetValueTypeValidated(data.Fields![0], data.Name, RuleValueType.String, RuleValueType.Select);
        var param2 = GetValueTypeValidated(data.Fields![1], data.Name, LeadFieldTypes[param1.Value!]);

        var result = new ChangeLeadFieldsValueExecutor(param1.Value!, param2.Value!);
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class ChangeLeadFieldsValueExecutor : IRuleActionExecutor
    {
        private readonly string _fieldName;
        private readonly object _fieldValue;

        public ChangeLeadFieldsValueExecutor(string fieldName, string fieldValue)
        {
            _fieldName = fieldName;
            _fieldValue = fieldValue;
        }

        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            var property = typeof(TrackedLead)
                .GetProperty(_fieldName, BindingFlags.Instance | BindingFlags.Public);

            if (property is null || !property.CanWrite)
                throw new ArgumentException($"Expecting non null TrackedLead {_fieldName} property");

            var fieldValue = Convert.ChangeType(_fieldValue, property.PropertyType);
            property.SetValue(target, fieldValue, null);
            return Task.CompletedTask;
        }
    }
}
