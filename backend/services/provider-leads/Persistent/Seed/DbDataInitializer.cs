namespace KL.Provider.Leads.Persistent.Seed;

public static class DbDataInitializer
{
    public static void InitializeDbData(this IServiceCollection services)
    {
        using var scoped = services.BuildServiceProvider().CreateScope();
        var context = scoped.ServiceProvider.GetService<KlDbContext>();

        SeedTimeZones.Seed(context!);
    }
}