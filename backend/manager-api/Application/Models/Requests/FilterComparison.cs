using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests;

public record FilterComparison<T>(FilterComparisonOperation Operation, T Value);
