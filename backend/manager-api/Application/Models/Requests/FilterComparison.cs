using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests;

public record FilterComparison<T>(FilterComparisonOperation Operation, T Value);
