using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Application.Extensions;

public static class AgentExtensions
{
    public static string FullName(this User user)
        => $"{user.FirstName} {user.LastName}".Trim();

    public static string FullName(this AgentInfoProjection user)
        => $"{user.FirstName} {user.LastName}".Trim();
}