namespace KL.Caller.Leads.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<KlDbContext>();
        var defaultClientId = SeedClient.Seed(context!);
        SeedSettings.Seed(context!, defaultClientId);
    }
}
