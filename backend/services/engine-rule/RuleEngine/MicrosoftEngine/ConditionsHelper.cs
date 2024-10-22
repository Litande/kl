using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.MicrosoftEngine;

public static class ConditionsHelper
{
    public static bool IsLastStatus(TrackedLead lead, LeadStatusTypes status)
    {
        var history = lead.StatusHistory;
        return history.Any()
               && history.Last().Value == status;
    }

    public static bool IsPreviousStatus(TrackedLead lead, LeadStatusTypes status)
    {
        var statusHistory = lead.StatusHistory;
        if (!statusHistory.Any()
            || statusHistory.Count < 2) return false;

        var previousStatusIndex = statusHistory.Count - 2;
        return statusHistory.ElementAtOrDefault(previousStatusIndex).Value == status;
    }

    public static int GetTotalCallsSeconds(TrackedLead lead, ICDRRepository cdr)
    {
        var totalCallsSeconds = cdr.GetLeadTotalCallsSeconds(lead.LeadId);
        return (int)totalCallsSeconds;
    }

    public static int GetTotalCallsCount(TrackedLead lead, ICDRRepository cdr) =>
        GetTotalCallsCount(lead, cdr, null);

    public static int GetTotalCallsCount(TrackedLead lead, ICDRRepository cdr, DateTimeOffset? fromDateTime)
    {
        var totalCallsCount = cdr.GetLeadTotalCallsCount(lead.LeadId, fromDateTime);
        return totalCallsCount;
    }

    public static int LastStatusConsecutiveTimes(TrackedLead lead) =>
        StatusConsecutiveTimes(lead.StatusHistory, lead.Status);

    public static int LastStatusConsecutiveTimes(TrackedLead lead, DateTimeOffset fromDateTime) =>
        StatusConsecutiveTimes(lead.StatusHistory.Where(r => r.Key > fromDateTime), lead.Status);

    public static int LastStatusTimes(TrackedLead lead, DateTimeOffset fromDateTime) =>
        StatusTimes(lead.StatusHistory.Where(r => r.Key > fromDateTime), lead.Status);

    public static int LastStatusTimes(TrackedLead lead) =>
        StatusTimes(lead.StatusHistory, lead.Status);

    private static int StatusConsecutiveTimes(
        IEnumerable<KeyValuePair<DateTimeOffset, LeadStatusTypes>> statusHistory,
        LeadStatusTypes status)
    {
        var maxConsecutiveTimes = 0;
        var consecutiveTimes = 0;

        foreach (var item in statusHistory)
        {
            if (item.Value == status)
            {
                consecutiveTimes++;
                if (consecutiveTimes > maxConsecutiveTimes)
                    maxConsecutiveTimes = consecutiveTimes;
            }
            else consecutiveTimes = 0;
        }

        return maxConsecutiveTimes;
    }

    private static int StatusTimes(
        IEnumerable<KeyValuePair<DateTimeOffset, LeadStatusTypes>> statusHistory,
        LeadStatusTypes status)
    {
        var lastStatusCount = statusHistory
            .Count(r => r.Value == status);

        // TODO should we also calculate current status, +1
        return lastStatusCount;
    }

    public static bool MetaDataStringValueCmp(TrackedLead lead, string name, ComparisonOperation op, string cmpValue)
    {
        var metaValue = lead.MetaData?[name];
        if (metaValue is null) return false;
        var value = metaValue.GetValue<string>();
        return op switch
        {
            ComparisonOperation.Equal => value.Equals(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.NotEqual => !value.Equals(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.Contains => value.Contains(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.NotContains => !value.Contains(cmpValue, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException("Unknown string type metafield ComparisonOperation")
        };
    }

    public static bool MetaDataIntegerValueCmp(TrackedLead lead, string name, ComparisonOperation op, long cmpValue)
    {
        var metaValue = lead.MetaData?[name];
        if (metaValue is null) return false;
        var value = metaValue.GetValue<long>();
        return op switch
        {
            ComparisonOperation.Equal => value == cmpValue,
            ComparisonOperation.NotEqual => value != cmpValue,
            ComparisonOperation.LessThan => value < cmpValue,
            ComparisonOperation.LessThanEqual => value <= cmpValue,
            ComparisonOperation.MoreThan => value > cmpValue,
            ComparisonOperation.MoreThanEqual => value >= cmpValue,
            _ => throw new ArgumentException("Unknown integer type metafield ComparisonOperation")
        };
    }

    public static bool MetaDataStringSetValueCmp(TrackedLead lead, string name, ComparisonOperation op, string cmpValue)
    {
        var metaValue = lead.MetaData?[name];
        if (metaValue is null) return false;
        var value = metaValue.GetValue<string>();
        return op switch
        {
            ComparisonOperation.Equal => value.Equals(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.NotEqual => !value.Equals(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.Contains => value.Contains(cmpValue, StringComparison.OrdinalIgnoreCase),
            ComparisonOperation.NotContains => !value.Contains(cmpValue, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException("Unknown string type metafield ComparisonOperation")
        };
    }

    public static bool MetaDataIntegerSetValueCmp(TrackedLead lead, string name, ComparisonOperation op, IEnumerable<long> cmpValue)
    {
        var metaValue = lead.MetaData?[name];
        if (metaValue is null) return false;
        var value = metaValue.AsArray().ToArray().Select(x => x!.GetValue<long>()).ToHashSet();
        return op switch
        {
            ComparisonOperation.Equal => value.SetEquals(cmpValue),
            ComparisonOperation.NotEqual => !value.SetEquals(cmpValue),
            ComparisonOperation.Contains => value.IsSupersetOf(cmpValue),
            ComparisonOperation.NotContains => !value.IsSupersetOf(cmpValue),
            _ => throw new ArgumentException("Unknown set/integer type metafield ComparisonOperation")
        };
    }

    public static bool MetaDataStringSetValueCmp(TrackedLead lead, string name, ComparisonOperation op, IEnumerable<string> cmpValue)
    {
        var metaValue = lead.MetaData?[name];
        if (metaValue is null) return false;
        var value = metaValue.AsArray().ToArray().Select(x => x!.GetValue<string>()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return op switch
        {
            ComparisonOperation.Equal => value.SetEquals(cmpValue),
            ComparisonOperation.NotEqual => !value.SetEquals(cmpValue),
            ComparisonOperation.Contains => value.IsSupersetOf(cmpValue),
            ComparisonOperation.NotContains => !value.IsSupersetOf(cmpValue),
            _ => throw new ArgumentException("Unknown set/string type metafield ComparisonOperation")
        };
    }

}
