using Microsoft.AspNetCore.Identity;
using Plat4Me.DialAgentApi.Persistent.Seed.Authentication;

namespace Plat4Me.DialAgentApi.Persistent.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<DialDbContext>();
        var roleManager = scoped.ServiceProvider.GetService<RoleManager<IdentityRole<long>>>();

        var defaultClientId = SeedClient.Seed(context!);

        AdminRegistration.Seed(roleManager!);

        SeedSettings.Seed(context!, defaultClientId);
    }
}
