using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Tags;

public record TagResponse(
    long Id,
    TagStatusTypes Status,
    string Name,
    int Value,
    int? LifetimeSeconds);