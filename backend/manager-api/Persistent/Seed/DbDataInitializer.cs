using Microsoft.AspNetCore.Identity;
using Plat4Me.DialClientApi.Persistent.Seed.Authentication;

namespace Plat4Me.DialClientApi.Persistent.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<DialDbContext>();
        var rolesManager = scoped.ServiceProvider.GetService<RoleManager<IdentityRole<long>>>();
        var userManager = scoped.ServiceProvider.GetService<UserManager<IdentityUser<long>>>();

        var defaultClientId = SeedClient.Seed(context!);

        AdminRegistration.Seed(rolesManager!, userManager!, context!, defaultClientId);

        SeedSettings.Seed(context!, defaultClientId);
        
        SeedRoles.Seed(rolesManager!);
    }
}
