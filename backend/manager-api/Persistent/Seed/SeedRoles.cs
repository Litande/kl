using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;

namespace KL.Manager.API.Persistent.Seed;

public class SeedRoles
{
    public static void Seed(RoleManager<Role> roleManager)
    {
        foreach (UserRoleTypes role in Enum.GetValues(typeof(UserRoleTypes)))
        {
            if (!roleManager.RoleExistsAsync(role.ToString()).Result)
            {
                var identityRole = new Role()
                {
                    Name = role.ToString(),
                };
                var result = roleManager.CreateAsync(identityRole).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Error creating role '{role}'.");
                }
            }
        }
    }
}