using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;

namespace KL.Agent.API.Application.Extensions;

public static class UserExtensions
{
    public static MeResponse ToResponse(this User user, string[]? iceServers = null)
        => new(
            user.UserId,
            user.FirstName,
            user.LastName,
            iceServers);
}
