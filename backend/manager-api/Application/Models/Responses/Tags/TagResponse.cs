using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Tags;

public record TagResponse(
    long Id,
    TagStatusTypes Status,
    string Name,
    int Value,
    int? LifetimeSeconds);