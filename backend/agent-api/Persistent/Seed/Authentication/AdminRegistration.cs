using Microsoft.AspNetCore.Identity;

namespace Plat4Me.DialAgentApi.Persistent.Seed.Authentication;

public static class AdminRegistration
{
    public static void Seed(RoleManager<IdentityRole<long>> roleManager)
    {
        var agentRole = roleManager.FindByNameAsync("Agent").Result;
        if (agentRole == null)
        {
            // CREATE ADMIN USER
            var role = new IdentityRole<long>
            {
                Name = "Agent",
            };

            var _ = roleManager.CreateAsync(role).Result;
        }
    }
}
