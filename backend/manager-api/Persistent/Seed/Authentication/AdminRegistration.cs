using Microsoft.AspNetCore.Identity;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Seed.Authentication;

public static class AdminRegistration
{
    public static void Seed(
        RoleManager<IdentityRole<long>> rolesManager,
        UserManager<IdentityUser<long>> userManager,
        DialDbContext context,
        long defaultClientId)
    {
        var admin = userManager.FindByNameAsync("manager@plat4me.com").Result;
        if (admin == null)
        {
            // CREATE ADMIN USER
            var user = new IdentityUser<long>
            {
                Email = "manager@plat4me.com",
                EmailConfirmed = true,
                UserName = "manager@plat4me.com",
            };

            var _ = userManager.CreateAsync(user, "gkHe1GJn").Result;
            admin = user;
        }

        var adminUser = context.Users
            .Where(r => r.UserId == admin.Id)
            .FirstOrDefault();

        if (adminUser is null)
        {
            adminUser = new User
            {
                UserId = admin.Id,
                ClientId = defaultClientId,
                FirstName = "Main",
                LastName = "Manager",
                CreatedAt = DateTimeOffset.UtcNow,
                RoleType = RoleTypes.Manager,
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }

        var managerRole = rolesManager.FindByNameAsync("Manager").Result;
        if (managerRole == null)
        {
            // CREATE ADMIN USER
            var role = new IdentityRole<long>
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
