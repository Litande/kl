using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;

namespace Plat4Me.DialAgentApi.Application.Extensions;

public static class UserExtensions
{
    public static MeResponse ToResponse(this User user, string[]? iceServers = null)
        => new(
            user.UserId,
            user.FirstName,
            user.LastName,
            iceServers);
}
