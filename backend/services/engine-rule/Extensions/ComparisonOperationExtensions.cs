using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.Extensions;

public static class ComparisonOperationExtensions
{
    private static readonly ComparisonOperation[] HoursOperations = {
        ComparisonOperation.EqualForLastYHours,
        ComparisonOperation.NotEqualForLastYHours,
        ComparisonOperation.MoreThanForLastYHours,
        ComparisonOperation.LessThanForLastYHours,
    };

    private static readonly ComparisonOperation[] DaysOperations = {
        ComparisonOperation.EqualForLastYDays,
        ComparisonOperation.NotEqualForLastYDays,
        ComparisonOperation.MoreThanForLastYDays,
        ComparisonOperation.LessThanForLastYDays,
    };

    public static bool IsHoursOperation(this ComparisonOperation operation) =>
        HoursOperations.Contains(operation);

    public static bool IsDaysOperation(this ComparisonOperation operation) =>
        DaysOperations.Contains(operation);
}