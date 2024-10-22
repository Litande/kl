using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Actions;

public class FreezeLeadForXAction : RuleActionBase
{
    public FreezeLeadForXAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior
        },
        new RuleActionDescription(
            RulesAction.FreezeLeadForX.ToString(),
            RulesAction.FreezeLeadForX.ToDescription(),
            null, // ActionOperation.Set
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "value", Enumerable.Empty<LabelValue>(), true),
                new(2, RuleValueType.Select.ToDescription(), "units", TimeUnitsList, true)
            }
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        ValidateFields(data, 2);

        var param1Value = ParseLong(data.Fields![0], data.Name);
        var param2 = GetValueTypeValidated(data.Fields[1], data.Name, RuleValueType.String, RuleValueType.Select);

        if (!Enum.TryParse(param2.Value, true, out TimeUnits operationTimeUnits))
            throw new ArgumentException("Invalid parameter(s) value", data.Name);

        var param2TimeSpan = ConvertToTimeSpan(operationTimeUnits, param1Value, data.Name);

        var result = new FreezeLeadForXExecutor(param2TimeSpan);
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class FreezeLeadForXExecutor : IRuleActionExecutor
    {
        private readonly TimeSpan _value;

        public FreezeLeadForXExecutor(TimeSpan value)
        {
            _value = value;
        }

        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            lead.FreezeTo = DateTimeOffset.UtcNow + _value;
            return Task.CompletedTask;
        }
    }
}
