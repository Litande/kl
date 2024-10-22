using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

public enum UserRoleTypes
{
    [Description("Agent")]
    Agent = 1,
    [Description("Manager")]
    Manager = 2,
}