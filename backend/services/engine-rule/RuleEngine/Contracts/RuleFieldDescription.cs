using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleFieldDescription
{
    public RuleFieldDescription(int fieldId,
        string type,
        string dimension,
        IEnumerable<LabelValue>? values,
        bool isRequired)
    {
        FieldId = fieldId;
        Type = type;
        Dimension = dimension;
        Values = values;
        IsRequired = isRequired;
    }
    public int FieldId { get; set; }
    public string Type { get; set; }
    public string Dimension { get; set; }
    public IEnumerable<LabelValue>? Values { get; set; }
    public bool IsRequired { get; set; }
}