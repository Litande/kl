using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class UserExtensions
{
    public static MeResponse ToResponse(this User user, string[]? iceServers = null)
        => new(
            user.UserId,
            user.FirstName,
            user.LastName,
            iceServers);
}
