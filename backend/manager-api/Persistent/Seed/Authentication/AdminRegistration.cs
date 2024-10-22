using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;

namespace KL.Manager.API.Persistent.Seed.Authentication;

public static class AdminRegistration
{
    public static void Seed(
        RoleManager<Role> rolesManager,
        UserManager<User> userManager,
        KlDbContext context,
        long defaultClientId)
    {
        var admin = userManager.FindByNameAsync("manager@localhost.com").Result;
        if (admin == null)
        {
            // CREATE ADMIN USER
            var user = new User()
            {
                Email = "manager@localhost.com",
                EmailConfirmed = true,
                UserName = "manager@localhost.com",
                ClientId = defaultClientId,
                FirstName = "Main",
                LastName = "Manager",
                CreatedAt = DateTimeOffset.UtcNow,
                RoleType = RoleTypes.Manager,
            };

            var _ = userManager.CreateAsync(user, "gkHe1GJn").Result;
            admin = user;
        }

        var managerRole = rolesManager.FindByNameAsync("Manager").Result;
        if (managerRole == null)
        {
            // CREATE ADMIN USER
            var role = new Role
            {
                Name = "Manager",
            };

            var _ = rolesManager.CreateAsync(role).Result;
        }

        if (userManager.IsInRoleAsync(admin, "MANAGER").Result == false)
        {
            var _ = userManager.AddToRoleAsync(admin, "MANAGER").Result;
        }
    }
}
