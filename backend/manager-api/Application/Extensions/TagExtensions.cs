using Plat4Me.DialClientApi.Application.Models.Requests.Tags;
using Plat4Me.DialClientApi.Application.Models.Responses.Tags;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class TagExtensions
{
    public static Tag ToModel(this CreateTagRequest request, long clientId) =>
        new()
        {
            Id = 0,
            ClientId = clientId,
            Status = request.Status,
            Name = request.Name,
            Value = request.Value,
            LifetimeSeconds = request.LifetimeSeconds
        };

    public static Tag ToModel(this Tag tag, UpdateTagRequest request)
    {
        tag.Status = request.Status;
        tag.Name = request.Name;
        tag.Value = request.Value;
        tag.LifetimeSeconds = request.LifetimeSeconds;

        return tag;
    }

    public static TagResponse ToResponse(this Tag tag) =>
        new(tag.Id,
            tag.Status,
            tag.Name,
            tag.Value,
            tag.LifetimeSeconds);
}
