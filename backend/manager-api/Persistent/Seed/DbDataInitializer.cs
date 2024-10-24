﻿using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Seed.Authentication;
using Microsoft.AspNetCore.Identity;

namespace KL.Manager.API.Persistent.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<KlDbContext>();
        var rolesManager = scoped.ServiceProvider.GetService<RoleManager<Role>>();
        var userManager = scoped.ServiceProvider.GetService<UserManager<User>>();

        var defaultClientId = SeedClient.Seed(context!);

        AdminRegistration.Seed(rolesManager!, userManager!, context!, defaultClientId);

        SeedSettings.Seed(context!, defaultClientId);
        
        SeedRoles.Seed(rolesManager!);
    }
}
