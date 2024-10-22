using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Application.Extensions;

public static class UserExtensions
{
    public static MeResponse ToResponse(this User user, string[]? iceServers = null)
        => new(
            user.UserId,
            user.FirstName,
            user.LastName,
            iceServers);
}
