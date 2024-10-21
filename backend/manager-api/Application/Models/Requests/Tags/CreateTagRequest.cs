using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Tags;

public record CreateTagRequest(
    TagStatusTypes Status,
    string Name,
    int Value,
    int? LifetimeSeconds);