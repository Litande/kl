using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class AgentExtensions
{
    public static string FullName(this User user)
        => $"{user.FirstName} {user.LastName}".Trim();

    public static string FullName(this AgentInfoProjection user)
        => $"{user.FirstName} {user.LastName}".Trim();
}