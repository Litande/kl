using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Seed;

public static class SeedClient
{
    public static long Seed(KlDbContext context)
    {
        var clientId = AddClientIfNeed(context);
        return clientId;
    }

    private static long AddClientIfNeed(KlDbContext context)
    {
        const long defaultClientId = 1;
        var client = context.Clients
            .FirstOrDefault(r => r.Id == defaultClientId);

        if (client is not null) return client.Id;

        client = new Client { Id = defaultClientId, };
        context.Clients.Add(client);
        context.SaveChanges();

        return client.Id;
    }
}
