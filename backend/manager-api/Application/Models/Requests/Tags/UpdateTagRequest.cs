using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Tags;

public record UpdateTagRequest(
    TagStatusTypes Status,
    string Name,
    int Value,
    int? LifetimeSeconds);