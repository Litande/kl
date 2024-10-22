using KL.Agent.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;

namespace KL.Agent.API.Persistent.Seed.Authentication;

public static class AdminRegistration
{
    public static void Seed(RoleManager<Role> roleManager)
    {
        var agentRole = roleManager.FindByNameAsync("Agent").Result;
        if (agentRole == null)
        {
            // CREATE ADMIN USER
            var role = new Role
            {
                Name = "Agent",
            };

            var _ = roleManager.CreateAsync(role).Result;
        }
    }
}
