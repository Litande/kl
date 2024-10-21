using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Extensions;

public static class EntitiesExtensions
{
    public static string? FullName(this Lead? lead)
        => lead is null ? null : $"{lead.FirstName} {lead.LastName}".Trim();

    public static string? FullName(this User? user)
        => user is null ? null : $"{user.FirstName} {user.LastName}".Trim();
}
