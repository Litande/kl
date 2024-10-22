using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Seed.Authentication;
using Microsoft.AspNetCore.Identity;

namespace KL.Agent.API.Persistent.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<KlDbContext>();
        var roleManager = scoped.ServiceProvider.GetService<RoleManager<Role>>();

        var defaultClientId = SeedClient.Seed(context!);

        AdminRegistration.Seed(roleManager!);

        SeedSettings.Seed(context!, defaultClientId);
    }
}
