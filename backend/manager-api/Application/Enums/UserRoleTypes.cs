using System.ComponentModel;

namespace Plat4Me.DialClientApi.Application.Enums;

public enum UserRoleTypes
{
    [Description("Agent")]
    Agent = 1,
    [Description("Manager")]
    Manager = 2,
}