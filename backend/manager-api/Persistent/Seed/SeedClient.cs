using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Seed;

public static class SeedClient
{
    public static long Seed(DialDbContext context)
    {
        var clientId = AddClientIfNeed(context);
        return clientId;
    }

    private static long AddClientIfNeed(DialDbContext context)
    {
        const long defaultClientId = 1;
        var client = context.Clients
            .Where(r => r.Id == defaultClientId)
            .FirstOrDefault();

        if (client is not null) return client.Id;

        client = new Client { Id = defaultClientId, };
        context.Clients.Add(client);
        context.SaveChanges();

        return client.Id;
    }
}
