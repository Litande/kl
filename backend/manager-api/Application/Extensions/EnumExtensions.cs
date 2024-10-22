using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Extensions;

public static class EnumExtensions
{
    public static string ToDescription(this Enum en)
    {
        var descriptionAttribute = GetAttributeOrDefault<DescriptionAttribute>(en);
        if (descriptionAttribute != null)
        {
            return descriptionAttribute.Description;
        }

        var displayAttribute = GetAttributeOrDefault<DisplayAttribute>(en);
        if (displayAttribute != null)
        {
            return displayAttribute.Name;
        }

        var displayNameAttribute = GetAttributeOrDefault<DisplayNameAttribute>(en);
        return displayNameAttribute != null ? displayNameAttribute.DisplayName : en.ToString();
    }

    public static List<KeyValuePair<T, string?>> EnumToList<T>()
    {
        var enumType = typeof(T);
        var enumValArray = Enum.GetValues(enumType);
        var enumValList = new List<KeyValuePair<T, string?>>(enumValArray.Length);
        foreach (int val in enumValArray)
        {
            var item = Enum.Parse(enumType, val.ToString());
            enumValList.Add(new KeyValuePair<T, string?>((T)item, ((Enum)item).ToDescription()));
        }

        return enumValList;
    }

    public static T? GetAttributeOrDefault<T>(this Enum enumeration)
        where T : Attribute
    {
        return GetAttributeInner<T>(enumeration);
    }

    private static T? GetAttributeInner<T>(Enum enumeration) where T : Attribute
    {
        var members = enumeration
            .GetType()
            .GetMember(enumeration.ToString()).ToList();
        if (members.Count <= 0)
            throw new Exception($"Value {enumeration} does not belong to enumeration {enumeration.GetType()}");

        return members[0]
            .GetCustomAttributes(typeof(T), false)
            .Cast<T>()
            .SingleOrDefault();
    }

    public static RuleGroupTypes GetRuleGroupType(this DisplayRuleGroupTypes ruleGroupTypes)
    {
        return ruleGroupTypes switch
        {
            DisplayRuleGroupTypes.NewLeads => RuleGroupTypes.ForwardRules,
            DisplayRuleGroupTypes.Main => RuleGroupTypes.Behavior,
            DisplayRuleGroupTypes.LeadScoring => RuleGroupTypes.LeadScoring,
            DisplayRuleGroupTypes.ApiRules => RuleGroupTypes.ApiRules,
            _ => throw new ArgumentException($"Group types: {ruleGroupTypes} not implemented")
        };
    }

    public static DisplayRuleGroupTypes GetRuleGroupType(this RuleGroupTypes groupTypes)
    {
        return groupTypes switch
        {
            RuleGroupTypes.ForwardRules => DisplayRuleGroupTypes.NewLeads,
            RuleGroupTypes.Behavior => DisplayRuleGroupTypes.Main,
            RuleGroupTypes.LeadScoring => DisplayRuleGroupTypes.LeadScoring,
            RuleGroupTypes.ApiRules => DisplayRuleGroupTypes.ApiRules,
            _ => throw new ArgumentException($"Group types: {groupTypes} not implemented")
        };
    }
}